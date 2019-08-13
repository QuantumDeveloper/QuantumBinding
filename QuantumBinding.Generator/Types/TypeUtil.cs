using QuantumBinding.Generator.AST;

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
            if (primitive == PrimitiveType.IntPtr
                || primitive == PrimitiveType.UintPtr)
            {
                return true;
            }

            return false;
        }

        public static bool IsPurePointer(this BindingType type)
        {
            return (type.IsPointerToVoid() || type.IsPointerToPointer() || type.IsPointerToIntPtr()) && type.Declaration == null;
        }

        public static bool IsPointerToVoid(this BindingType type)
        {
            type.IsPointerToBuiltInType(out var primitive);
            if (primitive == PrimitiveType.Void)
            {
                return true;
            }

            return false;
        }

        public static bool IsPointerToBuiltInType(this BindingType type, out PrimitiveType primitive)
        {
            var isPrimitive = IsPointerToPrimitiveType(type, out primitive);
            if (isPrimitive &&
                (primitive == PrimitiveType.IntPtr
                || primitive == PrimitiveType.UintPtr
                || primitive == PrimitiveType.Void)
                )
            {
                return false;
            }

            if (CanConvertToString(type))
            {
                return false;
            }
            

            return isPrimitive;
        }

        public static bool IsPointerToPrimitiveType(this BindingType type, out PrimitiveType primitive)
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

        public static bool IsPointerToPointer(this BindingType type)
        {
            var pointer = type as PointerType;
            var pPtr = pointer?.Pointee as PointerType;
            if (pPtr == null)
            {
                return false;
            }

            return true;
        }

        public static bool IsPointerToArray(this BindingType type)
        {
            var pointer = type as PointerType;
            var array = pointer?.Pointee as ArrayType;
            if (array == null)
            {
                return false;
            }

            return true;
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

        public static bool IsPointerToStruct(this BindingType type)
        {
            return IsPointerToStruct(type, out Class @class);
        }

        public static bool IsPointerToStruct(this BindingType type, out Class @class)
        {
            @class = null;
            var p = type as PointerType;

            var decl = p?.Declaration as Class;
            if (decl == null)
            {
                return false;
            }

            if (decl.ClassType != ClassType.Struct)
            {
                return false;
            }

            @class = decl;
            return true;
        }

        public static bool IsPrimitiveType(this BindingType type, out PrimitiveType primitive)
        {
            var builtin = type as BuiltinType;
            if (builtin == null)
            {
                primitive = PrimitiveType.Null;
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

                    if (elementType.Type == PrimitiveType.SChar ||
                        elementType.Type == PrimitiveType.UChar ||
                        elementType.Type == PrimitiveType.WideChar)
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
                    if (builtin.Type == PrimitiveType.SChar || builtin.Type == PrimitiveType.WideChar)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsPrimitiveTypeEquals(this BindingType type, PrimitiveType primitiveType)
        {
            type.IsPrimitiveType(out PrimitiveType t);

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
            if (type.Declaration is Class decl && decl.IsSimpleType)
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
                        pointer2.Pointee.IsPrimitiveTypeEquals(PrimitiveType.WideChar))
                    {
                        {
                            return true;
                        }
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
                    if (pointer2.Pointee.IsPrimitiveTypeEquals(PrimitiveType.SChar))
                    {
                        return true;
                    }
                    else if (pointer2.Pointee.IsPrimitiveTypeEquals(PrimitiveType.WideChar))
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