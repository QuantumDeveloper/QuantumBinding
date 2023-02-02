
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBComment : QBDisposableObject
{
    public QBComment()
    {
    }

    public QBComment(QuantumBinding.Clang.Interop.CXComment _internal)
    {
        ASTNode = _internal.ASTNode;
        TranslationUnit = new QBTranslationUnit(_internal.TranslationUnit);
    }

    public void* ASTNode { get; set; }
    public QBTranslationUnit TranslationUnit { get; set; }
    ///<summary>
    /// Returns text of the specified word-like argument.
    ///</summary>
    public QBString BlockCommandComment_getArgText(uint ArgIdx)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_BlockCommandComment_getArgText(ToNative(), ArgIdx);
    }

    ///<summary>
    /// Returns name of the block command.
    ///</summary>
    public QBString BlockCommandComment_getCommandName()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_BlockCommandComment_getCommandName(ToNative());
    }

    ///<summary>
    /// Returns number of word-like arguments.
    ///</summary>
    public uint BlockCommandComment_getNumArgs()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_BlockCommandComment_getNumArgs(ToNative());
    }

    ///<summary>
    /// Returns paragraph argument of the block command.
    ///</summary>
    public QBComment BlockCommandComment_getParagraph()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_BlockCommandComment_getParagraph(ToNative());
    }

    ///<summary>
    /// Returns the specified child of the AST node.
    ///</summary>
    public QBComment Comment_getChild(uint ChildIdx)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Comment_getChild(ToNative(), ChildIdx);
    }

    ///<summary>
    /// Returns the type of the AST node.
    ///</summary>
    public CXCommentKind Comment_getKind()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Comment_getKind(ToNative());
    }

    ///<summary>
    /// Returns number of children of the AST node.
    ///</summary>
    public uint Comment_getNumChildren()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Comment_getNumChildren(ToNative());
    }

    ///<summary>
    /// A CXComment_Paragraph node is considered whitespace if it contains only CXComment_Text nodes that are empty or whitespace.
    ///</summary>
    public uint Comment_isWhitespace()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Comment_isWhitespace(ToNative());
    }

    ///<summary>
    /// Convert a given full parsed comment to an HTML fragment.
    ///</summary>
    public QBString FullComment_getAsHTML()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_FullComment_getAsHTML(ToNative());
    }

    ///<summary>
    /// Convert a given full parsed comment to an XML document.
    ///</summary>
    public QBString FullComment_getAsXML()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_FullComment_getAsXML(ToNative());
    }

    ///<summary>
    /// Returns name of the specified attribute.
    ///</summary>
    public QBString HTMLStartTag_getAttrName(uint AttrIdx)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_HTMLStartTag_getAttrName(ToNative(), AttrIdx);
    }

    ///<summary>
    /// Returns value of the specified attribute.
    ///</summary>
    public QBString HTMLStartTag_getAttrValue(uint AttrIdx)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_HTMLStartTag_getAttrValue(ToNative(), AttrIdx);
    }

    ///<summary>
    /// Returns number of attributes (name-value pairs) attached to the start tag.
    ///</summary>
    public uint HTMLStartTag_getNumAttrs()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_HTMLStartTag_getNumAttrs(ToNative());
    }

    ///<summary>
    /// Returns non-zero if tag is self-closing (for example, <br />).
    ///</summary>
    public uint HTMLStartTagComment_isSelfClosing()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_HTMLStartTagComment_isSelfClosing(ToNative());
    }

    ///<summary>
    /// Convert an HTML tag AST node to string.
    ///</summary>
    public QBString HTMLTagComment_getAsString()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_HTMLTagComment_getAsString(ToNative());
    }

    ///<summary>
    /// Returns HTML tag name.
    ///</summary>
    public QBString HTMLTagComment_getTagName()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_HTMLTagComment_getTagName(ToNative());
    }

    ///<summary>
    /// Returns text of the specified argument.
    ///</summary>
    public QBString InlineCommandComment_getArgText(uint ArgIdx)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_InlineCommandComment_getArgText(ToNative(), ArgIdx);
    }

    ///<summary>
    /// Returns name of the inline command.
    ///</summary>
    public QBString InlineCommandComment_getCommandName()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_InlineCommandComment_getCommandName(ToNative());
    }

    ///<summary>
    /// Returns number of command arguments.
    ///</summary>
    public uint InlineCommandComment_getNumArgs()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_InlineCommandComment_getNumArgs(ToNative());
    }

    ///<summary>
    /// Returns the most appropriate rendering mode, chosen on command semantics in Doxygen.
    ///</summary>
    public CXCommentInlineCommandRenderKind InlineCommandComment_getRenderKind()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_InlineCommandComment_getRenderKind(ToNative());
    }

    ///<summary>
    /// Returns non-zero if Comment is inline content and has a newline immediately following it in the comment text. Newlines between paragraphs do not count.
    ///</summary>
    public uint InlineContentComment_hasTrailingNewline()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_InlineContentComment_hasTrailingNewline(ToNative());
    }

    ///<summary>
    /// Returns parameter passing direction.
    ///</summary>
    public CXCommentParamPassDirection ParamCommandComment_getDirection()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_ParamCommandComment_getDirection(ToNative());
    }

    ///<summary>
    /// Returns zero-based parameter index in function prototype.
    ///</summary>
    public uint ParamCommandComment_getParamIndex()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_ParamCommandComment_getParamIndex(ToNative());
    }

    ///<summary>
    /// Returns parameter name.
    ///</summary>
    public QBString ParamCommandComment_getParamName()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_ParamCommandComment_getParamName(ToNative());
    }

    ///<summary>
    /// Returns non-zero if parameter passing direction was specified explicitly in the comment.
    ///</summary>
    public uint ParamCommandComment_isDirectionExplicit()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_ParamCommandComment_isDirectionExplicit(ToNative());
    }

    ///<summary>
    /// Returns non-zero if the parameter that this AST node represents was found in the function prototype and clang_ParamCommandComment_getParamIndex function will return a meaningful value.
    ///</summary>
    public uint ParamCommandComment_isParamIndexValid()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_ParamCommandComment_isParamIndexValid(ToNative());
    }

    ///<summary>
    /// Returns text contained in the AST node.
    ///</summary>
    public QBString TextComment_getText()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_TextComment_getText(ToNative());
    }

    ///<summary>
    /// Returns zero-based nesting depth of this parameter in the template parameter list.
    ///</summary>
    public uint TParamCommandComment_getDepth()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_TParamCommandComment_getDepth(ToNative());
    }

    ///<summary>
    /// Returns zero-based parameter index in the template parameter list at a given nesting depth.
    ///</summary>
    public uint TParamCommandComment_getIndex(uint Depth)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_TParamCommandComment_getIndex(ToNative(), Depth);
    }

    ///<summary>
    /// Returns template parameter name.
    ///</summary>
    public QBString TParamCommandComment_getParamName()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_TParamCommandComment_getParamName(ToNative());
    }

    ///<summary>
    /// Returns non-zero if the parameter that this AST node represents was found in the template parameter list and clang_TParamCommandComment_getDepth and clang_TParamCommandComment_getIndex functions will return a meaningful value.
    ///</summary>
    public uint TParamCommandComment_isParamPositionValid()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_TParamCommandComment_isParamPositionValid(ToNative());
    }

    ///<summary>
    /// Returns text contained in the AST node.
    ///</summary>
    public QBString VerbatimBlockLineComment_getText()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_VerbatimBlockLineComment_getText(ToNative());
    }

    ///<summary>
    /// Returns text contained in the AST node.
    ///</summary>
    public QBString VerbatimLineComment_getText()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_VerbatimLineComment_getText(ToNative());
    }


    public QuantumBinding.Clang.Interop.CXComment ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXComment();
        _internal.ASTNode = ASTNode;
        _internal.TranslationUnit = TranslationUnit;
        return _internal;
    }

    public static implicit operator QBComment(QuantumBinding.Clang.Interop.CXComment q)
    {
        return new QBComment(q);
    }

}



