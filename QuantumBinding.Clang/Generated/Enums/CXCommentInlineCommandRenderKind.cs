
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// The most appropriate rendering mode for an inline command, chosen on command semantics in Doxygen.
///</summary>
public enum CXCommentInlineCommandRenderKind : uint
{
    ///<summary>
    /// Command argument should be rendered in a normal font.
    ///</summary>
    CXCommentInlineCommandRenderKind_Normal = 0,

    ///<summary>
    /// Command argument should be rendered in a bold font.
    ///</summary>
    CXCommentInlineCommandRenderKind_Bold = 1,

    ///<summary>
    /// Command argument should be rendered in a monospaced font.
    ///</summary>
    CXCommentInlineCommandRenderKind_Monospaced = 2,

    ///<summary>
    /// Command argument should be rendered emphasized (typically italic font).
    ///</summary>
    CXCommentInlineCommandRenderKind_Emphasized = 3,

    ///<summary>
    /// Command argument should not be rendered (since it only defines an anchor).
    ///</summary>
    CXCommentInlineCommandRenderKind_Anchor = 4,

}


