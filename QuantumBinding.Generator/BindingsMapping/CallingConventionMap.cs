using System;
using System.Collections.Generic;
using System.Text;

namespace QuantumBinding.Generator.BindingsMapping
{
    public enum CallingConventionMap
    {
        Undefined = 0,
        Winapi = 1,
        Cdecl = 2,
        StdCall = 3,
        ThisCall = 4,
        FastCall = 5
    }
}
