
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

///<summary>
/// Opaque pointer representing a policy that controls pretty printing for clang_getCursorPrettyPrinted.
///</summary>
public unsafe partial class QBPrintingPolicy
{
    internal CXPrintingPolicyImpl __Instance;
    public QBPrintingPolicy()
    {
    }

    public QBPrintingPolicy(QuantumBinding.Clang.Interop.CXPrintingPolicyImpl __Instance)
    {
        this.__Instance = __Instance;
    }

    ///<summary>
    /// Release a printing policy.
    ///</summary>
    public void PrintingPolicy_dispose()
    {
        QuantumBinding.Clang.Interop.ClangInterop.clang_PrintingPolicy_dispose(this);
    }

    ///<summary>
    /// Get a property value for the given printing policy.
    ///</summary>
    public uint PrintingPolicy_getProperty(CXPrintingPolicyProperty Property)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_PrintingPolicy_getProperty(this, Property);
    }

    ///<summary>
    /// Set a property value for the given printing policy.
    ///</summary>
    public void PrintingPolicy_setProperty(CXPrintingPolicyProperty Property, uint Value)
    {
        QuantumBinding.Clang.Interop.ClangInterop.clang_PrintingPolicy_setProperty(this, Property, Value);
    }

    public ref readonly CXPrintingPolicyImpl GetPinnableReference() => ref __Instance;

    public static implicit operator QuantumBinding.Clang.Interop.CXPrintingPolicyImpl(QBPrintingPolicy q)
    {
        return q?.__Instance ?? new QuantumBinding.Clang.Interop.CXPrintingPolicyImpl();
    }

    public static implicit operator QBPrintingPolicy(QuantumBinding.Clang.Interop.CXPrintingPolicyImpl q)
    {
        return new QBPrintingPolicy(q);
    }

}



