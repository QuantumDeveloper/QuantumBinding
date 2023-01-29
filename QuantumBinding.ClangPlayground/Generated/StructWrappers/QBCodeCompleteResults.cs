
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBCodeCompleteResults : QBDisposableObject
{
    private NativeStruct<QuantumBinding.Clang.Interop.CXCompletionResult> _results;

    public QBCodeCompleteResults()
    {
    }

    public QBCodeCompleteResults(QuantumBinding.Clang.Interop.CXCodeCompleteResults _internal)
    {
        Results = new QBCompletionResult(*_internal.Results);
        NativeUtils.Free(_internal.Results);
        NumResults = _internal.NumResults;
    }

    public QBCompletionResult Results { get; set; }
    public uint NumResults { get; set; }
    ///<summary>
    /// Free the given set of code-completion results.
    ///</summary>
    public void disposeCodeCompleteResults()
    {
        var arg0 = NativeUtils.StructOrEnumToPointer(ToNative());
        QuantumBinding.Clang.Interop.ClangInterop.clang_disposeCodeCompleteResults(arg0);
        NativeUtils.Free(arg0);
    }

    ///<summary>
    /// Fix-its that *must* be applied before inserting the text for the corresponding completion.
    ///</summary>
    public QBString getCompletionFixIt(uint completion_index, uint fixit_index, QBSourceRange replacement_range)
    {
        var arg0 = NativeUtils.StructOrEnumToPointer(ToNative());
        var arg3 = ReferenceEquals(replacement_range, null) ? null : NativeUtils.StructOrEnumToPointer(replacement_range.ToNative());
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getCompletionFixIt(arg0, completion_index, fixit_index, arg3);
        NativeUtils.Free(arg0);
        NativeUtils.Free(arg3);
        return result;
    }

    ///<summary>
    /// Retrieve the number of fix-its for the given completion index.
    ///</summary>
    public uint getCompletionNumFixIts(uint completion_index)
    {
        var arg0 = NativeUtils.StructOrEnumToPointer(ToNative());
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getCompletionNumFixIts(arg0, completion_index);
        NativeUtils.Free(arg0);
        return result;
    }


    public QuantumBinding.Clang.Interop.CXCodeCompleteResults ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXCodeCompleteResults();
        _results.Dispose();
        if (Results != null)
        {
            var struct0 = Results.ToNative();
            _results = new NativeStruct<QuantumBinding.Clang.Interop.CXCompletionResult>(struct0);
            _internal.Results = _results.Handle;
        }
        _internal.NumResults = NumResults;
        return _internal;
    }

    protected override void UnmanagedDisposeOverride()
    {
        _results.Dispose();
    }


    public static implicit operator QBCodeCompleteResults(QuantumBinding.Clang.Interop.CXCodeCompleteResults q)
    {
        return new QBCodeCompleteResults(q);
    }

}



