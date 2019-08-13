using System.Collections.Generic;
using QuantumBinding.Generator.AST;

namespace QuantumBinding.Generator.CodeGeneration
{
    public abstract class Generator
    {
        protected Generator(ProcessingContext context)
        {
            Context = context;
        }

        public ProcessingContext Context { get; }

        public abstract string FileExtension { get; }

        public abstract List<CodeGenerator> Generate(IEnumerable<TranslationUnit> units, GeneratorSpecializations specializations);

        public virtual List<GeneratorOutput> GenerateOutputs(Module module)
        {
            var outputs = new List<GeneratorOutput>();

            var units = Context.AstContext.TranslationUnits;

            if (module.GeneratorSpecializations == GeneratorSpecializations.None)
            {
                outputs.Add(GenerateFromModule(module));
            }
            else
            {
                GenerateFromTranslationUnits(outputs, units);
            }

            return outputs;
        }

        private GeneratorOutput GenerateFromModule(Module module)
        {
            var tu = new TranslationUnit(module.OutputFileName, module);
            var output = new GeneratorOutput()
            {
                TranslationUnit = tu,
                Outputs = Generate(module.Units, GeneratorSpecializations.None)
            };

            foreach (var generator in output.Outputs)
            {
                generator.Run();
            }

            return output;
        }

        private void GenerateFromTranslationUnits(List<GeneratorOutput> outputs, List<TranslationUnit> units)
        {
            foreach (var unit in units)
            {
                var generators = Generate(new[] {unit}, unit.Module.GeneratorSpecializations);

                foreach (var codeGenerator in generators)
                {
                    codeGenerator.Run();
                }

                var output = new GeneratorOutput()
                {
                    TranslationUnit = unit,
                    Outputs = generators
                };

                outputs.Add(output);
            }
        }
    }
}