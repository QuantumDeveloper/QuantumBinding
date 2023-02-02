
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBString : QBDisposableObject
{
    public QBString()
    {
    }

    public QBString(QuantumBinding.Clang.Interop.CXString _internal)
    {
        Data = _internal.data;
        Private_flags = _internal.private_flags;
    }

    public void* Data { get; set; }
    public uint Private_flags { get; set; }
    ///<summary>
    /// Free the given string.
    ///</summary>
    public void disposeString()
    {
        QuantumBinding.Clang.Interop.ClangInterop.clang_disposeString(ToNative());
    }

    ///<summary>
    /// Retrieve the character data associated with the given string.
    ///</summary>
    public string getCString()
    {
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getCString(ToNative());
        return new string(result);
    }


    public QuantumBinding.Clang.Interop.CXString ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXString();
        _internal.data = Data;
        _internal.private_flags = Private_flags;
        return _internal;
    }

    public static implicit operator QBString(QuantumBinding.Clang.Interop.CXString q)
    {
        return new QBString(q);
    }

}



