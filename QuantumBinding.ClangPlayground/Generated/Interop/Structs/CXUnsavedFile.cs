
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

///<summary>
/// Provides the contents of a file that has not yet been saved to disk.
///</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXUnsavedFile
{
    ///<summary>
    /// The file whose contents have not yet been saved.
    ///</summary>
    public sbyte* Filename;
    ///<summary>
    /// A buffer containing the unsaved contents of this file.
    ///</summary>
    public sbyte* Contents;
    ///<summary>
    /// The length of the unsaved contents of this buffer.
    ///</summary>
    public uint Length;
}



