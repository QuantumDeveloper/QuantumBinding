using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.AST
{
    public class Method : Function
    {
        public Method()
        {
            AccessSpecifier = AccessSpecifier.Public;
        }

        public Function Function { get; set; }

        public bool IsVirtual { get; set; }

        public bool IsStatic { get; set; }

        public bool IsExtensionMethod { get; set; }

        public bool IsExplicit { get; set; }

        public bool IsSealed { get; set; }

        public bool ConvertToProperty { get; set; }

        public static bool ContainsOutParameters(Method method)
        {
            foreach (var parameter in method.Parameters)
            {
                if (parameter.ParameterKind == ParameterKind.Out)
                {
                    return true;
                }
            }

            return false;
        }

        public override T Visit<T>(IDeclarationVisitor<T> visitor)
        {
            return visitor.VisitMethod(this);
        }

        public override object Clone()
        {
            var method = new Method()
            {
                Function = (Function)Function.Clone(),
                IsVirtual = IsVirtual,
                IsSealed = IsSealed,
                IsStatic = IsStatic,
                IsExtensionMethod = IsExtensionMethod,
                IsExplicit = IsExplicit,
                ConvertToProperty = ConvertToProperty,
                Name = Name,
                Id = Id,
                Owner = Owner,
                Class = Class,
                ReturnType = ReturnType,
                Location = Location,
                AlternativeNamespace = AlternativeNamespace,
                IsIgnored = IsIgnored,
            };
            method.Parameters.AddRange(Parameters);
            if (Comment != null)
            {
                method.Comment = (Comment) Comment.Clone();
            }

            return method;
        }
    }
}