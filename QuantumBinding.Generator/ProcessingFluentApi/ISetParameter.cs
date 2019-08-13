namespace QuantumBinding.Generator.ProcessingFluentApi
{
    public interface ISetParameter : ITreatFunctionParameterByName
    {
        ISetParameter SetParameterKind(ParameterKind parameterKind);
    }
}