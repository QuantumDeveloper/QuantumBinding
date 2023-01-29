
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBCompletionResult
{
    public QBCompletionResult()
    {
    }

    public QBCompletionResult(QuantumBinding.Clang.Interop.CXCompletionResult _internal)
    {
        CursorKind = _internal.CursorKind;
        CompletionString = new QBCompletionString(_internal.CompletionString);
    }

    public CXCursorKind CursorKind { get; set; }
    public QBCompletionString CompletionString { get; set; }

    public QuantumBinding.Clang.Interop.CXCompletionResult ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXCompletionResult();
        _internal.CursorKind = CursorKind;
        _internal.CompletionString = CompletionString;
        return _internal;
    }

    public static implicit operator QBCompletionResult(QuantumBinding.Clang.Interop.CXCompletionResult q)
    {
        return new QBCompletionResult(q);
    }

}



