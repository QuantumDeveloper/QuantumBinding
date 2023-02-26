namespace QuantumBinding.Generator.Processors;

public abstract class RenamePassBase : PreGeneratorPass
{
    protected RenameTargets _renameTargets;
    
    protected void InitializeVisitOptions(RenameTargets targets)
    {
        if (targets != RenameTargets.Any)
        {
            if (targets.HasFlag(RenameTargets.Enum))
            {
                Options.VisitEnums = true;
            }

            if (targets.HasFlag(RenameTargets.EnumItem))
            {
                Options.VisitEnumItems = true;
            }

            if (targets.HasFlag(RenameTargets.Class)
                || targets.HasFlag(RenameTargets.Struct)
                || targets.HasFlag(RenameTargets.StructWrapper))
            {
                Options.VisitClasses = true;
            }

            if (targets.HasFlag(RenameTargets.Field))
            {
                Options.VisitFields = true;
            }

            if (targets.HasFlag(RenameTargets.Property))
            {
                Options.VisitProperties = true;
            }

            if (targets.HasFlag(RenameTargets.Method))
            {
                Options.VisitMethods = true;
            }

            if (targets.HasFlag(RenameTargets.Function))
            {
                Options.VisitFunctions = true;
            }

            if (targets.HasFlag(RenameTargets.Delegate))
            {
                Options.VisitDelegates = true;
            }

            if (targets.HasFlag(RenameTargets.Parameter))
            {
                Options.VisitParameters = true;
            }
        }
        else
        {
            Options.VisitEnums = true;
            Options.VisitClasses = true;
            Options.VisitFields = true;
            Options.VisitProperties = true;
            Options.VisitMethods = true;
            Options.VisitFunctions = true;
            Options.VisitDelegates = true;
            Options.VisitParameters = true;
        }

        _renameTargets = targets;
    }
}