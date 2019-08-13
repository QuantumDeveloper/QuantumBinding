using System.Collections.Generic;
using System.Linq;
using QuantumBinding.Generator.AST;

namespace QuantumBinding.Generator.CodeGeneration
{
    public class CsFilesGenerator : Generator
    {
        public CsFilesGenerator(ProcessingContext context)
            : base(context)
        {
        }

        public override string FileExtension => "cs";

        public override List<CodeGenerator> Generate(
            IEnumerable<TranslationUnit> units,
            GeneratorSpecializations specializations)
        {
            var outputs = new List<CodeGenerator>();

            var translationUnits = units.ToList();

            if (specializations == GeneratorSpecializations.None)
            {
                outputs.Add(new CSharpCodeGenerator(Context, translationUnits.Where(x=>!x.IsEmpty), specializations));
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

                        var generator = new CSharpCodeGenerator(Context, unit, spec);
                        outputs.Add(generator);
                    }
                }
            }

            return outputs;
        }
    }
}