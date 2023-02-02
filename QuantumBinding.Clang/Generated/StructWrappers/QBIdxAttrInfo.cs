
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBIdxAttrInfo
{
    public QBIdxAttrInfo()
    {
    }

    public QBIdxAttrInfo(QuantumBinding.Clang.Interop.CXIdxAttrInfo _internal)
    {
        Kind = _internal.kind;
        Cursor = new QBCursor(_internal.cursor);
        Loc = new QBIdxLoc(_internal.loc);
    }

    public CXIdxAttrKind Kind { get; set; }
    public QBCursor Cursor { get; set; }
    public QBIdxLoc Loc { get; set; }

    public QuantumBinding.Clang.Interop.CXIdxAttrInfo ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXIdxAttrInfo();
        _internal.kind = Kind;
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

    public static implicit operator QBIdxAttrInfo(QuantumBinding.Clang.Interop.CXIdxAttrInfo q)
    {
        return new QBIdxAttrInfo(q);
    }

}



