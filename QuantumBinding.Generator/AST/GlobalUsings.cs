using System.Collections.Generic;

namespace QuantumBinding.Generator.AST;

public class GlobalUsings : Declaration
{
    private readonly List<GlobalUsingItem> _usings = new List<GlobalUsingItem>();

    public GlobalUsings()
    {
        
    }

    public GlobalUsings(GlobalUsings copy)
    {
        _usings.AddRange(copy._usings);
    }
    
    public void AddUsing(Declaration alias, Declaration source)
    {
        if (source == null)
            return;

        var item = new GlobalUsingItem();
        item.Source = source;
        item.Alias = alias;
        _usings.Add(item);
    }
    
    public IReadOnlyList<GlobalUsingItem> Usings => _usings;
    
    public override T Visit<T>(IDeclarationVisitor<T> visitor)
    {
        return visitor.VisitGlobalUsings(this);
    }

    public override object Clone()
    {
        return new GlobalUsings(this);
    }
}