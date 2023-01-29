
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

///<summary>
/// CXAPISet is an opaque type that represents a data structure containing all the API information for a given translation unit. This can be used for a single symbol symbol graph for a given symbol.
///</summary>
public unsafe partial class QBAPISet
{
    internal CXAPISetImpl __Instance;
    public QBAPISet()
    {
    }

    public QBAPISet(QuantumBinding.Clang.Interop.CXAPISetImpl __Instance)
    {
        this.__Instance = __Instance;
    }

    ///<summary>
    /// Dispose of an APISet.
    ///</summary>
    public void disposeAPISet()
    {
        QuantumBinding.Clang.Interop.ClangInterop.clang_disposeAPISet(this);
    }

    public ref readonly CXAPISetImpl GetPinnableReference() => ref __Instance;

    public static implicit operator QuantumBinding.Clang.Interop.CXAPISetImpl(QBAPISet q)
    {
        return q?.__Instance ?? new QuantumBinding.Clang.Interop.CXAPISetImpl();
    }

    public static implicit operator QBAPISet(QuantumBinding.Clang.Interop.CXAPISetImpl q)
    {
        return new QBAPISet(q);
    }

}



