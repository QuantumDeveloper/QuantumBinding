namespace QuantumBinding.Generator.Types
{
    public interface ITypeVisitor<out T>
    {
        T VisitArrayType(ArrayType array);
        T VisitPointerType(PointerType pointer);
        T VisitBuiltinType(BuiltinType builtin);
        T VisitCustomType(CustomType customType);
        T VisitDependentNameType(DependentNameType dependentNameType);
    }
}