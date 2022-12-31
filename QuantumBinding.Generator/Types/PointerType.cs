using System;
using System.Collections.Generic;

namespace QuantumBinding.Generator.Types
{
    public class PointerType : BindingType, IEquatable<PointerType>
    {
        public PointerType()
        {
        }

        public PointerType(PointerType type) : base(type)
        {
            Declaration = type.Declaration;
            Pointee = type.Pointee;
            IsNullable = type.IsNullable;
            //Pointee = Pointee.Clone() as BindingType;
        }

        public BindingType Pointee { get; set; }

        public bool IsNullable { get; set; } //Add "?" to the type

        public bool IsNullableForDelegate { get; set; }

        public override T Visit<T>(ITypeVisitor<T> typeVisitor)
        {
            return typeVisitor.VisitPointerType(this);
        }

        public uint GetDepth()
        {
            uint depth = 1;
            var pointee = Pointee;
            while (pointee is PointerType pointer)
            {
                pointee = pointer.Pointee;
                depth++;
            }

            return depth;
        }

        public override object Clone()
        {
            return new PointerType(this);
        }

        public override bool Equals(object obj)
        {
            var type = obj as PointerType;
            if (type == null) return false;

            return Pointee.Equals(type.Pointee);
        }

        public bool Equals(PointerType other)
        {
            return other != null &&
                   EqualityComparer<BindingType>.Default.Equals(Pointee, other.Pointee) &&
                   IsNullable == other.IsNullable;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Pointee, IsNullable);
        }

        public static bool operator ==(PointerType type1, PointerType type2)
        {
            return EqualityComparer<PointerType>.Default.Equals(type1, type2);
        }

        public static bool operator !=(PointerType type1, PointerType type2)
        {
            return !(type1 == type2);
        }
    }
}