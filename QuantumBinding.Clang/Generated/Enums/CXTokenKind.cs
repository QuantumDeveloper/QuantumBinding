
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Describes a kind of token.
///</summary>
public enum CXTokenKind : uint
{
    ///<summary>
    /// A token that contains some kind of punctuation.
    ///</summary>
    CXToken_Punctuation = 0,

    ///<summary>
    /// A language keyword.
    ///</summary>
    CXToken_Keyword = 1,

    ///<summary>
    /// An identifier (that is not a keyword).
    ///</summary>
    CXToken_Identifier = 2,

    ///<summary>
    /// A numeric, string, or character literal.
    ///</summary>
    CXToken_Literal = 3,

    ///<summary>
    /// A comment.
    ///</summary>
    CXToken_Comment = 4,

}



