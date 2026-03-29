using System;
using System.Collections.Generic;

namespace QuantumBinding.Utils;
public unsafe struct MarshaledStringArray
{
    private MarshaledString[] values;
    
    public MarshaledStringArray(ReadOnlySpan<string> strings, bool isUnicode)
    {
        IsUnicode = isUnicode;
        if (strings.Length == 0)
        {
            values = null;
        }
        else
        {
            values = new MarshaledString[strings.Length];

            for (var i = 0; i < strings.Length; i++)
            {
                values[i] = new MarshaledString(strings[i], isUnicode);
            }
        }
    }
    public MarshaledStringArray(IReadOnlyList<string> strings, bool isUnicode)
    {
        IsUnicode = isUnicode;
        if (strings.Count == 0)
        {
            values = null;
        }
        else
        {
            values = new MarshaledString[strings.Count];

            for (var i = 0; i < strings.Count; i++)
            {
                values[i] = new MarshaledString(strings[i], isUnicode);
            }
        }
    }

    public bool IsUnicode { get; }

#if NET6_0_OR_GREATER
    public ReadOnlySpan<MarshaledString> Values => values;
#else
    public MarshaledString[] Values => values;
#endif

    public void Dispose()
    {
        if (values != null)
        {
            for (var i = 0; i < values.Length; i++)
            {
                values[i].Dispose();
            }

            values = null;
        }
    }

    public void Fill(sbyte** pDestination)
    {
        if (values != null)
        {
            for (var i = 0; i < values.Length; i++)
            {
                pDestination[i] = Values[i];
            }
        }
    }
    
    public void Fill(char** pDestination)
    {
        if (values != null)
        {
            for (var i = 0; i < values.Length; i++)
            {
                pDestination[i] = Values[i];
            }
        }
    }
}