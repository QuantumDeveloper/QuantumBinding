using System.Collections.Generic;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.AST
{
    public class Macro : DeclarationUnit
    {
        public Macro()
        {
            Tokens = new List<MacroToken>();
            Parameters = new List<Parameter>();
        }

        public string Value { get; set; }

        public List<MacroToken> Tokens { get; set; }

        public bool IsFunctionLike { get; set; }

        public bool IsBuiltIn { get; set; }

        public BindingType Type { get; set; }

        public List<Parameter> Parameters { get; set; }

        public override T Visit<T>(IDeclarationVisitor<T> visitor)
        {
            return visitor.VisitMacro(this);
        }

        public override object Clone()
        {
            return new Macro()
            {
                Value = Value,
                Tokens = Tokens,
                IsFunctionLike = IsFunctionLike,
                IsBuiltIn = IsBuiltIn,
                Type = (BindingType)Type.Clone(),
                Parameters = new List<Parameter>(Parameters)
            };
        }
    }
}