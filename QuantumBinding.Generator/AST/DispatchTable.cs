using System;
using System.Collections.Generic;

namespace QuantumBinding.Generator.AST;

public class DispatchTable : Declaration
{
    private readonly List<Field> _fields = new List<Field>();
    private readonly List<string> _functionsToIgnore = new List<string>();
    
    public DispatchTable()
    {
    }

    public DispatchTable(DispatchTable copy)
    {
        _fields = new List<Field>(copy._fields);
        TableOwner = copy.TableOwner;
        Metadata = copy.Metadata;
    }
    
    public Class TableOwner { get; set; }
    
    public string FunctionName { get; set; }
    
    public IReadOnlyList<String> FunctionsToIgnore => _functionsToIgnore;
    
    public void AddField(Field field)
    {
        _fields.Add(field);
    }
    
    public void IgnoreFunction(string functionName)
    {
        _functionsToIgnore.Add(functionName);
    }
    
    public IReadOnlyList<Field> Fields => _fields;

    public Metadata Metadata { get; } = new Metadata();
    
    public override T Visit<T>(IDeclarationVisitor<T> visitor)
    {
        return visitor.VisitDispatchTable(this);
    }

    public override object Clone()
    {
        return new DispatchTable(this);
    }
}