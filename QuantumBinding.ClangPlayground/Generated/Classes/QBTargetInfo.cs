
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

///<summary>
/// An opaque type representing target information for a given translation unit.
///</summary>
public unsafe partial class QBTargetInfo
{
    internal CXTargetInfoImpl __Instance;
    public QBTargetInfo()
    {
    }

    public QBTargetInfo(QuantumBinding.Clang.Interop.CXTargetInfoImpl __Instance)
    {
        this.__Instance = __Instance;
    }

    ///<summary>
    /// Destroy the CXTargetInfo object.
    ///</summary>
    public void TargetInfo_dispose()
    {
        QuantumBinding.Clang.Interop.ClangInterop.clang_TargetInfo_dispose(this);
    }

    ///<summary>
    /// Get the pointer width of the target in bits.
    ///</summary>
    public int TargetInfo_getPointerWidth()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_TargetInfo_getPointerWidth(this);
    }

    ///<summary>
    /// Get the normalized target triple as a string.
    ///</summary>
    public QBString TargetInfo_getTriple()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_TargetInfo_getTriple(this);
    }

    public ref readonly CXTargetInfoImpl GetPinnableReference() => ref __Instance;

    public static implicit operator QuantumBinding.Clang.Interop.CXTargetInfoImpl(QBTargetInfo q)
    {
        return q?.__Instance ?? new QuantumBinding.Clang.Interop.CXTargetInfoImpl();
    }

    public static implicit operator QBTargetInfo(QuantumBinding.Clang.Interop.CXTargetInfoImpl q)
    {
        return new QBTargetInfo(q);
    }

}



