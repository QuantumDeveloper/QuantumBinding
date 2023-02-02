
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

///<summary>
/// A remapping of original source files and their translated files.
///</summary>
public unsafe partial class QBRemapping
{
    internal CXRemappingImpl __Instance;
    public QBRemapping()
    {
    }

    public QBRemapping(QuantumBinding.Clang.Interop.CXRemappingImpl __Instance)
    {
        this.__Instance = __Instance;
    }

    ///<summary>
    /// Dispose the remapping.
    ///</summary>
    public void remap_dispose()
    {
        QuantumBinding.Clang.Interop.ClangInterop.clang_remap_dispose(this);
    }

    ///<summary>
    /// Get the original and the associated filename from the remapping.
    ///</summary>
    public void remap_getFilenames(uint index, out QBString original, out QBString transformed)
    {
        QuantumBinding.Clang.Interop.CXString arg2;
        QuantumBinding.Clang.Interop.CXString arg3;
        QuantumBinding.Clang.Interop.ClangInterop.clang_remap_getFilenames(this, index, out arg2, out arg3);
        original = new QBString(arg2);
        transformed = new QBString(arg3);
    }

    ///<summary>
    /// Determine the number of remappings.
    ///</summary>
    public uint remap_getNumFiles()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_remap_getNumFiles(this);
    }

    public ref readonly CXRemappingImpl GetPinnableReference() => ref __Instance;

    public static implicit operator QuantumBinding.Clang.Interop.CXRemappingImpl(QBRemapping q)
    {
        return q?.__Instance ?? new QuantumBinding.Clang.Interop.CXRemappingImpl();
    }

    public static implicit operator QBRemapping(QuantumBinding.Clang.Interop.CXRemappingImpl q)
    {
        return new QBRemapping(q);
    }

}



