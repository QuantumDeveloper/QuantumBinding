using System;

namespace QuantumBinding.Generator.CodeGeneration
{
    [Flags]
    public enum GeneratorSpecializations
    {
        None = 0,
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
        Extensions = 2048,
        All = Macros|Enums|Structs|Unions|Classes|Delegates|Functions|StructWrappers|UnionWrappers|StaticMethods|ExtensionMethods|Extensions
    }
}