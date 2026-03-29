using System.Collections.Generic;
using System.Linq;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Utils;

namespace QuantumBinding.Generator.Processors;

public class ContextGenerationAnalyzerPass : PreGeneratorPass
{
    private HashSet<string> _existingMarshalStructNames = new HashSet<string>();
    
    public ContextGenerationAnalyzerPass()
    {
        Options.VisitClasses = true;
        Options.VisitMethods = true;
    }
    
    public override bool VisitMethod(Method method)
    {
        if (IsVisited(method))
        {
            return false;
        }

        var shouldCreateMarshalContextStruct = method.Parameters.Where(x=>x.IsAvailableForContextGeneration());
        method.GenerateMarshalContext = shouldCreateMarshalContextStruct.Any();
        
        if (!method.GenerateMarshalContext) return true;

        var name = $"{method.Name}MarshalContext";
        int i = 0;
        do
        {
            if (i > 0)
            {
                name = $"{method.Name}{i}MarshalContext";
            }

            i++;
        }
        while (!_existingMarshalStructNames.Add(name));
        
        method.MarshalContextName = name;

        return true;
    }
}