using QuantumBinding.Generator.AST;

namespace QuantumBinding.Generator.Processors;

public class DecapitalizePass : RenamePassBase
{
    public DecapitalizePass(RenameTargets targets)
    {
        InitializeVisitOptions(targets);
    }
    
    public override bool VisitMethod(Method method)
    {
        if (IsVisited(method) || !_renameTargets.HasFlag(RenameTargets.Method))
            return false;
        
        method.Name = DecapitalizeString(method.Name);
        return true;
    }

    public override bool VisitFunction(Function function)
    {
        if (IsVisited(function))
            return false;
        
        function.Name = DecapitalizeString(function.Name);
        return true;
    }

    public override bool VisitParameter(Parameter parameter)
    {
        parameter.Name = DecapitalizeString(parameter.Name);
        return true;
    }

    public override bool VisitClass(Class @class)
    {
        if (IsVisited(@class))
            return false;

        @class.Name = DecapitalizeString(@class.Name);
        
        return true;
    }

    public override bool VisitField(Field field)
    {
        if (IsVisited(field))
        {
            return false;
        }

        field.Name = DecapitalizeString(field.Name);

        return true;
    }

    private string DecapitalizeString(string name)
    {
        return name[0].ToString().ToLower() + name.Substring(1);
    }
}