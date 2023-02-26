namespace QuantumBinding.Generator
{
    public enum ParameterKind
    {
        Unknown,
        In,
        Out,
        Ref, // ref
        Readonly // in (analogue of const in C++) => starting from C# 7.2
    }
}