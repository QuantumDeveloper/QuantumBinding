using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;
using System.Collections.Generic;

namespace QuantumBinding.Generator.Processors
{
    public interface ICodeGenerationPass
    {
        ProcessingContext ProcessingContext { get; set; }

        ASTContext AstContext { get; }

        string OutputFileName { get; set; }

        List<GeneratorOutput> Generate();
    }
}
