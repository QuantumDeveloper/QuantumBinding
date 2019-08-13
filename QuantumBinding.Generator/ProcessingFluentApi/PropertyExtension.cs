using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.ProcessingFluentApi
{
    public class PropertyExtension
    {
        public PropertyExtension()
        {

        }

        public PropertyExtension(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public FieldExtension Field { get; set; }

        public BindingType Type { get; set; }

        public Method Getter { get; set; }

        public Method Setter { get; set; }

        public bool IsAutoProperty { get; set; }
    }
}