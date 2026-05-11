using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.AST;

public class Delegate : DeclarationUnit
{
    public Delegate()
    {
        AccessSpecifier = AccessSpecifier.Public;
        Parameters = new List<Parameter>();
    }

    public CallingConvention CallingConvention { get; set; }

    public AccessSpecifier AccessSpecifier { get; set; }

    public BindingType ReturnType { get; set; }

    public List<Parameter> Parameters { get; }

    public Class Class { get; set; }

    public override string ToString()
    {
        return $"{AccessSpecifier} {ReturnType} {Name}";
    }
    
    public void AddParameter(Parameter parameter)
    {
        var lastIndex = Parameters.Count == 0 ? 0: Parameters.Last().Index;
        if (Parameters.Count > 0)
        {
            lastIndex++;
        }
        parameter.Index = lastIndex;
        Parameters.Add(parameter);
    }

    public override T Visit<T>(IDeclarationVisitor<T> visitor)
    {
        return visitor.VisitDelegate(this);
    }

    public override object Clone()
    {
        return new Delegate()
        {
            AccessSpecifier = AccessSpecifier,
            CallingConvention = CallingConvention,
            ReturnType = ReturnType,
            Class = Class,
            Name = Name,
            Location = Location,
            Owner = Owner,
            Comment = (Comment)Comment?.Clone(),
            IsIgnored = IsIgnored,
            Id = Id,
        };
    }
}