
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBFileUniqueID
{
    public QBFileUniqueID()
    {
    }

    public QBFileUniqueID(QuantumBinding.Clang.Interop.CXFileUniqueID _internal)
    {
        Data = NativeUtils.PointerToManagedArray(_internal.data, 3);
    }

    public ulong[] Data { get; set; }

    public QuantumBinding.Clang.Interop.CXFileUniqueID ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXFileUniqueID();
        if(Data != null)
        {
            if (Data.Length > 3)
                throw new System.ArgumentOutOfRangeException(nameof(Data), "Array is out of bounds. Size should not be more than 3");

            NativeUtils.PrimitiveToFixedArray(_internal.data, 3, Data);
        }
        return _internal;
    }

    public static implicit operator QBFileUniqueID(QuantumBinding.Clang.Interop.CXFileUniqueID q)
    {
        return new QBFileUniqueID(q);
    }

}


