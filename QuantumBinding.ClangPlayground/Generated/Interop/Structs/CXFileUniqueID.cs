
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

///<summary>
/// Uniquely identifies a CXFile, that refers to the same underlying file, across an indexing session.
///</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXFileUniqueID
{
    public unsafe fixed ulong data[3];
}



