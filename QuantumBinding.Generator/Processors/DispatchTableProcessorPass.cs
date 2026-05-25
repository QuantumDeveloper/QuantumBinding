using System.Collections.Generic;
using System.Linq;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.Processors;

public class DispatchTableProcessorPass : PreGeneratorPass
{
    public DispatchTableProcessorPass()
    {
        Options.VisitDispatchTables = true;
    }
    
    public override bool VisitDispatchTable(DispatchTable dispatchTable)
    {
        if (IsVisited(dispatchTable))
            return false;

        var handles = dispatchTable.Metadata.Get<List<string>>("Handles");

        foreach (var handle in handles)
        {
            var classHandle = CurrentNamespace.Classes.FirstOrDefault(x=>x.Name == handle);
            foreach (var method in classHandle.Methods)
            {
                if (method.IsOverload)
                    continue;
                
                var function = method.Function;
                
                if (dispatchTable.FunctionsToIgnore.Contains(function.Name))
                    continue;
                
                var field = new Field();
                field.Name = function.Name;
                var returnParameter = new Parameter();
                returnParameter.Type = function.ReturnType;
                field.Type = new DelegateType() { Name = function.Name, Parameters = [..function.Parameters, returnParameter] };
                dispatchTable.AddField(field);
            }
        }

        return true;
    }
}