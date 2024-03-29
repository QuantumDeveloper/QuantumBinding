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

public unsafe partial class QBIdxObjCPropertyDeclInfo : QBDisposableObject
{
    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxDeclInfo> _declInfo;

    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxEntityInfo> _getter;

    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxEntityInfo> _setter;

    public QBIdxObjCPropertyDeclInfo()
    {
    }

    public QBIdxObjCPropertyDeclInfo(QuantumBinding.Clang.Interop.CXIdxObjCPropertyDeclInfo _internal)
    {
        DeclInfo = new QBIdxDeclInfo(*_internal.declInfo);
        NativeUtils.Free(_internal.declInfo);
        Getter = new QBIdxEntityInfo(*_internal.getter);
        NativeUtils.Free(_internal.getter);
        Setter = new QBIdxEntityInfo(*_internal.setter);
        NativeUtils.Free(_internal.setter);
    }

    public QBIdxDeclInfo DeclInfo { get; set; }
    public QBIdxEntityInfo Getter { get; set; }
    public QBIdxEntityInfo Setter { get; set; }

    public QuantumBinding.Clang.Interop.CXIdxObjCPropertyDeclInfo ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXIdxObjCPropertyDeclInfo();
        _declInfo.Dispose();
        if (DeclInfo != null)
        {
            var struct0 = DeclInfo.ToNative();
            _declInfo = new NativeStruct<QuantumBinding.Clang.Interop.CXIdxDeclInfo>(struct0);
            _internal.declInfo = _declInfo.Handle;
        }
        _getter.Dispose();
        if (Getter != null)
        {
            var struct1 = Getter.ToNative();
            _getter = new NativeStruct<QuantumBinding.Clang.Interop.CXIdxEntityInfo>(struct1);
            _internal.getter = _getter.Handle;
        }
        _setter.Dispose();
        if (Setter != null)
        {
            var struct2 = Setter.ToNative();
            _setter = new NativeStruct<QuantumBinding.Clang.Interop.CXIdxEntityInfo>(struct2);
            _internal.setter = _setter.Handle;
        }
        return _internal;
    }

    protected override void UnmanagedDisposeOverride()
    {
        _declInfo.Dispose();
        _getter.Dispose();
        _setter.Dispose();
    }


    public static implicit operator QBIdxObjCPropertyDeclInfo(QuantumBinding.Clang.Interop.CXIdxObjCPropertyDeclInfo q)
    {
        return new QBIdxObjCPropertyDeclInfo(q);
    }

}



