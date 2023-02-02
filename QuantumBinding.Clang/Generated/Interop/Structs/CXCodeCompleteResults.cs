
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

///<summary>
/// Contains the results of code-completion.
///</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXCodeCompleteResults
{
    ///<summary>
    /// The code-completion results.
    ///</summary>
    public CXCompletionResult* Results;
    ///<summary>
    /// The number of code-completion results stored in the Results array.
    ///</summary>
    public uint NumResults;
}



