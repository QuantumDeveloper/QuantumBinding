
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBIdxEntityRefInfo : QBDisposableObject
{
    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxEntityInfo> _referencedEntity;

    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxEntityInfo> _parentEntity;

    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxContainerInfo> _container;

    public QBIdxEntityRefInfo()
    {
    }

    public QBIdxEntityRefInfo(QuantumBinding.Clang.Interop.CXIdxEntityRefInfo _internal)
    {
        Kind = _internal.kind;
        Cursor = new QBCursor(_internal.cursor);
        Loc = new QBIdxLoc(_internal.loc);
        ReferencedEntity = new QBIdxEntityInfo(*_internal.referencedEntity);
        NativeUtils.Free(_internal.referencedEntity);
        ParentEntity = new QBIdxEntityInfo(*_internal.parentEntity);
        NativeUtils.Free(_internal.parentEntity);
        Container = new QBIdxContainerInfo(*_internal.container);
        NativeUtils.Free(_internal.container);
        Role = _internal.role;
    }

    public CXIdxEntityRefKind Kind { get; set; }
    public QBCursor Cursor { get; set; }
    public QBIdxLoc Loc { get; set; }
    public QBIdxEntityInfo ReferencedEntity { get; set; }
    public QBIdxEntityInfo ParentEntity { get; set; }
    public QBIdxContainerInfo Container { get; set; }
    public CXSymbolRole Role { get; set; }

    public QuantumBinding.Clang.Interop.CXIdxEntityRefInfo ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXIdxEntityRefInfo();
        _internal.kind = Kind;
        if (Cursor != null)
        {
            _internal.cursor = Cursor.ToNative();
        }
        if (Loc != null)
        {
            _internal.loc = Loc.ToNative();
        }
        _referencedEntity.Dispose();
        if (ReferencedEntity != null)
        {
            var struct0 = ReferencedEntity.ToNative();
            _referencedEntity = new NativeStruct<QuantumBinding.Clang.Interop.CXIdxEntityInfo>(struct0);
            _internal.referencedEntity = _referencedEntity.Handle;
        }
        _parentEntity.Dispose();
        if (ParentEntity != null)
        {
            var struct1 = ParentEntity.ToNative();
            _parentEntity = new NativeStruct<QuantumBinding.Clang.Interop.CXIdxEntityInfo>(struct1);
            _internal.parentEntity = _parentEntity.Handle;
        }
        _container.Dispose();
        if (Container != null)
        {
            var struct2 = Container.ToNative();
            _container = new NativeStruct<QuantumBinding.Clang.Interop.CXIdxContainerInfo>(struct2);
            _internal.container = _container.Handle;
        }
        _internal.role = Role;
        return _internal;
    }

    protected override void UnmanagedDisposeOverride()
    {
        _referencedEntity.Dispose();
        _parentEntity.Dispose();
        _container.Dispose();
    }


    public static implicit operator QBIdxEntityRefInfo(QuantumBinding.Clang.Interop.CXIdxEntityRefInfo q)
    {
        return new QBIdxEntityRefInfo(q);
    }

}



