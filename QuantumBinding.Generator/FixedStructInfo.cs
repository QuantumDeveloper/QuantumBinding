using System;
using QuantumBinding.Generator.AST;

namespace QuantumBinding.Generator;

public class FixedStructInfo
{
    public FixedStructInfo(String name, Class @class, Field field, int size)
    {
        Name = name;
        Class = @class;
        Field = field;
        Size = size;
    }
    
    public string Name { get; set; }
        
    public Class Class { get; set; }
        
    public Field Field { get; set; }
        
    public int Size { get; set; }
}