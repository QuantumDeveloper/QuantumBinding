using System;
using System.Collections.Generic;
using System.Linq;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Utils;

namespace QuantumBinding.Generator.CodeGeneration
{
    public class CsFilesFileGenerator : FileGenerator
    {
        public CsFilesFileGenerator(ProcessingContext context)
            : base(context)
        {
        }

        public override string FileExtension => "cs";

        protected override List<CodeGenerator> Generate(
            IEnumerable<TranslationUnit> units,
            GeneratorSpecializations specializations)
        {
            var codeGenerators = new List<CodeGenerator>();

            var translationUnits = units.ToList();

            if (specializations == GeneratorSpecializations.None)
            {
                codeGenerators.Add(new CSharpCodeGenerator(Context, translationUnits.Where(x=>!x.IsEmpty), GeneratorCategory.Undefined));
            }
            else
            {
                var specs = specializations.GetFlags();

                foreach (var unit in translationUnits)
                {
                    foreach (var spec in specs)
                    {
                        if (spec == GeneratorSpecializations.All || unit.IsEmpty || !unit.IsSpecializationsAvailable(spec))
                            continue;

                        var generator = GetGenerator(unit, spec);
                        codeGenerators.Add(generator);
                    }
                }
            }

            return codeGenerators;
        }

        protected override CodeGenerator GetGenerator(TranslationUnit unit, GeneratorSpecializations specialization)
        {
            return specialization switch
            {
                GeneratorSpecializations.StructWrappers or GeneratorSpecializations.UnionWrappers =>
                    new WrapperGenerator(Context, unit, (GeneratorCategory)specialization),
                _ => new CSharpCodeGenerator(Context, unit, (GeneratorCategory)specialization)
            };
        }
        
        protected override CodeGenerator GetGenerator(TranslationUnit unit, GeneratorCategory specialization)
        {
            return specialization switch
            {
                GeneratorCategory.StructWrappers or GeneratorCategory.UnionWrappers =>
                    new WrapperGenerator(Context, unit, specialization),
                _ => new CSharpCodeGenerator(Context, unit, specialization)
            };
        }
    }
}