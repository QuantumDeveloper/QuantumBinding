
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

///<summary>
/// The memory usage of a CXTranslationUnit, broken into categories.
///</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXTUResourceUsage
{
    ///<summary>
    /// Private data member, used for queries.
    ///</summary>
    public void* data;
    ///<summary>
    /// The number of entries in the 'entries' array.
    ///</summary>
    public uint numEntries;
    ///<summary>
    /// An array of key-value pairs, representing the breakdown of memory usage.
    ///</summary>
    public CXTUResourceUsageEntry* entries;
}



