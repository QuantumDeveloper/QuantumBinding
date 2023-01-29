
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Clang;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXTUResourceUsageEntry
{
    ///<summary>
    /// The memory usage category.
    ///</summary>
    public CXTUResourceUsageKind kind;
    ///<summary>
    /// Amount of resources used. The units will depend on the resource kind.
    ///</summary>
    public uint amount;
}



