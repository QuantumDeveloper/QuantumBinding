using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.Utils;

public static class ParameterUtils
{
    public static bool IsAvailableForContextGeneration(this Parameter parameter)
    {
        return (parameter.Type.IsAvailableForMarshalContext() && parameter.ParameterKind != ParameterKind.Out) &&
               (!(parameter.Type.IsPointerToClass(out _) || parameter.Type.IsPointerToArrayOfHandleWrappers()));
    }
}