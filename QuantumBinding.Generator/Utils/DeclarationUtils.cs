using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;

namespace QuantumBinding.Generator.Utils;

public static class DeclarationUtils
{
    public static GeneratorCategory GetCategory(this Declaration decl)
    {
        switch (decl)
        {
            case Enumeration:
                return GeneratorCategory.Enums;
            case Class @class:
                switch (@class.ClassType)
                {
                    case ClassType.Class:
                        return GeneratorCategory.Classes;
                    case ClassType.Struct:
                        return GeneratorCategory.Structs;
                    case ClassType.Union:
                        return GeneratorCategory.Unions;
                    case ClassType.StructWrapper:
                        return GeneratorCategory.StructWrappers;
                    case ClassType.UnionWrapper:
                        return GeneratorCategory.UnionWrappers;
                }
                break;
            case Delegate:
                return GeneratorCategory.Delegates;
            case Function:
                return GeneratorCategory.Functions;
        }

        return GeneratorCategory.Undefined;
    }

    public static bool IsAllowed(this Declaration decl, GeneratorSpecializations spec)
    {
        var specs = spec.GetFlags();
        foreach (var specialization in specs)
        {
            var declSpec = (GeneratorSpecializations)decl.GetCategory();
            if (declSpec == specialization) return true;
        }

        return false;
    }

    public static bool IsInteropType(this Declaration decl)
    {
        switch (decl)
        {
            case Class @class:
                switch (@class.ClassType)
                {
                    case ClassType.Struct:
                    case ClassType.Union:
                        return true;
                }
                break;
            case Delegate:
            case Function:
                return true;
            default:
                return false;
        }

        return false;
    }
}