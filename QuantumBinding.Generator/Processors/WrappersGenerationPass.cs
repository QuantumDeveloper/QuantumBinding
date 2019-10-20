using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;

namespace QuantumBinding.Generator.Processors
{
    public class WrappersGenerationPass : CodeGeneratorPass
    {
        public WrappersGenerationPass()
        {
            CodeGeneratorPassKind = ExecutionPassKind.PerTranslationUnit;
            GeneratorSpecializations = GeneratorSpecializations.StructWrappers | GeneratorSpecializations.UnionWrappers;
        }

        protected override CodeGenerator OnCreateGenerator(ProcessingContext context, GeneratorSpecializations specializations, params TranslationUnit[] units)
        {
            if (CodeGeneratorPassKind == ExecutionPassKind.PerTranslationUnit)
            {
                OutputPath = units[0].OutputPath;
            }

            return new WrapperGenerator(ProcessingContext, units, specializations);
        }
    }
}
