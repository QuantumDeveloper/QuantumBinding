
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Clang;

namespace QuantumBinding.Clang.Interop;

///<summary>
/// A single result of code completion.
///</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXCompletionResult
{
    ///<summary>
    /// The kind of entity that this completion refers to.
    ///</summary>
    public CXCursorKind CursorKind;
    ///<summary>
    /// The code-completion string that describes how to insert this code-completion result into the editing buffer.
    ///</summary>
    public CXCompletionStringImpl CompletionString;
}



