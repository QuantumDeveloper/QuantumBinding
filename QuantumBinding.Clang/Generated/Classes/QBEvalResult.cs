
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

///<summary>
/// Evaluation result of a cursor
///</summary>
public unsafe partial class QBEvalResult
{
    internal CXEvalResultImpl __Instance;
    public QBEvalResult()
    {
    }

    public QBEvalResult(QuantumBinding.Clang.Interop.CXEvalResultImpl __Instance)
    {
        this.__Instance = __Instance;
    }

    ///<summary>
    /// Disposes the created Eval memory.
    ///</summary>
    public void EvalResult_dispose()
    {
        QuantumBinding.Clang.Interop.ClangInterop.clang_EvalResult_dispose(this);
    }

    ///<summary>
    /// Returns the evaluation result as double if the kind is double.
    ///</summary>
    public double EvalResult_getAsDouble()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_EvalResult_getAsDouble(this);
    }

    ///<summary>
    /// Returns the evaluation result as integer if the kind is Int.
    ///</summary>
    public int EvalResult_getAsInt()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_EvalResult_getAsInt(this);
    }

    ///<summary>
    /// Returns the evaluation result as a long long integer if the kind is Int. This prevents overflows that may happen if the result is returned with clang_EvalResult_getAsInt.
    ///</summary>
    public long EvalResult_getAsLongLong()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_EvalResult_getAsLongLong(this);
    }

    ///<summary>
    /// Returns the evaluation result as a constant string if the kind is other than Int or float. User must not free this pointer, instead call clang_EvalResult_dispose on the CXEvalResult returned by clang_Cursor_Evaluate.
    ///</summary>
    public string EvalResult_getAsStr()
    {
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_EvalResult_getAsStr(this);
        return new string(result);
    }

    ///<summary>
    /// Returns the evaluation result as an unsigned integer if the kind is Int and clang_EvalResult_isUnsignedInt is non-zero.
    ///</summary>
    public ulong EvalResult_getAsUnsigned()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_EvalResult_getAsUnsigned(this);
    }

    ///<summary>
    /// Returns the kind of the evaluated result.
    ///</summary>
    public CXEvalResultKind EvalResult_getKind()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_EvalResult_getKind(this);
    }

    ///<summary>
    /// Returns a non-zero value if the kind is Int and the evaluation result resulted in an unsigned integer.
    ///</summary>
    public uint EvalResult_isUnsignedInt()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_EvalResult_isUnsignedInt(this);
    }

    public ref readonly CXEvalResultImpl GetPinnableReference() => ref __Instance;

    public static implicit operator QuantumBinding.Clang.Interop.CXEvalResultImpl(QBEvalResult q)
    {
        return q?.__Instance ?? new QuantumBinding.Clang.Interop.CXEvalResultImpl();
    }

    public static implicit operator QBEvalResult(QuantumBinding.Clang.Interop.CXEvalResultImpl q)
    {
        return new QBEvalResult(q);
    }

}


