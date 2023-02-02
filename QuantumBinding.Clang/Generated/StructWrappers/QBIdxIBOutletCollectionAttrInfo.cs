
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBIdxIBOutletCollectionAttrInfo : QBDisposableObject
{
    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxAttrInfo> _attrInfo;

    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxEntityInfo> _objcClass;

    public QBIdxIBOutletCollectionAttrInfo()
    {
    }

    public QBIdxIBOutletCollectionAttrInfo(QuantumBinding.Clang.Interop.CXIdxIBOutletCollectionAttrInfo _internal)
    {
        AttrInfo = new QBIdxAttrInfo(*_internal.attrInfo);
        NativeUtils.Free(_internal.attrInfo);
        ObjcClass = new QBIdxEntityInfo(*_internal.objcClass);
        NativeUtils.Free(_internal.objcClass);
        ClassCursor = new QBCursor(_internal.classCursor);
        ClassLoc = new QBIdxLoc(_internal.classLoc);
    }

    public QBIdxAttrInfo AttrInfo { get; set; }
    public QBIdxEntityInfo ObjcClass { get; set; }
    public QBCursor ClassCursor { get; set; }
    public QBIdxLoc ClassLoc { get; set; }

    public QuantumBinding.Clang.Interop.CXIdxIBOutletCollectionAttrInfo ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXIdxIBOutletCollectionAttrInfo();
        _attrInfo.Dispose();
        if (AttrInfo != null)
        {
            var struct0 = AttrInfo.ToNative();
            _attrInfo = new NativeStruct<QuantumBinding.Clang.Interop.CXIdxAttrInfo>(struct0);
            _internal.attrInfo = _attrInfo.Handle;
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
        return _internal;
    }

    protected override void UnmanagedDisposeOverride()
    {
        _attrInfo.Dispose();
        _objcClass.Dispose();
    }


    public static implicit operator QBIdxIBOutletCollectionAttrInfo(QuantumBinding.Clang.Interop.CXIdxIBOutletCollectionAttrInfo q)
    {
        return new QBIdxIBOutletCollectionAttrInfo(q);
    }

}



