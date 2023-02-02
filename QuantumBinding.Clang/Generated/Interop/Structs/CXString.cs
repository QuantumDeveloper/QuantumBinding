
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

///<summary>
/// A character string.
///</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXString
{
    public void* data;
    public uint private_flags;
}



