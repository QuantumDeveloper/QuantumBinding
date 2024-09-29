using System;
using System.Collections.Generic;
using System.Linq;

namespace QuantumBinding.Generator.ProcessingFluentApi;

public partial class PostProcessingApi : IEnumParameters
{
    private Dictionary<string, EnumExtension> enums;
    private EnumExtension _currentEnum;
    
    public IEnumParameters Enum(string enumName)
    {
        if (!enums.TryGetValue(enumName, out _currentEnum))
        {
            var @enum = new EnumExtension(enumName);

            _currentEnum = @enum;
            enums.Add(enumName, @enum);
        }

        lastOperation = LastOperation.AddEnum;
        return this;
    }
    
    IEnumParameters IEnumParameters.SetIsFlagsEnum(bool value)
    {
        _currentEnum.IsFlagsEnum = value;
        return this;
    }
    
    public bool TryGetEnum(string enumName, bool matchCase, out EnumExtension @enum)
    {
        if (matchCase)
        {
            return enums.TryGetValue(enumName, out @enum);
        }

        var key = enums.Keys.FirstOrDefault(x => x.Equals(enumName, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrEmpty(key))
        {
            return enums.TryGetValue(key, out @enum);
        }

        @enum = null;
        return false;
    }
}