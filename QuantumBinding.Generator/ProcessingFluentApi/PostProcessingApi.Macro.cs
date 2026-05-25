using System;
using System.Collections.Generic;
using System.Linq;

namespace QuantumBinding.Generator.ProcessingFluentApi;

public partial class PostProcessingApi : IMacroParameters
{
    private readonly Dictionary<string, MacroExtension> _macros = new Dictionary<string, MacroExtension>();
    
    private MacroExtension _currentMacro;

    public IMacroParameters Macro(string macroName)
    {
        if (string.IsNullOrEmpty(macroName))
        {
            throw new ArgumentNullException(nameof(macroName));
        }

        if (!_macros.TryGetValue(macroName, out _currentMacro))
        {
            var macro = new MacroExtension()
            {
                Name = macroName
            };

            _currentMacro = macro;
            _macros.Add(macroName, macro);
        }

        return this;
    }
    
    public bool TryGetMacro(string functionName, bool matchCase, out MacroExtension macro)
    {
        if (matchCase)
        {
            return _macros.TryGetValue(functionName, out macro);
        }

        var key = _macros.Keys.FirstOrDefault(x => x.Equals(functionName, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrEmpty(key))
        {
            return _macros.TryGetValue(key, out macro);
        }

        macro = null;
        return false;
    }
    
    public IMacroParameters SetPrimitiveType(PrimitiveType type)
    {
        _currentMacro.PrimitiveType = type;

        return this;
    }
}