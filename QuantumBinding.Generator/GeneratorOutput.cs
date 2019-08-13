using System.Collections.Generic;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;

namespace QuantumBinding.Generator
{
    public class GeneratorOutput
    {
        public GeneratorOutput()
        {
            Outputs = new List<CodeGenerator>();
        }

        public TranslationUnit TranslationUnit { get; internal set; }

        public List<CodeGenerator> Outputs { get; internal set; }
    }
}