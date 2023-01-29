
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBIdxObjCCategoryDeclInfo : QBDisposableObject
{
    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxObjCContainerDeclInfo> _containerInfo;

    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxEntityInfo> _objcClass;

    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxObjCProtocolRefListInfo> _protocols;

    public QBIdxObjCCategoryDeclInfo()
    {
    }

    public QBIdxObjCCategoryDeclInfo(QuantumBinding.Clang.Interop.CXIdxObjCCategoryDeclInfo _internal)
    {
        ContainerInfo = new QBIdxObjCContainerDeclInfo(*_internal.containerInfo);
        NativeUtils.Free(_internal.containerInfo);
        ObjcClass = new QBIdxEntityInfo(*_internal.objcClass);
        NativeUtils.Free(_internal.objcClass);
        ClassCursor = new QBCursor(_internal.classCursor);
        ClassLoc = new QBIdxLoc(_internal.classLoc);
        Protocols = new QBIdxObjCProtocolRefListInfo(*_internal.protocols);
        NativeUtils.Free(_internal.protocols);
    }

    public QBIdxObjCContainerDeclInfo ContainerInfo { get; set; }
    public QBIdxEntityInfo ObjcClass { get; set; }
    public QBCursor ClassCursor { get; set; }
    public QBIdxLoc ClassLoc { get; set; }
    public QBIdxObjCProtocolRefListInfo Protocols { get; set; }

    public QuantumBinding.Clang.Interop.CXIdxObjCCategoryDeclInfo ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXIdxObjCCategoryDeclInfo();
        _containerInfo.Dispose();
        if (ContainerInfo != null)
        {
            var struct0 = ContainerInfo.ToNative();
            _containerInfo = new NativeStruct<QuantumBinding.Clang.Interop.CXIdxObjCContainerDeclInfo>(struct0);
            _internal.containerInfo = _containerInfo.Handle;
        }
        _objcClass.Dispose();
        if (ObjcClass != null)
        {
            var struct1 = ObjcClass.ToNative();
            _objcClass = new NativeStruct<QuantumBinding.Clang.Interop.CXIdxEntityInfo>(struct1);
            _internal.objcClass = _objcClass.Handle;
        }
        if (ClassCursor != null)
        {
            _internal.classCursor = ClassCursor.ToNative();
        }
        if (ClassLoc != null)
        {
            _internal.classLoc = ClassLoc.ToNative();
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
        _objcClass.Dispose();
        _protocols.Dispose();
    }


    public static implicit operator QBIdxObjCCategoryDeclInfo(QuantumBinding.Clang.Interop.CXIdxObjCCategoryDeclInfo q)
    {
        return new QBIdxObjCCategoryDeclInfo(q);
    }

}



