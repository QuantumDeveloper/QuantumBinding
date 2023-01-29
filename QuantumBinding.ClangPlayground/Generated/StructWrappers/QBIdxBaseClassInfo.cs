
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBIdxBaseClassInfo : QBDisposableObject
{
    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxEntityInfo> _base;

    public QBIdxBaseClassInfo()
    {
    }

    public QBIdxBaseClassInfo(QuantumBinding.Clang.Interop.CXIdxBaseClassInfo _internal)
    {
        Base = new QBIdxEntityInfo(*_internal.@base);
        NativeUtils.Free(_internal.@base);
        Cursor = new QBCursor(_internal.cursor);
        Loc = new QBIdxLoc(_internal.loc);
    }

    public QBIdxEntityInfo Base { get; set; }
    public QBCursor Cursor { get; set; }
    public QBIdxLoc Loc { get; set; }

    public QuantumBinding.Clang.Interop.CXIdxBaseClassInfo ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXIdxBaseClassInfo();
        _base.Dispose();
        if (Base != null)
        {
            var struct0 = Base.ToNative();
            _base = new NativeStruct<QuantumBinding.Clang.Interop.CXIdxEntityInfo>(struct0);
            _internal.@base = _base.Handle;
        }
        if (Cursor != null)
        {
            _internal.cursor = Cursor.ToNative();
        }
        if (Loc != null)
        {
            _internal.loc = Loc.ToNative();
        }
        return _internal;
    }

    protected override void UnmanagedDisposeOverride()
    {
        _base.Dispose();
    }


    public static implicit operator QBIdxBaseClassInfo(QuantumBinding.Clang.Interop.CXIdxBaseClassInfo q)
    {
        return new QBIdxBaseClassInfo(q);
    }

}



