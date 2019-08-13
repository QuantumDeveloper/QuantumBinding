namespace QuantumBinding.Generator.AST
{
    public class EnumerationItem : Declaration
    {
        public Enumeration Enumeration { get; set; }

        public string Value { get; set; }

        public override string ToString()
        {
            return $"{Name} = {Value}";
        }

        public override T Visit<T>(IDeclarationVisitor<T> visitor)
        {
            return visitor.VisitEnumItem(this);
        }

        public override object Clone()
        {
            return new EnumerationItem()
            {
                Enumeration = Enumeration,
                Value = Value,
                Name = Name,
                Owner = Owner
            };
        }
    }
}