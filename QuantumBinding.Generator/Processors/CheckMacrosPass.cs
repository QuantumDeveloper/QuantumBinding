using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Utils;

namespace QuantumBinding.Generator.Processors
{
    public class CheckMacrosPass : PreGeneratorPass
    {
        public CheckMacrosPass()
        {
            Options.VisitMacros = true;
        }

        public override bool VisitMacro(Macro macro)
        {
            if (IsVisited(macro))
            {
                return false;
            }

            if (macro.Value.Contains("ULL"))
            {
                macro.Value = macro.Value.Replace("ULL", "UL");
            }

            if (macro.Value.StartsWith("(") && macro.Value.EndsWith(")"))
            {
                macro.Value = macro.Value[1..^1];
            }

            macro.Type = ClangUtils.GetMacroType(macro.Value);

            return true;
        }
    }
}