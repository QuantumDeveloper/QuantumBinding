using QuantumBinding.Clang;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace QuantumBinding.Generator.Utils
{
    public static class ClangUtils
    {
        public static bool IsInSystemHeader(this QBCursor cursor)
        {
            return cursor.GetCursorLocation().Location_isInSystemHeader() != 0;
        }

        public static bool IsPtrToConstChar(this QBType type)
        {
            var pointee = type.GetPointeeType();

            if (pointee.IsConstQualifiedType() != 0)
            {
                switch (pointee.Kind)
                {
                    case CXTypeKind.CXType_Char_S:
                        return true;
                }
            }

            return false;
        }

        public static BindingType GetBindingType(this QBType cursorType)
        {
            BindingType type = null;
            bool isConst = cursorType.IsConstQualifiedType() != 0;
            bool isVolatile = cursorType.IsVolatileQualifiedType() != 0;
            bool isRestrict = cursorType.IsRestrictQualifiedType() != 0;

            if (cursorType.ToString().StartsWith("const"))
            {
                isConst = true;
            }

            if (cursorType.IsPrimitiveType())
            {
                BuiltinType t = new BuiltinType();
                t.Type = GetPrimitiveType(cursorType);
                type = t;
            }
            else
            {
                switch (cursorType.Kind)
                {
                    case CXTypeKind.CXType_Unexposed:
                        if (cursorType.Kind == CXTypeKind.CXType_Unexposed)
                        {
                            var typeSpelling = cursorType.GetTypeSpelling().ToString();
                            type = new CustomType(typeSpelling);
                            break;
                        }

                        type = cursorType.GetBindingType();
                        break;
                    // We need to unwrap elaborated types
                    case CXTypeKind.CXType_Elaborated:
                        return GetBindingType(cursorType.Type_getNamedType());
                    case CXTypeKind.CXType_Typedef:
                        var cursor = cursorType.GetTypeDeclaration();
                        // For some reason size_t isn't considered as within a system header.
                        // We work around this by asking for the file name - if it's unknown, probably it's a system header
                        var isInSystemHeader = cursor.IsInSystemHeader();
                        cursor.GetCursorLocation().GetPresumedLocation(
                            out var filename,
                            out uint line,
                            out uint column
                            );
                        isInSystemHeader |= filename.ToString() == string.Empty;

                        if (isInSystemHeader || IsPrimitiveTypedef(cursor.ToString()))
                        {
                            // Cross-plat:
                            // Getting the actual type of a typedef is painful, since platforms don't even agree on the meaning of types;
                            // 64-bit is "long long" on Windows but "long" on Linux, for historical reasons.
                            // The easiest way is to just get the size & signed-ness and write the type ourselves
                            var primitive = cursor.GetTypedefDeclUnderlyingType().GetPrimitiveType();
                            //var primitive = clang.getCursorType(cursor).GetPrimitiveType();
                            if (primitive != PrimitiveType.Unknown)
                            {
                                type = new BuiltinType(primitive);
                            }
                            else
                            {
                                var canonical = cursor.GetTypedefDeclUnderlyingType().GetCanonicalType();
                                type = canonical.GetBindingType();
                                if (type.IsCustomType(out var custom))
                                {
                                    custom.IsInSystemHeader = isInSystemHeader;
                                }
                            }
                        }
                        else
                        {
                            var typedefSpelling = cursor.GetCursorSpelling().ToString();
                            type = new CustomType(typedefSpelling);
                        }

                        break;
                    case CXTypeKind.CXType_FunctionProto:
                        var functionType = cursorType.GetTypeSpelling().ToString();
                        type = new CustomType(functionType);
                        break;
                    case CXTypeKind.CXType_Pointer:
                        //var pointeeType = clang.getCanonicalType(clang.getPointeeType(cursorType));
                        var pointeeType = cursorType.GetPointeeType();
                        var pointer = new PointerType();
                        var isBuiltin = pointeeType.IsPrimitiveType();
                        if (isBuiltin)
                        {
                            pointer.Pointee = new BuiltinType(pointeeType.GetPrimitiveType());
                        }
                        else
                        {
                            pointer.Pointee = GetBindingType(pointeeType);
                        }

                        type = pointer;
                        break;
                    case CXTypeKind.CXType_Record:
                    case CXTypeKind.CXType_Enum:
                        var enumSpelling = cursorType.GetTypeSpelling().ToString();
                        enumSpelling = NormalizeTypeName(enumSpelling);
                        type = new CustomType(enumSpelling);
                        break;
                    case CXTypeKind.CXType_ConstantArray:
                    case CXTypeKind.CXType_VariableArray:
                    case CXTypeKind.CXType_DependentSizedArray:
                    case CXTypeKind.CXType_IncompleteArray:
                        var arrayType = new ArrayType();

                        var elementType = cursorType.GetArrayElementType().GetCanonicalType();
                        arrayType.SizeType = GetArraySizeType(cursorType.Kind);
                        arrayType.Size = cursorType.GetArraySize();
                        arrayType.Sizes.Add(cursorType.GetArraySize());
                        arrayType.ElementSize = elementType.Type_getSizeOf();

                        while (elementType.IsArrayType())
                        {
                            arrayType.DimensionsCount++;
                            arrayType.Sizes.Add(elementType.GetArraySize());
                            elementType = elementType.GetArrayElementType().GetCanonicalType();
                        }

                        if (arrayType.IsMultiDimensional)
                        {
                            // Reset size to 1 for proper calculation of array size if we have multidimensional array
                            arrayType.Size = 1;

                            foreach (var size in arrayType.Sizes)
                            {
                                arrayType.Size *= size;
                            }
                        }

                        if (elementType.IsPrimitiveType())
                        {
                            var paramType = elementType.GetPrimitiveType();
                            arrayType.ElementType = new BuiltinType(paramType);
                        }
                        else
                        {
                            if (elementType.Kind == CXTypeKind.CXType_Pointer)
                            {
                                var pointerType = (PointerType)elementType.GetBindingType();
                                arrayType.ElementType = pointerType;
                            }
                            else
                            {
                                arrayType.ElementType = elementType.GetBindingType();
                            }
                        }

                        type = arrayType;
                        break;
                    default:
                        var name = cursorType.GetTypeSpelling().ToString();
                        name = NormalizeTypeName(name);
                        type = new CustomType(name);
                        break;
                }
            }

            type.Qualifiers.IsConst = isConst;
            type.Qualifiers.IsRestrict = isRestrict;
            type.Qualifiers.IsVolatile = isVolatile;

            return type;
        }

        private static string NormalizeTypeName(string name)
        {
            return name.Replace("const", "").Trim();
        }

        public static bool IsPrimitiveType(this QBType type)
        {
            switch (type.Kind)
            {
                case CXTypeKind.CXType_Bool:
                case CXTypeKind.CXType_UChar:
                case CXTypeKind.CXType_Char_U:
                case CXTypeKind.CXType_SChar:
                case CXTypeKind.CXType_Char_S:
                case CXTypeKind.CXType_UShort:
                case CXTypeKind.CXType_WChar:
                case CXTypeKind.CXType_Short:
                case CXTypeKind.CXType_Float:
                case CXTypeKind.CXType_Double:
                case CXTypeKind.CXType_Int:
                case CXTypeKind.CXType_Long:
                case CXTypeKind.CXType_UInt:
                case CXTypeKind.CXType_Int128:
                case CXTypeKind.CXType_NullPtr:
                case CXTypeKind.CXType_ULong:
                case CXTypeKind.CXType_LongLong:
                case CXTypeKind.CXType_ULongLong:
                case CXTypeKind.CXType_Void:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsArrayType(this QBType type)
        {
            switch (type.Kind)
            {
                case CXTypeKind.CXType_ConstantArray:
                case CXTypeKind.CXType_VariableArray:
                case CXTypeKind.CXType_DependentSizedArray:
                case CXTypeKind.CXType_IncompleteArray:
                    return true;
                default:
                    return false;
            }
        }

        public static ArraySizeType GetArraySizeType(CXTypeKind typeKind)
        {
            switch (typeKind)
            {
                case CXTypeKind.CXType_ConstantArray:
                    return ArraySizeType.Constant;
                case CXTypeKind.CXType_VariableArray:
                    return ArraySizeType.Variable;
                case CXTypeKind.CXType_DependentSizedArray:
                    return ArraySizeType.Dependent;
                case CXTypeKind.CXType_IncompleteArray:
                    return ArraySizeType.Incomplete;
                default:
                    throw new ArgumentException("Parameter does not belongs to an array type", nameof(typeKind));
            }
        }

        public static string GetEnumUnderlyingType(this QBCursor cursor)
        {
            string inheritedEnumType;
            CXTypeKind kind = cursor.GetEnumDeclIntegerType().Kind;

            switch (kind)
            {
                case CXTypeKind.CXType_Int:
                    inheritedEnumType = "int";
                    break;
                case CXTypeKind.CXType_UInt:
                    inheritedEnumType = "uint";
                    break;
                case CXTypeKind.CXType_Short:
                    inheritedEnumType = "short";
                    break;
                case CXTypeKind.CXType_UShort:
                    inheritedEnumType = "ushort";
                    break;
                case CXTypeKind.CXType_LongLong:
                    inheritedEnumType = "long";
                    break;
                case CXTypeKind.CXType_ULongLong:
                    inheritedEnumType = "ulong";
                    break;
                default:
                    inheritedEnumType = "int";
                    break;
            }

            return inheritedEnumType;
        }

        public static PrimitiveType GetPrimitiveType(this QBType type)
        {
            var canonical = type.GetCanonicalType();
            switch (type.Kind)
            {
                case CXTypeKind.CXType_Bool:
                    return PrimitiveType.Bool;
                case CXTypeKind.CXType_UChar:
                case CXTypeKind.CXType_Char_U:
                    return PrimitiveType.UChar;
                case CXTypeKind.CXType_SChar:
                case CXTypeKind.CXType_Char_S:
                    return PrimitiveType.SChar;
                case CXTypeKind.CXType_Char16:
                    return PrimitiveType.Char;
                case CXTypeKind.CXType_UShort:
                    return PrimitiveType.UInt16;
                case CXTypeKind.CXType_WChar:
                    return PrimitiveType.WideChar;
                case CXTypeKind.CXType_Short:
                    return PrimitiveType.Int16;
                case CXTypeKind.CXType_Float:
                    return PrimitiveType.Float;
                case CXTypeKind.CXType_Double:
                    return PrimitiveType.Double;
                case CXTypeKind.CXType_Int:
                case CXTypeKind.CXType_Long:
                    return PrimitiveType.Int32;
                case CXTypeKind.CXType_UInt:
                case CXTypeKind.CXType_ULong:
                    return PrimitiveType.UInt32;
                case CXTypeKind.CXType_Int128:
                    return PrimitiveType.Decimal;
                case CXTypeKind.CXType_NullPtr:
                    return PrimitiveType.IntPtr;
                case CXTypeKind.CXType_LongLong:
                    return PrimitiveType.Int64;
                case CXTypeKind.CXType_ULongLong:
                    return PrimitiveType.UInt64;
                case CXTypeKind.CXType_Void:
                    return PrimitiveType.Void;
                case CXTypeKind.CXType_Pointer:
                    var pointeeType = canonical.GetPointeeType().GetCanonicalType();
                    switch (pointeeType.Kind)
                    {
                        case CXTypeKind.CXType_Char_S:
                            return PrimitiveType.SChar;
                        case CXTypeKind.CXType_WChar:
                            //attribute = "[MarshalAs(UnmanagedType.LPStr)]";
                            //attribute = "[MarshalAs(UnmanagedType.LPWStr)]";
                            return PrimitiveType.WideChar;
                        default:
                            return PrimitiveType.IntPtr;
                    }
                default:
                    return PrimitiveType.Unknown;
            }
        }

        public static bool IsPrimitiveTypedef(string typeName)
        {
            switch (typeName)
            {
                case "int8_t":  
                case "uint8_t": 
                case "int16_t": 
                case "uint16_t":
                case "int32_t": 
                case "uint32_t":
                case "int64_t":
                case "uint64_t":
                    return true;
                default:
                    return false;
            }
        }

        public static Function GetFunctionInfo(QBCursor cursor)
        {
            var func = new Function();

            var functionType = cursor.GetCursorType();
            var functionName = cursor.GetCursorSpelling().ToString();
            var resultType = cursor.GetCursorResultType();

            func.Name = functionName;
            func.EntryPoint = functionName;
            func.IsPtrToConstChar = resultType.IsPtrToConstChar();
            func.CallingConvention = functionType.GetCallingConvention();
            func.ReturnType = resultType.GetBindingType();

            int numArgTypes = functionType.GetNumArgTypes();

            for (uint i = 0; i < numArgTypes; ++i)
            {
                var argument = ArgumentHelper(functionType, cursor.Cursor_getArgument(i), i);
                argument.Parent = func;
                func.Parameters.Add(argument);
            }

            return func;
        }

        public static CallingConvention GetCallingConvention(this QBType type)
        {
            var callingConvention = type.GetFunctionTypeCallingConv();
            switch (callingConvention)
            {
                case CXCallingConv.CXCallingConv_X86StdCall:
                case CXCallingConv.CXCallingConv_X86_64Win64:
                    return CallingConvention.StdCall;
                default:
                    return CallingConvention.Cdecl;
            }
        }

        public static Parameter ArgumentHelper(QBType functionType, QBCursor paramCursor, uint index)
        {
            var type = paramCursor.GetCursorType();
            var spelling = paramCursor.GetCursorSpelling().ToString();
            if (string.IsNullOrEmpty(spelling))
            {
                spelling = "param" + index;
            }

            var functionParameter = new Parameter();
            functionParameter.Index = index;
            functionParameter.Name = spelling;
            BindingType bindingType;
            switch (type.Kind)
            {
                case CXTypeKind.CXType_Pointer:
                    var pointee = type.GetPointeeType();
                    var pointerType = new PointerType();
                    pointerType.Pointee = pointee.GetBindingType();
                    pointerType.Qualifiers.IsConst = pointerType.Pointee.IsConst;
                    pointerType.Pointee.Qualifiers.IsConst = false;
                    bindingType = pointerType;
                    break;
                default:
                    bindingType = type.GetBindingType();
                    break;
            }

            bindingType.Declaration = functionParameter;
            functionParameter.Type = bindingType;

            return functionParameter;
        }

        public static FileLocation GetCurrentCursorLocation(QBCursor cursor)
        {
            cursor.GetCursorLocation().GetPresumedLocation(out var filename, out uint line, out uint column);
            var location = new FileLocation { FileName = filename.ToString(), LineNumber = line, Column = column };

            return location;
        }

        public static bool IsPowerOfTwo(uint number)
        {
            return (number & (number - 1)) == 0;
        }

        public static string ChangeStringStyle(string input, NamingStyle style)
        {
            string result = input[0].ToString().ToUpper() + input.Substring(1);

            return result;
        }

        public static List<T> GetFlags<T>(this T input) where T : Enum
        {
            return Enum.GetValues(input.GetType()).Cast<T>().Where(x => input.HasFlag(x) && Convert.ToInt64(x) != 0).ToList();
        }

        public static BindingType GetMacroType(string macroValue)
        {
            if (macroValue.StartsWith("\"") && macroValue.EndsWith("\""))
            {
                return new BuiltinType(PrimitiveType.String);
            }

            if (macroValue.EndsWith("f"))
            {
                return new BuiltinType(PrimitiveType.Float);
            }

            if (macroValue.EndsWith("UL") || macroValue.EndsWith("ULL"))
            {
                return new BuiltinType(PrimitiveType.UInt64);
            }

            if (macroValue.EndsWith("U"))
            {
                return new BuiltinType(PrimitiveType.UInt32);
            }

            return new BuiltinType(PrimitiveType.UInt32);
        }
    }
}
