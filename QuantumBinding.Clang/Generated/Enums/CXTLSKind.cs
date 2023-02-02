
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Describe the "thread-local storage (TLS) kind" of the declaration referred to by a cursor.
///</summary>
[Flags]
public enum CXTLSKind : uint
{
    CXTLS_None = 0,

    CXTLS_Dynamic = 1,

    CXTLS_Static = 2,

}



