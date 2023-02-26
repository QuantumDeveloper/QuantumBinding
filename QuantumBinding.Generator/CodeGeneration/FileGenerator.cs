using System.Collections.Generic;
using System.Linq;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Utils;

namespace QuantumBinding.Generator.CodeGeneration
{
    public abstract class FileGenerator
    {
        protected FileGenerator(ProcessingContext context)
        {
            Context = context;
        }

        public ProcessingContext Context { get; }

        public abstract string FileExtension { get; }

        protected abstract List<CodeGenerator> Generate(IEnumerable<TranslationUnit> units, GeneratorSpecializations specializations);

        protected abstract CodeGenerator GetGenerator(TranslationUnit unit, GeneratorSpecializations specializations);

        protected abstract CodeGenerator GetGenerator(TranslationUnit unit, GeneratorCategory specialization);

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

        // Simulating code generation path. Using for some extension classes 
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

        // Real generation from sources
        private void GenerateFromTranslationUnits(List<GeneratorOutput> outputs, List<TranslationUnit> units)
        {
            foreach (var unit in units)
            {
                List<CodeGenerator> codeGenerators = null;
                if (unit.Module.EachTypeInSeparateFile)
                {
                    codeGenerators = new List<CodeGenerator>();
                    var specs = unit.Module.GeneratorSpecializations;
                    var types = unit.Declarations.Where(x => x is not Macro && x is not Method && x is not Function && x.IsAllowed(specs));
                    foreach (var type in types)
                    {
                        var category = type.GetCategory();

                        var generator = GetGenerator(unit, category);
                        generator.Run(type);
                        codeGenerators.Add(generator);
                    }

                    var extensions = unit.ExtensionClasses.ToArray();
                    if (extensions.Length > 0)
                    {
                        foreach (var extension in extensions)
                        {
                            var extensionCategory = GeneratorCategory.ExtensionMethods;
                            var generator = GetGenerator(unit, extensionCategory);
                            generator.Run(extension);
                            codeGenerators.Add(generator);
                        }
                    }
                    
                    // Generate macros/constants separately to have all macros in one file
                    var macrosGenerator = GetGenerator(unit, GeneratorSpecializations.Macros);
                    macrosGenerator.Run();
                    codeGenerators.Add(macrosGenerator);
                    
                    var functionsGenerator = GetGenerator(unit, GeneratorSpecializations.Functions);
                    functionsGenerator.Run();
                    codeGenerators.Add(functionsGenerator);
                    
                    var methodsGenerator = GetGenerator(unit, GeneratorSpecializations.StaticMethods);
                    methodsGenerator.Run();
                    codeGenerators.Add(methodsGenerator);
                }
                else
                {
                    codeGenerators = Generate(new[] {unit}, unit.Module.GeneratorSpecializations);
                    foreach (var codeGenerator in codeGenerators)
                    {
                        codeGenerator.Run();
                    }
                }

                var output = new GeneratorOutput()
                {
                    TranslationUnit = unit,
                    Outputs = codeGenerators
                };

                outputs.Add(output);
            }
        }
    }
}