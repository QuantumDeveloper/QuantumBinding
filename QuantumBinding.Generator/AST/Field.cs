using System.Collections.Generic;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.AST
{
    public class Field : Declaration
    {
        public Field()
        {
            Attributes = new List<string>();
        }

        public Field(string name) : this()
        {
            Name = name;
        }

        public Class Class { get; set; }

        public BindingType Type { get; set; }

        public int Index { get; set; }

        public List<string> Attributes { get; }

        public AccessSpecifier AccessSpecifier { get; set; }

        public bool ShouldDispose { get; set; }

        public bool IsPointer => Type.IsPointer();

        public bool CanGenerateGetter { get; set; } = true;

        public bool CanGenerateSetter { get; set; } = true;

        public bool HasPredefinedValue => !string.IsNullOrEmpty(PredefinedValue);

        public string PredefinedValue { get; set; }

        public bool IsPredefinedValueReadOnly { get; set; }

        public void AddAttribute(string attr)
        {
            if (!Attributes.Contains(attr))
            {
                Attributes.Add(attr);
            }
        }

        public override string ToString()
        {
            return $"{AccessSpecifier} {Type} {Name}";
        }

        public override T Visit<T>(IDeclarationVisitor<T> visitor)
        {
            return visitor.VisitField(this);
        }

        public override object Clone()
        {
            var field = new Field()
            {
                Class = Class,
                Type = (BindingType)Type.Clone(),
                Index = Index,
                AccessSpecifier = AccessSpecifier,
                ShouldDispose = ShouldDispose,
                CanGenerateGetter = CanGenerateGetter,
                CanGenerateSetter = CanGenerateSetter
            };
            field.Attributes.AddRange(Attributes);

            return field;
        }
    }
}