using System.Collections.Generic;

namespace QuantumBinding.Generator.AST;

public class Metadata
{
    private readonly Dictionary<string, object> _props = new();

    public void Set<T>(string key, T value)
    {
        _props[key] = value;
    }

    public T Get<T>(string key, T defaultValue = default)
    {
        if (_props.TryGetValue(key, out var props))
        {
            return (T)props;
        }
        return defaultValue;
    }
}