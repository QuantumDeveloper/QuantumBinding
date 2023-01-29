using System;
using QuantumBinding.Generator.AST;

namespace QuantumBinding.Generator;

public class FixedStructInfo
{
    public FixedStructInfo(String name, Class @class, Field field, int size, int index)
    {
        Name = name;
        Class = @class;
        Field = field;
        Size = size;
        Index = index;
    }
    
    public string Name { get; set; }
        
    public Class Class { get; set; }
        
    public Field Field { get; set; }
        
    public int Size { get; set; }
    
    public int Index { get; set; }
}