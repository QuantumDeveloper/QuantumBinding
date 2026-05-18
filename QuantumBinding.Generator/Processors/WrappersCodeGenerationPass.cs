using System.Collections.Generic;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;

namespace QuantumBinding.Generator.Processors;

public class WrappersCodeGenerationPass : CodeGeneratorPass
{
    public WrappersCodeGenerationPass()
    {
        CodeGeneratorPassKind = ExecutionPassKind.PerTranslationUnit;
        GeneratorSpecializations = GeneratorSpecializations.StructWrappers | GeneratorSpecializations.UnionWrappers;
    }

    protected override CodeGenerator OnCreateGenerator(GeneratorCategory category, params TranslationUnit[] units)
    {
        if (CodeGeneratorPassKind == ExecutionPassKind.PerTranslationUnit)
        {
            OutputPath = units[0].OutputPath;
        }

        return new WrapperGenerator(ProcessingContext, units, category);
    }

    protected override List<CodeGenerator> ProcessPerTypeCodeGeneration(TranslationUnit unit, GeneratorSpecializations specs)
    {
        switch (specs)
        {
            case GeneratorSpecializations.StructWrappers:
                return ProcessDeclarations(unit.StructWrappers, unit);
            case GeneratorSpecializations.UnionWrappers:
                return ProcessDeclarations(unit.UnionWrappers, unit);
            default:
                return new List<CodeGenerator>();
        }
    }
}