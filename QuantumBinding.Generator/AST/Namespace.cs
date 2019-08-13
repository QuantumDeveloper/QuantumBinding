namespace QuantumBinding.Generator.AST
{
    public class Namespace : DeclarationUnit
    {
        public string FullNamespace => GetNamespace();

        public string NamespaceExtension { get; set; }

        private string GetNamespace()
        {
            if (!string.IsNullOrEmpty(NamespaceExtension))
            {
                return $"{Name}.{NamespaceExtension}";
            }

            return Name;
        }

        public bool IsInline { get; set; }

        public override T Visit<T>(IDeclarationVisitor<T> visitor)
        {
            return visitor.VisitNamespace(this);
        }

        public override object Clone()
        {
            return new Namespace()
            {
                NamespaceExtension = NamespaceExtension,
                IsInline = IsInline,
                IsIgnored = IsIgnored,
                Id = Id,
                Owner = Owner,
                Name = Name
            };
        }
    }
}