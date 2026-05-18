using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.AST;

public class Parameter : Declaration
{
    public Parameter()
    {

    }

    public Parameter(string name)
    {
        Name = name;
    }

    public Parameter(Parameter copy)
    {
        ParameterKind = copy.ParameterKind;
        Type = copy.Type.Clone() as BindingType;
        WrappedType = copy.WrappedType;
        Name = copy.Name;
        Index = copy.Index;
        DefaultValue = copy.DefaultValue;
        Id = copy.Id;
        Parent = Parent;
        IsOverload = copy.IsOverload;
        IsOptional = copy.IsOptional;
        OriginalParameter = copy.OriginalParameter;
    }
        
    public bool IsOverload { get; set; }
    
    public Parameter OriginalParameter { get; set; }

    public Declaration Parent { get; set; }

    public ParameterKind ParameterKind { get; set; }

    public BindingType Type { get; set; }

    public BindingType WrappedType { get; set; }

    public uint Index { get; set; }

    public bool HasDefaultValue => !string.IsNullOrEmpty(DefaultValue);

    public string DefaultValue { get; set; }
    
    public bool IsOptional { get; set; }

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