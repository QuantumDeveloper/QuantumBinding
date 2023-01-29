
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBToken : QBDisposableObject
{
    public QBToken()
    {
    }

    public QBToken(QuantumBinding.Clang.Interop.CXToken _internal)
    {
        Int_data = NativeUtils.PointerToManagedArray(_internal.int_data, 4);
        Ptr_data = _internal.ptr_data;
    }

    public uint[] Int_data { get; set; }
    public void* Ptr_data { get; set; }
    ///<summary>
    /// Determine the kind of the given token.
    ///</summary>
    public CXTokenKind getTokenKind()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getTokenKind(ToNative());
    }


    public QuantumBinding.Clang.Interop.CXToken ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXToken();
        if(Int_data != null)
        {
            if (Int_data.Length > 4)
                throw new System.ArgumentOutOfRangeException(nameof(Int_data), "Array is out of bounds. Size should not be more than 4");

            NativeUtils.PrimitiveToFixedArray(_internal.int_data, 4, Int_data);
        }
        _internal.ptr_data = Ptr_data;
        return _internal;
    }

    public static implicit operator QBToken(QuantumBinding.Clang.Interop.CXToken q)
    {
        return new QBToken(q);
    }

}



