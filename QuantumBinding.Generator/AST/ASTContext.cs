using System.Collections.Generic;

namespace QuantumBinding.Generator.AST
{
    public class ASTContext
    {
        public ASTContext(IEnumerable<TranslationUnit> units)
        {
            TranslationUnits = new List<TranslationUnit>(units);
            GeneratorOutputs = new List<GeneratorOutput>();
        }

        public List<TranslationUnit> TranslationUnits { get; }

        public List<GeneratorOutput> GeneratorOutputs { get; }

        public Module Module { get; set; }
    }
}