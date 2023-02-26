namespace QuantumBinding.Generator.CodeGeneration;

public enum GeneratorCategory
{
    Undefined = 0,
    Macros = 1,
    Enums = 2,
    Structs = 4,
    Unions = 8,
    Classes = 16,
    Delegates = 32,
    OldFashionDelegates = 64,
    Functions = 128,
    StructWrappers = 256,
    UnionWrappers = 512,
    StaticMethods = 1024,
    ExtensionMethods = 2048,
    Extensions = 4096
}