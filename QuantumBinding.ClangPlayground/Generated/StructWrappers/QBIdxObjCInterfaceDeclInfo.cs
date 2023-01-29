
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBIdxObjCInterfaceDeclInfo : QBDisposableObject
{
    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxObjCContainerDeclInfo> _containerInfo;

    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxBaseClassInfo> _superInfo;

    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxObjCProtocolRefListInfo> _protocols;

    public QBIdxObjCInterfaceDeclInfo()
    {
    }

    public QBIdxObjCInterfaceDeclInfo(QuantumBinding.Clang.Interop.CXIdxObjCInterfaceDeclInfo _internal)
    {
        ContainerInfo = new QBIdxObjCContainerDeclInfo(*_internal.containerInfo);
        NativeUtils.Free(_internal.containerInfo);
        SuperInfo = new QBIdxBaseClassInfo(*_internal.superInfo);
        NativeUtils.Free(_internal.superInfo);
        Protocols = new QBIdxObjCProtocolRefListInfo(*_internal.protocols);
        NativeUtils.Free(_internal.protocols);
    }

    public QBIdxObjCContainerDeclInfo ContainerInfo { get; set; }
    public QBIdxBaseClassInfo SuperInfo { get; set; }
    public QBIdxObjCProtocolRefListInfo Protocols { get; set; }

    public QuantumBinding.Clang.Interop.CXIdxObjCInterfaceDeclInfo ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXIdxObjCInterfaceDeclInfo();
        _containerInfo.Dispose();
        if (ContainerInfo != null)
        {
            var struct0 = ContainerInfo.ToNative();
            _containerInfo = new NativeStruct<QuantumBinding.Clang.Interop.CXIdxObjCContainerDeclInfo>(struct0);
            _internal.containerInfo = _containerInfo.Handle;
        }
        _superInfo.Dispose();
        if (SuperInfo != null)
        {
            var struct1 = SuperInfo.ToNative();
            _superInfo = new NativeStruct<QuantumBinding.Clang.Interop.CXIdxBaseClassInfo>(struct1);
            _internal.superInfo = _superInfo.Handle;
        }
        _protocols.Dispose();
        if (Protocols != null)
        {
            var struct2 = Protocols.ToNative();
            _protocols = new NativeStruct<QuantumBinding.Clang.Interop.CXIdxObjCProtocolRefListInfo>(struct2);
            _internal.protocols = _protocols.Handle;
        }
        return _internal;
    }

    protected override void UnmanagedDisposeOverride()
    {
        _containerInfo.Dispose();
        _superInfo.Dispose();
        _protocols.Dispose();
    }


    public static implicit operator QBIdxObjCInterfaceDeclInfo(QuantumBinding.Clang.Interop.CXIdxObjCInterfaceDeclInfo q)
    {
        return new QBIdxObjCInterfaceDeclInfo(q);
    }

}



