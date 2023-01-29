
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Flags that can be passed to clang_codeCompleteAt() to modify its behavior.
///</summary>
[Flags]
public enum CXCodeComplete_Flags : uint
{
    ///<summary>
    /// Whether to include macros within the set of code completions returned.
    ///</summary>
    CXCodeComplete_IncludeMacros = 1,

    ///<summary>
    /// Whether to include code patterns for language constructs within the set of code completions, e.g., for loops.
    ///</summary>
    CXCodeComplete_IncludeCodePatterns = 2,

    ///<summary>
    /// Whether to include brief documentation within the set of code completions returned.
    ///</summary>
    CXCodeComplete_IncludeBriefComments = 4,

    ///<summary>
    /// Whether to speed up completion by omitting top- or namespace-level entities defined in the preamble. There's no guarantee any particular entity is omitted. This may be useful if the headers are indexed externally.
    ///</summary>
    CXCodeComplete_SkipPreamble = 8,

    ///<summary>
    /// Whether to include completions with small fix-its, e.g. change '.' to '->' on member access, etc.
    ///</summary>
    CXCodeComplete_IncludeCompletionsWithFixIts = 16,

}


