
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Describes the type of the comment AST node ( CXComment). A comment node can be considered block content (e. g., paragraph), inline content (plain text) or neither (the root AST node).
///</summary>
public enum CXCommentKind : uint
{
    ///<summary>
    /// Null comment. No AST node is constructed at the requested location because there is no text or a syntax error.
    ///</summary>
    CXComment_Null = 0,

    ///<summary>
    /// Plain text. Inline content.
    ///</summary>
    CXComment_Text = 1,

    ///<summary>
    /// A command with word-like arguments that is considered inline content.
    ///</summary>
    CXComment_InlineCommand = 2,

    ///<summary>
    /// HTML start tag with attributes (name-value pairs). Considered inline content.
    ///</summary>
    CXComment_HTMLStartTag = 3,

    ///<summary>
    /// HTML end tag. Considered inline content.
    ///</summary>
    CXComment_HTMLEndTag = 4,

    ///<summary>
    /// A paragraph, contains inline comment. The paragraph itself is block content.
    ///</summary>
    CXComment_Paragraph = 5,

    ///<summary>
    /// A command that has zero or more word-like arguments (number of word-like arguments depends on command name) and a paragraph as an argument. Block command is block content.
    ///</summary>
    CXComment_BlockCommand = 6,

    ///<summary>
    /// A \param or \arg command that describes the function parameter (name, passing direction, description).
    ///</summary>
    CXComment_ParamCommand = 7,

    ///<summary>
    /// A \tparam command that describes a template parameter (name and description).
    ///</summary>
    CXComment_TParamCommand = 8,

    ///<summary>
    /// A verbatim block command (e. g., preformatted code). Verbatim block has an opening and a closing command and contains multiple lines of text ( CXComment_VerbatimBlockLine child nodes).
    ///</summary>
    CXComment_VerbatimBlockCommand = 9,

    ///<summary>
    /// A line of text that is contained within a CXComment_VerbatimBlockCommand node.
    ///</summary>
    CXComment_VerbatimBlockLine = 10,

    ///<summary>
    /// A verbatim line command. Verbatim line has an opening command, a single line of text (up to the newline after the opening command) and has no closing command.
    ///</summary>
    CXComment_VerbatimLine = 11,

    ///<summary>
    /// A full comment attached to a declaration, contains block content.
    ///</summary>
    CXComment_FullComment = 12,

}



