
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

///<summary>
/// The client's data object that is associated with a semantic container of entities.
///</summary>
public unsafe partial class QBIdxClientContainer
{
    internal CXIdxClientContainerImpl __Instance;
    public QBIdxClientContainer()
    {
    }

    public QBIdxClientContainer(QuantumBinding.Clang.Interop.CXIdxClientContainerImpl __Instance)
    {
        this.__Instance = __Instance;
    }

    public ref readonly CXIdxClientContainerImpl GetPinnableReference() => ref __Instance;

    public static implicit operator QuantumBinding.Clang.Interop.CXIdxClientContainerImpl(QBIdxClientContainer q)
    {
        return q?.__Instance ?? new QuantumBinding.Clang.Interop.CXIdxClientContainerImpl();
    }

    public static implicit operator QBIdxClientContainer(QuantumBinding.Clang.Interop.CXIdxClientContainerImpl q)
    {
        return new QBIdxClientContainer(q);
    }

}



