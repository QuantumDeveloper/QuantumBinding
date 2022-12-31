using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.AST
{
    public class Parameter : Declaration
    {
        public Parameter()
        {

        }

        public Parameter(Parameter parameter)
        {
            ParameterKind = parameter.ParameterKind;
            Type = parameter.Type.Clone() as BindingType;
            WrappedType = parameter.WrappedType;
            Name = parameter.Name;
            Index = parameter.Index;
            DefaultValue = parameter.DefaultValue;
            Id = parameter.Id;
            Parent = Parent;
            IsOverload = parameter.IsOverload;
        }
        
        public bool IsOverload { get; set; }

        public Declaration Parent { get; set; }

        public ParameterKind ParameterKind { get; set; }

        public BindingType Type { get; set; }

        public BindingType WrappedType { get; set; }

        public uint Index { get; set; }

        public bool HasDefaultValue => !string.IsNullOrEmpty(DefaultValue);

        public string DefaultValue { get; set; }

        public override string ToString()
        {
            return $"{Type} {Name}";
        }

        public override T Visit<T>(IDeclarationVisitor<T> visitor)
        {
            return visitor.VisitParameter(this);
        }

        public override object Clone()
        {
            return new Parameter(this);
        }
    }
}