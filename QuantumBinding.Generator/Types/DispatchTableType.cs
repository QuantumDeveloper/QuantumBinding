namespace QuantumBinding.Generator.Types;

public class DispatchTableType : BindingType
{
    public DispatchTableType()
    {
        
    }

    public DispatchTableType(DispatchTableType copy)
    {
        Declaration = copy.Declaration;
    }
    
    public override T Visit<T>(ITypeVisitor<T> typeVisitor)
    {
        return typeVisitor.VisitDispatchTableType(this);
    }

    public override object Clone()
    {
        return new DispatchTableType(this);
    }
}