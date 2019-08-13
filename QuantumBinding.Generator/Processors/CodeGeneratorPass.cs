using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;
using System.Collections.Generic;
using System.Linq;

namespace QuantumBinding.Generator.Processors
{
    public abstract class CodeGeneratorPass : ICodeGenerationPass
    {
        protected CodeGeneratorPass()
        {
        }

        public ExecutionPassKind CodeGeneratorPassKind { get; set; }

        public GeneratorSpecializations GeneratorSpecializations { get; set; }

        public ProcessingContext ProcessingContext { get; set; }

        public ASTContext AstContext => ProcessingContext.AstContext;

        public string OutputFileName { get; set; }

        public string OutputPath { get; set; }

        public string Namespace { get; set; }

        public List<GeneratorOutput> Generate()
        {
            var generatorOutputs = new List<GeneratorOutput>();
            if (CodeGeneratorPassKind == ExecutionPassKind.Once)
            {
                var output = GenerateInternal(AstContext.TranslationUnits.ToArray());
                string name = $"{AstContext.Module.OutputFileName}";
                if (!string.IsNullOrEmpty(Namespace))
                {
                    name = Namespace;
                }

                if (!string.IsNullOrEmpty(OutputFileName))
                {
                    name += $".{ OutputFileName}";
                }

                var unit = new TranslationUnit(OutputFileName, AstContext.Module) { Name = name, OutputPath = OutputPath };
                var generatorOutput = new GeneratorOutput();
                generatorOutput.TranslationUnit = unit;
                generatorOutput.Outputs.AddRange(output);
                generatorOutputs.Add(generatorOutput);
            }
            else
            {
                foreach(var unit in AstContext.TranslationUnits)
                {
                    string name = $"{unit.FullNamespace}";
                    if (!string.IsNullOrEmpty(OutputFileName))
                    {
                        name += $".{ OutputFileName}";
                    }
                    var outputs = GenerateInternal(unit);
                    var tu = new TranslationUnit(OutputFileName, AstContext.Module) { Name = name, OutputPath = unit.OutputPath };
                    var generatorOutput = new GeneratorOutput();
                    generatorOutput.TranslationUnit = tu;
                    generatorOutput.Outputs.AddRange(outputs);
                    generatorOutputs.Add(generatorOutput);
                }
            }
            return generatorOutputs;
        }

        private List<CodeGenerator> GenerateInternal(params TranslationUnit[] translationUnits)
        {
            var outputs = new List<CodeGenerator>();
            if (GeneratorSpecializations == GeneratorSpecializations.None)
            {
                var generator = OnCreateGenerator(ProcessingContext, GeneratorSpecializations,translationUnits.ToArray());
                generator.Run();
                if (!generator.IsGeneratorEmpty)
                {
                    outputs.Add(generator);
                }
            }
            else
            {
                var specs = GeneratorSpecializations.GetFlags();

                foreach (var unit in translationUnits)
                {
                    foreach (var spec in specs)
                    {
                        if (spec == GeneratorSpecializations.All || unit.IsEmpty || !unit.IsSpecializationsAvailable(spec))
                            continue;

                        var generator = OnCreateGenerator(ProcessingContext, spec, unit);
                        generator.Run();
                        if (generator.IsGeneratorEmpty)
                        {
                            continue;
                        }
                        outputs.Add(generator);
                    }
                }
            }

            return outputs;
        }

        protected virtual CodeGenerator OnCreateGenerator(ProcessingContext context, GeneratorSpecializations specializations, params TranslationUnit[] units) { return null; }
    }
}
