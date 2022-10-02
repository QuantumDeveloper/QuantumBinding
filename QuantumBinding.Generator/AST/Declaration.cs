using System;

namespace QuantumBinding.Generator.AST
{
    public abstract class Declaration : ICloneable
    {
        protected Declaration()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Name { get; set; }
        
        public string OriginalName { get; set; }

        public FileLocation Location { get; set; }

        public TranslationUnit Owner { get; set; }

        public string AlternativeNamespace { get; set; }

        public Comment Comment { get; set; }

        public bool IsIgnored { get; set; }

        public string Id { get; set; }

        public abstract T Visit<T>(IDeclarationVisitor<T> visitor);

        public abstract object Clone();
    }
}