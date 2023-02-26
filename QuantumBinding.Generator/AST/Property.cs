using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.AST
{
    public class Property : Declaration
    {
        public BindingType Type { get; set; }

        public Method Getter { get; set; }

        public Method Setter { get; set; }

        public bool IsAutoProperty { get; set; }

        public AccessSpecifier AccessSpecifier { get; set; }

        public Field Field { get; set; }

        public Field PairedField { get; set; }

        public override T Visit<T>(IDeclarationVisitor<T> visitor)
        {
            return visitor.VisitProperty(this);
        }

        public override string ToString()
        {
            return $"{AccessSpecifier} {Type} {Name}";
        }

        public override object Clone()
        {
            return new Property()
            {
                Name = Name,
                Id = Id,
                Owner = Owner,
                Type = (BindingType)Type.Clone(),
                Getter = Getter,
                Setter = Setter,
                IsAutoProperty = IsAutoProperty,
                AccessSpecifier = AccessSpecifier,
                Field = (Field)Field.Clone(),
                PairedField = (Field)PairedField.Clone()
            };
        }

        public bool HasSimpleGetterOnly => Setter == null && Getter.AccessSpecifier == AccessSpecifier.Public;
    }
}