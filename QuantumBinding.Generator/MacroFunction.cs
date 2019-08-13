using QuantumBinding.Generator.AST;
using System.Collections.Generic;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator
{
    public class MacroFunction
    {
        public MacroFunction()
        {
            Parameters = new List<Parameter>();
        }

        public List<Parameter> Parameters { get; }

        public string FunctionBody { get; set; }

        public BindingType ReturnType { get; set; }

        public bool ApplyOnlyReturnType { get; set; }
    }
}