
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Describes a single piece of text within a code-completion string.
///</summary>
public enum CXCompletionChunkKind : uint
{
    ///<summary>
    /// A code-completion string that describes "optional" text that could be a part of the template (but is not required).
    ///</summary>
    CXCompletionChunk_Optional = 0,

    ///<summary>
    /// Text that a user would be expected to type to get this code-completion result.
    ///</summary>
    CXCompletionChunk_TypedText = 1,

    ///<summary>
    /// Text that should be inserted as part of a code-completion result.
    ///</summary>
    CXCompletionChunk_Text = 2,

    ///<summary>
    /// Placeholder text that should be replaced by the user.
    ///</summary>
    CXCompletionChunk_Placeholder = 3,

    ///<summary>
    /// Informative text that should be displayed but never inserted as part of the template.
    ///</summary>
    CXCompletionChunk_Informative = 4,

    ///<summary>
    /// Text that describes the current parameter when code-completion is referring to function call, message send, or template specialization.
    ///</summary>
    CXCompletionChunk_CurrentParameter = 5,

    ///<summary>
    /// A left parenthesis ('('), used to initiate a function call or signal the beginning of a function parameter list.
    ///</summary>
    CXCompletionChunk_LeftParen = 6,

    ///<summary>
    /// A right parenthesis (')'), used to finish a function call or signal the end of a function parameter list.
    ///</summary>
    CXCompletionChunk_RightParen = 7,

    ///<summary>
    /// A left bracket ('[').
    ///</summary>
    CXCompletionChunk_LeftBracket = 8,

    ///<summary>
    /// A right bracket (']').
    ///</summary>
    CXCompletionChunk_RightBracket = 9,

    ///<summary>
    /// A left brace ('{').
    ///</summary>
    CXCompletionChunk_LeftBrace = 10,

    ///<summary>
    /// A right brace ('}').
    ///</summary>
    CXCompletionChunk_RightBrace = 11,

    ///<summary>
    /// A left angle bracket ('<').
    ///</summary>
    CXCompletionChunk_LeftAngle = 12,

    ///<summary>
    /// A right angle bracket ('>').
    ///</summary>
    CXCompletionChunk_RightAngle = 13,

    ///<summary>
    /// A comma separator (',').
    ///</summary>
    CXCompletionChunk_Comma = 14,

    ///<summary>
    /// Text that specifies the result type of a given result.
    ///</summary>
    CXCompletionChunk_ResultType = 15,

    ///<summary>
    /// A colon (':').
    ///</summary>
    CXCompletionChunk_Colon = 16,

    ///<summary>
    /// A semicolon (';').
    ///</summary>
    CXCompletionChunk_SemiColon = 17,

    ///<summary>
    /// An '=' sign.
    ///</summary>
    CXCompletionChunk_Equal = 18,

    ///<summary>
    /// Horizontal space (' ').
    ///</summary>
    CXCompletionChunk_HorizontalSpace = 19,

    ///<summary>
    /// Vertical space ('\n'), after which it is generally a good idea to perform indentation.
    ///</summary>
    CXCompletionChunk_VerticalSpace = 20,

}


