
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

///<summary>
/// A single translation unit, which resides in an index.
///</summary>
public unsafe partial class QBTranslationUnit
{
    internal CXTranslationUnitImpl __Instance;
    public QBTranslationUnit()
    {
    }

    public QBTranslationUnit(QuantumBinding.Clang.Interop.CXTranslationUnitImpl __Instance)
    {
        this.__Instance = __Instance;
    }

    ///<summary>
    /// Annotate the given set of tokens by providing cursors for each token that can be mapped to a specific entity within the abstract syntax tree.
    ///</summary>
    public void annotateTokens(QBToken[] Tokens, uint NumTokens, out QBCursor[] Cursors)
    {
        var arg1 = ReferenceEquals(Tokens, null) ? null : NativeUtils.GetPointerToManagedArray<QuantumBinding.Clang.Interop.CXToken>(Tokens.Length);
        if (!ReferenceEquals(Tokens, null))
        {
            for (var i = 0U; i < Tokens.Length; ++i)
            {
                arg1[i] = Tokens[i].ToNative();
            }
        }
        QuantumBinding.Clang.Interop.CXCursor* arg3 = null;
        QuantumBinding.Clang.Interop.ClangInterop.clang_annotateTokens(this, arg1, NumTokens, arg3);
        if (!ReferenceEquals(Tokens, null))
        {
            for (var i = 0U; i < Tokens.Length; ++i)
            {
                Tokens[i]?.Dispose();
            }
        }
        var _Cursors = NativeUtils.PointerToManagedArray(arg3, (long)NumTokens);
        Cursors = new QuantumBinding.Clang.QBCursor[NumTokens];
        for (var i = 0U; i< NumTokens; ++i)
        {
            Cursors[i] = new QuantumBinding.Clang.QBCursor(_Cursors[i]);
        }
    }

    ///<summary>
    /// Perform code completion at a given location in a translation unit.
    ///</summary>
    public QBCodeCompleteResults codeCompleteAt(string complete_filename, uint complete_line, uint complete_column, QBUnsavedFile unsaved_files, uint num_unsaved_files, uint options)
    {
        var arg1 = (sbyte*)NativeUtils.PointerToString(complete_filename, false);
        var arg4 = ReferenceEquals(unsaved_files, null) ? null : NativeUtils.StructOrEnumToPointer(unsaved_files.ToNative());
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_codeCompleteAt(this, arg1, complete_line, complete_column, arg4, num_unsaved_files, options);
        NativeUtils.Free(arg1);
        unsaved_files?.Dispose();
        NativeUtils.Free(arg4);
        var wrappedResult = new QBCodeCompleteResults(*result);
        NativeUtils.Free(result);
        return wrappedResult;
    }

    ///<summary>
    /// Traverses the translation unit to create a CXAPISet.
    ///</summary>
    public CXErrorCode createAPISet(out QBAPISet out_api)
    {
        CXAPISetImpl arg1;
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_createAPISet(this, out arg1);
        out_api = new QBAPISet(arg1);
        return result;
    }

    ///<summary>
    /// Returns the set of flags that is suitable for reparsing a translation unit.
    ///</summary>
    public uint defaultReparseOptions()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_defaultReparseOptions(this);
    }

    ///<summary>
    /// Returns the set of flags that is suitable for saving a translation unit.
    ///</summary>
    public uint defaultSaveOptions()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_defaultSaveOptions(this);
    }

    ///<summary>
    /// Free the given set of tokens.
    ///</summary>
    public void disposeTokens(QBToken Tokens, uint NumTokens)
    {
        var arg1 = ReferenceEquals(Tokens, null) ? null : NativeUtils.StructOrEnumToPointer(Tokens.ToNative());
        QuantumBinding.Clang.Interop.ClangInterop.clang_disposeTokens(this, arg1, NumTokens);
        Tokens?.Dispose();
        NativeUtils.Free(arg1);
    }

    ///<summary>
    /// Destroy the specified CXTranslationUnit object.
    ///</summary>
    public void disposeTranslationUnit()
    {
        QuantumBinding.Clang.Interop.ClangInterop.clang_disposeTranslationUnit(this);
    }

    ///<summary>
    /// Find #import/#include directives in a specific file.
    ///</summary>
    public CXResult findIncludesInFile(QBFile file, QBCursorAndRangeVisitor visitor)
    {
        var arg1 = ReferenceEquals(file, null) ? new CXFileImpl() : (CXFileImpl)file;
        var arg2 = ReferenceEquals(visitor, null) ? new QuantumBinding.Clang.Interop.CXCursorAndRangeVisitor() : visitor.ToNative();
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_findIncludesInFile(this, arg1, arg2);
        visitor?.Dispose();
        return result;
    }

    ///<summary>
    /// Retrieve all ranges from all files that were skipped by the preprocessor.
    ///</summary>
    public QBSourceRangeList getAllSkippedRanges()
    {
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getAllSkippedRanges(this);
        var wrappedResult = new QBSourceRangeList(*result);
        NativeUtils.Free(result);
        return wrappedResult;
    }

    ///<summary>
    /// Map a source location to the cursor that describes the entity at that location in the source code.
    ///</summary>
    public QBCursor getCursor(QBSourceLocation param1)
    {
        var arg1 = ReferenceEquals(param1, null) ? new QuantumBinding.Clang.Interop.CXSourceLocation() : param1.ToNative();
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getCursor(this, arg1);
        return result;
    }

    ///<summary>
    /// Return the memory usage of a translation unit. This object should be released with clang_disposeCXTUResourceUsage().
    ///</summary>
    public QBTUResourceUsage getCXTUResourceUsage()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCXTUResourceUsage(this);
    }

    ///<summary>
    /// Retrieve a diagnostic associated with the given translation unit.
    ///</summary>
    public QBDiagnostic getDiagnostic(uint Index)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getDiagnostic(this, Index);
    }

    ///<summary>
    /// Retrieve the complete set of diagnostics associated with a translation unit.
    ///</summary>
    public QBDiagnosticSet getDiagnosticSetFromTU()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getDiagnosticSetFromTU(this);
    }

    ///<summary>
    /// Retrieve a file handle within the given translation unit.
    ///</summary>
    public QBFile getFile(string file_name)
    {
        var arg1 = (sbyte*)NativeUtils.PointerToString(file_name, false);
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getFile(this, arg1);
        NativeUtils.Free(arg1);
        return result;
    }

    ///<summary>
    /// Retrieve the buffer associated with the given file.
    ///</summary>
    public string getFileContents(QBFile file, out ulong size)
    {
        var arg1 = ReferenceEquals(file, null) ? new CXFileImpl() : (CXFileImpl)file;
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getFileContents(this, arg1, out size);
        return new string(result);
    }

    ///<summary>
    /// Visit the set of preprocessor inclusions in a translation unit. The visitor function is called with the provided data for every included file. This does not include headers included by the PCH file (unless one is inspecting the inclusions in the PCH file itself).
    ///</summary>
    public void getInclusions(void* visitor, QBClientData client_data)
    {
        var arg2 = ReferenceEquals(client_data, null) ? new CXClientDataImpl() : (CXClientDataImpl)client_data;
        QuantumBinding.Clang.Interop.ClangInterop.clang_getInclusions(this, visitor, arg2);
    }

    ///<summary>
    /// Retrieves the source location associated with a given file/line/column in a particular translation unit.
    ///</summary>
    public QBSourceLocation getLocation(QBFile file, uint line, uint column)
    {
        var arg1 = ReferenceEquals(file, null) ? new CXFileImpl() : (CXFileImpl)file;
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getLocation(this, arg1, line, column);
    }

    ///<summary>
    /// Retrieves the source location associated with a given character offset in a particular translation unit.
    ///</summary>
    public QBSourceLocation getLocationForOffset(QBFile file, uint offset)
    {
        var arg1 = ReferenceEquals(file, null) ? new CXFileImpl() : (CXFileImpl)file;
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getLocationForOffset(this, arg1, offset);
    }

    ///<summary>
    /// Given a CXFile header file, return the module that contains it, if one exists.
    ///</summary>
    public QBModule getModuleForFile(QBFile param1)
    {
        var arg1 = ReferenceEquals(param1, null) ? new CXFileImpl() : (CXFileImpl)param1;
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getModuleForFile(this, arg1);
    }

    ///<summary>
    /// Determine the number of diagnostics produced for the given translation unit.
    ///</summary>
    public uint getNumDiagnostics()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getNumDiagnostics(this);
    }

    ///<summary>
    /// Retrieve all ranges that were skipped by the preprocessor.
    ///</summary>
    public QBSourceRangeList getSkippedRanges(QBFile file)
    {
        var arg1 = ReferenceEquals(file, null) ? new CXFileImpl() : (CXFileImpl)file;
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getSkippedRanges(this, arg1);
        var wrappedResult = new QBSourceRangeList(*result);
        NativeUtils.Free(result);
        return wrappedResult;
    }

    ///<summary>
    /// Get the raw lexical token starting with the given location.
    ///</summary>
    public QBToken getToken(QBSourceLocation Location)
    {
        var arg1 = ReferenceEquals(Location, null) ? new QuantumBinding.Clang.Interop.CXSourceLocation() : Location.ToNative();
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getToken(this, arg1);
        var wrappedResult = new QBToken(*result);
        NativeUtils.Free(result);
        return wrappedResult;
    }

    ///<summary>
    /// Retrieve a source range that covers the given token.
    ///</summary>
    public QBSourceRange getTokenExtent(QBToken param1)
    {
        var arg1 = ReferenceEquals(param1, null) ? new QuantumBinding.Clang.Interop.CXToken() : param1.ToNative();
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getTokenExtent(this, arg1);
        param1?.Dispose();
        return result;
    }

    ///<summary>
    /// Retrieve the source location of the given token.
    ///</summary>
    public QBSourceLocation getTokenLocation(QBToken param1)
    {
        var arg1 = ReferenceEquals(param1, null) ? new QuantumBinding.Clang.Interop.CXToken() : param1.ToNative();
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getTokenLocation(this, arg1);
        param1?.Dispose();
        return result;
    }

    ///<summary>
    /// Determine the spelling of the given token.
    ///</summary>
    public QBString getTokenSpelling(QBToken param1)
    {
        var arg1 = ReferenceEquals(param1, null) ? new QuantumBinding.Clang.Interop.CXToken() : param1.ToNative();
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getTokenSpelling(this, arg1);
        param1?.Dispose();
        return result;
    }

    ///<summary>
    /// Retrieve the cursor that represents the given translation unit.
    ///</summary>
    public QBCursor getTranslationUnitCursor()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getTranslationUnitCursor(this);
    }

    ///<summary>
    /// Get the original translation unit source file name.
    ///</summary>
    public QBString getTranslationUnitSpelling()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getTranslationUnitSpelling(this);
    }

    ///<summary>
    /// Get target information for this translation unit.
    ///</summary>
    public QBTargetInfo getTranslationUnitTargetInfo()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getTranslationUnitTargetInfo(this);
    }

    ///<summary>
    /// Determine whether the given header is guarded against multiple inclusions, either with the conventional #ifndef/#define/#endif macro guards or with #pragma once.
    ///</summary>
    public uint isFileMultipleIncludeGuarded(QBFile file)
    {
        var arg1 = ReferenceEquals(file, null) ? new CXFileImpl() : (CXFileImpl)file;
        return QuantumBinding.Clang.Interop.ClangInterop.clang_isFileMultipleIncludeGuarded(this, arg1);
    }

    ///<summary>
    /// Returns the number of top level headers associated with this module.
    ///</summary>
    public uint Module_getNumTopLevelHeaders(QBModule Module)
    {
        var arg1 = ReferenceEquals(Module, null) ? new CXModuleImpl() : (CXModuleImpl)Module;
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Module_getNumTopLevelHeaders(this, arg1);
    }

    ///<summary>
    /// Returns the specified top level header associated with the module.
    ///</summary>
    public QBFile Module_getTopLevelHeader(QBModule Module, uint Index)
    {
        var arg1 = ReferenceEquals(Module, null) ? new CXModuleImpl() : (CXModuleImpl)Module;
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Module_getTopLevelHeader(this, arg1, Index);
    }

    ///<summary>
    /// Reparse the source files that produced this translation unit.
    ///</summary>
    public int reparseTranslationUnit(uint num_unsaved_files, QBUnsavedFile unsaved_files, uint options)
    {
        var arg2 = ReferenceEquals(unsaved_files, null) ? null : NativeUtils.StructOrEnumToPointer(unsaved_files.ToNative());
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_reparseTranslationUnit(this, num_unsaved_files, arg2, options);
        unsaved_files?.Dispose();
        NativeUtils.Free(arg2);
        return result;
    }

    ///<summary>
    /// Saves a translation unit into a serialized representation of that translation unit on disk.
    ///</summary>
    public int saveTranslationUnit(string FileName, uint options)
    {
        var arg1 = (sbyte*)NativeUtils.PointerToString(FileName, false);
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_saveTranslationUnit(this, arg1, options);
        NativeUtils.Free(arg1);
        return result;
    }

    ///<summary>
    /// Suspend a translation unit in order to free memory associated with it.
    ///</summary>
    public uint suspendTranslationUnit()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_suspendTranslationUnit(this);
    }

    ///<summary>
    /// Tokenize the source code described by the given range into raw lexical tokens.
    ///</summary>
    public void tokenize(QBSourceRange Range, out QBToken[] Tokens, out uint NumTokens)
    {
        var arg1 = ReferenceEquals(Range, null) ? new QuantumBinding.Clang.Interop.CXSourceRange() : Range.ToNative();
        QuantumBinding.Clang.Interop.CXToken* arg2 = null;
        QuantumBinding.Clang.Interop.ClangInterop.clang_tokenize(this, arg1, arg2, out NumTokens);
        var _Tokens = NativeUtils.PointerToManagedArray(arg2, (long)NumTokens);
        Tokens = new QuantumBinding.Clang.QBToken[NumTokens];
        for (var i = 0U; i< NumTokens; ++i)
        {
            Tokens[i] = new QuantumBinding.Clang.QBToken(_Tokens[i]);
        }
    }

    public ref readonly CXTranslationUnitImpl GetPinnableReference() => ref __Instance;

    public static implicit operator QuantumBinding.Clang.Interop.CXTranslationUnitImpl(QBTranslationUnit q)
    {
        return q?.__Instance ?? new QuantumBinding.Clang.Interop.CXTranslationUnitImpl();
    }

    public static implicit operator QBTranslationUnit(QuantumBinding.Clang.Interop.CXTranslationUnitImpl q)
    {
        return new QBTranslationUnit(q);
    }

}



