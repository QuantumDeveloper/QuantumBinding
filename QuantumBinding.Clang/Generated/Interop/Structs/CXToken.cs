
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

///<summary>
/// Describes a single preprocessing token.
///</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXToken
{
    public unsafe fixed uint int_data[4];
    public void* ptr_data;
}



