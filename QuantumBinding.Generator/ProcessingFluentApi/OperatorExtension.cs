namespace QuantumBinding.Generator.ProcessingFluentApi;

public class OperatorExtension
{
    public OperatorKind OperatorKind { get; set; }
    
    public TransformationKind TransformationKind { get; set; }
    
    public string FieldName { get; set; }
    
    public bool PassValueToConstructor { get; set; }
}