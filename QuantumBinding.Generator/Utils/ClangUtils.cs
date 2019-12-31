using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using QuantumBinding.Generator.AST;
using System.Linq;
using QuantumBinding.Clang;
using QuantumBinding.Generator.Types;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Generator
{
    internal static partial class ClangUtils
    {
        public static bool IsInSystemHeader(this CXCursor cursor)
        {
            return clang.Location_isInSystemHeader(clang.getCursorLocation(cursor)) != 0;
        }

        public static bool IsPtrToConstChar(this CXType type)
        {
            var pointee = clang.getPointeeType(type);

            if (clang.isConstQualifiedType(pointee) != 0)
            {
                switch (pointee.kind)
                {
                    case CXTypeKind.CXType_Char_S:
                        return true;
                }
            }

            return false;
        }

        public static BindingType GetBindingType(this CXType cursorType)
        {
            BindingType type = null;
            bool isConst = clang.isConstQualifiedType(cursorType) != 0;
            bool isVolatile = clang.isVolatileQualifiedType(cursorType) != 0;
            bool isRestrict = clang.isRestrictQualifiedType(cursorType) != 0;

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
                switch (cursorType.kind)
                {
                    case CXTypeKind.CXType_Unexposed:
                        if (cursorType.kind == CXTypeKind.CXType_Unexposed)
                        {
                            var typeSpelling = clang.getTypeSpelling(cursorType).ToString();
                            type = new CustomType(typeSpelling);
                            break;
                        }

                        type = cursorType.GetBindingType();
                        break;
                    // We need to unwrap elaborated types
                    case CXTypeKind.CXType_Elaborated:
                        return GetBindingType(clang.Type_getNamedType(cursorType));
                    case CXTypeKind.CXType_Typedef:
                        var cursor = clang.getTypeDeclaration(cursorType);
                        // For some reason size_t isn't considered as within a system header.
                        // We work around this by asking for the file name - if it's unknown, probably it's a system header
                        var isInSystemHeader = clang.Location_isInSystemHeader(clang.getCursorLocation(cursor)) != 0;
                        clang.getPresumedLocation(
                            clang.getCursorLocation(cursor),
                            out CXString filename,
                            out uint line,
                            out uint column);
                        isInSystemHeader |= filename.ToString() == string.Empty;

                        if (isInSystemHeader)
                        {
                            // Cross-plat:
                            // Getting the actual type of a typedef is painful, since platforms don't even agree on the meaning of types;
                            // 64-bit is "long long" on Windows but "long" on Linux, for historical reasons.
                            // The easiest way is to just get the size & signed-ness and write the type ourselves
                            var primitive = clang.getTypedefDeclUnderlyingType(cursor).GetPrimitiveType();
                            //var primitive = clang.getCursorType(cursor).GetPrimitiveType();
                            if (primitive != PrimitiveType.Unknown)
                            {
                                type = new BuiltinType(primitive);
                            }
                            else
                            {
                                var canonical = clang.getCanonicalType(clang.getTypedefDeclUnderlyingType(cursor));
                                type = canonical.GetBindingType();
                                if (type.IsCustomType(out var custom))
                                {
                                    custom.IsInSystemHeader = isInSystemHeader;
                                }
                            }
                        }
                        else
                        {
                            var typedefSpelling = clang.getCursorSpelling(cursor).ToString();
                            type = new CustomType(typedefSpelling);
                        }

                        break;
                    case CXTypeKind.CXType_FunctionProto:
                        var functionType = clang.getTypeSpelling(cursorType).ToString();
                        type = new CustomType(functionType);
                        break;
                    case CXTypeKind.CXType_Pointer:
                        //var pointeeType = clang.getCanonicalType(clang.getPointeeType(cursorType));
                        var pointeeType = clang.getPointeeType(cursorType);
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
                        var enumSpelling = clang.getTypeSpelling(cursorType).ToString();
                        enumSpelling = NormalizeTypeName(enumSpelling);
                        type = new CustomType(enumSpelling);
                        break;
                    case CXTypeKind.CXType_ConstantArray:
                    case CXTypeKind.CXType_VariableArray:
                    case CXTypeKind.CXType_DependentSizedArray:
                    case CXTypeKind.CXType_IncompleteArray:
                        var elementType = clang.getCanonicalType(clang.getArrayElementType(cursorType));
                        var arraySizeType = GetArraySizeType(cursorType.kind);
                        var arrayType = new ArrayType();
                        arrayType.SizeType = arraySizeType;
                        arrayType.Size = clang.getArraySize(cursorType);
                        arrayType.ElementSize = clang.Type_getSizeOf(elementType);
                        if (elementType.IsPrimitiveType())
                        {
                            var paramType = elementType.GetPrimitiveType();
                            var t = new BuiltinType(paramType);
                            arrayType.ElementType = t;
                        }
                        else
                        {
                            if (elementType.kind == CXTypeKind.CXType_Pointer)
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
                        var name = clang.getTypeSpelling(cursorType).ToString();
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

        public static bool IsPrimitiveType(this CXType type)
        {
            var canonical = clang.getCanonicalType(type);
            switch (type.kind)
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
                //case CXTypeKind.CXType_Pointer:
                //    var pointeeType = clang.getCanonicalType(clang.getPointeeType(canonical));
                //    switch (pointeeType.kind)
                //    {
                //        case CXTypeKind.CXType_Char_S:
                //        case CXTypeKind.CXType_WChar:
                //            //attribute = "[MarshalAs(UnmanagedType.LPStr)]";
                //            //attribute = "[MarshalAs(UnmanagedType.LPWStr)]";
                //            return true;
                //        default:
                //            return false;
                //    }
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

        public static string GetEnumUnderlyingType(this CXCursor cursor)
        {
            string inheritedEnumType;
            CXTypeKind kind = clang.getEnumDeclIntegerType(cursor).kind;

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

        public static PrimitiveType GetPrimitiveType(this CXType type)
        {
            var canonical = clang.getCanonicalType(type);
            switch (type.kind)
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
                    var pointeeType = clang.getCanonicalType(clang.getPointeeType(canonical));
                    switch (pointeeType.kind)
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

        public static Function GetFunctionInfo(CXCursor cursor)
        {
            var func = new Function();

            var functionType = clang.getCursorType(cursor);
            var functionName = clang.getCursorSpelling(cursor).ToString();
            var resultType = clang.getCursorResultType(cursor);

            func.Name = functionName;
            func.EntryPoint = functionName;
            func.IsPtrToConstChar = resultType.IsPtrToConstChar();
            func.CallingConvention = functionType.GetCallingConvention();
            func.ReturnType = resultType.GetBindingType();

            int numArgTypes = clang.getNumArgTypes(functionType);

            for (uint i = 0; i < numArgTypes; ++i)
            {
                var argument = ArgumentHelper(functionType, clang.Cursor_getArgument(cursor, i), i);
                argument.Parent = func;
                func.Parameters.Add(argument);
            }

            return func;
        }

        public static CallingConvention GetCallingConvention(this CXType type)
        {
            var callingConvention = clang.getFunctionTypeCallingConv(type);
            switch (callingConvention)
            {
                case CXCallingConv.CXCallingConv_X86StdCall:
                case CXCallingConv.CXCallingConv_X86_64Win64:
                    return CallingConvention.StdCall;
                default:
                    return CallingConvention.Cdecl;
            }
        }

        public static Parameter ArgumentHelper(CXType functionType, CXCursor paramCursor, uint index)
        {
            var type = clang.getCursorType(paramCursor);
            var spelling = clang.getCursorSpelling(paramCursor).ToString();
            if (string.IsNullOrEmpty(spelling))
            {
                spelling = "param" + index;
            }

            var functionParameter = new Parameter();
            functionParameter.Index = index;
            functionParameter.Name = spelling;
            BindingType bindingType;
            switch (type.kind)
            {
                case CXTypeKind.CXType_Pointer:
                    var pointee = clang.getPointeeType(type);
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

        public static FileLocation GetCurrentCursorLocation(CXCursor cursor)
        {
            clang.getPresumedLocation(clang.getCursorLocation(cursor), out CXString filename, out uint line, out uint column);
            var location = new FileLocation {FileName = filename.ToString(), LineNumber = line, Column = column};

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

        public static List<T> GetFlags<T>(this T input) where T: Enum
        {
            return Enum.GetValues(input.GetType()).Cast<T>().Where(x=>input.HasFlag(x) && Convert.ToInt64(x) != 0).ToList();
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

            return new BuiltinType(PrimitiveType.Int32);
        }
    }
}
