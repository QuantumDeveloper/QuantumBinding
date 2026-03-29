using System.Collections.Generic;
using System.Linq;

namespace QuantumBinding.Generator.AST;

public class Interface : DeclarationUnit
{
    private readonly List<Method> methods;
    private readonly List<Property> properties;

    public Interface()
    {
        methods = new List<Method>();
        properties = new List<Property>();
    }

    public Interface(Interface other)
    {
        methods = new List<Method>(other.methods);
        properties = new List<Property>(other.Properties);
        IsGeneric = other.IsGeneric;
        GenericType = other.GenericType;
    }
    
    public IReadOnlyList<Method> Methods => methods;
    public IReadOnlyList<Property> Properties => properties;
    
    public bool IsGeneric { get; set; }
    
    public string GenericType { get; set; }
    
    public bool AddProperty(Property prop)
    {
        var property = properties.FirstOrDefault(x => x.Name == prop.Name);
        if (property == null)
        {
            properties.Add(prop);
        }

        return property == null;
    }

    public void RemoveProperty(string name)
    {
        var property = properties.FirstOrDefault(x => x.Name == name);
        if (property != null)
        {
            properties.Remove(property);
        }
    }
    
    public bool AddMethod(Method method)
    {
        var m = methods.FirstOrDefault(x => x.Name == method.Name);
        if (m == null)
        {
            methods.Add(method);
        }
        
        return m == null;
    }
    
    public void RemoveMethod(string name)
    {
        var m = methods.FirstOrDefault(x => x.Name == name);
        if (m != null)
        {
            methods.Remove(m);
        }
    }
    
    public override T Visit<T>(IDeclarationVisitor<T> visitor)
    {
        return visitor.VisitInterface(this);
    }

    public override object Clone()
    {
        return new Interface(this);
    }
}