using System.Collections.Generic;

namespace QuantumBinding.Generator.AST
{
    public class Constructor
    {
        public Constructor()
        {
            AccessSpecifier = AccessSpecifier.Public;
            InputParameters = new List<Field>();
        }

        public AccessSpecifier AccessSpecifier { get; set; }

        public bool IsDefault { get; set; }

        public bool IsStatic { get; set; }

        public Class Class { get; set; }

        public List<Field> InputParameters { get; }
    }
}