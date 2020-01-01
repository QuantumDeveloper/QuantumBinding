using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.AST
{
    public class Operator
    {
        public OperatorKind OperatorKind { get; set; }

        public TransformationKind TransformationKind { get; set; }

        public Class Class { get; set; }

        public BindingType Type { get; set; }

        public string FieldName { get; set; }

        public bool PassValueToConstructor { get; set; }
    }
}