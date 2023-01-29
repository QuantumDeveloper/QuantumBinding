
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

///<summary>
/// Opaque pointer representing client data that will be passed through to various callbacks and visitors.
///</summary>
public unsafe partial class QBClientData
{
    internal CXClientDataImpl __Instance;
    public QBClientData()
    {
    }

    public QBClientData(QuantumBinding.Clang.Interop.CXClientDataImpl __Instance)
    {
        this.__Instance = __Instance;
    }

    public ref readonly CXClientDataImpl GetPinnableReference() => ref __Instance;

    public static implicit operator QuantumBinding.Clang.Interop.CXClientDataImpl(QBClientData q)
    {
        return q?.__Instance ?? new QuantumBinding.Clang.Interop.CXClientDataImpl();
    }

    public static implicit operator QBClientData(QuantumBinding.Clang.Interop.CXClientDataImpl q)
    {
        return new QBClientData(q);
    }

}


