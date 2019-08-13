using System.Collections.Generic;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.ProcessingFluentApi
{
    public class FieldExtension
    {
        public FieldExtension()
        {
            Attributes = new List<string>();
        }

        public FieldExtension(string name) : this()
        {
            Name = name;
        }

        public BindingType Type { get; set; }

        public string Name { get; set; }

        public List<string> Attributes { get; set; }

        public bool RemoveExistingAttributes { get; set; }
    }
}