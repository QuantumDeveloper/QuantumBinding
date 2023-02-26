using System.Collections.Generic;
using QuantumBinding.Generator.AST;

namespace QuantumBinding.Generator.Types;

public class DelegateType : BindingType
{
    public DelegateType()
    {
        Parameters = new List<Parameter>();
    }

    public DelegateType(DelegateType type) : base(type)
    {
        Name = type.Name;
        Parameters = new List<Parameter>(type.Parameters);
    }
    
    public string Name { get; set; }
    
    public List<Parameter> Parameters { get; set; }

    public override T Visit<T>(ITypeVisitor<T> typeVisitor)
    {
        return typeVisitor.VisitDelegateType(this);
    }

    public override object Clone()
    {
        return new DelegateType(this);
    }

    public override string ToString()
    {
        return Name;
    }
}