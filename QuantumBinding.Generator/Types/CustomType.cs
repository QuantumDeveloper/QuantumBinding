using System;
using System.Collections.Generic;

namespace QuantumBinding.Generator.Types
{
    public class CustomType : BindingType, IEquatable<CustomType>
    {
        public CustomType()
        {
        }

        public CustomType(string typeName)
        {
            Name = typeName;
        }

        public CustomType(CustomType type) : base(type)
        {
            Name = type.Name;
            Declaration = type.Declaration;
            IsInSystemHeader = type.IsInSystemHeader;
        }

        public override T Visit<T>(ITypeVisitor<T> typeVisitor)
        {
            return typeVisitor.VisitCustomType(this);
        }

        public override object Clone()
        {
            return new CustomType(this);
        }

        public string Name { get; set; }

        public bool IsInSystemHeader { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            var type = obj as CustomType;
            if (type == null) return false;

            return Name == type.Name && IsInSystemHeader == type.IsInSystemHeader && Qualifiers.Equals(type.Qualifiers);
        }

        public bool Equals(CustomType other)
        {
            return other != null &&
                   Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        public static bool operator ==(CustomType type1, CustomType type2)
        {
            return EqualityComparer<CustomType>.Default.Equals(type1, type2);
        }

        public static bool operator !=(CustomType type1, CustomType type2)
        {
            return !(type1 == type2);
        }
    }
}