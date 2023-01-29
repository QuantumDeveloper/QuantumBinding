
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

///<summary>
/// The client's data object that is associated with a semantic entity.
///</summary>
public unsafe partial class QBIdxClientEntity
{
    internal CXIdxClientEntityImpl __Instance;
    public QBIdxClientEntity()
    {
    }

    public QBIdxClientEntity(QuantumBinding.Clang.Interop.CXIdxClientEntityImpl __Instance)
    {
        this.__Instance = __Instance;
    }

    public ref readonly CXIdxClientEntityImpl GetPinnableReference() => ref __Instance;

    public static implicit operator QuantumBinding.Clang.Interop.CXIdxClientEntityImpl(QBIdxClientEntity q)
    {
        return q?.__Instance ?? new QuantumBinding.Clang.Interop.CXIdxClientEntityImpl();
    }

    public static implicit operator QBIdxClientEntity(QuantumBinding.Clang.Interop.CXIdxClientEntityImpl q)
    {
        return new QBIdxClientEntity(q);
    }

}


