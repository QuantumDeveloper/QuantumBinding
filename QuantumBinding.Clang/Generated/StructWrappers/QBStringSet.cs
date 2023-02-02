
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBStringSet : QBDisposableObject
{
    private NativeStruct<QuantumBinding.Clang.Interop.CXString> _strings;

    public QBStringSet()
    {
    }

    public QBStringSet(QuantumBinding.Clang.Interop.CXStringSet _internal)
    {
        Strings = new QBString(*_internal.Strings);
        NativeUtils.Free(_internal.Strings);
        Count = _internal.Count;
    }

    public QBString Strings { get; set; }
    public uint Count { get; set; }
    ///<summary>
    /// Free the given string set.
    ///</summary>
    public void disposeStringSet()
    {
        var arg0 = NativeUtils.StructOrEnumToPointer(ToNative());
        QuantumBinding.Clang.Interop.ClangInterop.clang_disposeStringSet(arg0);
        NativeUtils.Free(arg0);
    }


    public QuantumBinding.Clang.Interop.CXStringSet ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXStringSet();
        _strings.Dispose();
        if (Strings != null)
        {
            var struct0 = Strings.ToNative();
            _strings = new NativeStruct<QuantumBinding.Clang.Interop.CXString>(struct0);
            _internal.Strings = _strings.Handle;
        }
        _internal.Count = Count;
        return _internal;
    }

    protected override void UnmanagedDisposeOverride()
    {
        _strings.Dispose();
    }


    public static implicit operator QBStringSet(QuantumBinding.Clang.Interop.CXStringSet q)
    {
        return new QBStringSet(q);
    }

}



