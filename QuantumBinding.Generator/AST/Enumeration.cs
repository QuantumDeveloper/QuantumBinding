using System.Collections.Generic;

namespace QuantumBinding.Generator.AST
{
    public class Enumeration : DeclarationUnit
    {
        public Enumeration()
        {
            Items = new List<EnumerationItem>();
            AccessSpecifier = AccessSpecifier.Public;
        }

        public bool IsFlagEnum { get; set; }

        public string InheritanceType { get; set; }

        public AccessSpecifier AccessSpecifier { get; set; }

        public List<EnumerationItem> Items { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override T Visit<T>(IDeclarationVisitor<T> visitor)
        {
            return visitor.VisitEnum(this);
        }

        public override object Clone()
        {
            return new Enumeration()
            {
                IsFlagEnum = IsFlagEnum,
                InheritanceType = InheritanceType,
                AccessSpecifier = AccessSpecifier,
                Items = new List<EnumerationItem>(Items),
                Name = Name,
                Owner = Owner,
                Location = Location,
                Comment = (Comment)Comment.Clone()
            };
        }
    }
}