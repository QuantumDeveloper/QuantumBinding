using QuantumBinding.Generator.AST;

namespace QuantumBinding.Generator.Processors;

public class CapitalizePass : RenamePassBase
{
    public CapitalizePass(RenameTargets targets)
    {
        InitializeVisitOptions(targets);
    }
    
    public override bool VisitFunction(Function function)
    {
        if (IsVisited(function))
            return false;
        
        function.Name = CapitalizeString(function.Name);
        return true;
    }

    public override bool VisitParameter(Parameter parameter)
    {
        parameter.Name = CapitalizeString(parameter.Name);
        return true;
    }

    public override bool VisitMethod(Method method)
    {
        method.Name = CapitalizeString(method.Name);
        return true;
    }

    private string CapitalizeString(string name)
    {
        return name[0].ToString().ToUpper() + name.Substring(1);
    }
}