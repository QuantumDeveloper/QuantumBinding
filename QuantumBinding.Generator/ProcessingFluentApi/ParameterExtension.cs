using System;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.ProcessingFluentApi;

public class ParameterExtension
{
    public string Name { get; set; }

    public ParameterKind ParameterKind { get; set; }

    public BindingType Type { get; set; }
        
    public Type DeclarationType { get; set; }

    public string DefaultValue { get; set; }

    public bool HasDefaultValue => !string.IsNullOrEmpty(DefaultValue);
        
    public bool ReplaceDeclaration { get; set; }
}