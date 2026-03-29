using System.Collections.Generic;

namespace QuantumBinding.Generator.ProcessingFluentApi;

public class ConstructorExtension
{
    public ConstructorExtension()
    {
        Parameters = new List<ParameterExtension>();
    }
    
    public bool IsDefault { get; set; }
    
    public List<ParameterExtension> Parameters { get; set; }
}