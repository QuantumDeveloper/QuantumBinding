using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;

namespace QuantumBinding.Generator.Processors
{
    internal class UtilsExtensionPass: CodeGeneratorPass
    {
        public UtilsExtensionPass()
        {
            CodeGeneratorPassKind = ExecutionPassKind.Once;
        }

        protected override CodeGenerator OnCreateGenerator(ProcessingContext context, GeneratorSpecializations specializations, params TranslationUnit[] units)
        {
            return new FileExtensionGenerator(ProcessingContext, units, FileExtensionKind.Utils);
        }
    }
}
