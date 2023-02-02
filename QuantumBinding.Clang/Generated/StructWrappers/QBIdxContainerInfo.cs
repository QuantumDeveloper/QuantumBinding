
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBIdxContainerInfo
{
    public QBIdxContainerInfo()
    {
    }

    public QBIdxContainerInfo(QuantumBinding.Clang.Interop.CXIdxContainerInfo _internal)
    {
        Cursor = new QBCursor(_internal.cursor);
    }

    public QBCursor Cursor { get; set; }

    public QuantumBinding.Clang.Interop.CXIdxContainerInfo ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXIdxContainerInfo();
        if (Cursor != null)
        {
            _internal.cursor = Cursor.ToNative();
        }
        return _internal;
    }

    public static implicit operator QBIdxContainerInfo(QuantumBinding.Clang.Interop.CXIdxContainerInfo q)
    {
        return new QBIdxContainerInfo(q);
    }

}



