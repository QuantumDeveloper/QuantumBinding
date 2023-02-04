// ----------------------------------------------------------------------------------------------
// <auto-generated>
// This file was autogenerated by QuantumBindingGenerator.
// Do not edit this file manually, because you will lose all your changes after next generation.
// </auto-generated>
// ----------------------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe static class clang
{
    ///<summary>
    /// Return the timestamp for use with Clang's -fbuild-session-timestamp= option.
    ///</summary>
    public static ulong GetBuildSessionTimestamp()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getBuildSessionTimestamp();
    }

    ///<summary>
    /// Create a CXVirtualFileOverlay object. Must be disposed with clang_VirtualFileOverlay_dispose().
    ///</summary>
    public static QBVirtualFileOverlay VirtualFileOverlay_create(uint options)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_VirtualFileOverlay_create(options);
    }

    ///<summary>
    /// free memory allocated by libclang, such as the buffer returned by CXVirtualFileOverlay() or clang_ModuleMapDescriptor_writeToBuffer().
    ///</summary>
    public static void Free(ref void* buffer)
    {
        QuantumBinding.Clang.Interop.ClangInterop.clang_free(buffer);
    }

    ///<summary>
    /// Create a CXModuleMapDescriptor object. Must be disposed with clang_ModuleMapDescriptor_dispose().
    ///</summary>
    public static QBModuleMapDescriptor ModuleMapDescriptor_create(uint options)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_ModuleMapDescriptor_create(options);
    }

    ///<summary>
    /// Retrieve a NULL (invalid) source location.
    ///</summary>
    public static QBSourceLocation GetNullLocation()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getNullLocation();
    }

    ///<summary>
    /// Retrieve a NULL (invalid) source range.
    ///</summary>
    public static QBSourceRange GetNullRange()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getNullRange();
    }

    ///<summary>
    /// Deserialize a set of diagnostics from a Clang diagnostics bitcode file.
    ///</summary>
    public static QBDiagnosticSet LoadDiagnostics(string file, out CXLoadDiag_Error error, out QBString errorString)
    {
        var arg0 = (sbyte*)NativeUtils.StringToPointer(file, false);
        QuantumBinding.Clang.Interop.CXString arg2;
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_loadDiagnostics(arg0, out error, out arg2);
        NativeUtils.Free(arg0);
        errorString = new QBString(arg2);
        return result;
    }

    ///<summary>
    /// Retrieve the set of display options most similar to the default behavior of the clang compiler.
    ///</summary>
    public static uint DefaultDiagnosticDisplayOptions()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_defaultDiagnosticDisplayOptions();
    }

    ///<summary>
    /// Retrieve the name of a particular diagnostic category. This is now deprecated. Use clang_getDiagnosticCategoryText() instead.
    ///</summary>
    public static QBString GetDiagnosticCategoryName(uint category)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getDiagnosticCategoryName(category);
    }

    ///<summary>
    /// Provides a shared context for creating translation units.
    ///</summary>
    public static QBIndex CreateIndex(int excludeDeclarationsFromPCH, int displayDiagnostics)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_createIndex(excludeDeclarationsFromPCH, displayDiagnostics);
    }

    ///<summary>
    /// Returns the set of flags that is suitable for parsing a translation unit that is being edited.
    ///</summary>
    public static uint DefaultEditingTranslationUnitOptions()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_defaultEditingTranslationUnitOptions();
    }

    ///<summary>
    /// Returns the human-readable null-terminated C string that represents the name of the memory category. This string should never be freed.
    ///</summary>
    public static string GetTUResourceUsageName(CXTUResourceUsageKind kind)
    {
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getTUResourceUsageName(kind);
        return new string(result);
    }

    ///<summary>
    /// Retrieve the NULL cursor, which represents no entity.
    ///</summary>
    public static QBCursor GetNullCursor()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getNullCursor();
    }

    ///<summary>
    /// Determine whether the given cursor kind represents a declaration.
    ///</summary>
    public static uint IsDeclaration(CXCursorKind param0)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_isDeclaration(param0);
    }

    ///<summary>
    /// Determine whether the given cursor kind represents a simple reference.
    ///</summary>
    public static uint IsReference(CXCursorKind param0)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_isReference(param0);
    }

    ///<summary>
    /// Determine whether the given cursor kind represents an expression.
    ///</summary>
    public static uint IsExpression(CXCursorKind param0)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_isExpression(param0);
    }

    ///<summary>
    /// Determine whether the given cursor kind represents a statement.
    ///</summary>
    public static uint IsStatement(CXCursorKind param0)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_isStatement(param0);
    }

    ///<summary>
    /// Determine whether the given cursor kind represents an attribute.
    ///</summary>
    public static uint IsAttribute(CXCursorKind param0)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_isAttribute(param0);
    }

    ///<summary>
    /// Determine whether the given cursor kind represents an invalid cursor.
    ///</summary>
    public static uint IsInvalid(CXCursorKind param0)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_isInvalid(param0);
    }

    ///<summary>
    /// Determine whether the given cursor kind represents a translation unit.
    ///</summary>
    public static uint IsTranslationUnit(CXCursorKind param0)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_isTranslationUnit(param0);
    }

    ///<summary>
    /// * Determine whether the given cursor represents a preprocessing element, such as a preprocessor directive or macro instantiation.
    ///</summary>
    public static uint IsPreprocessing(CXCursorKind param0)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_isPreprocessing(param0);
    }

    ///<summary>
    /// * Determine whether the given cursor represents a currently unexposed piece of the AST (e.g., CXCursor_UnexposedStmt).
    ///</summary>
    public static uint IsUnexposed(CXCursorKind param0)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_isUnexposed(param0);
    }

    ///<summary>
    /// Retrieve the spelling of a given CXTypeKind.
    ///</summary>
    public static QBString GetTypeKindSpelling(CXTypeKind k)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getTypeKindSpelling(k);
    }

    ///<summary>
    /// Construct a USR for a specified Objective-C class.
    ///</summary>
    public static QBString ConstructUSR_ObjCClass(string class_name)
    {
        var arg0 = (sbyte*)NativeUtils.StringToPointer(class_name, false);
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_constructUSR_ObjCClass(arg0);
        NativeUtils.Free(arg0);
        return result;
    }

    ///<summary>
    /// Construct a USR for a specified Objective-C category.
    ///</summary>
    public static QBString ConstructUSR_ObjCCategory(string class_name, string category_name)
    {
        var arg0 = (sbyte*)NativeUtils.StringToPointer(class_name, false);
        var arg1 = (sbyte*)NativeUtils.StringToPointer(category_name, false);
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_constructUSR_ObjCCategory(arg0, arg1);
        NativeUtils.Free(arg0);
        NativeUtils.Free(arg1);
        return result;
    }

    ///<summary>
    /// Construct a USR for a specified Objective-C protocol.
    ///</summary>
    public static QBString ConstructUSR_ObjCProtocol(string protocol_name)
    {
        var arg0 = (sbyte*)NativeUtils.StringToPointer(protocol_name, false);
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_constructUSR_ObjCProtocol(arg0);
        NativeUtils.Free(arg0);
        return result;
    }

    ///<summary>
    /// Construct a USR for a specified Objective-C instance variable and the USR for its containing class.
    ///</summary>
    public static QBString ConstructUSR_ObjCIvar(string name, QBString classUSR)
    {
        var arg0 = (sbyte*)NativeUtils.StringToPointer(name, false);
        var arg1 = ReferenceEquals(classUSR, null) ? new QuantumBinding.Clang.Interop.CXString() : classUSR.ToNative();
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_constructUSR_ObjCIvar(arg0, arg1);
        NativeUtils.Free(arg0);
        classUSR?.Dispose();
        return result;
    }

    ///<summary>
    /// Construct a USR for a specified Objective-C method and the USR for its containing class.
    ///</summary>
    public static QBString ConstructUSR_ObjCMethod(string name, uint isInstanceMethod, QBString classUSR)
    {
        var arg0 = (sbyte*)NativeUtils.StringToPointer(name, false);
        var arg2 = ReferenceEquals(classUSR, null) ? new QuantumBinding.Clang.Interop.CXString() : classUSR.ToNative();
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_constructUSR_ObjCMethod(arg0, isInstanceMethod, arg2);
        NativeUtils.Free(arg0);
        classUSR?.Dispose();
        return result;
    }

    ///<summary>
    /// Construct a USR for a specified Objective-C property and the USR for its containing class.
    ///</summary>
    public static QBString ConstructUSR_ObjCProperty(string property, QBString classUSR)
    {
        var arg0 = (sbyte*)NativeUtils.StringToPointer(property, false);
        var arg1 = ReferenceEquals(classUSR, null) ? new QuantumBinding.Clang.Interop.CXString() : classUSR.ToNative();
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_constructUSR_ObjCProperty(arg0, arg1);
        NativeUtils.Free(arg0);
        classUSR?.Dispose();
        return result;
    }

    ///<summary>
    /// for debug/testing
    ///</summary>
    public static QBString GetCursorKindSpelling(CXCursorKind kind)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getCursorKindSpelling(kind);
    }

    public static void EnableStackTraces()
    {
        QuantumBinding.Clang.Interop.ClangInterop.clang_enableStackTraces();
    }

    public static void ExecuteOnThread(void* fn, ref void* user_data, uint stack_size)
    {
        QuantumBinding.Clang.Interop.ClangInterop.clang_executeOnThread(fn, user_data, stack_size);
    }

    ///<summary>
    /// Returns a default set of code-completion options that can be passed to clang_codeCompleteAt().
    ///</summary>
    public static uint DefaultCodeCompleteOptions()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_defaultCodeCompleteOptions();
    }

    ///<summary>
    /// Sort the code-completion results in case-insensitive alphabetical order.
    ///</summary>
    public static void SortCodeCompletionResults(QBCodeCompleteResults[] results, uint numResults)
    {
        var arg0 = ReferenceEquals(results, null) ? null : NativeUtils.GetPointerToManagedArray<QuantumBinding.Clang.Interop.CXCodeCompleteResults>(results.Length);
        if (!ReferenceEquals(results, null))
        {
            for (var i = 0U; i < results.Length; ++i)
            {
                arg0[i] = results[i].ToNative();
            }
        }
        QuantumBinding.Clang.Interop.ClangInterop.clang_sortCodeCompletionResults(arg0, numResults);
        if (!ReferenceEquals(results, null))
        {
            for (var i = 0U; i < results.Length; ++i)
            {
                results[i]?.Dispose();
            }
        }
    }

    ///<summary>
    /// Determine the number of diagnostics produced prior to the location where code completion was performed.
    ///</summary>
    public static uint CodeCompleteGetNumDiagnostics(params QBCodeCompleteResults[] results)
    {
        var arg0 = ReferenceEquals(results, null) ? null : NativeUtils.GetPointerToManagedArray<QuantumBinding.Clang.Interop.CXCodeCompleteResults>(results.Length);
        if (!ReferenceEquals(results, null))
        {
            for (var i = 0U; i < results.Length; ++i)
            {
                arg0[i] = results[i].ToNative();
            }
        }
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_codeCompleteGetNumDiagnostics(arg0);
        if (!ReferenceEquals(results, null))
        {
            for (var i = 0U; i < results.Length; ++i)
            {
                results[i]?.Dispose();
            }
        }
        return result;
    }

    ///<summary>
    /// Retrieve a diagnostic associated with the given code completion.
    ///</summary>
    public static QBDiagnostic CodeCompleteGetDiagnostic(QBCodeCompleteResults[] results, uint index)
    {
        var arg0 = ReferenceEquals(results, null) ? null : NativeUtils.GetPointerToManagedArray<QuantumBinding.Clang.Interop.CXCodeCompleteResults>(results.Length);
        if (!ReferenceEquals(results, null))
        {
            for (var i = 0U; i < results.Length; ++i)
            {
                arg0[i] = results[i].ToNative();
            }
        }
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_codeCompleteGetDiagnostic(arg0, index);
        if (!ReferenceEquals(results, null))
        {
            for (var i = 0U; i < results.Length; ++i)
            {
                results[i]?.Dispose();
            }
        }
        return result;
    }

    ///<summary>
    /// Determines what completions are appropriate for the context the given code completion.
    ///</summary>
    public static ulong CodeCompleteGetContexts(params QBCodeCompleteResults[] results)
    {
        var arg0 = ReferenceEquals(results, null) ? null : NativeUtils.GetPointerToManagedArray<QuantumBinding.Clang.Interop.CXCodeCompleteResults>(results.Length);
        if (!ReferenceEquals(results, null))
        {
            for (var i = 0U; i < results.Length; ++i)
            {
                arg0[i] = results[i].ToNative();
            }
        }
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_codeCompleteGetContexts(arg0);
        if (!ReferenceEquals(results, null))
        {
            for (var i = 0U; i < results.Length; ++i)
            {
                results[i]?.Dispose();
            }
        }
        return result;
    }

    ///<summary>
    /// Returns the cursor kind for the container for the current code completion context. The container is only guaranteed to be set for contexts where a container exists (i.e. member accesses or Objective-C message sends); if there is not a container, this function will return CXCursor_InvalidCode.
    ///</summary>
    public static CXCursorKind CodeCompleteGetContainerKind(QBCodeCompleteResults[] results, out uint isIncomplete)
    {
        var arg0 = ReferenceEquals(results, null) ? null : NativeUtils.GetPointerToManagedArray<QuantumBinding.Clang.Interop.CXCodeCompleteResults>(results.Length);
        if (!ReferenceEquals(results, null))
        {
            for (var i = 0U; i < results.Length; ++i)
            {
                arg0[i] = results[i].ToNative();
            }
        }
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_codeCompleteGetContainerKind(arg0, out isIncomplete);
        if (!ReferenceEquals(results, null))
        {
            for (var i = 0U; i < results.Length; ++i)
            {
                results[i]?.Dispose();
            }
        }
        return result;
    }

    ///<summary>
    /// Returns the USR for the container for the current code completion context. If there is not a container for the current context, this function will return the empty string.
    ///</summary>
    public static QBString CodeCompleteGetContainerUSR(params QBCodeCompleteResults[] results)
    {
        var arg0 = ReferenceEquals(results, null) ? null : NativeUtils.GetPointerToManagedArray<QuantumBinding.Clang.Interop.CXCodeCompleteResults>(results.Length);
        if (!ReferenceEquals(results, null))
        {
            for (var i = 0U; i < results.Length; ++i)
            {
                arg0[i] = results[i].ToNative();
            }
        }
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_codeCompleteGetContainerUSR(arg0);
        if (!ReferenceEquals(results, null))
        {
            for (var i = 0U; i < results.Length; ++i)
            {
                results[i]?.Dispose();
            }
        }
        return result;
    }

    ///<summary>
    /// Returns the currently-entered selector for an Objective-C message send, formatted like "initWithFoo:bar:". Only guaranteed to return a non-empty string for CXCompletionContext_ObjCInstanceMessage and CXCompletionContext_ObjCClassMessage.
    ///</summary>
    public static QBString CodeCompleteGetObjCSelector(params QBCodeCompleteResults[] results)
    {
        var arg0 = ReferenceEquals(results, null) ? null : NativeUtils.GetPointerToManagedArray<QuantumBinding.Clang.Interop.CXCodeCompleteResults>(results.Length);
        if (!ReferenceEquals(results, null))
        {
            for (var i = 0U; i < results.Length; ++i)
            {
                arg0[i] = results[i].ToNative();
            }
        }
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_codeCompleteGetObjCSelector(arg0);
        if (!ReferenceEquals(results, null))
        {
            for (var i = 0U; i < results.Length; ++i)
            {
                results[i]?.Dispose();
            }
        }
        return result;
    }

    ///<summary>
    /// Return a version string, suitable for showing to a user, but not intended to be parsed (the format is not guaranteed to be stable).
    ///</summary>
    public static QBString GetClangVersion()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getClangVersion();
    }

    ///<summary>
    /// Enable/disable crash recovery.
    ///</summary>
    public static void ToggleCrashRecovery(uint isEnabled)
    {
        QuantumBinding.Clang.Interop.ClangInterop.clang_toggleCrashRecovery(isEnabled);
    }

    ///<summary>
    /// Retrieve a remapping.
    ///</summary>
    public static QBRemapping GetRemappings(string path)
    {
        var arg0 = (sbyte*)NativeUtils.StringToPointer(path, false);
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getRemappings(arg0);
        NativeUtils.Free(arg0);
        return result;
    }

    ///<summary>
    /// Retrieve a remapping.
    ///</summary>
    public static QBRemapping GetRemappingsFromFileList(in string[] filePaths, uint numFiles)
    {
        var arg0 = (sbyte**)NativeUtils.StringArrayToPointer(filePaths, false);
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getRemappingsFromFileList(arg0, numFiles);
        NativeUtils.Free(arg0);
        return result;
    }

    public static int Index_isEntityObjCContainerKind(CXIdxEntityKind param0)
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_index_isEntityObjCContainerKind(param0);
    }

    public static QBIdxObjCContainerDeclInfo Index_getObjCContainerDeclInfo(in QBIdxDeclInfo param0)
    {
        var arg0 = ReferenceEquals(param0, null) ? null : NativeUtils.StructOrEnumToPointer(param0.ToNative());
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_index_getObjCContainerDeclInfo(arg0);
        param0?.Dispose();
        NativeUtils.Free(arg0);
        var wrappedResult = new QBIdxObjCContainerDeclInfo(*result);
        NativeUtils.Free(result);
        return wrappedResult;
    }

    public static QBIdxObjCInterfaceDeclInfo Index_getObjCInterfaceDeclInfo(in QBIdxDeclInfo param0)
    {
        var arg0 = ReferenceEquals(param0, null) ? null : NativeUtils.StructOrEnumToPointer(param0.ToNative());
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_index_getObjCInterfaceDeclInfo(arg0);
        param0?.Dispose();
        NativeUtils.Free(arg0);
        var wrappedResult = new QBIdxObjCInterfaceDeclInfo(*result);
        NativeUtils.Free(result);
        return wrappedResult;
    }

    public static QBIdxObjCCategoryDeclInfo Index_getObjCCategoryDeclInfo(in QBIdxDeclInfo param0)
    {
        var arg0 = ReferenceEquals(param0, null) ? null : NativeUtils.StructOrEnumToPointer(param0.ToNative());
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_index_getObjCCategoryDeclInfo(arg0);
        param0?.Dispose();
        NativeUtils.Free(arg0);
        var wrappedResult = new QBIdxObjCCategoryDeclInfo(*result);
        NativeUtils.Free(result);
        return wrappedResult;
    }

    public static QBIdxObjCProtocolRefListInfo Index_getObjCProtocolRefListInfo(in QBIdxDeclInfo param0)
    {
        var arg0 = ReferenceEquals(param0, null) ? null : NativeUtils.StructOrEnumToPointer(param0.ToNative());
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_index_getObjCProtocolRefListInfo(arg0);
        param0?.Dispose();
        NativeUtils.Free(arg0);
        var wrappedResult = new QBIdxObjCProtocolRefListInfo(*result);
        NativeUtils.Free(result);
        return wrappedResult;
    }

    public static QBIdxObjCPropertyDeclInfo Index_getObjCPropertyDeclInfo(in QBIdxDeclInfo param0)
    {
        var arg0 = ReferenceEquals(param0, null) ? null : NativeUtils.StructOrEnumToPointer(param0.ToNative());
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_index_getObjCPropertyDeclInfo(arg0);
        param0?.Dispose();
        NativeUtils.Free(arg0);
        var wrappedResult = new QBIdxObjCPropertyDeclInfo(*result);
        NativeUtils.Free(result);
        return wrappedResult;
    }

    public static QBIdxIBOutletCollectionAttrInfo Index_getIBOutletCollectionAttrInfo(in QBIdxAttrInfo param0)
    {
        var arg0 = ReferenceEquals(param0, null) ? null : NativeUtils.StructOrEnumToPointer(param0.ToNative());
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_index_getIBOutletCollectionAttrInfo(arg0);
        NativeUtils.Free(arg0);
        var wrappedResult = new QBIdxIBOutletCollectionAttrInfo(*result);
        NativeUtils.Free(result);
        return wrappedResult;
    }

    public static QBIdxCXXClassDeclInfo Index_getCXXClassDeclInfo(in QBIdxDeclInfo param0)
    {
        var arg0 = ReferenceEquals(param0, null) ? null : NativeUtils.StructOrEnumToPointer(param0.ToNative());
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_index_getCXXClassDeclInfo(arg0);
        param0?.Dispose();
        NativeUtils.Free(arg0);
        var wrappedResult = new QBIdxCXXClassDeclInfo(*result);
        NativeUtils.Free(result);
        return wrappedResult;
    }

    ///<summary>
    /// For retrieving a custom CXIdxClientContainer attached to a container.
    ///</summary>
    public static QBIdxClientContainer Index_getClientContainer(in QBIdxContainerInfo param0)
    {
        var arg0 = ReferenceEquals(param0, null) ? null : NativeUtils.StructOrEnumToPointer(param0.ToNative());
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_index_getClientContainer(arg0);
        NativeUtils.Free(arg0);
        return result;
    }

    ///<summary>
    /// For setting a custom CXIdxClientContainer attached to a container.
    ///</summary>
    public static void Index_setClientContainer(in QBIdxContainerInfo param0, QBIdxClientContainer param1)
    {
        var arg0 = ReferenceEquals(param0, null) ? null : NativeUtils.StructOrEnumToPointer(param0.ToNative());
        var arg1 = ReferenceEquals(param1, null) ? new CXIdxClientContainerImpl() : (CXIdxClientContainerImpl)param1;
        QuantumBinding.Clang.Interop.ClangInterop.clang_index_setClientContainer(arg0, arg1);
        NativeUtils.Free(arg0);
    }

    ///<summary>
    /// For retrieving a custom CXIdxClientEntity attached to an entity.
    ///</summary>
    public static QBIdxClientEntity Index_getClientEntity(in QBIdxEntityInfo param0)
    {
        var arg0 = ReferenceEquals(param0, null) ? null : NativeUtils.StructOrEnumToPointer(param0.ToNative());
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_index_getClientEntity(arg0);
        param0?.Dispose();
        NativeUtils.Free(arg0);
        return result;
    }

    ///<summary>
    /// For setting a custom CXIdxClientEntity attached to an entity.
    ///</summary>
    public static void Index_setClientEntity(in QBIdxEntityInfo param0, QBIdxClientEntity param1)
    {
        var arg0 = ReferenceEquals(param0, null) ? null : NativeUtils.StructOrEnumToPointer(param0.ToNative());
        var arg1 = ReferenceEquals(param1, null) ? new CXIdxClientEntityImpl() : (CXIdxClientEntityImpl)param1;
        QuantumBinding.Clang.Interop.ClangInterop.clang_index_setClientEntity(arg0, arg1);
        param0?.Dispose();
        NativeUtils.Free(arg0);
    }

    ///<summary>
    /// Generate a single symbol symbol graph for the given USR. Returns a null string if the associated symbol can not be found in the provided CXAPISet.
    ///</summary>
    public static QBString GetSymbolGraphForUSR(string usr, QBAPISet api)
    {
        var arg0 = (sbyte*)NativeUtils.StringToPointer(usr, false);
        var arg1 = ReferenceEquals(api, null) ? new CXAPISetImpl() : (CXAPISetImpl)api;
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getSymbolGraphForUSR(arg0, arg1);
        NativeUtils.Free(arg0);
        return result;
    }

}


