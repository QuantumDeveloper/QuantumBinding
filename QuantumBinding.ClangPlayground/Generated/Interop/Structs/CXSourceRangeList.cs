
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

///<summary>
/// Identifies an array of ranges.
///</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXSourceRangeList
{
    ///<summary>
    /// The number of ranges in the ranges array.
    ///</summary>
    public uint count;
    ///<summary>
    /// An array of CXSourceRanges.
    ///</summary>
    public CXSourceRange* ranges;
}



