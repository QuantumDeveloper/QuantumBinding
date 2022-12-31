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
    Functions = 64,
    StructWrappers = 128,
    UnionWrappers = 256,
    StaticMethods = 512,
    ExtensionMethods = 1024,
    Extensions = 2048
}