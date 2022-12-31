using System.Collections.Generic;
using System.Linq;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;
using QuantumBinding.Generator.Utils;

namespace QuantumBinding.Generator.Processors
{
    public class WrappersGenerationPass : CodeGeneratorPass
    {
        public WrappersGenerationPass()
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
            var outputs = new List<CodeGenerator>();

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
}
