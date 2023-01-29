
using System.Security;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang;

namespace QuantumBinding.Clang.Interop;

public static unsafe partial class ClangInterop
{
    public const string LibraryPath = "libclang";

    ///<summary>
    /// Annotate the given set of tokens by providing cursors for each token that can be mapped to a specific entity within the abstract syntax tree.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_annotateTokens", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_annotateTokens(CXTranslationUnitImpl TU, CXToken* Tokens, uint NumTokens, CXCursor* Cursors);

    ///<summary>
    /// Returns text of the specified word-like argument.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_BlockCommandComment_getArgText", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_BlockCommandComment_getArgText(CXComment Comment, uint ArgIdx);

    ///<summary>
    /// Returns name of the block command.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_BlockCommandComment_getCommandName", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_BlockCommandComment_getCommandName(CXComment Comment);

    ///<summary>
    /// Returns number of word-like arguments.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_BlockCommandComment_getNumArgs", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_BlockCommandComment_getNumArgs(CXComment Comment);

    ///<summary>
    /// Returns paragraph argument of the block command.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_BlockCommandComment_getParagraph", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXComment clang_BlockCommandComment_getParagraph(CXComment Comment);

    ///<summary>
    /// Perform code completion at a given location in a translation unit.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_codeCompleteAt", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCodeCompleteResults* clang_codeCompleteAt(CXTranslationUnitImpl TU, sbyte* complete_filename, uint complete_line, uint complete_column, CXUnsavedFile* unsaved_files, uint num_unsaved_files, uint options);

    ///<summary>
    /// Returns the cursor kind for the container for the current code completion context. The container is only guaranteed to be set for contexts where a container exists (i.e. member accesses or Objective-C message sends); if there is not a container, this function will return CXCursor_InvalidCode.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_codeCompleteGetContainerKind", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCursorKind clang_codeCompleteGetContainerKind(CXCodeCompleteResults* Results, out uint IsIncomplete);

    ///<summary>
    /// Returns the USR for the container for the current code completion context. If there is not a container for the current context, this function will return the empty string.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_codeCompleteGetContainerUSR", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_codeCompleteGetContainerUSR(CXCodeCompleteResults* Results);

    ///<summary>
    /// Determines what completions are appropriate for the context the given code completion.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_codeCompleteGetContexts", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong clang_codeCompleteGetContexts(CXCodeCompleteResults* Results);

    ///<summary>
    /// Retrieve a diagnostic associated with the given code completion.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_codeCompleteGetDiagnostic", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXDiagnosticImpl clang_codeCompleteGetDiagnostic(CXCodeCompleteResults* Results, uint Index);

    ///<summary>
    /// Determine the number of diagnostics produced prior to the location where code completion was performed.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_codeCompleteGetNumDiagnostics", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_codeCompleteGetNumDiagnostics(CXCodeCompleteResults* Results);

    ///<summary>
    /// Returns the currently-entered selector for an Objective-C message send, formatted like "initWithFoo:bar:". Only guaranteed to return a non-empty string for CXCompletionContext_ObjCInstanceMessage and CXCompletionContext_ObjCClassMessage.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_codeCompleteGetObjCSelector", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_codeCompleteGetObjCSelector(CXCodeCompleteResults* Results);

    ///<summary>
    /// Returns the specified child of the AST node.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Comment_getChild", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXComment clang_Comment_getChild(CXComment Comment, uint ChildIdx);

    ///<summary>
    /// Returns the type of the AST node.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Comment_getKind", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCommentKind clang_Comment_getKind(CXComment Comment);

    ///<summary>
    /// Returns number of children of the AST node.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Comment_getNumChildren", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_Comment_getNumChildren(CXComment Comment);

    ///<summary>
    /// A CXComment_Paragraph node is considered whitespace if it contains only CXComment_Text nodes that are empty or whitespace.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Comment_isWhitespace", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_Comment_isWhitespace(CXComment Comment);

    ///<summary>
    /// Construct a USR for a specified Objective-C category.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_constructUSR_ObjCCategory", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_constructUSR_ObjCCategory(sbyte* class_name, sbyte* category_name);

    ///<summary>
    /// Construct a USR for a specified Objective-C class.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_constructUSR_ObjCClass", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_constructUSR_ObjCClass(sbyte* class_name);

    ///<summary>
    /// Construct a USR for a specified Objective-C instance variable and the USR for its containing class.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_constructUSR_ObjCIvar", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_constructUSR_ObjCIvar(sbyte* name, CXString classUSR);

    ///<summary>
    /// Construct a USR for a specified Objective-C method and the USR for its containing class.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_constructUSR_ObjCMethod", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_constructUSR_ObjCMethod(sbyte* name, uint isInstanceMethod, CXString classUSR);

    ///<summary>
    /// Construct a USR for a specified Objective-C property and the USR for its containing class.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_constructUSR_ObjCProperty", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_constructUSR_ObjCProperty(sbyte* property, CXString classUSR);

    ///<summary>
    /// Construct a USR for a specified Objective-C protocol.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_constructUSR_ObjCProtocol", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_constructUSR_ObjCProtocol(sbyte* protocol_name);

    ///<summary>
    /// Traverses the translation unit to create a CXAPISet.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_createAPISet", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXErrorCode clang_createAPISet(CXTranslationUnitImpl tu, out CXAPISetImpl out_api);

    ///<summary>
    /// Creates an empty CXCursorSet.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_createCXCursorSet", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCursorSetImpl clang_createCXCursorSet();

    ///<summary>
    /// Provides a shared context for creating translation units.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_createIndex", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXIndexImpl clang_createIndex(int excludeDeclarationsFromPCH, int displayDiagnostics);

    ///<summary>
    /// Same as clang_createTranslationUnit2, but returns the CXTranslationUnit instead of an error code. In case of an error this routine returns a NULL CXTranslationUnit, without further detailed error codes.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_createTranslationUnit", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXTranslationUnitImpl clang_createTranslationUnit(CXIndexImpl CIdx, sbyte* ast_filename);

    ///<summary>
    /// Create a translation unit from an AST file ( -emit-ast).
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_createTranslationUnit2", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXErrorCode clang_createTranslationUnit2(CXIndexImpl CIdx, sbyte* ast_filename, out CXTranslationUnitImpl out_TU);

    ///<summary>
    /// Return the CXTranslationUnit for a given source file and the provided command line arguments one would pass to the compiler.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_createTranslationUnitFromSourceFile", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXTranslationUnitImpl clang_createTranslationUnitFromSourceFile(CXIndexImpl CIdx, sbyte* source_filename, int num_clang_command_line_args, sbyte** clang_command_line_args, uint num_unsaved_files, CXUnsavedFile* unsaved_files);

    ///<summary>
    /// If cursor is a statement declaration tries to evaluate the statement and if its variable, tries to evaluate its initializer, into its corresponding type. If it's an expression, tries to evaluate the expression.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_Evaluate", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXEvalResultImpl clang_Cursor_Evaluate(CXCursor C);

    ///<summary>
    /// Retrieve the argument cursor of a function or method.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getArgument", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCursor clang_Cursor_getArgument(CXCursor C, uint i);

    ///<summary>
    /// Given a cursor that represents a documentable entity (e.g., declaration), return the associated first paragraph.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getBriefCommentText", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_Cursor_getBriefCommentText(CXCursor C);

    ///<summary>
    /// Given a cursor that represents a declaration, return the associated comment's source range. The range may include multiple consecutive comments with whitespace in between.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getCommentRange", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXSourceRange clang_Cursor_getCommentRange(CXCursor C);

    ///<summary>
    /// Retrieve the CXStrings representing the mangled symbols of the C++ constructor or destructor at the cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getCXXManglings", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXStringSet* clang_Cursor_getCXXManglings(CXCursor param0);

    ///<summary>
    /// Retrieve the CXString representing the mangled name of the cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getMangling", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_Cursor_getMangling(CXCursor param0);

    ///<summary>
    /// Given a CXCursor_ModuleImportDecl cursor, return the associated module.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getModule", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXModuleImpl clang_Cursor_getModule(CXCursor C);

    ///<summary>
    /// Retrieve the number of non-variadic arguments associated with a given cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getNumArguments", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_Cursor_getNumArguments(CXCursor C);

    ///<summary>
    /// Returns the number of template args of a function, struct, or class decl representing a template specialization.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getNumTemplateArguments", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_Cursor_getNumTemplateArguments(CXCursor C);

    ///<summary>
    /// Given a cursor that represents an Objective-C method or parameter declaration, return the associated Objective-C qualifiers for the return type or the parameter respectively. The bits are formed from CXObjCDeclQualifierKind.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getObjCDeclQualifiers", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_Cursor_getObjCDeclQualifiers(CXCursor C);

    ///<summary>
    /// Retrieve the CXStrings representing the mangled symbols of the ObjC class interface or implementation at the cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getObjCManglings", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXStringSet* clang_Cursor_getObjCManglings(CXCursor param0);

    ///<summary>
    /// Given a cursor that represents a property declaration, return the associated property attributes. The bits are formed from CXObjCPropertyAttrKind.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getObjCPropertyAttributes", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_Cursor_getObjCPropertyAttributes(CXCursor C, uint reserved);

    ///<summary>
    /// Given a cursor that represents a property declaration, return the name of the method that implements the getter.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getObjCPropertyGetterName", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_Cursor_getObjCPropertyGetterName(CXCursor C);

    ///<summary>
    /// Given a cursor that represents a property declaration, return the name of the method that implements the setter, if any.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getObjCPropertySetterName", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_Cursor_getObjCPropertySetterName(CXCursor C);

    ///<summary>
    /// If the cursor points to a selector identifier in an Objective-C method or message expression, this returns the selector index.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getObjCSelectorIndex", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_Cursor_getObjCSelectorIndex(CXCursor param0);

    ///<summary>
    /// Return the offset of the field represented by the Cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getOffsetOfField", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern long clang_Cursor_getOffsetOfField(CXCursor C);

    ///<summary>
    /// Given a cursor that represents a documentable entity (e.g., declaration), return the associated parsed comment as a CXComment_FullComment AST node.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getParsedComment", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXComment clang_Cursor_getParsedComment(CXCursor C);

    ///<summary>
    /// Given a cursor that represents a declaration, return the associated comment text, including comment markers.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getRawCommentText", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_Cursor_getRawCommentText(CXCursor C);

    ///<summary>
    /// Given a cursor pointing to an Objective-C message or property reference, or C++ method call, returns the CXType of the receiver.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getReceiverType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_Cursor_getReceiverType(CXCursor C);

    ///<summary>
    /// Retrieve a range for a piece that forms the cursors spelling name. Most of the times there is only one range for the complete spelling but for Objective-C methods and Objective-C message expressions, there are multiple pieces for each selector identifier.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getSpellingNameRange", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXSourceRange clang_Cursor_getSpellingNameRange(CXCursor param0, uint pieceIndex, uint options);

    ///<summary>
    /// Returns the storage class for a function or variable declaration.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getStorageClass", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CX_StorageClass clang_Cursor_getStorageClass(CXCursor param0);

    ///<summary>
    /// Retrieve the kind of the I'th template argument of the CXCursor C.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getTemplateArgumentKind", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXTemplateArgumentKind clang_Cursor_getTemplateArgumentKind(CXCursor C, uint I);

    ///<summary>
    /// Retrieve a CXType representing the type of a TemplateArgument of a function decl representing a template specialization.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getTemplateArgumentType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_Cursor_getTemplateArgumentType(CXCursor C, uint I);

    ///<summary>
    /// Retrieve the value of an Integral TemplateArgument (of a function decl representing a template specialization) as an unsigned long long.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getTemplateArgumentUnsignedValue", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong clang_Cursor_getTemplateArgumentUnsignedValue(CXCursor C, uint I);

    ///<summary>
    /// Retrieve the value of an Integral TemplateArgument (of a function decl representing a template specialization) as a signed long long.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getTemplateArgumentValue", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern long clang_Cursor_getTemplateArgumentValue(CXCursor C, uint I);

    ///<summary>
    /// Returns the translation unit that a cursor originated from.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getTranslationUnit", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXTranslationUnitImpl clang_Cursor_getTranslationUnit(CXCursor param0);

    ///<summary>
    /// If cursor refers to a variable declaration and it has initializer returns cursor referring to the initializer otherwise return null cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_getVarDeclInitializer", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCursor clang_Cursor_getVarDeclInitializer(CXCursor cursor);

    ///<summary>
    /// Determine whether the given cursor has any attributes.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_hasAttrs", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_Cursor_hasAttrs(CXCursor C);

    ///<summary>
    /// If cursor refers to a variable declaration that has external storage returns 1. If cursor refers to a variable declaration that doesn't have external storage returns 0. Otherwise returns -1.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_hasVarDeclExternalStorage", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_Cursor_hasVarDeclExternalStorage(CXCursor cursor);

    ///<summary>
    /// If cursor refers to a variable declaration that has global storage returns 1. If cursor refers to a variable declaration that doesn't have global storage returns 0. Otherwise returns -1.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_hasVarDeclGlobalStorage", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_Cursor_hasVarDeclGlobalStorage(CXCursor cursor);

    ///<summary>
    /// Determine whether the given cursor represents an anonymous tag or namespace
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_isAnonymous", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_Cursor_isAnonymous(CXCursor C);

    ///<summary>
    /// Determine whether the given cursor represents an anonymous record declaration.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_isAnonymousRecordDecl", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_Cursor_isAnonymousRecordDecl(CXCursor C);

    ///<summary>
    /// Returns non-zero if the cursor specifies a Record member that is a bitfield.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_isBitField", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_Cursor_isBitField(CXCursor C);

    ///<summary>
    /// Given a cursor pointing to a C++ method call or an Objective-C message, returns non-zero if the method/message is "dynamic", meaning:
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_isDynamicCall", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_Cursor_isDynamicCall(CXCursor C);

    ///<summary>
    /// Returns non-zero if the given cursor points to a symbol marked with external_source_symbol attribute.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_isExternalSymbol", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_Cursor_isExternalSymbol(CXCursor C, CXString* language, CXString* definedIn, uint* isGenerated);

    ///<summary>
    /// Determine whether a CXCursor that is a function declaration, is an inline declaration.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_isFunctionInlined", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_Cursor_isFunctionInlined(CXCursor C);

    ///<summary>
    /// Determine whether the given cursor represents an inline namespace declaration.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_isInlineNamespace", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_Cursor_isInlineNamespace(CXCursor C);

    ///<summary>
    /// Determine whether a CXCursor that is a macro, is a builtin one.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_isMacroBuiltin", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_Cursor_isMacroBuiltin(CXCursor C);

    ///<summary>
    /// Determine whether a CXCursor that is a macro, is function like.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_isMacroFunctionLike", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_Cursor_isMacroFunctionLike(CXCursor C);

    ///<summary>
    /// Returns non-zero if cursor is null.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_isNull", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_Cursor_isNull(CXCursor cursor);

    ///<summary>
    /// Given a cursor that represents an Objective-C method or property declaration, return non-zero if the declaration was affected by "@optional". Returns zero if the cursor is not such a declaration or it is "@required".
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_isObjCOptional", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_Cursor_isObjCOptional(CXCursor C);

    ///<summary>
    /// Returns non-zero if the given cursor is a variadic function or method.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Cursor_isVariadic", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_Cursor_isVariadic(CXCursor C);

    ///<summary>
    /// Queries a CXCursorSet to see if it contains a specific CXCursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_CXCursorSet_contains", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_CXCursorSet_contains(CXCursorSetImpl cset, CXCursor cursor);

    ///<summary>
    /// Inserts a CXCursor into a CXCursorSet.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_CXCursorSet_insert", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_CXCursorSet_insert(CXCursorSetImpl cset, CXCursor cursor);

    ///<summary>
    /// Gets the general options associated with a CXIndex.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_CXIndex_getGlobalOptions", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_CXIndex_getGlobalOptions(CXIndexImpl param0);

    ///<summary>
    /// Sets general options associated with a CXIndex.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_CXIndex_setGlobalOptions", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_CXIndex_setGlobalOptions(CXIndexImpl param0, uint options);

    ///<summary>
    /// Sets the invocation emission path option in a CXIndex.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_CXIndex_setInvocationEmissionPathOption", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_CXIndex_setInvocationEmissionPathOption(CXIndexImpl param0, sbyte* Path);

    ///<summary>
    /// Determine if a C++ constructor is a converting constructor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_CXXConstructor_isConvertingConstructor", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_CXXConstructor_isConvertingConstructor(CXCursor C);

    ///<summary>
    /// Determine if a C++ constructor is a copy constructor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_CXXConstructor_isCopyConstructor", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_CXXConstructor_isCopyConstructor(CXCursor C);

    ///<summary>
    /// Determine if a C++ constructor is the default constructor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_CXXConstructor_isDefaultConstructor", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_CXXConstructor_isDefaultConstructor(CXCursor C);

    ///<summary>
    /// Determine if a C++ constructor is a move constructor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_CXXConstructor_isMoveConstructor", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_CXXConstructor_isMoveConstructor(CXCursor C);

    ///<summary>
    /// Determine if a C++ field is declared 'mutable'.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_CXXField_isMutable", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_CXXField_isMutable(CXCursor C);

    ///<summary>
    /// Determine if a C++ member function or member function template is declared 'const'.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_CXXMethod_isConst", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_CXXMethod_isConst(CXCursor C);

    ///<summary>
    /// Determine if a C++ member function is a copy-assignment operator, returning 1 if such is the case and 0 otherwise.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_CXXMethod_isCopyAssignmentOperator", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_CXXMethod_isCopyAssignmentOperator(CXCursor C);

    ///<summary>
    /// Determine if a C++ method is declared '= default'.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_CXXMethod_isDefaulted", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_CXXMethod_isDefaulted(CXCursor C);

    ///<summary>
    /// Determine if a C++ method is declared '= delete'.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_CXXMethod_isDeleted", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_CXXMethod_isDeleted(CXCursor C);

    ///<summary>
    /// Determine if a C++ member function is a move-assignment operator, returning 1 if such is the case and 0 otherwise.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_CXXMethod_isMoveAssignmentOperator", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_CXXMethod_isMoveAssignmentOperator(CXCursor C);

    ///<summary>
    /// Determine if a C++ member function or member function template is pure virtual.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_CXXMethod_isPureVirtual", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_CXXMethod_isPureVirtual(CXCursor C);

    ///<summary>
    /// Determine if a C++ member function or member function template is declared 'static'.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_CXXMethod_isStatic", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_CXXMethod_isStatic(CXCursor C);

    ///<summary>
    /// Determine if a C++ member function or member function template is explicitly declared 'virtual' or if it overrides a virtual method from one of the base classes.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_CXXMethod_isVirtual", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_CXXMethod_isVirtual(CXCursor C);

    ///<summary>
    /// Determine if a C++ record is abstract, i.e. whether a class or struct has a pure virtual member function.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_CXXRecord_isAbstract", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_CXXRecord_isAbstract(CXCursor C);

    ///<summary>
    /// Returns a default set of code-completion options that can be passed to clang_codeCompleteAt().
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_defaultCodeCompleteOptions", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_defaultCodeCompleteOptions();

    ///<summary>
    /// Retrieve the set of display options most similar to the default behavior of the clang compiler.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_defaultDiagnosticDisplayOptions", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_defaultDiagnosticDisplayOptions();

    ///<summary>
    /// Returns the set of flags that is suitable for parsing a translation unit that is being edited.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_defaultEditingTranslationUnitOptions", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_defaultEditingTranslationUnitOptions();

    ///<summary>
    /// Returns the set of flags that is suitable for reparsing a translation unit.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_defaultReparseOptions", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_defaultReparseOptions(CXTranslationUnitImpl TU);

    ///<summary>
    /// Returns the set of flags that is suitable for saving a translation unit.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_defaultSaveOptions", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_defaultSaveOptions(CXTranslationUnitImpl TU);

    ///<summary>
    /// Dispose of an APISet.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_disposeAPISet", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_disposeAPISet(CXAPISetImpl api);

    ///<summary>
    /// Free the given set of code-completion results.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_disposeCodeCompleteResults", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_disposeCodeCompleteResults(CXCodeCompleteResults* Results);

    ///<summary>
    /// Disposes a CXCursorSet and releases its associated memory.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_disposeCXCursorSet", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_disposeCXCursorSet(CXCursorSetImpl cset);

    ///<summary>
    /// Free the memory associated with a CXPlatformAvailability structure.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_disposeCXPlatformAvailability", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_disposeCXPlatformAvailability(CXPlatformAvailability* availability);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_disposeCXTUResourceUsage", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_disposeCXTUResourceUsage(CXTUResourceUsage usage);

    ///<summary>
    /// Destroy a diagnostic.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_disposeDiagnostic", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_disposeDiagnostic(CXDiagnosticImpl Diagnostic);

    ///<summary>
    /// Release a CXDiagnosticSet and all of its contained diagnostics.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_disposeDiagnosticSet", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_disposeDiagnosticSet(CXDiagnosticSetImpl Diags);

    ///<summary>
    /// Destroy the given index.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_disposeIndex", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_disposeIndex(CXIndexImpl index);

    ///<summary>
    /// Free the set of overridden cursors returned by clang_getOverriddenCursors().
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_disposeOverriddenCursors", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_disposeOverriddenCursors(CXCursor* overridden);

    ///<summary>
    /// Destroy the given CXSourceRangeList.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_disposeSourceRangeList", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_disposeSourceRangeList(CXSourceRangeList* ranges);

    ///<summary>
    /// Free the given string.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_disposeString", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_disposeString(CXString @string);

    ///<summary>
    /// Free the given string set.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_disposeStringSet", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_disposeStringSet(CXStringSet* set);

    ///<summary>
    /// Free the given set of tokens.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_disposeTokens", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_disposeTokens(CXTranslationUnitImpl TU, CXToken* Tokens, uint NumTokens);

    ///<summary>
    /// Destroy the specified CXTranslationUnit object.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_disposeTranslationUnit", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_disposeTranslationUnit(CXTranslationUnitImpl param0);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_enableStackTraces", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_enableStackTraces();

    ///<summary>
    /// Determine if an enum declaration refers to a scoped enum.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_EnumDecl_isScoped", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_EnumDecl_isScoped(CXCursor C);

    ///<summary>
    /// Determine whether two cursors are equivalent.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_equalCursors", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_equalCursors(CXCursor param0, CXCursor param1);

    ///<summary>
    /// Determine whether two source locations, which must refer into the same translation unit, refer to exactly the same point in the source code.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_equalLocations", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_equalLocations(CXSourceLocation loc1, CXSourceLocation loc2);

    ///<summary>
    /// Determine whether two ranges are equivalent.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_equalRanges", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_equalRanges(CXSourceRange range1, CXSourceRange range2);

    ///<summary>
    /// Determine whether two CXTypes represent the same type.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_equalTypes", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_equalTypes(CXType A, CXType B);

    ///<summary>
    /// Disposes the created Eval memory.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_EvalResult_dispose", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_EvalResult_dispose(CXEvalResultImpl E);

    ///<summary>
    /// Returns the evaluation result as double if the kind is double.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_EvalResult_getAsDouble", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern double clang_EvalResult_getAsDouble(CXEvalResultImpl E);

    ///<summary>
    /// Returns the evaluation result as integer if the kind is Int.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_EvalResult_getAsInt", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_EvalResult_getAsInt(CXEvalResultImpl E);

    ///<summary>
    /// Returns the evaluation result as a long long integer if the kind is Int. This prevents overflows that may happen if the result is returned with clang_EvalResult_getAsInt.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_EvalResult_getAsLongLong", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern long clang_EvalResult_getAsLongLong(CXEvalResultImpl E);

    ///<summary>
    /// Returns the evaluation result as a constant string if the kind is other than Int or float. User must not free this pointer, instead call clang_EvalResult_dispose on the CXEvalResult returned by clang_Cursor_Evaluate.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_EvalResult_getAsStr", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte* clang_EvalResult_getAsStr(CXEvalResultImpl E);

    ///<summary>
    /// Returns the evaluation result as an unsigned integer if the kind is Int and clang_EvalResult_isUnsignedInt is non-zero.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_EvalResult_getAsUnsigned", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong clang_EvalResult_getAsUnsigned(CXEvalResultImpl E);

    ///<summary>
    /// Returns the kind of the evaluated result.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_EvalResult_getKind", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXEvalResultKind clang_EvalResult_getKind(CXEvalResultImpl E);

    ///<summary>
    /// Returns a non-zero value if the kind is Int and the evaluation result resulted in an unsigned integer.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_EvalResult_isUnsignedInt", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_EvalResult_isUnsignedInt(CXEvalResultImpl E);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_executeOnThread", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_executeOnThread(void* fn, void* user_data, uint stack_size);

    ///<summary>
    /// Returns non-zero if the file1 and file2 point to the same file, or they are both NULL.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_File_isEqual", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_File_isEqual(CXFileImpl file1, CXFileImpl file2);

    ///<summary>
    /// Returns the real path name of file.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_File_tryGetRealPathName", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_File_tryGetRealPathName(CXFileImpl file);

    ///<summary>
    /// Find #import/#include directives in a specific file.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_findIncludesInFile", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXResult clang_findIncludesInFile(CXTranslationUnitImpl TU, CXFileImpl file, CXCursorAndRangeVisitor visitor);

    ///<summary>
    /// Find references of a declaration in a specific file.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_findReferencesInFile", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXResult clang_findReferencesInFile(CXCursor cursor, CXFileImpl file, CXCursorAndRangeVisitor visitor);

    ///<summary>
    /// Format the given diagnostic in a manner that is suitable for display.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_formatDiagnostic", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_formatDiagnostic(CXDiagnosticImpl Diagnostic, uint Options);

    ///<summary>
    /// free memory allocated by libclang, such as the buffer returned by CXVirtualFileOverlay() or clang_ModuleMapDescriptor_writeToBuffer().
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_free", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_free(void* buffer);

    ///<summary>
    /// Convert a given full parsed comment to an HTML fragment.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_FullComment_getAsHTML", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_FullComment_getAsHTML(CXComment Comment);

    ///<summary>
    /// Convert a given full parsed comment to an XML document.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_FullComment_getAsXML", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_FullComment_getAsXML(CXComment Comment);

    ///<summary>
    /// Returns the address space of the given type.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getAddressSpace", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_getAddressSpace(CXType T);

    ///<summary>
    /// Retrieve all ranges from all files that were skipped by the preprocessor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getAllSkippedRanges", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXSourceRangeList* clang_getAllSkippedRanges(CXTranslationUnitImpl tu);

    ///<summary>
    /// Retrieve the type of a parameter of a function type.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getArgType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_getArgType(CXType T, uint i);

    ///<summary>
    /// Return the element type of an array type.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getArrayElementType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_getArrayElementType(CXType T);

    ///<summary>
    /// Return the array size of a constant array.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getArraySize", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern long clang_getArraySize(CXType T);

    ///<summary>
    /// Return the timestamp for use with Clang's -fbuild-session-timestamp= option.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getBuildSessionTimestamp", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong clang_getBuildSessionTimestamp();

    ///<summary>
    /// Retrieve the canonical cursor corresponding to the given cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCanonicalCursor", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCursor clang_getCanonicalCursor(CXCursor param0);

    ///<summary>
    /// Return the canonical type for a CXType.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCanonicalType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_getCanonicalType(CXType T);

    ///<summary>
    /// Retrieve the child diagnostics of a CXDiagnostic.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getChildDiagnostics", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXDiagnosticSetImpl clang_getChildDiagnostics(CXDiagnosticImpl D);

    ///<summary>
    /// Return a version string, suitable for showing to a user, but not intended to be parsed (the format is not guaranteed to be stable).
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getClangVersion", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getClangVersion();

    ///<summary>
    /// Retrieve the annotation associated with the given completion string.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCompletionAnnotation", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getCompletionAnnotation(CXCompletionStringImpl completion_string, uint annotation_number);

    ///<summary>
    /// Determine the availability of the entity that this code-completion string refers to.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCompletionAvailability", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXAvailabilityKind clang_getCompletionAvailability(CXCompletionStringImpl completion_string);

    ///<summary>
    /// Retrieve the brief documentation comment attached to the declaration that corresponds to the given completion string.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCompletionBriefComment", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getCompletionBriefComment(CXCompletionStringImpl completion_string);

    ///<summary>
    /// Retrieve the completion string associated with a particular chunk within a completion string.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCompletionChunkCompletionString", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCompletionStringImpl clang_getCompletionChunkCompletionString(CXCompletionStringImpl completion_string, uint chunk_number);

    ///<summary>
    /// Determine the kind of a particular chunk within a completion string.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCompletionChunkKind", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCompletionChunkKind clang_getCompletionChunkKind(CXCompletionStringImpl completion_string, uint chunk_number);

    ///<summary>
    /// Retrieve the text associated with a particular chunk within a completion string.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCompletionChunkText", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getCompletionChunkText(CXCompletionStringImpl completion_string, uint chunk_number);

    ///<summary>
    /// Fix-its that *must* be applied before inserting the text for the corresponding completion.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCompletionFixIt", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getCompletionFixIt(CXCodeCompleteResults* results, uint completion_index, uint fixit_index, CXSourceRange* replacement_range);

    ///<summary>
    /// Retrieve the number of annotations associated with the given completion string.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCompletionNumAnnotations", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_getCompletionNumAnnotations(CXCompletionStringImpl completion_string);

    ///<summary>
    /// Retrieve the number of fix-its for the given completion index.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCompletionNumFixIts", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_getCompletionNumFixIts(CXCodeCompleteResults* results, uint completion_index);

    ///<summary>
    /// Retrieve the parent context of the given completion string.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCompletionParent", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getCompletionParent(CXCompletionStringImpl completion_string, CXCursorKind* kind);

    ///<summary>
    /// Determine the priority of this code completion.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCompletionPriority", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_getCompletionPriority(CXCompletionStringImpl completion_string);

    ///<summary>
    /// Retrieve the character data associated with the given string.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCString", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte* clang_getCString(CXString @string);

    ///<summary>
    /// Map a source location to the cursor that describes the entity at that location in the source code.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursor", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCursor clang_getCursor(CXTranslationUnitImpl param0, CXSourceLocation param1);

    ///<summary>
    /// Determine the availability of the entity that this cursor refers to, taking the current target platform into account.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorAvailability", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXAvailabilityKind clang_getCursorAvailability(CXCursor cursor);

    ///<summary>
    /// Retrieve a completion string for an arbitrary declaration or macro definition cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorCompletionString", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCompletionStringImpl clang_getCursorCompletionString(CXCursor cursor);

    ///<summary>
    /// For a cursor that is either a reference to or a declaration of some entity, retrieve a cursor that describes the definition of that entity.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorDefinition", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCursor clang_getCursorDefinition(CXCursor param0);

    ///<summary>
    /// Retrieve the display name for the entity referenced by this cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorDisplayName", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getCursorDisplayName(CXCursor param0);

    ///<summary>
    /// Retrieve the exception specification type associated with a given cursor. This is a value of type CXCursor_ExceptionSpecificationKind.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorExceptionSpecificationType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_getCursorExceptionSpecificationType(CXCursor C);

    ///<summary>
    /// Retrieve the physical extent of the source construct referenced by the given cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorExtent", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXSourceRange clang_getCursorExtent(CXCursor param0);

    ///<summary>
    /// Retrieve the kind of the given cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorKind", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCursorKind clang_getCursorKind(CXCursor param0);

    ///<summary>
    /// for debug/testing
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorKindSpelling", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getCursorKindSpelling(CXCursorKind Kind);

    ///<summary>
    /// Determine the "language" of the entity referred to by a given cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorLanguage", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXLanguageKind clang_getCursorLanguage(CXCursor cursor);

    ///<summary>
    /// Determine the lexical parent of the given cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorLexicalParent", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCursor clang_getCursorLexicalParent(CXCursor cursor);

    ///<summary>
    /// Determine the linkage of the entity referred to by a given cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorLinkage", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXLinkageKind clang_getCursorLinkage(CXCursor cursor);

    ///<summary>
    /// Retrieve the physical location of the source constructor referenced by the given cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorLocation", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXSourceLocation clang_getCursorLocation(CXCursor param0);

    ///<summary>
    /// Determine the availability of the entity that this cursor refers to on any platforms for which availability information is known.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorPlatformAvailability", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_getCursorPlatformAvailability(CXCursor cursor, int* always_deprecated, CXString* deprecated_message, int* always_unavailable, CXString* unavailable_message, CXPlatformAvailability* availability, int availability_size);

    ///<summary>
    /// Pretty print declarations.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorPrettyPrinted", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getCursorPrettyPrinted(CXCursor Cursor, CXPrintingPolicyImpl Policy);

    ///<summary>
    /// Retrieve the default policy for the cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorPrintingPolicy", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXPrintingPolicyImpl clang_getCursorPrintingPolicy(CXCursor param0);

    ///<summary>
    /// For a cursor that is a reference, retrieve a cursor representing the entity that it references.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorReferenced", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCursor clang_getCursorReferenced(CXCursor param0);

    ///<summary>
    /// Given a cursor that references something else, return the source range covering that reference.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorReferenceNameRange", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXSourceRange clang_getCursorReferenceNameRange(CXCursor C, uint NameFlags, uint PieceIndex);

    ///<summary>
    /// Retrieve the return type associated with a given cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorResultType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_getCursorResultType(CXCursor C);

    ///<summary>
    /// Determine the semantic parent of the given cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorSemanticParent", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCursor clang_getCursorSemanticParent(CXCursor cursor);

    ///<summary>
    /// Retrieve a name for the entity referenced by this cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorSpelling", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getCursorSpelling(CXCursor param0);

    ///<summary>
    /// Determine the "thread-local storage (TLS) kind" of the declaration referred to by a cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorTLSKind", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXTLSKind clang_getCursorTLSKind(CXCursor cursor);

    ///<summary>
    /// Retrieve the type of a CXCursor (if any).
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_getCursorType(CXCursor C);

    ///<summary>
    /// Retrieve a Unified Symbol Resolution (USR) for the entity referenced by the given cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorUSR", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getCursorUSR(CXCursor param0);

    ///<summary>
    /// Describe the visibility of the entity referred to by a cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCursorVisibility", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXVisibilityKind clang_getCursorVisibility(CXCursor cursor);

    ///<summary>
    /// Return the memory usage of a translation unit. This object should be released with clang_disposeCXTUResourceUsage().
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCXTUResourceUsage", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXTUResourceUsage clang_getCXTUResourceUsage(CXTranslationUnitImpl TU);

    ///<summary>
    /// Returns the access control level for the referenced object.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getCXXAccessSpecifier", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CX_CXXAccessSpecifier clang_getCXXAccessSpecifier(CXCursor param0);

    ///<summary>
    /// Returns the Objective-C type encoding for the specified declaration.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getDeclObjCTypeEncoding", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getDeclObjCTypeEncoding(CXCursor C);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getDefinitionSpellingAndExtent", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_getDefinitionSpellingAndExtent(CXCursor param0, sbyte** startBuf, sbyte** endBuf, uint* startLine, uint* startColumn, uint* endLine, uint* endColumn);

    ///<summary>
    /// Retrieve a diagnostic associated with the given translation unit.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getDiagnostic", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXDiagnosticImpl clang_getDiagnostic(CXTranslationUnitImpl Unit, uint Index);

    ///<summary>
    /// Retrieve the category number for this diagnostic.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getDiagnosticCategory", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_getDiagnosticCategory(CXDiagnosticImpl param0);

    ///<summary>
    /// Retrieve the name of a particular diagnostic category. This is now deprecated. Use clang_getDiagnosticCategoryText() instead.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getDiagnosticCategoryName", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getDiagnosticCategoryName(uint Category);

    ///<summary>
    /// Retrieve the diagnostic category text for a given diagnostic.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getDiagnosticCategoryText", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getDiagnosticCategoryText(CXDiagnosticImpl param0);

    ///<summary>
    /// Retrieve the replacement information for a given fix-it.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getDiagnosticFixIt", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getDiagnosticFixIt(CXDiagnosticImpl Diagnostic, uint FixIt, CXSourceRange* ReplacementRange);

    ///<summary>
    /// Retrieve a diagnostic associated with the given CXDiagnosticSet.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getDiagnosticInSet", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXDiagnosticImpl clang_getDiagnosticInSet(CXDiagnosticSetImpl Diags, uint Index);

    ///<summary>
    /// Retrieve the source location of the given diagnostic.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getDiagnosticLocation", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXSourceLocation clang_getDiagnosticLocation(CXDiagnosticImpl param0);

    ///<summary>
    /// Determine the number of fix-it hints associated with the given diagnostic.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getDiagnosticNumFixIts", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_getDiagnosticNumFixIts(CXDiagnosticImpl Diagnostic);

    ///<summary>
    /// Determine the number of source ranges associated with the given diagnostic.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getDiagnosticNumRanges", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_getDiagnosticNumRanges(CXDiagnosticImpl param0);

    ///<summary>
    /// Retrieve the name of the command-line option that enabled this diagnostic.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getDiagnosticOption", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getDiagnosticOption(CXDiagnosticImpl Diag, CXString* Disable);

    ///<summary>
    /// Retrieve a source range associated with the diagnostic.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getDiagnosticRange", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXSourceRange clang_getDiagnosticRange(CXDiagnosticImpl Diagnostic, uint Range);

    ///<summary>
    /// Retrieve the complete set of diagnostics associated with a translation unit.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getDiagnosticSetFromTU", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXDiagnosticSetImpl clang_getDiagnosticSetFromTU(CXTranslationUnitImpl Unit);

    ///<summary>
    /// Determine the severity of the given diagnostic.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getDiagnosticSeverity", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXDiagnosticSeverity clang_getDiagnosticSeverity(CXDiagnosticImpl param0);

    ///<summary>
    /// Retrieve the text of the given diagnostic.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getDiagnosticSpelling", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getDiagnosticSpelling(CXDiagnosticImpl param0);

    ///<summary>
    /// Return the element type of an array, complex, or vector type.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getElementType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_getElementType(CXType T);

    ///<summary>
    /// Retrieve the integer value of an enum constant declaration as an unsigned long long.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getEnumConstantDeclUnsignedValue", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong clang_getEnumConstantDeclUnsignedValue(CXCursor C);

    ///<summary>
    /// Retrieve the integer value of an enum constant declaration as a signed long long.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getEnumConstantDeclValue", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern long clang_getEnumConstantDeclValue(CXCursor C);

    ///<summary>
    /// Retrieve the integer type of an enum declaration.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getEnumDeclIntegerType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_getEnumDeclIntegerType(CXCursor C);

    ///<summary>
    /// Retrieve the exception specification type associated with a function type. This is a value of type CXCursor_ExceptionSpecificationKind.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getExceptionSpecificationType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_getExceptionSpecificationType(CXType T);

    ///<summary>
    /// Retrieve the file, line, column, and offset represented by the given source location.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getExpansionLocation", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_getExpansionLocation(CXSourceLocation location, out CXFileImpl file, out uint line, out uint column, out uint offset);

    ///<summary>
    /// Retrieve the bit width of a bit field declaration as an integer.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getFieldDeclBitWidth", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_getFieldDeclBitWidth(CXCursor C);

    ///<summary>
    /// Retrieve a file handle within the given translation unit.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getFile", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXFileImpl clang_getFile(CXTranslationUnitImpl tu, sbyte* file_name);

    ///<summary>
    /// Retrieve the buffer associated with the given file.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getFileContents", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte* clang_getFileContents(CXTranslationUnitImpl tu, CXFileImpl file, out ulong size);

    ///<summary>
    /// Retrieve the file, line, column, and offset represented by the given source location.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getFileLocation", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_getFileLocation(CXSourceLocation location, out CXFileImpl file, out uint line, out uint column, out uint offset);

    ///<summary>
    /// Retrieve the complete file and path name of the given file.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getFileName", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getFileName(CXFileImpl SFile);

    ///<summary>
    /// Retrieve the last modification time of the given file.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getFileTime", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern long clang_getFileTime(CXFileImpl SFile);

    ///<summary>
    /// Retrieve the unique ID for the given file.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getFileUniqueID", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_getFileUniqueID(CXFileImpl file, out CXFileUniqueID outID);

    ///<summary>
    /// Retrieve the calling convention associated with a function type.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getFunctionTypeCallingConv", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCallingConv clang_getFunctionTypeCallingConv(CXType T);

    ///<summary>
    /// For cursors representing an iboutletcollection attribute, this function returns the collection element type.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getIBOutletCollectionType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_getIBOutletCollectionType(CXCursor param0);

    ///<summary>
    /// Retrieve the file that is included by the given inclusion directive cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getIncludedFile", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXFileImpl clang_getIncludedFile(CXCursor cursor);

    ///<summary>
    /// Visit the set of preprocessor inclusions in a translation unit. The visitor function is called with the provided data for every included file. This does not include headers included by the PCH file (unless one is inspecting the inclusions in the PCH file itself).
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getInclusions", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_getInclusions(CXTranslationUnitImpl tu, void* visitor, CXClientDataImpl client_data);

    ///<summary>
    /// Legacy API to retrieve the file, line, column, and offset represented by the given source location.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getInstantiationLocation", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_getInstantiationLocation(CXSourceLocation location, out CXFileImpl file, out uint line, out uint column, out uint offset);

    ///<summary>
    /// Retrieves the source location associated with a given file/line/column in a particular translation unit.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getLocation", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXSourceLocation clang_getLocation(CXTranslationUnitImpl tu, CXFileImpl file, uint line, uint column);

    ///<summary>
    /// Retrieves the source location associated with a given character offset in a particular translation unit.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getLocationForOffset", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXSourceLocation clang_getLocationForOffset(CXTranslationUnitImpl tu, CXFileImpl file, uint offset);

    ///<summary>
    /// Given a CXFile header file, return the module that contains it, if one exists.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getModuleForFile", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXModuleImpl clang_getModuleForFile(CXTranslationUnitImpl param0, CXFileImpl param1);

    ///<summary>
    /// For reference types (e.g., "const int&"), returns the type that the reference refers to (e.g "const int").
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getNonReferenceType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_getNonReferenceType(CXType CT);

    ///<summary>
    /// Retrieve the NULL cursor, which represents no entity.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getNullCursor", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCursor clang_getNullCursor();

    ///<summary>
    /// Retrieve a NULL (invalid) source location.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getNullLocation", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXSourceLocation clang_getNullLocation();

    ///<summary>
    /// Retrieve a NULL (invalid) source range.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getNullRange", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXSourceRange clang_getNullRange();

    ///<summary>
    /// Retrieve the number of non-variadic parameters associated with a function type.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getNumArgTypes", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_getNumArgTypes(CXType T);

    ///<summary>
    /// Retrieve the number of chunks in the given code-completion string.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getNumCompletionChunks", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_getNumCompletionChunks(CXCompletionStringImpl completion_string);

    ///<summary>
    /// Determine the number of diagnostics produced for the given translation unit.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getNumDiagnostics", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_getNumDiagnostics(CXTranslationUnitImpl Unit);

    ///<summary>
    /// Determine the number of diagnostics in a CXDiagnosticSet.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getNumDiagnosticsInSet", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_getNumDiagnosticsInSet(CXDiagnosticSetImpl Diags);

    ///<summary>
    /// Return the number of elements of an array or vector type.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getNumElements", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern long clang_getNumElements(CXType T);

    ///<summary>
    /// Determine the number of overloaded declarations referenced by a CXCursor_OverloadedDeclRef cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getNumOverloadedDecls", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_getNumOverloadedDecls(CXCursor cursor);

    ///<summary>
    /// Retrieve a cursor for one of the overloaded declarations referenced by a CXCursor_OverloadedDeclRef cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getOverloadedDecl", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCursor clang_getOverloadedDecl(CXCursor cursor, uint index);

    ///<summary>
    /// Determine the set of methods that are overridden by the given method.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getOverriddenCursors", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_getOverriddenCursors(CXCursor cursor, CXCursor* overridden, out uint num_overridden);

    ///<summary>
    /// For pointer types, returns the type of the pointee.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getPointeeType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_getPointeeType(CXType T);

    ///<summary>
    /// Retrieve the file, line and column represented by the given source location, as specified in a # line directive.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getPresumedLocation", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_getPresumedLocation(CXSourceLocation location, out CXString filename, out uint line, out uint column);

    ///<summary>
    /// Retrieve a source range given the beginning and ending source locations.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getRange", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXSourceRange clang_getRange(CXSourceLocation begin, CXSourceLocation end);

    ///<summary>
    /// Retrieve a source location representing the last character within a source range.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getRangeEnd", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXSourceLocation clang_getRangeEnd(CXSourceRange range);

    ///<summary>
    /// Retrieve a source location representing the first character within a source range.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getRangeStart", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXSourceLocation clang_getRangeStart(CXSourceRange range);

    ///<summary>
    /// Retrieve a remapping.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getRemappings", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXRemappingImpl clang_getRemappings(sbyte* path);

    ///<summary>
    /// Retrieve a remapping.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getRemappingsFromFileList", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXRemappingImpl clang_getRemappingsFromFileList(sbyte** filePaths, uint numFiles);

    ///<summary>
    /// Retrieve the return type associated with a function type.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getResultType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_getResultType(CXType T);

    ///<summary>
    /// Retrieve all ranges that were skipped by the preprocessor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getSkippedRanges", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXSourceRangeList* clang_getSkippedRanges(CXTranslationUnitImpl tu, CXFileImpl file);

    ///<summary>
    /// Given a cursor that may represent a specialization or instantiation of a template, retrieve the cursor that represents the template that it specializes or from which it was instantiated.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getSpecializedCursorTemplate", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCursor clang_getSpecializedCursorTemplate(CXCursor C);

    ///<summary>
    /// Retrieve the file, line, column, and offset represented by the given source location.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getSpellingLocation", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_getSpellingLocation(CXSourceLocation location, out CXFileImpl file, out uint line, out uint column, out uint offset);

    ///<summary>
    /// Generate a single symbol symbol graph for the declaration at the given cursor. Returns a null string if the AST node for the cursor isn't a declaration.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getSymbolGraphForCursor", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getSymbolGraphForCursor(CXCursor cursor);

    ///<summary>
    /// Generate a single symbol symbol graph for the given USR. Returns a null string if the associated symbol can not be found in the provided CXAPISet.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getSymbolGraphForUSR", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getSymbolGraphForUSR(sbyte* usr, CXAPISetImpl api);

    ///<summary>
    /// Given a cursor that represents a template, determine the cursor kind of the specializations would be generated by instantiating the template.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getTemplateCursorKind", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCursorKind clang_getTemplateCursorKind(CXCursor C);

    ///<summary>
    /// Get the raw lexical token starting with the given location.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getToken", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXToken* clang_getToken(CXTranslationUnitImpl TU, CXSourceLocation Location);

    ///<summary>
    /// Retrieve a source range that covers the given token.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getTokenExtent", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXSourceRange clang_getTokenExtent(CXTranslationUnitImpl param0, CXToken param1);

    ///<summary>
    /// Determine the kind of the given token.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getTokenKind", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXTokenKind clang_getTokenKind(CXToken param0);

    ///<summary>
    /// Retrieve the source location of the given token.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getTokenLocation", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXSourceLocation clang_getTokenLocation(CXTranslationUnitImpl param0, CXToken param1);

    ///<summary>
    /// Determine the spelling of the given token.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getTokenSpelling", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getTokenSpelling(CXTranslationUnitImpl param0, CXToken param1);

    ///<summary>
    /// Retrieve the cursor that represents the given translation unit.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getTranslationUnitCursor", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCursor clang_getTranslationUnitCursor(CXTranslationUnitImpl param0);

    ///<summary>
    /// Get the original translation unit source file name.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getTranslationUnitSpelling", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getTranslationUnitSpelling(CXTranslationUnitImpl CTUnit);

    ///<summary>
    /// Get target information for this translation unit.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getTranslationUnitTargetInfo", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXTargetInfoImpl clang_getTranslationUnitTargetInfo(CXTranslationUnitImpl CTUnit);

    ///<summary>
    /// Returns the human-readable null-terminated C string that represents the name of the memory category. This string should never be freed.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getTUResourceUsageName", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern sbyte* clang_getTUResourceUsageName(CXTUResourceUsageKind kind);

    ///<summary>
    /// Return the cursor for the declaration of the given type.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getTypeDeclaration", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCursor clang_getTypeDeclaration(CXType T);

    ///<summary>
    /// Retrieve the underlying type of a typedef declaration.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getTypedefDeclUnderlyingType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_getTypedefDeclUnderlyingType(CXCursor C);

    ///<summary>
    /// Returns the typedef name of the given type.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getTypedefName", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getTypedefName(CXType CT);

    ///<summary>
    /// Retrieve the spelling of a given CXTypeKind.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getTypeKindSpelling", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getTypeKindSpelling(CXTypeKind K);

    ///<summary>
    /// Pretty-print the underlying type using the rules of the language of the translation unit from which it came.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getTypeSpelling", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_getTypeSpelling(CXType CT);

    ///<summary>
    /// Retrieve the unqualified variant of the given type, removing as little sugar as possible.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_getUnqualifiedType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_getUnqualifiedType(CXType CT);

    ///<summary>
    /// Compute a hash value for the given cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_hashCursor", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_hashCursor(CXCursor param0);

    ///<summary>
    /// Returns name of the specified attribute.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_HTMLStartTag_getAttrName", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_HTMLStartTag_getAttrName(CXComment Comment, uint AttrIdx);

    ///<summary>
    /// Returns value of the specified attribute.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_HTMLStartTag_getAttrValue", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_HTMLStartTag_getAttrValue(CXComment Comment, uint AttrIdx);

    ///<summary>
    /// Returns number of attributes (name-value pairs) attached to the start tag.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_HTMLStartTag_getNumAttrs", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_HTMLStartTag_getNumAttrs(CXComment Comment);

    ///<summary>
    /// Returns non-zero if tag is self-closing (for example, <br />).
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_HTMLStartTagComment_isSelfClosing", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_HTMLStartTagComment_isSelfClosing(CXComment Comment);

    ///<summary>
    /// Convert an HTML tag AST node to string.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_HTMLTagComment_getAsString", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_HTMLTagComment_getAsString(CXComment Comment);

    ///<summary>
    /// Returns HTML tag name.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_HTMLTagComment_getTagName", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_HTMLTagComment_getTagName(CXComment Comment);

    ///<summary>
    /// For retrieving a custom CXIdxClientContainer attached to a container.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_index_getClientContainer", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXIdxClientContainerImpl clang_index_getClientContainer(CXIdxContainerInfo* param0);

    ///<summary>
    /// For retrieving a custom CXIdxClientEntity attached to an entity.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_index_getClientEntity", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXIdxClientEntityImpl clang_index_getClientEntity(CXIdxEntityInfo* param0);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_index_getCXXClassDeclInfo", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXIdxCXXClassDeclInfo* clang_index_getCXXClassDeclInfo(CXIdxDeclInfo* param0);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_index_getIBOutletCollectionAttrInfo", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXIdxIBOutletCollectionAttrInfo* clang_index_getIBOutletCollectionAttrInfo(CXIdxAttrInfo* param0);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_index_getObjCCategoryDeclInfo", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXIdxObjCCategoryDeclInfo* clang_index_getObjCCategoryDeclInfo(CXIdxDeclInfo* param0);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_index_getObjCContainerDeclInfo", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXIdxObjCContainerDeclInfo* clang_index_getObjCContainerDeclInfo(CXIdxDeclInfo* param0);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_index_getObjCInterfaceDeclInfo", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXIdxObjCInterfaceDeclInfo* clang_index_getObjCInterfaceDeclInfo(CXIdxDeclInfo* param0);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_index_getObjCPropertyDeclInfo", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXIdxObjCPropertyDeclInfo* clang_index_getObjCPropertyDeclInfo(CXIdxDeclInfo* param0);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_index_getObjCProtocolRefListInfo", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXIdxObjCProtocolRefListInfo* clang_index_getObjCProtocolRefListInfo(CXIdxDeclInfo* param0);

    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_index_isEntityObjCContainerKind", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_index_isEntityObjCContainerKind(CXIdxEntityKind param0);

    ///<summary>
    /// For setting a custom CXIdxClientContainer attached to a container.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_index_setClientContainer", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_index_setClientContainer(CXIdxContainerInfo* param0, CXIdxClientContainerImpl param1);

    ///<summary>
    /// For setting a custom CXIdxClientEntity attached to an entity.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_index_setClientEntity", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_index_setClientEntity(CXIdxEntityInfo* param0, CXIdxClientEntityImpl param1);

    ///<summary>
    /// An indexing action/session, to be applied to one or multiple translation units.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_IndexAction_create", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXIndexActionImpl clang_IndexAction_create(CXIndexImpl CIdx);

    ///<summary>
    /// Destroy the given index action.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_IndexAction_dispose", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_IndexAction_dispose(CXIndexActionImpl param0);

    ///<summary>
    /// Retrieve the CXSourceLocation represented by the given CXIdxLoc.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_indexLoc_getCXSourceLocation", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXSourceLocation clang_indexLoc_getCXSourceLocation(CXIdxLoc loc);

    ///<summary>
    /// Retrieve the CXIdxFile, file, line, column, and offset represented by the given CXIdxLoc.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_indexLoc_getFileLocation", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_indexLoc_getFileLocation(CXIdxLoc loc, out CXIdxClientFileImpl indexFile, out CXFileImpl file, out uint line, out uint column, out uint offset);

    ///<summary>
    /// Index the given source file and the translation unit corresponding to that file via callbacks implemented through #IndexerCallbacks.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_indexSourceFile", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_indexSourceFile(CXIndexActionImpl param0, CXClientDataImpl client_data, IndexerCallbacks* index_callbacks, uint index_callbacks_size, uint index_options, sbyte* source_filename, sbyte** command_line_args, int num_command_line_args, CXUnsavedFile* unsaved_files, uint num_unsaved_files, out CXTranslationUnitImpl out_TU, uint TU_options);

    ///<summary>
    /// Same as clang_indexSourceFile but requires a full command line for command_line_args including argv[0]. This is useful if the standard library paths are relative to the binary.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_indexSourceFileFullArgv", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_indexSourceFileFullArgv(CXIndexActionImpl param0, CXClientDataImpl client_data, IndexerCallbacks* index_callbacks, uint index_callbacks_size, uint index_options, sbyte* source_filename, sbyte** command_line_args, int num_command_line_args, CXUnsavedFile* unsaved_files, uint num_unsaved_files, out CXTranslationUnitImpl out_TU, uint TU_options);

    ///<summary>
    /// Index the given translation unit via callbacks implemented through #IndexerCallbacks.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_indexTranslationUnit", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_indexTranslationUnit(CXIndexActionImpl param0, CXClientDataImpl client_data, IndexerCallbacks* index_callbacks, uint index_callbacks_size, uint index_options, CXTranslationUnitImpl param5);

    ///<summary>
    /// Returns text of the specified argument.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_InlineCommandComment_getArgText", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_InlineCommandComment_getArgText(CXComment Comment, uint ArgIdx);

    ///<summary>
    /// Returns name of the inline command.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_InlineCommandComment_getCommandName", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_InlineCommandComment_getCommandName(CXComment Comment);

    ///<summary>
    /// Returns number of command arguments.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_InlineCommandComment_getNumArgs", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_InlineCommandComment_getNumArgs(CXComment Comment);

    ///<summary>
    /// Returns the most appropriate rendering mode, chosen on command semantics in Doxygen.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_InlineCommandComment_getRenderKind", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCommentInlineCommandRenderKind clang_InlineCommandComment_getRenderKind(CXComment Comment);

    ///<summary>
    /// Returns non-zero if Comment is inline content and has a newline immediately following it in the comment text. Newlines between paragraphs do not count.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_InlineContentComment_hasTrailingNewline", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_InlineContentComment_hasTrailingNewline(CXComment Comment);

    ///<summary>
    /// Determine whether the given cursor kind represents an attribute.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_isAttribute", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_isAttribute(CXCursorKind param0);

    ///<summary>
    /// Determine whether a CXType has the "const" qualifier set, without looking through typedefs that may have added "const" at a different level.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_isConstQualifiedType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_isConstQualifiedType(CXType T);

    ///<summary>
    /// Determine whether the declaration pointed to by this cursor is also a definition of that entity.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_isCursorDefinition", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_isCursorDefinition(CXCursor param0);

    ///<summary>
    /// Determine whether the given cursor kind represents a declaration.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_isDeclaration", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_isDeclaration(CXCursorKind param0);

    ///<summary>
    /// Determine whether the given cursor kind represents an expression.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_isExpression", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_isExpression(CXCursorKind param0);

    ///<summary>
    /// Determine whether the given header is guarded against multiple inclusions, either with the conventional #ifndef/#define/#endif macro guards or with #pragma once.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_isFileMultipleIncludeGuarded", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_isFileMultipleIncludeGuarded(CXTranslationUnitImpl tu, CXFileImpl file);

    ///<summary>
    /// Return 1 if the CXType is a variadic function type, and 0 otherwise.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_isFunctionTypeVariadic", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_isFunctionTypeVariadic(CXType T);

    ///<summary>
    /// Determine whether the given cursor kind represents an invalid cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_isInvalid", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_isInvalid(CXCursorKind param0);

    ///<summary>
    /// Determine whether the given declaration is invalid.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_isInvalidDeclaration", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_isInvalidDeclaration(CXCursor param0);

    ///<summary>
    /// Return 1 if the CXType is a POD (plain old data) type, and 0 otherwise.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_isPODType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_isPODType(CXType T);

    ///<summary>
    /// * Determine whether the given cursor represents a preprocessing element, such as a preprocessor directive or macro instantiation.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_isPreprocessing", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_isPreprocessing(CXCursorKind param0);

    ///<summary>
    /// Determine whether the given cursor kind represents a simple reference.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_isReference", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_isReference(CXCursorKind param0);

    ///<summary>
    /// Determine whether a CXType has the "restrict" qualifier set, without looking through typedefs that may have added "restrict" at a different level.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_isRestrictQualifiedType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_isRestrictQualifiedType(CXType T);

    ///<summary>
    /// Determine whether the given cursor kind represents a statement.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_isStatement", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_isStatement(CXCursorKind param0);

    ///<summary>
    /// Determine whether the given cursor kind represents a translation unit.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_isTranslationUnit", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_isTranslationUnit(CXCursorKind param0);

    ///<summary>
    /// * Determine whether the given cursor represents a currently unexposed piece of the AST (e.g., CXCursor_UnexposedStmt).
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_isUnexposed", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_isUnexposed(CXCursorKind param0);

    ///<summary>
    /// Returns 1 if the base class specified by the cursor with kind CX_CXXBaseSpecifier is virtual.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_isVirtualBase", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_isVirtualBase(CXCursor param0);

    ///<summary>
    /// Determine whether a CXType has the "volatile" qualifier set, without looking through typedefs that may have added "volatile" at a different level.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_isVolatileQualifiedType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_isVolatileQualifiedType(CXType T);

    ///<summary>
    /// Deserialize a set of diagnostics from a Clang diagnostics bitcode file.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_loadDiagnostics", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXDiagnosticSetImpl clang_loadDiagnostics(sbyte* file, out CXLoadDiag_Error error, out CXString errorString);

    ///<summary>
    /// Returns non-zero if the given source location is in the main file of the corresponding translation unit.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Location_isFromMainFile", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_Location_isFromMainFile(CXSourceLocation location);

    ///<summary>
    /// Returns non-zero if the given source location is in a system header.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Location_isInSystemHeader", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_Location_isInSystemHeader(CXSourceLocation location);

    ///<summary>
    /// Returns the module file where the provided module object came from.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Module_getASTFile", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXFileImpl clang_Module_getASTFile(CXModuleImpl Module);

    ///<summary>
    /// Returns the full name of the module, e.g. "std.vector".
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Module_getFullName", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_Module_getFullName(CXModuleImpl Module);

    ///<summary>
    /// Returns the name of the module, e.g. for the 'std.vector' sub-module it will return "vector".
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Module_getName", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_Module_getName(CXModuleImpl Module);

    ///<summary>
    /// Returns the number of top level headers associated with this module.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Module_getNumTopLevelHeaders", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_Module_getNumTopLevelHeaders(CXTranslationUnitImpl param0, CXModuleImpl Module);

    ///<summary>
    /// Returns the parent of a sub-module or NULL if the given module is top-level, e.g. for 'std.vector' it will return the 'std' module.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Module_getParent", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXModuleImpl clang_Module_getParent(CXModuleImpl Module);

    ///<summary>
    /// Returns the specified top level header associated with the module.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Module_getTopLevelHeader", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXFileImpl clang_Module_getTopLevelHeader(CXTranslationUnitImpl param0, CXModuleImpl Module, uint Index);

    ///<summary>
    /// Returns non-zero if the module is a system one.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Module_isSystem", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_Module_isSystem(CXModuleImpl Module);

    ///<summary>
    /// Create a CXModuleMapDescriptor object. Must be disposed with clang_ModuleMapDescriptor_dispose().
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_ModuleMapDescriptor_create", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXModuleMapDescriptorImpl clang_ModuleMapDescriptor_create(uint options);

    ///<summary>
    /// Dispose a CXModuleMapDescriptor object.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_ModuleMapDescriptor_dispose", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_ModuleMapDescriptor_dispose(CXModuleMapDescriptorImpl param0);

    ///<summary>
    /// Sets the framework module name that the module.map describes.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_ModuleMapDescriptor_setFrameworkModuleName", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXErrorCode clang_ModuleMapDescriptor_setFrameworkModuleName(CXModuleMapDescriptorImpl param0, sbyte* name);

    ///<summary>
    /// Sets the umbrella header name that the module.map describes.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_ModuleMapDescriptor_setUmbrellaHeader", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXErrorCode clang_ModuleMapDescriptor_setUmbrellaHeader(CXModuleMapDescriptorImpl param0, sbyte* name);

    ///<summary>
    /// Write out the CXModuleMapDescriptor object to a char buffer.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_ModuleMapDescriptor_writeToBuffer", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXErrorCode clang_ModuleMapDescriptor_writeToBuffer(CXModuleMapDescriptorImpl param0, uint options, out sbyte* out_buffer_ptr, out uint out_buffer_size);

    ///<summary>
    /// Returns parameter passing direction.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_ParamCommandComment_getDirection", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCommentParamPassDirection clang_ParamCommandComment_getDirection(CXComment Comment);

    ///<summary>
    /// Returns zero-based parameter index in function prototype.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_ParamCommandComment_getParamIndex", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_ParamCommandComment_getParamIndex(CXComment Comment);

    ///<summary>
    /// Returns parameter name.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_ParamCommandComment_getParamName", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_ParamCommandComment_getParamName(CXComment Comment);

    ///<summary>
    /// Returns non-zero if parameter passing direction was specified explicitly in the comment.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_ParamCommandComment_isDirectionExplicit", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_ParamCommandComment_isDirectionExplicit(CXComment Comment);

    ///<summary>
    /// Returns non-zero if the parameter that this AST node represents was found in the function prototype and clang_ParamCommandComment_getParamIndex function will return a meaningful value.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_ParamCommandComment_isParamIndexValid", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_ParamCommandComment_isParamIndexValid(CXComment Comment);

    ///<summary>
    /// Same as clang_parseTranslationUnit2, but returns the CXTranslationUnit instead of an error code. In case of an error this routine returns a NULL CXTranslationUnit, without further detailed error codes.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_parseTranslationUnit", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXTranslationUnitImpl clang_parseTranslationUnit(CXIndexImpl CIdx, sbyte* source_filename, sbyte** command_line_args, int num_command_line_args, CXUnsavedFile* unsaved_files, uint num_unsaved_files, uint options);

    ///<summary>
    /// Parse the given source file and the translation unit corresponding to that file.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_parseTranslationUnit2", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXErrorCode clang_parseTranslationUnit2(CXIndexImpl CIdx, sbyte* source_filename, sbyte** command_line_args, int num_command_line_args, CXUnsavedFile* unsaved_files, uint num_unsaved_files, uint options, out CXTranslationUnitImpl out_TU);

    ///<summary>
    /// Same as clang_parseTranslationUnit2 but requires a full command line for command_line_args including argv[0]. This is useful if the standard library paths are relative to the binary.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_parseTranslationUnit2FullArgv", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXErrorCode clang_parseTranslationUnit2FullArgv(CXIndexImpl CIdx, sbyte* source_filename, sbyte** command_line_args, int num_command_line_args, CXUnsavedFile* unsaved_files, uint num_unsaved_files, uint options, out CXTranslationUnitImpl out_TU);

    ///<summary>
    /// Release a printing policy.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_PrintingPolicy_dispose", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_PrintingPolicy_dispose(CXPrintingPolicyImpl Policy);

    ///<summary>
    /// Get a property value for the given printing policy.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_PrintingPolicy_getProperty", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_PrintingPolicy_getProperty(CXPrintingPolicyImpl Policy, CXPrintingPolicyProperty Property);

    ///<summary>
    /// Set a property value for the given printing policy.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_PrintingPolicy_setProperty", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_PrintingPolicy_setProperty(CXPrintingPolicyImpl Policy, CXPrintingPolicyProperty Property, uint Value);

    ///<summary>
    /// Returns non-zero if range is null.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Range_isNull", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_Range_isNull(CXSourceRange range);

    ///<summary>
    /// Dispose the remapping.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_remap_dispose", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_remap_dispose(CXRemappingImpl param0);

    ///<summary>
    /// Get the original and the associated filename from the remapping.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_remap_getFilenames", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_remap_getFilenames(CXRemappingImpl param0, uint index, out CXString original, out CXString transformed);

    ///<summary>
    /// Determine the number of remappings.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_remap_getNumFiles", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_remap_getNumFiles(CXRemappingImpl param0);

    ///<summary>
    /// Reparse the source files that produced this translation unit.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_reparseTranslationUnit", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_reparseTranslationUnit(CXTranslationUnitImpl TU, uint num_unsaved_files, CXUnsavedFile* unsaved_files, uint options);

    ///<summary>
    /// Saves a translation unit into a serialized representation of that translation unit on disk.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_saveTranslationUnit", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_saveTranslationUnit(CXTranslationUnitImpl TU, sbyte* FileName, uint options);

    ///<summary>
    /// Sort the code-completion results in case-insensitive alphabetical order.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_sortCodeCompletionResults", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_sortCodeCompletionResults(CXCodeCompleteResults* Results, uint NumResults);

    ///<summary>
    /// Suspend a translation unit in order to free memory associated with it.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_suspendTranslationUnit", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_suspendTranslationUnit(CXTranslationUnitImpl param0);

    ///<summary>
    /// Destroy the CXTargetInfo object.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_TargetInfo_dispose", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_TargetInfo_dispose(CXTargetInfoImpl Info);

    ///<summary>
    /// Get the pointer width of the target in bits.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_TargetInfo_getPointerWidth", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_TargetInfo_getPointerWidth(CXTargetInfoImpl Info);

    ///<summary>
    /// Get the normalized target triple as a string.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_TargetInfo_getTriple", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_TargetInfo_getTriple(CXTargetInfoImpl Info);

    ///<summary>
    /// Returns text contained in the AST node.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_TextComment_getText", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_TextComment_getText(CXComment Comment);

    ///<summary>
    /// Enable/disable crash recovery.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_toggleCrashRecovery", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_toggleCrashRecovery(uint isEnabled);

    ///<summary>
    /// Tokenize the source code described by the given range into raw lexical tokens.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_tokenize", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_tokenize(CXTranslationUnitImpl TU, CXSourceRange Range, CXToken* Tokens, out uint NumTokens);

    ///<summary>
    /// Returns zero-based nesting depth of this parameter in the template parameter list.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_TParamCommandComment_getDepth", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_TParamCommandComment_getDepth(CXComment Comment);

    ///<summary>
    /// Returns zero-based parameter index in the template parameter list at a given nesting depth.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_TParamCommandComment_getIndex", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_TParamCommandComment_getIndex(CXComment Comment, uint Depth);

    ///<summary>
    /// Returns template parameter name.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_TParamCommandComment_getParamName", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_TParamCommandComment_getParamName(CXComment Comment);

    ///<summary>
    /// Returns non-zero if the parameter that this AST node represents was found in the template parameter list and clang_TParamCommandComment_getDepth and clang_TParamCommandComment_getIndex functions will return a meaningful value.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_TParamCommandComment_isParamPositionValid", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_TParamCommandComment_isParamPositionValid(CXComment Comment);

    ///<summary>
    /// Return the alignment of a type in bytes as per C++[expr.alignof] standard.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Type_getAlignOf", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern long clang_Type_getAlignOf(CXType T);

    ///<summary>
    /// Return the class type of an member pointer type.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Type_getClassType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_Type_getClassType(CXType T);

    ///<summary>
    /// Retrieve the ref-qualifier kind of a function or method.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Type_getCXXRefQualifier", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXRefQualifierKind clang_Type_getCXXRefQualifier(CXType T);

    ///<summary>
    /// Return the type that was modified by this attributed type.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Type_getModifiedType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_Type_getModifiedType(CXType T);

    ///<summary>
    /// Retrieve the type named by the qualified-id.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Type_getNamedType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_Type_getNamedType(CXType T);

    ///<summary>
    /// Retrieve the nullability kind of a pointer type.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Type_getNullability", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXTypeNullabilityKind clang_Type_getNullability(CXType T);

    ///<summary>
    /// Retrieve the number of protocol references associated with an ObjC object/id.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Type_getNumObjCProtocolRefs", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_Type_getNumObjCProtocolRefs(CXType T);

    ///<summary>
    /// Retrieve the number of type arguments associated with an ObjC object.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Type_getNumObjCTypeArgs", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_Type_getNumObjCTypeArgs(CXType T);

    ///<summary>
    /// Returns the number of template arguments for given template specialization, or -1 if type T is not a template specialization.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Type_getNumTemplateArguments", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int clang_Type_getNumTemplateArguments(CXType T);

    ///<summary>
    /// Returns the Objective-C type encoding for the specified CXType.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Type_getObjCEncoding", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_Type_getObjCEncoding(CXType type);

    ///<summary>
    /// Retrieves the base type of the ObjCObjectType.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Type_getObjCObjectBaseType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_Type_getObjCObjectBaseType(CXType T);

    ///<summary>
    /// Retrieve the decl for a protocol reference for an ObjC object/id.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Type_getObjCProtocolDecl", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXCursor clang_Type_getObjCProtocolDecl(CXType T, uint i);

    ///<summary>
    /// Retrieve a type argument associated with an ObjC object.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Type_getObjCTypeArg", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_Type_getObjCTypeArg(CXType T, uint i);

    ///<summary>
    /// Return the offset of a field named S in a record of type T in bits as it would be returned by __offsetof__ as per C++11[18.2p4]
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Type_getOffsetOf", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern long clang_Type_getOffsetOf(CXType T, sbyte* S);

    ///<summary>
    /// Return the size of a type in bytes as per C++[expr.sizeof] standard.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Type_getSizeOf", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern long clang_Type_getSizeOf(CXType T);

    ///<summary>
    /// Returns the type template argument of a template class specialization at given index.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Type_getTemplateArgumentAsType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_Type_getTemplateArgumentAsType(CXType T, uint i);

    ///<summary>
    /// Gets the type contained by this atomic type.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Type_getValueType", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXType clang_Type_getValueType(CXType CT);

    ///<summary>
    /// Determine if a typedef is 'transparent' tag.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Type_isTransparentTagTypedef", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_Type_isTransparentTagTypedef(CXType T);

    ///<summary>
    /// Visit the fields of a particular type.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_Type_visitFields", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_Type_visitFields(CXType T, void* visitor, CXClientDataImpl client_data);

    ///<summary>
    /// Returns text contained in the AST node.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_VerbatimBlockLineComment_getText", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_VerbatimBlockLineComment_getText(CXComment Comment);

    ///<summary>
    /// Returns text contained in the AST node.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_VerbatimLineComment_getText", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXString clang_VerbatimLineComment_getText(CXComment Comment);

    ///<summary>
    /// Map an absolute virtual file path to an absolute real one. The virtual path must be canonicalized (not contain "."/"..").
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_VirtualFileOverlay_addFileMapping", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXErrorCode clang_VirtualFileOverlay_addFileMapping(CXVirtualFileOverlayImpl param0, sbyte* virtualPath, sbyte* realPath);

    ///<summary>
    /// Create a CXVirtualFileOverlay object. Must be disposed with clang_VirtualFileOverlay_dispose().
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_VirtualFileOverlay_create", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXVirtualFileOverlayImpl clang_VirtualFileOverlay_create(uint options);

    ///<summary>
    /// Dispose a CXVirtualFileOverlay object.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_VirtualFileOverlay_dispose", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clang_VirtualFileOverlay_dispose(CXVirtualFileOverlayImpl param0);

    ///<summary>
    /// Set the case sensitivity for the CXVirtualFileOverlay object. The CXVirtualFileOverlay object is case-sensitive by default, this option can be used to override the default.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_VirtualFileOverlay_setCaseSensitivity", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXErrorCode clang_VirtualFileOverlay_setCaseSensitivity(CXVirtualFileOverlayImpl param0, int caseSensitive);

    ///<summary>
    /// Write out the CXVirtualFileOverlay object to a char buffer.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_VirtualFileOverlay_writeToBuffer", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern CXErrorCode clang_VirtualFileOverlay_writeToBuffer(CXVirtualFileOverlayImpl param0, uint options, out sbyte* out_buffer_ptr, out uint out_buffer_size);

    ///<summary>
    /// Visit the children of a particular cursor.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [DllImport(LibraryPath, EntryPoint = "clang_visitChildren", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint clang_visitChildren(CXCursor parent, void* visitor, CXClientDataImpl client_data);

}


