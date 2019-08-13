namespace QuantumBinding.Generator
{
    public enum ParameterKind
    {
        Unknown,
        In,
        Out,
        InOut, // ref
        Readonly // in (analogue of const in C++) => starting from C# 7.2
    }
}