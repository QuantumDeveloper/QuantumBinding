using System;
using System.Collections.Generic;

namespace QuantumBinding.Generator.Types
{
    public class DependentNameType : BindingType, IEquatable<DependentNameType>
    {
        public DependentNameType()
        {
        }

        public DependentNameType(string identifier, string pointsTo)
        {
            Identifier = identifier;
            PointsTo = pointsTo;
        }

        public DependentNameType(DependentNameType type)
        {
            Identifier = type.Identifier;
        }

        public string Identifier { get; set; }

        public string PointsTo { get; set; }

        public override T Visit<T>(ITypeVisitor<T> typeVisitor)
        {
            return typeVisitor.VisitDependentNameType(this);
        }

        public override string ToString()
        {
            return Identifier;
        }

        public override object Clone()
        {
            return new DependentNameType(this);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DependentNameType);
        }

        public bool Equals(DependentNameType other)
        {
            return other != null &&
                   Identifier == other.Identifier &&
                   PointsTo == other.PointsTo;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Identifier, PointsTo);
        }

        public static bool operator ==(DependentNameType type1, DependentNameType type2)
        {
            return EqualityComparer<DependentNameType>.Default.Equals(type1, type2);
        }

        public static bool operator !=(DependentNameType type1, DependentNameType type2)
        {
            return !(type1 == type2);
        }
    }
}