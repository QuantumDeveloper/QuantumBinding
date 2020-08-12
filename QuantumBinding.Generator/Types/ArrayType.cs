using System;
using System.Collections.Generic;

namespace QuantumBinding.Generator.Types
{
    public class ArrayType : BindingType, IEquatable<ArrayType>
    {
        public ArrayType()
        {
            Sizes = new List<long>();
            DimensionsCount = 1;
        }

        public ArrayType(ArrayType type) : base(type)
        {
            Declaration = type.Declaration;
            ElementType = (BindingType)type.ElementType.Clone();
            SizeType = type.SizeType;
            Size = type.Size;
            ArraySizeSource = type.ArraySizeSource;
            DimensionsCount = type.DimensionsCount;
            Sizes = type.Sizes;
        }

        public BindingType ElementType { get; set; }

        public uint DimensionsCount { get; set; }

        public bool IsMultiDimensional => DimensionsCount > 1;

        public ArraySizeType SizeType { get; set; }

        public long Size { get; set; }

        public long ElementSize { get; set; }

        public bool IsConstArray => SizeType == ArraySizeType.Constant;

        public string ArraySizeSource { get; set; }

        public List<long> Sizes { get; private set; }

        public override bool Equals(object obj)
        {
            var type = obj as ArrayType;
            if (type == null) return false;

            return Equals(type);
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
                   IsConstArray == other.IsConstArray &&
                   DimensionsCount == other.DimensionsCount;
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