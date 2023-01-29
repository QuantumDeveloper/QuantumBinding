
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBIdxObjCProtocolRefInfo : QBDisposableObject
{
    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxEntityInfo> _protocol;

    public QBIdxObjCProtocolRefInfo()
    {
    }

    public QBIdxObjCProtocolRefInfo(QuantumBinding.Clang.Interop.CXIdxObjCProtocolRefInfo _internal)
    {
        Protocol = new QBIdxEntityInfo(*_internal.protocol);
        NativeUtils.Free(_internal.protocol);
        Cursor = new QBCursor(_internal.cursor);
        Loc = new QBIdxLoc(_internal.loc);
    }

    public QBIdxEntityInfo Protocol { get; set; }
    public QBCursor Cursor { get; set; }
    public QBIdxLoc Loc { get; set; }

    public QuantumBinding.Clang.Interop.CXIdxObjCProtocolRefInfo ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXIdxObjCProtocolRefInfo();
        _protocol.Dispose();
        if (Protocol != null)
        {
            var struct0 = Protocol.ToNative();
            _protocol = new NativeStruct<QuantumBinding.Clang.Interop.CXIdxEntityInfo>(struct0);
            _internal.protocol = _protocol.Handle;
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
        _protocol.Dispose();
    }


    public static implicit operator QBIdxObjCProtocolRefInfo(QuantumBinding.Clang.Interop.CXIdxObjCProtocolRefInfo q)
    {
        return new QBIdxObjCProtocolRefInfo(q);
    }

}



