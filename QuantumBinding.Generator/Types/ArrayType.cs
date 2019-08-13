using System;
using System.Collections.Generic;

namespace QuantumBinding.Generator.Types
{
    public class ArrayType : BindingType, IEquatable<ArrayType>
    {
        public ArrayType()
        {
        }

        public ArrayType(ArrayType type) : base(type)
        {
            ElementType = (BindingType)type.ElementType.Clone();
            SizeType = type.SizeType;
            Size = type.Size;
            ArraySizeSource = type.ArraySizeSource;
        }

        public BindingType ElementType { get; set; }

        public ArraySizeType SizeType { get; set; }

        public long Size { get; set; }

        public long ElementSize { get; set; }

        public bool IsConstArray => SizeType == ArraySizeType.Constant;

        public string ArraySizeSource { get; set; }

        public override bool Equals(object obj)
        {
            var type = obj as ArrayType;
            if (type == null) return false;

            return ElementType.Equals(type) && 
                   SizeType == type.SizeType && 
                   Qualifiers.Equals(type.Qualifiers) &&
                   ArraySizeSource == type.ArraySizeSource &&
                   Size == type.Size;
        }

        public override T Visit<T>(ITypeVisitor<T> typeVisitor)
        {
            return typeVisitor.VisitArrayType(this);
        }

        public override object Clone()
        {
            return new ArrayType(this);
        }

        public bool Equals(ArrayType other)
        {
            return other != null &&
                   EqualityComparer<BindingType>.Default.Equals(ElementType, other.ElementType) &&
                   SizeType == other.SizeType &&
                   Size == other.Size &&
                   ElementSize == other.ElementSize &&
                   ArraySizeSource == other.ArraySizeSource &&
                   IsConstArray == other.IsConstArray;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ElementType, SizeType, Size, ElementSize, IsConstArray);
        }

        public static bool operator ==(ArrayType type1, ArrayType type2)
        {
            return EqualityComparer<ArrayType>.Default.Equals(type1, type2);
        }

        public static bool operator !=(ArrayType type1, ArrayType type2)
        {
            return !(type1 == type2);
        }
    }
}