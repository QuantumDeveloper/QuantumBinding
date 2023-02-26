using System.Collections.Generic;

namespace QuantumBinding.Generator.Processors;

public class StructurePredefinedInput
{
    public StructurePredefinedInput()
    {
        FieldValues = new Dictionary<string, PredefinedField>();
    }

    public string StructType { get; set; }

    public Dictionary<string, PredefinedField> FieldValues { get; set; }
}