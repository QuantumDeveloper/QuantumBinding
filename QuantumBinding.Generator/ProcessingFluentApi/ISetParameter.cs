namespace QuantumBinding.Generator.ProcessingFluentApi
{
    public interface ISetParameter : IInterpretFunctionParameterByName
    {
        ISetParameter SetParameterKind(ParameterKind parameterKind);
    }
}