using System;
using QuantumBinding.Generator.AST;
using Delegate = QuantumBinding.Generator.AST.Delegate;

namespace QuantumBinding.Generator.Types
{
    public static class TypeUtil
    {
        public static bool IsArray(this BindingType type)
        {
            return type is ArrayType;
        }

        public static bool IsArray(this BindingType type, out ArraySizeType sizeType)
        {
            sizeType = ArraySizeType.Unknown;
            if (type is ArrayType array)
            {
                sizeType = array.SizeType;
                return true;
            }

            return false;
        }

        public static bool IsConstArray(this BindingType type)
        {
            if (type is ArrayType t)
            {
                return t.IsConstArray;
            }

            return false;
        }

        public static bool IsConstArray(this BindingType type, out int size)
        {
            size = -1;
            if (type is ArrayType t)
            {
                size = (int)t.Size;
                return t.IsConstArray;
            }

            return false;
        }

        public static bool IsPointer(this BindingType type)
        {
            return type is PointerType;
        }

        public static bool IsPointerToIntPtr(this BindingType type)
        {
            type.IsPointerToBuiltInType(out var primitive);
            return primitive is PrimitiveType.IntPtr or PrimitiveType.UintPtr;
        }

        public static bool IsPurePointer(this BindingType type)
        {
            return (type.IsPointerToVoid() || type.IsDoublePointer() || type.IsPointerToIntPtr()) && type.Declaration == null;
        }

        public static bool IsPointerToVoid(this BindingType type)
        {
            var depth = 1;
            PrimitiveType primitive = PrimitiveType.Unknown;
            var pointer = type as PointerType;
            if (pointer == null) return false;
            
            do
            {
                if (pointer.Pointee is PointerType pointee)
                {
                    depth++;
                    pointer = pointee;
                }
                
                if (pointer.Pointee is BuiltinType builtin)
                {
                    pointer.IsPointerToBuiltInType(out primitive);
                }
            } while (pointer.Pointee is PointerType);
            
            return primitive == PrimitiveType.Void;
        }

        public static bool IsPointerToBuiltInType(this BindingType type, out PrimitiveType primitive)
        {
            var isPrimitive = IsPointerToPrimitiveType(type, out primitive);
            
            if (isPrimitive &&
                (primitive is PrimitiveType.IntPtr or PrimitiveType.UintPtr or PrimitiveType.Void))
            {
                return false;
            }

            if (CanConvertToString(type))
            {
                return false;
            }
            

            return isPrimitive;
        }
        
        public static bool IsPointerToSimpleType(this BindingType type)
        {
            var pointer = type as PointerType;

            if (pointer == null)
            {
                return false;
            }

            if (pointer.Declaration is Class classDecl && classDecl.IsSimpleType) return true;

            return false;
        }

        private static bool IsPointerToPrimitiveType(this BindingType type, out PrimitiveType primitive)
        {
            primitive = PrimitiveType.Null;

            var pointer = type as PointerType;

            if (pointer == null)
            {
                return false;
            }

            var builtin = pointer.Pointee as BuiltinType;
            if (builtin == null)
            {
                primitive = PrimitiveType.Null;
                return false;
            }

            return builtin.IsPrimitiveType(out primitive);
        }

        public static bool IsDoublePointer(this BindingType type)
        {
            var pointer = type as PointerType;
            var pPtr = pointer?.Pointee as PointerType;
            return pPtr != null;
        }
        
        public static bool IsPointerToVoidArray(this BindingType type)
        {
            var array = type as ArrayType;
            if (array != null && array.ElementType is PointerType pointerType)
            {
                return pointerType.IsPointerToVoid();
            }

            return false;
        }

        public static bool IsPointerToArray(this BindingType type)
        {
            var pointer = type as PointerType;
            var array = pointer?.Pointee as ArrayType;
            return array != null;
        }

        public static bool IsPointerToArray(this BindingType type, out ArrayType arrayType, out uint depth)
        {
            depth = 1;
            arrayType = null;
            var pointer = type as PointerType;
            if (pointer == null)
            {
                return false;
            }

            do
            {
                if (pointer.Pointee is PointerType pointee)
                {
                    depth++;
                    pointer = pointee;
                }

                if (pointer.Pointee is ArrayType array)
                {
                    arrayType = array;
                }
            } while (pointer.Pointee is PointerType);

            return arrayType != null;
        }

        public static bool IsPointerToEnum(this BindingType type)
        {
            var pointer = type as PointerType;
            if (pointer != null && type.Declaration is Enumeration)
            {
                return true;
            }

            return false;
        }

        public static bool IsConstArrayOfPrimitiveTypes(this BindingType type, out int size)
        {
            if (IsConstArray(type, out size))
            {
                var t = (ArrayType)type;
                return t.ElementType.IsPrimitiveType;
            }

            return false;
        }
        
        public static bool IsConstArrayOfCustomTypes(this BindingType type, out int size)
        {
            if (IsConstArray(type, out size))
            {
                var t = (ArrayType)type;
                return !t.ElementType.IsPrimitiveType && !t.IsEnum();
            }

            return false;
        }
        
        public static bool IsConstArrayOfEnums(this BindingType type, out int size)
        {
            if (IsConstArray(type, out size))
            {
                var t = (ArrayType)type;
                return t.IsEnum();
            }

            return false;
        }

        public static bool IsArrayOfPrimitiveTypes(this BindingType type)
        {
            if (IsArray(type))
            {
                var t = (ArrayType)type;
                return t.ElementType.IsPrimitiveType;
            }

            return false;
        }

        public static bool IsPointerToArrayOfEnums(this BindingType type)
        {
            if (IsPointerToArray(type) && type.Declaration is Enumeration)
            {
                return true;
            }

            return false;
        }

        public static bool IsArrayOfEnums(this BindingType type)
        {
            if (IsArray(type))
            {
                var t = (ArrayType)type;
                if (t.ElementType.Declaration != null)
                {
                    return t.ElementType.IsEnum();
                }

                var decl = t.Declaration;
                return decl is Enumeration;
            }

            return false;
        }

        public static bool IsPointerToArrayOfPrimitiveTypes(this BindingType type)
        {
            if (IsPointerToArray(type))
            {
                var ptr = (PointerType) type;
                var t = (ArrayType)ptr.Pointee;
                return t.ElementType.IsPrimitiveType;
            }

            return false;
        }
        
        public static bool IsPointerToArrayOfPrimitiveTypes(this BindingType type, out BuiltinType primitiveType)
        {
            primitiveType = null;
            if (IsPointerToArray(type))
            {
                var ptr = (PointerType) type;
                var t = (ArrayType)ptr.Pointee;
                var isPrimitive = t.ElementType.IsPrimitiveType;
                if (isPrimitive)
                {
                    primitiveType = t.ElementType as BuiltinType;
                }
                return isPrimitive;
            }

            return false;
        }

        public static bool IsPointerToStructOrUnion(this BindingType type)
        {
            return IsPointerToStructOrUnion(type, out Class @class);
        }

        public static bool IsPointerToString(this BindingType type)
        {
            
            return type.IsAnsiString() || type.IsUnicodeString() || type.IsStringArray();
        }

        public static bool IsPointerToStructOrUnion(this BindingType type, out Class @class)
        {
            @class = null;
            var p = type as PointerType;

            var decl = p?.Declaration as Class;
            if (decl == null)
            {
                return false;
            }

            if (decl.ClassType is not (ClassType.Struct or ClassType.Union) &&
                (decl.ClassType is not (ClassType.StructWrapper or ClassType.UnionWrapper)))
            {
                return false;
            }

            @class = decl;
            return true;
        }

        public static bool IsPrimitiveType(this BindingType type, out PrimitiveType primitive)
        {
            primitive = PrimitiveType.Null;
            var builtin = type as BuiltinType;
            if (builtin == null)
            {
                return false;
            }

            primitive = builtin.Type;
            return true;
        }

        public static bool CanConvertToString(this BindingType type)
        {
            if (type is ArrayType t)
            {
                if (t.ElementType.IsPrimitiveType && t.IsConstArray)
                {
                    var elementType = t.ElementType as BuiltinType;
                    if (elementType == null)
                    {
                        return false;
                    }

                    if (elementType.Type is PrimitiveType.SChar or PrimitiveType.UChar or PrimitiveType.WideChar)
                    {
                        return true;
                    }
                }
            }
            else if (type is PointerType p)
            {
                if (p.IsConst && p.Pointee.IsPrimitiveType)
                {
                    var builtin = (BuiltinType)p.Pointee;
                    if (builtin.Type is PrimitiveType.SChar or PrimitiveType.WideChar)
                    {
                        return true;
                    }
                }
            }
            else if (type is BuiltinType builtin)
            {
                if (builtin.Type is PrimitiveType.SChar or PrimitiveType.WideChar)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsPrimitiveTypeEquals(this BindingType type, PrimitiveType primitiveType)
        {
            type.IsPrimitiveType(out var t);

            return t == primitiveType;
        }

        public static bool IsCustomType(this BindingType type, out CustomType customType)
        {
            customType = null;

            if (type is CustomType t)
            {
                customType = t;
                return true;
            }

            if (type is PointerType p)
            {
                return IsCustomType(p.Pointee, out customType);
            }

            if (type is ArrayType array)
            {
                return IsCustomType(array.ElementType, out customType);
            }

            return false;
        }

        public static bool IsPointerToCustomType(this BindingType type, out CustomType customType)
        {
            customType = null;
            if (type is PointerType p)
            {
                if (p.Pointee is CustomType custom)
                {
                    customType = custom;
                    return true;
                }
            }

            return false;
        }
        
        public static bool IsPointerToClass(this BindingType type, out Class @class)
        {
            @class = null;
            if (type is PointerType && type.Declaration is Class { ClassType: ClassType.Class } decl)
            {
                @class = decl;
                return true;
            }

            return false;
        }
        
        public static bool IsPointerToSystemType(this BindingType type, out CustomType customType)
        {
            customType = null;
            if (type is PointerType p)
            {
                if (p.Pointee is CustomType custom)
                {
                    customType = custom;
                    return customType.IsInSystemHeader;
                }
            }

            return false;
        }

        public static bool IsDelegate(this BindingType type)
        {
            return type.Declaration is Delegate;
        }

        public static bool IsEnum(this BindingType type)
        {
            return type.Declaration is Enumeration;
        }

        public static bool IsClass(this BindingType type)
        {
            return type.Declaration is Class;
        }

        public static bool IsSimpleType(this BindingType type)
        {
            if (type.Declaration is Class { IsSimpleType: true })
            {
                return true;
            }

            return false;
        }
        
        public static bool TryGetEnum(this BindingType type, out Enumeration @enum)
        {
            var decl = type.Declaration as Enumeration;
            @enum = decl;
            return decl != null;
        }

        public static bool TryGetClass(this BindingType type, out Class @class)
        {
            var decl = type.Declaration as Class;
            @class = decl;
            return decl != null;
        }

        public static bool TryGetDelegate(this BindingType type, out Delegate @class)
        {
            var decl = type.Declaration as Delegate;
            @class = decl;
            return decl != null;
        }

        public static PrimitiveType GetPrimitiveTypeFromString(string type)
        {
            switch (type)
            {
                case "bool":
                    return PrimitiveType.Bool;
                case "char":
                    return PrimitiveType.Char;
                case "ushort":
                    return PrimitiveType.UInt16;
                case "short":
                    return PrimitiveType.Int16;
                case "float":
                    return PrimitiveType.Float;
                case "double":
                    return PrimitiveType.Double;
                case "int":
                    return PrimitiveType.Int32;
                case "uint":
                    return PrimitiveType.UInt32;
                case "decimal":
                    return PrimitiveType.Decimal;
                case "null":
                    return PrimitiveType.IntPtr;
                case "long":
                    return PrimitiveType.Int64;
                case "ulong":
                    return PrimitiveType.UInt64;
                case "void":
                    return PrimitiveType.Void;
                case "IntPtr":
                    return PrimitiveType.IntPtr;
                default:
                    return PrimitiveType.Unknown;
            }
        }

        public static bool IsAnsiString(this BindingType type)
        {
            if (type is PointerType pointer)
            {
                if (pointer.IsConst &&
                    pointer.Pointee.IsPrimitiveTypeEquals(PrimitiveType.SChar))
                {
                    return true;
                }
                else if (pointer.IsConst && pointer.Pointee is PointerType pointer2)
                {
                    if (pointer2.Pointee is PointerType ptr3 && ptr3.Pointee.IsPrimitiveType(out var primitive))
                    {
                        return primitive is PrimitiveType.SChar;
                    }
                }
            }

            return false;
        }

        public static bool IsUnicodeString(this BindingType type)
        {
            if (type is PointerType pointer)
            {
                if (pointer.IsConst &&
                    pointer.Pointee.IsPrimitiveTypeEquals(PrimitiveType.WideChar))
                {
                    return true;
                }
                else if (pointer.IsConst && pointer.Pointee is PointerType pointer2)
                {
                    if (pointer2.Pointee is PointerType ptr3 && ptr3.Pointee.IsPrimitiveType(out var primitive))
                    {
                        return primitive is PrimitiveType.WideChar;
                    }
                }
            }

            return false;
        }
        
        public static bool IsString(this BindingType type)
        {
            if (type is PointerType pointer)
            {
                if (pointer.IsConst &&
                    (pointer.Pointee.IsPrimitiveTypeEquals(PrimitiveType.SChar) ||
                     pointer.Pointee.IsPrimitiveTypeEquals(PrimitiveType.WideChar)))
                {
                    return true;
                }
                else if (pointer.IsConst && pointer.Pointee is PointerType pointer2)
                {
                    if (pointer2.Pointee is PointerType ptr3 && ptr3.Pointee.IsPrimitiveType(out var primitive))
                    {
                        return primitive is PrimitiveType.SChar or PrimitiveType.WideChar;
                    }
                }
            }

            return false;
        }

        public static bool IsStringArray(this BindingType type)
        {
            if (type is PointerType pointer)
            {
                if (pointer.IsConst && pointer.Pointee is PointerType pointer2)
                {
                    if (pointer2.Pointee.IsPrimitiveTypeEquals(PrimitiveType.SChar) ||
                        pointer2.Pointee.IsPrimitiveTypeEquals(PrimitiveType.Sbyte) ||
                        pointer2.Pointee.IsPrimitiveTypeEquals(PrimitiveType.WideChar))
                    {
                        return true;
                    }
                }
                else if (pointer.Pointee is ArrayType arrayType)
                {
                    if (arrayType.ElementType.IsPrimitiveTypeEquals(PrimitiveType.SChar) ||
                        arrayType.ElementType.IsPrimitiveTypeEquals(PrimitiveType.Sbyte) ||
                        arrayType.ElementType.IsPrimitiveTypeEquals(PrimitiveType.WideChar))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsStringArray(this BindingType type, out bool isUnicode)
        {
            isUnicode = false;
            if (type is PointerType pointer)
            {
                if (pointer.IsConst && pointer.Pointee is PointerType pointer2)
                {
                    if (pointer2.Pointee.IsPrimitiveTypeEquals(PrimitiveType.SChar) ||
                        pointer2.Pointee.IsPrimitiveTypeEquals(PrimitiveType.Sbyte))
                    {
                        return true;
                    }
                    if (pointer2.Pointee.IsPrimitiveTypeEquals(PrimitiveType.WideChar))
                    {
                        isUnicode = true;
                        return true;
                    }
                }
                else if (pointer.Pointee is ArrayType arrayType)
                {
                    if (arrayType.ElementType.IsPrimitiveTypeEquals(PrimitiveType.SChar) ||
                        arrayType.ElementType.IsPrimitiveTypeEquals(PrimitiveType.Sbyte))
                    {
                        return true;
                    }
                    if (arrayType.ElementType.IsPrimitiveTypeEquals(PrimitiveType.WideChar))
                    {
                        isUnicode = true;
                        return true;
                    }
                }
            }

            return false;
        }

        //Only these types can be used as unsafe fixed: bool, byte, short, int, long, char, sbyte, ushort, uint, ulong, float or double 
        public static bool CanConvertToFixedArray(this BindingType type)
        {
            var primitive = type as BuiltinType;
            if (primitive == null)
                return false;

            switch(primitive.Type)
            {
                case PrimitiveType.Bool:
                case PrimitiveType.Bool32:
                case PrimitiveType.Char:
                case PrimitiveType.SChar:
                case PrimitiveType.UChar:
                case PrimitiveType.Byte:
                case PrimitiveType.Sbyte:
                case PrimitiveType.Int16:
                case PrimitiveType.UInt16:
                case PrimitiveType.Int32:
                case PrimitiveType.Int64:
                case PrimitiveType.UInt32:
                case PrimitiveType.UInt64:
                case PrimitiveType.Float:
                case PrimitiveType.Double:
                    return true;

                default:
                    return false;
            }

        }
    }
}