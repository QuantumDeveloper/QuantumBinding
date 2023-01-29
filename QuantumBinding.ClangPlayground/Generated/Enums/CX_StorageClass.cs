
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Represents the storage classes as declared in the source. CX_SC_Invalid was added for the case that the passed cursor in not a declaration.
///</summary>
public enum CX_StorageClass : uint
{
    CX_SC_Invalid = 0,

    CX_SC_None = 1,

    CX_SC_Extern = 2,

    CX_SC_Static = 3,

    CX_SC_PrivateExtern = 4,

    CX_SC_OpenCLWorkGroupLocal = 5,

    CX_SC_Auto = 6,

    CX_SC_Register = 7,

}



