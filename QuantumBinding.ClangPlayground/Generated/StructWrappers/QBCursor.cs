
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBCursor
{
    public QBCursor()
    {
    }

    public QBCursor(QuantumBinding.Clang.Interop.CXCursor _internal)
    {
        Kind = _internal.kind;
        Xdata = _internal.xdata;
        Data = new void*[3];
        for (int i = 0; i < 3; ++i)
        {
            Data[i] = _internal.data[i];
        }
    }

    public CXCursorKind Kind { get; set; }
    public int Xdata { get; set; }
    public void*[] Data { get; set; }
    ///<summary>
    /// If cursor is a statement declaration tries to evaluate the statement and if its variable, tries to evaluate its initializer, into its corresponding type. If it's an expression, tries to evaluate the expression.
    ///</summary>
    public QBEvalResult Cursor_Evaluate()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_Evaluate(ToNative());
    }

    ///<summary>
    /// Retrieve the argument cursor of a function or method.
    ///</summary>
    public QBCursor Cursor_getArgument(uint i)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getArgument(ToNative(), i);
    }

    ///<summary>
    /// Given a cursor that represents a documentable entity (e.g., declaration), return the associated first paragraph.
    ///</summary>
    public QBString Cursor_getBriefCommentText()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getBriefCommentText(ToNative());
    }

    ///<summary>
    /// Given a cursor that represents a declaration, return the associated comment's source range. The range may include multiple consecutive comments with whitespace in between.
    ///</summary>
    public QBSourceRange Cursor_getCommentRange()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getCommentRange(ToNative());
    }

    ///<summary>
    /// Retrieve the CXStrings representing the mangled symbols of the C++ constructor or destructor at the cursor.
    ///</summary>
    public QBStringSet Cursor_getCXXManglings()
    {
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getCXXManglings(ToNative());
        var wrappedResult = new QBStringSet(*result);
        NativeUtils.Free(result);
        return wrappedResult;
    }

    ///<summary>
    /// Retrieve the CXString representing the mangled name of the cursor.
    ///</summary>
    public QBString Cursor_getMangling()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getMangling(ToNative());
    }

    ///<summary>
    /// Given a CXCursor_ModuleImportDecl cursor, return the associated module.
    ///</summary>
    public QBModule Cursor_getModule()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getModule(ToNative());
    }

    ///<summary>
    /// Retrieve the number of non-variadic arguments associated with a given cursor.
    ///</summary>
    public int Cursor_getNumArguments()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getNumArguments(ToNative());
    }

    ///<summary>
    /// Returns the number of template args of a function, struct, or class decl representing a template specialization.
    ///</summary>
    public int Cursor_getNumTemplateArguments()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getNumTemplateArguments(ToNative());
    }

    ///<summary>
    /// Given a cursor that represents an Objective-C method or parameter declaration, return the associated Objective-C qualifiers for the return type or the parameter respectively. The bits are formed from CXObjCDeclQualifierKind.
    ///</summary>
    public uint Cursor_getObjCDeclQualifiers()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getObjCDeclQualifiers(ToNative());
    }

    ///<summary>
    /// Retrieve the CXStrings representing the mangled symbols of the ObjC class interface or implementation at the cursor.
    ///</summary>
    public QBStringSet Cursor_getObjCManglings()
    {
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getObjCManglings(ToNative());
        var wrappedResult = new QBStringSet(*result);
        NativeUtils.Free(result);
        return wrappedResult;
    }

    ///<summary>
    /// Given a cursor that represents a property declaration, return the associated property attributes. The bits are formed from CXObjCPropertyAttrKind.
    ///</summary>
    public uint Cursor_getObjCPropertyAttributes(uint reserved)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getObjCPropertyAttributes(ToNative(), reserved);
    }

    ///<summary>
    /// Given a cursor that represents a property declaration, return the name of the method that implements the getter.
    ///</summary>
    public QBString Cursor_getObjCPropertyGetterName()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getObjCPropertyGetterName(ToNative());
    }

    ///<summary>
    /// Given a cursor that represents a property declaration, return the name of the method that implements the setter, if any.
    ///</summary>
    public QBString Cursor_getObjCPropertySetterName()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getObjCPropertySetterName(ToNative());
    }

    ///<summary>
    /// If the cursor points to a selector identifier in an Objective-C method or message expression, this returns the selector index.
    ///</summary>
    public int Cursor_getObjCSelectorIndex()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getObjCSelectorIndex(ToNative());
    }

    ///<summary>
    /// Return the offset of the field represented by the Cursor.
    ///</summary>
    public long Cursor_getOffsetOfField()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getOffsetOfField(ToNative());
    }

    ///<summary>
    /// Given a cursor that represents a documentable entity (e.g., declaration), return the associated parsed comment as a CXComment_FullComment AST node.
    ///</summary>
    public QBComment Cursor_getParsedComment()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getParsedComment(ToNative());
    }

    ///<summary>
    /// Given a cursor that represents a declaration, return the associated comment text, including comment markers.
    ///</summary>
    public QBString Cursor_getRawCommentText()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getRawCommentText(ToNative());
    }

    ///<summary>
    /// Given a cursor pointing to an Objective-C message or property reference, or C++ method call, returns the CXType of the receiver.
    ///</summary>
    public QBType Cursor_getReceiverType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getReceiverType(ToNative());
    }

    ///<summary>
    /// Retrieve a range for a piece that forms the cursors spelling name. Most of the times there is only one range for the complete spelling but for Objective-C methods and Objective-C message expressions, there are multiple pieces for each selector identifier.
    ///</summary>
    public QBSourceRange Cursor_getSpellingNameRange(uint pieceIndex, uint options)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getSpellingNameRange(ToNative(), pieceIndex, options);
    }

    ///<summary>
    /// Returns the storage class for a function or variable declaration.
    ///</summary>
    public CX_StorageClass Cursor_getStorageClass()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getStorageClass(ToNative());
    }

    ///<summary>
    /// Retrieve the kind of the I'th template argument of the CXCursor C.
    ///</summary>
    public CXTemplateArgumentKind Cursor_getTemplateArgumentKind(uint I)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getTemplateArgumentKind(ToNative(), I);
    }

    ///<summary>
    /// Retrieve a CXType representing the type of a TemplateArgument of a function decl representing a template specialization.
    ///</summary>
    public QBType Cursor_getTemplateArgumentType(uint I)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getTemplateArgumentType(ToNative(), I);
    }

    ///<summary>
    /// Retrieve the value of an Integral TemplateArgument (of a function decl representing a template specialization) as an unsigned long long.
    ///</summary>
    public ulong Cursor_getTemplateArgumentUnsignedValue(uint I)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getTemplateArgumentUnsignedValue(ToNative(), I);
    }

    ///<summary>
    /// Retrieve the value of an Integral TemplateArgument (of a function decl representing a template specialization) as a signed long long.
    ///</summary>
    public long Cursor_getTemplateArgumentValue(uint I)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getTemplateArgumentValue(ToNative(), I);
    }

    ///<summary>
    /// Returns the translation unit that a cursor originated from.
    ///</summary>
    public QBTranslationUnit Cursor_getTranslationUnit()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getTranslationUnit(ToNative());
    }

    ///<summary>
    /// If cursor refers to a variable declaration and it has initializer returns cursor referring to the initializer otherwise return null cursor.
    ///</summary>
    public QBCursor Cursor_getVarDeclInitializer()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_getVarDeclInitializer(ToNative());
    }

    ///<summary>
    /// Determine whether the given cursor has any attributes.
    ///</summary>
    public uint Cursor_hasAttrs()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_hasAttrs(ToNative());
    }

    ///<summary>
    /// If cursor refers to a variable declaration that has external storage returns 1. If cursor refers to a variable declaration that doesn't have external storage returns 0. Otherwise returns -1.
    ///</summary>
    public int Cursor_hasVarDeclExternalStorage()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_hasVarDeclExternalStorage(ToNative());
    }

    ///<summary>
    /// If cursor refers to a variable declaration that has global storage returns 1. If cursor refers to a variable declaration that doesn't have global storage returns 0. Otherwise returns -1.
    ///</summary>
    public int Cursor_hasVarDeclGlobalStorage()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_hasVarDeclGlobalStorage(ToNative());
    }

    ///<summary>
    /// Determine whether the given cursor represents an anonymous tag or namespace
    ///</summary>
    public uint Cursor_isAnonymous()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_isAnonymous(ToNative());
    }

    ///<summary>
    /// Determine whether the given cursor represents an anonymous record declaration.
    ///</summary>
    public uint Cursor_isAnonymousRecordDecl()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_isAnonymousRecordDecl(ToNative());
    }

    ///<summary>
    /// Returns non-zero if the cursor specifies a Record member that is a bitfield.
    ///</summary>
    public uint Cursor_isBitField()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_isBitField(ToNative());
    }

    ///<summary>
    /// Given a cursor pointing to a C++ method call or an Objective-C message, returns non-zero if the method/message is "dynamic", meaning:
    ///</summary>
    public int Cursor_isDynamicCall()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_isDynamicCall(ToNative());
    }

    ///<summary>
    /// Returns non-zero if the given cursor points to a symbol marked with external_source_symbol attribute.
    ///</summary>
    public uint Cursor_isExternalSymbol(QBString language, QBString definedIn, ref uint isGenerated)
    {
        var arg1 = ReferenceEquals(language, null) ? null : NativeUtils.StructOrEnumToPointer(language.ToNative());
        var arg2 = ReferenceEquals(definedIn, null) ? null : NativeUtils.StructOrEnumToPointer(definedIn.ToNative());
        var arg3 = NativeUtils.StructOrEnumToPointer(isGenerated);
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_isExternalSymbol(ToNative(), arg1, arg2, arg3);
        language?.Dispose();
        NativeUtils.Free(arg1);
        definedIn?.Dispose();
        NativeUtils.Free(arg2);
        isGenerated = *arg3;
        NativeUtils.Free(arg3);
        return result;
    }

    ///<summary>
    /// Determine whether a CXCursor that is a function declaration, is an inline declaration.
    ///</summary>
    public uint Cursor_isFunctionInlined()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_isFunctionInlined(ToNative());
    }

    ///<summary>
    /// Determine whether the given cursor represents an inline namespace declaration.
    ///</summary>
    public uint Cursor_isInlineNamespace()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_isInlineNamespace(ToNative());
    }

    ///<summary>
    /// Determine whether a CXCursor that is a macro, is a builtin one.
    ///</summary>
    public uint Cursor_isMacroBuiltin()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_isMacroBuiltin(ToNative());
    }

    ///<summary>
    /// Determine whether a CXCursor that is a macro, is function like.
    ///</summary>
    public uint Cursor_isMacroFunctionLike()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_isMacroFunctionLike(ToNative());
    }

    ///<summary>
    /// Returns non-zero if cursor is null.
    ///</summary>
    public int Cursor_isNull()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_isNull(ToNative());
    }

    ///<summary>
    /// Given a cursor that represents an Objective-C method or property declaration, return non-zero if the declaration was affected by "@optional". Returns zero if the cursor is not such a declaration or it is "@required".
    ///</summary>
    public uint Cursor_isObjCOptional()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_isObjCOptional(ToNative());
    }

    ///<summary>
    /// Returns non-zero if the given cursor is a variadic function or method.
    ///</summary>
    public uint Cursor_isVariadic()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Cursor_isVariadic(ToNative());
    }

    ///<summary>
    /// Determine if a C++ constructor is a converting constructor.
    ///</summary>
    public uint CXXConstructor_isConvertingConstructor()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_CXXConstructor_isConvertingConstructor(ToNative());
    }

    ///<summary>
    /// Determine if a C++ constructor is a copy constructor.
    ///</summary>
    public uint CXXConstructor_isCopyConstructor()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_CXXConstructor_isCopyConstructor(ToNative());
    }

    ///<summary>
    /// Determine if a C++ constructor is the default constructor.
    ///</summary>
    public uint CXXConstructor_isDefaultConstructor()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_CXXConstructor_isDefaultConstructor(ToNative());
    }

    ///<summary>
    /// Determine if a C++ constructor is a move constructor.
    ///</summary>
    public uint CXXConstructor_isMoveConstructor()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_CXXConstructor_isMoveConstructor(ToNative());
    }

    ///<summary>
    /// Determine if a C++ field is declared 'mutable'.
    ///</summary>
    public uint CXXField_isMutable()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_CXXField_isMutable(ToNative());
    }

    ///<summary>
    /// Determine if a C++ member function or member function template is declared 'const'.
    ///</summary>
    public uint CXXMethod_isConst()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_CXXMethod_isConst(ToNative());
    }

    ///<summary>
    /// Determine if a C++ member function is a copy-assignment operator, returning 1 if such is the case and 0 otherwise.
    ///</summary>
    public uint CXXMethod_isCopyAssignmentOperator()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_CXXMethod_isCopyAssignmentOperator(ToNative());
    }

    ///<summary>
    /// Determine if a C++ method is declared '= default'.
    ///</summary>
    public uint CXXMethod_isDefaulted()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_CXXMethod_isDefaulted(ToNative());
    }

    ///<summary>
    /// Determine if a C++ method is declared '= delete'.
    ///</summary>
    public uint CXXMethod_isDeleted()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_CXXMethod_isDeleted(ToNative());
    }

    ///<summary>
    /// Determine if a C++ member function is a move-assignment operator, returning 1 if such is the case and 0 otherwise.
    ///</summary>
    public uint CXXMethod_isMoveAssignmentOperator()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_CXXMethod_isMoveAssignmentOperator(ToNative());
    }

    ///<summary>
    /// Determine if a C++ member function or member function template is pure virtual.
    ///</summary>
    public uint CXXMethod_isPureVirtual()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_CXXMethod_isPureVirtual(ToNative());
    }

    ///<summary>
    /// Determine if a C++ member function or member function template is declared 'static'.
    ///</summary>
    public uint CXXMethod_isStatic()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_CXXMethod_isStatic(ToNative());
    }

    ///<summary>
    /// Determine if a C++ member function or member function template is explicitly declared 'virtual' or if it overrides a virtual method from one of the base classes.
    ///</summary>
    public uint CXXMethod_isVirtual()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_CXXMethod_isVirtual(ToNative());
    }

    ///<summary>
    /// Determine if a C++ record is abstract, i.e. whether a class or struct has a pure virtual member function.
    ///</summary>
    public uint CXXRecord_isAbstract()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_CXXRecord_isAbstract(ToNative());
    }

    ///<summary>
    /// Free the set of overridden cursors returned by clang_getOverriddenCursors().
    ///</summary>
    public void disposeOverriddenCursors()
    {
        var arg0 = NativeUtils.StructOrEnumToPointer(ToNative());
        QuantumBinding.Clang.Interop.ClangInterop.clang_disposeOverriddenCursors(arg0);
        NativeUtils.Free(arg0);
    }

    ///<summary>
    /// Determine if an enum declaration refers to a scoped enum.
    ///</summary>
    public uint EnumDecl_isScoped()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_EnumDecl_isScoped(ToNative());
    }

    ///<summary>
    /// Determine whether two cursors are equivalent.
    ///</summary>
    public uint equalCursors(QBCursor param1)
    {
        var arg1 = ReferenceEquals(param1, null) ? new QuantumBinding.Clang.Interop.CXCursor() : param1.ToNative();
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_equalCursors(ToNative(), arg1);
        return result;
    }

    ///<summary>
    /// Find references of a declaration in a specific file.
    ///</summary>
    public CXResult findReferencesInFile(QBFile file, QBCursorAndRangeVisitor visitor)
    {
        var arg1 = ReferenceEquals(file, null) ? new CXFileImpl() : (CXFileImpl)file;
        var arg2 = ReferenceEquals(visitor, null) ? new QuantumBinding.Clang.Interop.CXCursorAndRangeVisitor() : visitor.ToNative();
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_findReferencesInFile(ToNative(), arg1, arg2);
        visitor?.Dispose();
        return result;
    }

    ///<summary>
    /// Retrieve the canonical cursor corresponding to the given cursor.
    ///</summary>
    public QBCursor getCanonicalCursor()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCanonicalCursor(ToNative());
    }

    ///<summary>
    /// Determine the availability of the entity that this cursor refers to, taking the current target platform into account.
    ///</summary>
    public CXAvailabilityKind getCursorAvailability()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorAvailability(ToNative());
    }

    ///<summary>
    /// Retrieve a completion string for an arbitrary declaration or macro definition cursor.
    ///</summary>
    public QBCompletionString getCursorCompletionString()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorCompletionString(ToNative());
    }

    ///<summary>
    /// For a cursor that is either a reference to or a declaration of some entity, retrieve a cursor that describes the definition of that entity.
    ///</summary>
    public QBCursor getCursorDefinition()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorDefinition(ToNative());
    }

    ///<summary>
    /// Retrieve the display name for the entity referenced by this cursor.
    ///</summary>
    public QBString getCursorDisplayName()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorDisplayName(ToNative());
    }

    ///<summary>
    /// Retrieve the exception specification type associated with a given cursor. This is a value of type CXCursor_ExceptionSpecificationKind.
    ///</summary>
    public int getCursorExceptionSpecificationType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorExceptionSpecificationType(ToNative());
    }

    ///<summary>
    /// Retrieve the physical extent of the source construct referenced by the given cursor.
    ///</summary>
    public QBSourceRange getCursorExtent()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorExtent(ToNative());
    }

    ///<summary>
    /// Retrieve the kind of the given cursor.
    ///</summary>
    public CXCursorKind getCursorKind()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorKind(ToNative());
    }

    ///<summary>
    /// Determine the "language" of the entity referred to by a given cursor.
    ///</summary>
    public CXLanguageKind getCursorLanguage()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorLanguage(ToNative());
    }

    ///<summary>
    /// Determine the lexical parent of the given cursor.
    ///</summary>
    public QBCursor getCursorLexicalParent()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorLexicalParent(ToNative());
    }

    ///<summary>
    /// Determine the linkage of the entity referred to by a given cursor.
    ///</summary>
    public CXLinkageKind getCursorLinkage()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorLinkage(ToNative());
    }

    ///<summary>
    /// Retrieve the physical location of the source constructor referenced by the given cursor.
    ///</summary>
    public QBSourceLocation getCursorLocation()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorLocation(ToNative());
    }

    ///<summary>
    /// Determine the availability of the entity that this cursor refers to on any platforms for which availability information is known.
    ///</summary>
    public int getCursorPlatformAvailability(int always_deprecated, QBString deprecated_message, ref int always_unavailable, QBString unavailable_message, QBPlatformAvailability[] availability, int availability_size)
    {
        var arg1 = NativeUtils.StructOrEnumToPointer(always_deprecated);
        var arg2 = ReferenceEquals(deprecated_message, null) ? null : NativeUtils.StructOrEnumToPointer(deprecated_message.ToNative());
        var arg3 = NativeUtils.StructOrEnumToPointer(always_unavailable);
        var arg4 = ReferenceEquals(unavailable_message, null) ? null : NativeUtils.StructOrEnumToPointer(unavailable_message.ToNative());
        var arg5 = ReferenceEquals(availability, null) ? null : NativeUtils.GetPointerToManagedArray<QuantumBinding.Clang.Interop.CXPlatformAvailability>(availability.Length);
        if (!ReferenceEquals(availability, null))
        {
            for (var i = 0U; i < availability.Length; ++i)
            {
                arg5[i] = availability[i].ToNative();
            }
        }
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorPlatformAvailability(ToNative(), arg1, arg2, arg3, arg4, arg5, availability_size);
        deprecated_message?.Dispose();
        NativeUtils.Free(arg2);
        always_unavailable = *arg3;
        NativeUtils.Free(arg3);
        unavailable_message?.Dispose();
        NativeUtils.Free(arg4);
        return result;
    }

    ///<summary>
    /// Pretty print declarations.
    ///</summary>
    public QBString getCursorPrettyPrinted(QBPrintingPolicy Policy)
    {
        var arg1 = ReferenceEquals(Policy, null) ? new CXPrintingPolicyImpl() : (CXPrintingPolicyImpl)Policy;
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorPrettyPrinted(ToNative(), arg1);
    }

    ///<summary>
    /// Retrieve the default policy for the cursor.
    ///</summary>
    public QBPrintingPolicy getCursorPrintingPolicy()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorPrintingPolicy(ToNative());
    }

    ///<summary>
    /// For a cursor that is a reference, retrieve a cursor representing the entity that it references.
    ///</summary>
    public QBCursor getCursorReferenced()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorReferenced(ToNative());
    }

    ///<summary>
    /// Given a cursor that references something else, return the source range covering that reference.
    ///</summary>
    public QBSourceRange getCursorReferenceNameRange(uint NameFlags, uint PieceIndex)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorReferenceNameRange(ToNative(), NameFlags, PieceIndex);
    }

    ///<summary>
    /// Retrieve the return type associated with a given cursor.
    ///</summary>
    public QBType getCursorResultType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorResultType(ToNative());
    }

    ///<summary>
    /// Determine the semantic parent of the given cursor.
    ///</summary>
    public QBCursor getCursorSemanticParent()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorSemanticParent(ToNative());
    }

    ///<summary>
    /// Retrieve a name for the entity referenced by this cursor.
    ///</summary>
    public QBString getCursorSpelling()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorSpelling(ToNative());
    }

    ///<summary>
    /// Determine the "thread-local storage (TLS) kind" of the declaration referred to by a cursor.
    ///</summary>
    public CXTLSKind getCursorTLSKind()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorTLSKind(ToNative());
    }

    ///<summary>
    /// Retrieve the type of a CXCursor (if any).
    ///</summary>
    public QBType getCursorType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorType(ToNative());
    }

    ///<summary>
    /// Retrieve a Unified Symbol Resolution (USR) for the entity referenced by the given cursor.
    ///</summary>
    public QBString getCursorUSR()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorUSR(ToNative());
    }

    ///<summary>
    /// Describe the visibility of the entity referred to by a cursor.
    ///</summary>
    public CXVisibilityKind getCursorVisibility()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorVisibility(ToNative());
    }

    ///<summary>
    /// Returns the access control level for the referenced object.
    ///</summary>
    public CX_CXXAccessSpecifier getCXXAccessSpecifier()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCXXAccessSpecifier(ToNative());
    }

    ///<summary>
    /// Returns the Objective-C type encoding for the specified declaration.
    ///</summary>
    public QBString getDeclObjCTypeEncoding()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getDeclObjCTypeEncoding(ToNative());
    }

    public void getDefinitionSpellingAndExtent(in string[] startBuf, in string[] endBuf, ref uint startLine, ref uint startColumn, ref uint endLine, ref uint endColumn)
    {
        var arg1 = (sbyte**)NativeUtils.GetPointerToStringArray((uint)startBuf.Length, false);
        var arg2 = (sbyte**)NativeUtils.GetPointerToStringArray((uint)endBuf.Length, false);
        var arg3 = NativeUtils.StructOrEnumToPointer(startLine);
        var arg4 = NativeUtils.StructOrEnumToPointer(startColumn);
        var arg5 = NativeUtils.StructOrEnumToPointer(endLine);
        var arg6 = NativeUtils.StructOrEnumToPointer(endColumn);
        QuantumBinding.Clang.Interop.ClangInterop.clang_getDefinitionSpellingAndExtent(ToNative(), arg1, arg2, arg3, arg4, arg5, arg6);
        NativeUtils.Free(arg1);
        NativeUtils.Free(arg2);
        startLine = *arg3;
        NativeUtils.Free(arg3);
        startColumn = *arg4;
        NativeUtils.Free(arg4);
        endLine = *arg5;
        NativeUtils.Free(arg5);
        endColumn = *arg6;
        NativeUtils.Free(arg6);
    }

    ///<summary>
    /// Retrieve the integer value of an enum constant declaration as an unsigned long long.
    ///</summary>
    public ulong getEnumConstantDeclUnsignedValue()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getEnumConstantDeclUnsignedValue(ToNative());
    }

    ///<summary>
    /// Retrieve the integer value of an enum constant declaration as a signed long long.
    ///</summary>
    public long getEnumConstantDeclValue()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getEnumConstantDeclValue(ToNative());
    }

    ///<summary>
    /// Retrieve the integer type of an enum declaration.
    ///</summary>
    public QBType getEnumDeclIntegerType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getEnumDeclIntegerType(ToNative());
    }

    ///<summary>
    /// Retrieve the bit width of a bit field declaration as an integer.
    ///</summary>
    public int getFieldDeclBitWidth()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getFieldDeclBitWidth(ToNative());
    }

    ///<summary>
    /// For cursors representing an iboutletcollection attribute, this function returns the collection element type.
    ///</summary>
    public QBType getIBOutletCollectionType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getIBOutletCollectionType(ToNative());
    }

    ///<summary>
    /// Retrieve the file that is included by the given inclusion directive cursor.
    ///</summary>
    public QBFile getIncludedFile()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getIncludedFile(ToNative());
    }

    ///<summary>
    /// Determine the number of overloaded declarations referenced by a CXCursor_OverloadedDeclRef cursor.
    ///</summary>
    public uint getNumOverloadedDecls()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getNumOverloadedDecls(ToNative());
    }

    ///<summary>
    /// Retrieve a cursor for one of the overloaded declarations referenced by a CXCursor_OverloadedDeclRef cursor.
    ///</summary>
    public QBCursor getOverloadedDecl(uint index)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getOverloadedDecl(ToNative(), index);
    }

    ///<summary>
    /// Determine the set of methods that are overridden by the given method.
    ///</summary>
    public void getOverriddenCursors(out QBCursor[] overridden, out uint num_overridden)
    {
        QuantumBinding.Clang.Interop.CXCursor* arg1 = null;
        QuantumBinding.Clang.Interop.ClangInterop.clang_getOverriddenCursors(ToNative(), arg1, out num_overridden);
        var _overridden = NativeUtils.PointerToManagedArray(arg1, (long)num_overridden);
        overridden = new QuantumBinding.Clang.QBCursor[num_overridden];
        for (var i = 0U; i< num_overridden; ++i)
        {
            overridden[i] = new QuantumBinding.Clang.QBCursor(_overridden[i]);
        }
    }

    ///<summary>
    /// Given a cursor that may represent a specialization or instantiation of a template, retrieve the cursor that represents the template that it specializes or from which it was instantiated.
    ///</summary>
    public QBCursor getSpecializedCursorTemplate()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getSpecializedCursorTemplate(ToNative());
    }

    ///<summary>
    /// Generate a single symbol symbol graph for the declaration at the given cursor. Returns a null string if the AST node for the cursor isn't a declaration.
    ///</summary>
    public QBString getSymbolGraphForCursor()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getSymbolGraphForCursor(ToNative());
    }

    ///<summary>
    /// Given a cursor that represents a template, determine the cursor kind of the specializations would be generated by instantiating the template.
    ///</summary>
    public CXCursorKind getTemplateCursorKind()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getTemplateCursorKind(ToNative());
    }

    ///<summary>
    /// Retrieve the underlying type of a typedef declaration.
    ///</summary>
    public QBType getTypedefDeclUnderlyingType()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getTypedefDeclUnderlyingType(ToNative());
    }

    ///<summary>
    /// Compute a hash value for the given cursor.
    ///</summary>
    public uint hashCursor()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_hashCursor(ToNative());
    }

    ///<summary>
    /// Determine whether the declaration pointed to by this cursor is also a definition of that entity.
    ///</summary>
    public uint isCursorDefinition()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_isCursorDefinition(ToNative());
    }

    ///<summary>
    /// Determine whether the given declaration is invalid.
    ///</summary>
    public uint isInvalidDeclaration()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_isInvalidDeclaration(ToNative());
    }

    ///<summary>
    /// Returns 1 if the base class specified by the cursor with kind CX_CXXBaseSpecifier is virtual.
    ///</summary>
    public uint isVirtualBase()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_isVirtualBase(ToNative());
    }

    ///<summary>
    /// Visit the children of a particular cursor.
    ///</summary>
    public uint visitChildren(void* visitor, QBClientData client_data)
    {
        var arg2 = ReferenceEquals(client_data, null) ? new CXClientDataImpl() : (CXClientDataImpl)client_data;
        return QuantumBinding.Clang.Interop.ClangInterop.clang_visitChildren(ToNative(), visitor, arg2);
    }


    public QuantumBinding.Clang.Interop.CXCursor ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXCursor();
        _internal.kind = Kind;
        _internal.xdata = Xdata;
        if(Data != null)
        {
            if (Data.Length > 3)
                throw new System.ArgumentOutOfRangeException(nameof(Data), "Array is out of bounds. Size should not be more than 3");

            for (int i = 0; i < 3; ++i)
            {
                _internal.data[i] = Data[i];
            }
        }
        return _internal;
    }

    public static implicit operator QBCursor(QuantumBinding.Clang.Interop.CXCursor q)
    {
        return new QBCursor(q);
    }

}



