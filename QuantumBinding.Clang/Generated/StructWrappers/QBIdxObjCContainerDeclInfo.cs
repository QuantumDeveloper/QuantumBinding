
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBIdxObjCContainerDeclInfo : QBDisposableObject
{
    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxDeclInfo> _declInfo;

    public QBIdxObjCContainerDeclInfo()
    {
    }

    public QBIdxObjCContainerDeclInfo(QuantumBinding.Clang.Interop.CXIdxObjCContainerDeclInfo _internal)
    {
        DeclInfo = new QBIdxDeclInfo(*_internal.declInfo);
        NativeUtils.Free(_internal.declInfo);
        Kind = _internal.kind;
    }

    public QBIdxDeclInfo DeclInfo { get; set; }
    public CXIdxObjCContainerKind Kind { get; set; }

    public QuantumBinding.Clang.Interop.CXIdxObjCContainerDeclInfo ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXIdxObjCContainerDeclInfo();
        _declInfo.Dispose();
        if (DeclInfo != null)
        {
            var struct0 = DeclInfo.ToNative();
            _declInfo = new NativeStruct<QuantumBinding.Clang.Interop.CXIdxDeclInfo>(struct0);
            _internal.declInfo = _declInfo.Handle;
        }
        _internal.kind = Kind;
        return _internal;
    }

    protected override void UnmanagedDisposeOverride()
    {
        _declInfo.Dispose();
    }


    public static implicit operator QBIdxObjCContainerDeclInfo(QuantumBinding.Clang.Interop.CXIdxObjCContainerDeclInfo q)
    {
        return new QBIdxObjCContainerDeclInfo(q);
    }

}



