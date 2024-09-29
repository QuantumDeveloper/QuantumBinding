namespace QuantumBinding.Generator.ProcessingFluentApi;

public class EnumExtension
{
    private string enumName;
    
    public EnumExtension(string name)
    {
        enumName = name;
    }
    
    public bool IsFlagsEnum { get; set; }
}