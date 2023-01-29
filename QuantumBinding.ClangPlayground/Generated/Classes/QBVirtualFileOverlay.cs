
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

///<summary>
/// Object encapsulating information about overlaying virtual file/directories over the real file system.
///</summary>
public unsafe partial class QBVirtualFileOverlay
{
    internal CXVirtualFileOverlayImpl __Instance;
    public QBVirtualFileOverlay()
    {
    }

    public QBVirtualFileOverlay(QuantumBinding.Clang.Interop.CXVirtualFileOverlayImpl __Instance)
    {
        this.__Instance = __Instance;
    }

    ///<summary>
    /// Map an absolute virtual file path to an absolute real one. The virtual path must be canonicalized (not contain "."/"..").
    ///</summary>
    public CXErrorCode VirtualFileOverlay_addFileMapping(string virtualPath, string realPath)
    {
        var arg1 = (sbyte*)NativeUtils.StringToPointer(virtualPath, false);
        var arg2 = (sbyte*)NativeUtils.StringToPointer(realPath, false);
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_VirtualFileOverlay_addFileMapping(this, arg1, arg2);
        NativeUtils.Free(arg1);
        NativeUtils.Free(arg2);
        return result;
    }

    ///<summary>
    /// Dispose a CXVirtualFileOverlay object.
    ///</summary>
    public void VirtualFileOverlay_dispose()
    {
        QuantumBinding.Clang.Interop.ClangInterop.clang_VirtualFileOverlay_dispose(this);
    }

    ///<summary>
    /// Set the case sensitivity for the CXVirtualFileOverlay object. The CXVirtualFileOverlay object is case-sensitive by default, this option can be used to override the default.
    ///</summary>
    public CXErrorCode VirtualFileOverlay_setCaseSensitivity(int caseSensitive)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_VirtualFileOverlay_setCaseSensitivity(this, caseSensitive);
    }

    ///<summary>
    /// Write out the CXVirtualFileOverlay object to a char buffer.
    ///</summary>
    public CXErrorCode VirtualFileOverlay_writeToBuffer(uint options, out sbyte* out_buffer_ptr, out uint out_buffer_size)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_VirtualFileOverlay_writeToBuffer(this, options, out out_buffer_ptr, out out_buffer_size);
    }

    public ref readonly CXVirtualFileOverlayImpl GetPinnableReference() => ref __Instance;

    public static implicit operator QuantumBinding.Clang.Interop.CXVirtualFileOverlayImpl(QBVirtualFileOverlay q)
    {
        return q?.__Instance ?? new QuantumBinding.Clang.Interop.CXVirtualFileOverlayImpl();
    }

    public static implicit operator QBVirtualFileOverlay(QuantumBinding.Clang.Interop.CXVirtualFileOverlayImpl q)
    {
        return new QBVirtualFileOverlay(q);
    }

}



