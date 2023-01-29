
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

///<summary>
/// A group of CXDiagnostics.
///</summary>
public unsafe partial class QBDiagnosticSet
{
    internal CXDiagnosticSetImpl __Instance;
    public QBDiagnosticSet()
    {
    }

    public QBDiagnosticSet(QuantumBinding.Clang.Interop.CXDiagnosticSetImpl __Instance)
    {
        this.__Instance = __Instance;
    }

    ///<summary>
    /// Release a CXDiagnosticSet and all of its contained diagnostics.
    ///</summary>
    public void disposeDiagnosticSet()
    {
        QuantumBinding.Clang.Interop.ClangInterop.clang_disposeDiagnosticSet(this);
    }

    ///<summary>
    /// Retrieve a diagnostic associated with the given CXDiagnosticSet.
    ///</summary>
    public QBDiagnostic getDiagnosticInSet(uint Index)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getDiagnosticInSet(this, Index);
    }

    ///<summary>
    /// Determine the number of diagnostics in a CXDiagnosticSet.
    ///</summary>
    public uint getNumDiagnosticsInSet()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getNumDiagnosticsInSet(this);
    }

    public ref readonly CXDiagnosticSetImpl GetPinnableReference() => ref __Instance;

    public static implicit operator QuantumBinding.Clang.Interop.CXDiagnosticSetImpl(QBDiagnosticSet q)
    {
        return q?.__Instance ?? new QuantumBinding.Clang.Interop.CXDiagnosticSetImpl();
    }

    public static implicit operator QBDiagnosticSet(QuantumBinding.Clang.Interop.CXDiagnosticSetImpl q)
    {
        return new QBDiagnosticSet(q);
    }

}



