namespace QuantumBinding.Generator.AST;

public class GlobalUsingItem : Declaration
{
    public Declaration Source { get; set; }
    public Declaration Alias { get; set; }

    public GlobalUsingItem()
    {
        
    }

    public GlobalUsingItem(GlobalUsingItem copy)
    {
        Source = copy.Source;
        Alias = copy.Alias;
    }
    
    public override T Visit<T>(IDeclarationVisitor<T> visitor)
    {
        return visitor.VisitGlobalUsingItem(this);
    }

    public override object Clone()
    {
        return new GlobalUsingItem(this);
    }
}