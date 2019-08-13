using System.Collections.Generic;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.Processors
{
    public class GenerateFunctionOverloadsPass : PreGeneratorPass
    {
        private List<Function> overloadedFunctions;
        public GenerateFunctionOverloadsPass()
        {
            Options.VisitFunctions = true;
            overloadedFunctions = new List<Function>();
        }

        public override bool VisitFunction(Function function)
        {
            if (IsVisited(function))
            {
                return false;
            }

            List<Parameter> parameters = new List<Parameter>();
            if (function.Name == "vkQueueSubmit")
            {

            }

            foreach (var parameter in function.Parameters)
            {
                if (parameter.Type.IsPointerToArray() && parameter.ParameterKind != ParameterKind.Out)
                {
                    parameters.Add(parameter);
                }
            }

            if (parameters.Count > 0)
            {
                var overloadedFunction = (Function)function.Clone();
                for (var index = 0; index < function.Parameters.Count; index++)
                {
                    var parameter = function.Parameters[index];
                    if (parameters.Contains(parameter))
                    {
                        var parameterClone = (Parameter)parameter.Clone();
                        var typeClone = (PointerType)parameter.Type.Clone();
                        var arrayType = typeClone.Pointee as ArrayType;
                        typeClone.Pointee = arrayType.ElementType;
                        typeClone.IsNullable = true;
                        parameterClone.Type = typeClone;

                        overloadedFunction.Parameters[index] = parameterClone;
                    }
                }
                overloadedFunctions.Add(overloadedFunction);
            }

            return true;
        }

        public override void OnPassCompleted()
        {
            CurrentNamespace.AddDeclarations(overloadedFunctions);
            overloadedFunctions.Clear();
        }
    }
}