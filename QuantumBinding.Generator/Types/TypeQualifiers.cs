using System;

namespace QuantumBinding.Generator.Types
{
    public struct TypeQualifiers : IEquatable<TypeQualifiers>
    {
        public bool IsConst;
        public bool IsVolatile;
        public bool IsRestrict;

        public bool Equals(TypeQualifiers other)
        {
            return IsConst == other.IsConst && IsVolatile == other.IsVolatile && IsRestrict == other.IsRestrict;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is TypeQualifiers other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = IsConst.GetHashCode();
                hashCode = (hashCode * 397) ^ IsVolatile.GetHashCode();
                hashCode = (hashCode * 397) ^ IsRestrict.GetHashCode();
                return hashCode;
            }
        }
    }
}