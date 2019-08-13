namespace QuantumBinding.Generator.AST
{
    public class Operator
    {
        public OperatorKind OperatorKind { get; set; }

        public TransformationKind TransformationKind { get; set; }

        public Class Class { get; set; }

        public Field Field { get; set; }

        public bool PassValueToConstructor { get; set; }
    }
}