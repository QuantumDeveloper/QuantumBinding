using System;

namespace QuantumBinding.Generator
{
    [Flags]
    public enum RenameTargets
    {
        Enum = 1,
        EnumItem = 2,
        Struct = 4,
        Union = 8,
        StructWrapper = 16,
        UnionWrapper = 32,
        Class = 64,
        Field = 128,
        Property = 256,
        Method = 512,
        Delegate = 1024,
        Function = 2048,
        Parameter = 4096,
        Any = Enum|EnumItem|Struct|StructWrapper|UnionWrapper|Class|Field|Property|Method|Delegate|Function|Parameter
    }
}