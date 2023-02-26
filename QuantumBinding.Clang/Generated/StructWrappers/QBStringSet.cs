// ----------------------------------------------------------------------------------------------
// <auto-generated>
// This file was autogenerated by QuantumBindingGenerator.
// Do not edit this file manually, because you will lose all your changes after next generation.
// </auto-generated>
// ----------------------------------------------------------------------------------------------

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
        Strings = new QBString(*_internal.strings);
        NativeUtils.Free(_internal.strings);
        Count = _internal.count;
    }

    public QBString Strings { get; set; }
    public uint Count { get; set; }
    ///<summary>
    /// Free the given string set.
    ///</summary>
    public void DisposeStringSet()
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
            _internal.strings = _strings.Handle;
        }
        _internal.count = Count;
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



