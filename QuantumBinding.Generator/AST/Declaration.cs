using System;
using QuantumBinding.Generator.Utils;

namespace QuantumBinding.Generator.AST
{
    public abstract class Declaration : ICloneable
    {
        protected Declaration()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Name { get; set; }

        public string FullName => string.IsNullOrEmpty(Namespace) ? Name : $"{Namespace}.{Name}";

        public string OriginalName { get; set; }

        public FileLocation Location { get; set; }

        public TranslationUnit Owner { get; set; }

        public Comment Comment { get; set; }

        public bool IsIgnored { get; set; }

        public string Id { get; init; }
        
        public string Namespace
        {
            get
            {
                if (Owner == null)
                {
                    return string.Empty;
                }
                
                var fullNamespace = Owner.FullNamespace;
                return this.IsInteropType() ? $"{fullNamespace}.{Owner.Module.InteropSubNamespace}" : fullNamespace;
            }
        }

        public abstract T Visit<T>(IDeclarationVisitor<T> visitor);

        public abstract object Clone();
    }
}