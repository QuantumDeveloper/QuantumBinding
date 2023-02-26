using System.Collections.Generic;
using QuantumBinding.Generator.AST;

namespace QuantumBinding.Generator.Processors
{
    public class MacroFunctionsToCSharpFunctionsPass : PreGeneratorPass
    {
        public MacroFunctionsToCSharpFunctionsPass()
        {
            Options.VisitMacros = true;
            IgnoreList = new List<string>();
            SubstitutionList = new Dictionary<string, MacroFunction>();
        }

        public List<string> IgnoreList { get; }

        public Dictionary<string, MacroFunction> SubstitutionList { get; }

        public override bool VisitMacro(Macro macro)
        {
            if (IsVisited(macro))
            {
                return false;
            }

            if (IgnoreList.Contains(macro.Name))
            {
                macro.IsIgnored = true;
                return true;
            }

            if (SubstitutionList.TryGetValue(macro.Name, out var function))
            {
                switch (function.ApplyStrategy)
                {
                    case MacroFunctionStrategy.ApplyOnlyReturnType:
                        macro.Type = function.ReturnType;
                        break;
                    case MacroFunctionStrategy.ApplyOnlyFunctionBody:
                        macro.Value = function.FunctionBody;
                        break;
                    default:
                        macro.Parameters = function.Parameters;
                        macro.Value = function.FunctionBody;
                        macro.Type = function.ReturnType;
                        break;
                }
            }

            return true;
        }
    }
}