using System;
using System.Collections.Generic;

namespace QuantumBinding.Generator.Types
{
    public class BuiltinType : BindingType, IEquatable<BuiltinType>
    {
        public BuiltinType()
        {
        }

        public BuiltinType(PrimitiveType primitiveType)
        {
            Type = primitiveType;
        }

        public BuiltinType(BuiltinType type) : base(type)
        {
            Type = type.Type;
        }

        public PrimitiveType Type { get; set; }

        public override bool IsPrimitiveType => Type != PrimitiveType.None && Type != PrimitiveType.Unknown;

        public override T Visit<T>(ITypeVisitor<T> typeVisitor)
        {
            return typeVisitor.VisitBuiltinType(this);
        }

        public override string ToString()
        {
            return Type.ToString();
        }

        public override object Clone()
        {
            return new BuiltinType(this);
        }

        public override bool Equals(object obj)
        {
            var type = obj as BuiltinType;
            if (type == null) return false;

            return Type == type.Type;
        }

        public bool Equals(BuiltinType other)
        {
            return other != null &&
                   Type == other.Type &&
                   IsPrimitiveType == other.IsPrimitiveType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, IsPrimitiveType);
        }

        public static bool operator ==(BuiltinType type1, BuiltinType type2)
        {
            return EqualityComparer<BuiltinType>.Default.Equals(type1, type2);
        }

        public static bool operator !=(BuiltinType type1, BuiltinType type2)
        {
            return !(type1 == type2);
        }
    }
}