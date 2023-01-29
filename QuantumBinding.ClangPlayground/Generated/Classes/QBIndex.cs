
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

///<summary>
/// An "index" that consists of a set of translation units that would typically be linked together into an executable or library.
///</summary>
public unsafe partial class QBIndex
{
    internal CXIndexImpl __Instance;
    public QBIndex()
    {
    }

    public QBIndex(QuantumBinding.Clang.Interop.CXIndexImpl __Instance)
    {
        this.__Instance = __Instance;
    }

    ///<summary>
    /// Same as clang_createTranslationUnit2, but returns the CXTranslationUnit instead of an error code. In case of an error this routine returns a NULL CXTranslationUnit, without further detailed error codes.
    ///</summary>
    public QBTranslationUnit createTranslationUnit(string ast_filename)
    {
        var arg1 = (sbyte*)NativeUtils.StringToPointer(ast_filename, false);
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_createTranslationUnit(this, arg1);
        NativeUtils.Free(arg1);
        return result;
    }

    ///<summary>
    /// Create a translation unit from an AST file ( -emit-ast).
    ///</summary>
    public CXErrorCode createTranslationUnit2(string ast_filename, out QBTranslationUnit out_TU)
    {
        var arg1 = (sbyte*)NativeUtils.StringToPointer(ast_filename, false);
        CXTranslationUnitImpl arg2;
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_createTranslationUnit2(this, arg1, out arg2);
        NativeUtils.Free(arg1);
        out_TU = new QBTranslationUnit(arg2);
        return result;
    }

    ///<summary>
    /// Return the CXTranslationUnit for a given source file and the provided command line arguments one would pass to the compiler.
    ///</summary>
    public QBTranslationUnit createTranslationUnitFromSourceFile(string source_filename, int num_clang_command_line_args, in string[] clang_command_line_args, uint num_unsaved_files, QBUnsavedFile unsaved_files)
    {
        var arg1 = (sbyte*)NativeUtils.StringToPointer(source_filename, false);
        var arg3 = (sbyte**)NativeUtils.StringArrayToPointer(clang_command_line_args, false);
        var arg5 = ReferenceEquals(unsaved_files, null) ? null : NativeUtils.StructOrEnumToPointer(unsaved_files.ToNative());
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_createTranslationUnitFromSourceFile(this, arg1, num_clang_command_line_args, arg3, num_unsaved_files, arg5);
        NativeUtils.Free(arg1);
        NativeUtils.Free(arg3);
        unsaved_files?.Dispose();
        NativeUtils.Free(arg5);
        return result;
    }

    ///<summary>
    /// Gets the general options associated with a CXIndex.
    ///</summary>
    public uint CXIndex_getGlobalOptions()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_CXIndex_getGlobalOptions(this);
    }

    ///<summary>
    /// Sets general options associated with a CXIndex.
    ///</summary>
    public void CXIndex_setGlobalOptions(uint options)
    {
        QuantumBinding.Clang.Interop.ClangInterop.clang_CXIndex_setGlobalOptions(this, options);
    }

    ///<summary>
    /// Sets the invocation emission path option in a CXIndex.
    ///</summary>
    public void CXIndex_setInvocationEmissionPathOption(string Path)
    {
        var arg1 = (sbyte*)NativeUtils.StringToPointer(Path, false);
        QuantumBinding.Clang.Interop.ClangInterop.clang_CXIndex_setInvocationEmissionPathOption(this, arg1);
        NativeUtils.Free(arg1);
    }

    ///<summary>
    /// Destroy the given index.
    ///</summary>
    public void disposeIndex()
    {
        QuantumBinding.Clang.Interop.ClangInterop.clang_disposeIndex(this);
    }

    ///<summary>
    /// An indexing action/session, to be applied to one or multiple translation units.
    ///</summary>
    public QBIndexAction IndexAction_create()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_IndexAction_create(this);
    }

    ///<summary>
    /// Same as clang_parseTranslationUnit2, but returns the CXTranslationUnit instead of an error code. In case of an error this routine returns a NULL CXTranslationUnit, without further detailed error codes.
    ///</summary>
    public QBTranslationUnit parseTranslationUnit(string source_filename, in string[] command_line_args, int num_command_line_args, QBUnsavedFile unsaved_files, uint num_unsaved_files, uint options)
    {
        var arg1 = (sbyte*)NativeUtils.StringToPointer(source_filename, false);
        var arg2 = (sbyte**)NativeUtils.StringArrayToPointer(command_line_args, false);
        var arg4 = ReferenceEquals(unsaved_files, null) ? null : NativeUtils.StructOrEnumToPointer(unsaved_files.ToNative());
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_parseTranslationUnit(this, arg1, arg2, num_command_line_args, arg4, num_unsaved_files, options);
        NativeUtils.Free(arg1);
        NativeUtils.Free(arg2);
        unsaved_files?.Dispose();
        NativeUtils.Free(arg4);
        return result;
    }

    ///<summary>
    /// Parse the given source file and the translation unit corresponding to that file.
    ///</summary>
    public CXErrorCode parseTranslationUnit2(string source_filename, in string[] command_line_args, int num_command_line_args, QBUnsavedFile[] unsaved_files, uint num_unsaved_files, uint options, out QBTranslationUnit out_TU)
    {
        var arg1 = (sbyte*)NativeUtils.StringToPointer(source_filename, false);
        var arg2 = (sbyte**)NativeUtils.StringArrayToPointer(command_line_args, false);
        var arg4 = ReferenceEquals(unsaved_files, null) ? null : NativeUtils.GetPointerToManagedArray<QuantumBinding.Clang.Interop.CXUnsavedFile>(unsaved_files.Length);
        if (!ReferenceEquals(unsaved_files, null))
        {
            for (var i = 0U; i < unsaved_files.Length; ++i)
            {
                arg4[i] = unsaved_files[i].ToNative();
            }
        }
        CXTranslationUnitImpl arg7;
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_parseTranslationUnit2(this, arg1, arg2, num_command_line_args, arg4, num_unsaved_files, options, out arg7);
        NativeUtils.Free(arg1);
        NativeUtils.Free(arg2);
        if (!ReferenceEquals(unsaved_files, null))
        {
            for (var i = 0U; i < unsaved_files.Length; ++i)
            {
                unsaved_files[i]?.Dispose();
            }
        }
        out_TU = new QBTranslationUnit(arg7);
        return result;
    }

    ///<summary>
    /// Same as clang_parseTranslationUnit2 but requires a full command line for command_line_args including argv[0]. This is useful if the standard library paths are relative to the binary.
    ///</summary>
    public CXErrorCode parseTranslationUnit2FullArgv(string source_filename, in string[] command_line_args, int num_command_line_args, QBUnsavedFile unsaved_files, uint num_unsaved_files, uint options, out QBTranslationUnit out_TU)
    {
        var arg1 = (sbyte*)NativeUtils.StringToPointer(source_filename, false);
        var arg2 = (sbyte**)NativeUtils.StringArrayToPointer(command_line_args, false);
        var arg4 = ReferenceEquals(unsaved_files, null) ? null : NativeUtils.StructOrEnumToPointer(unsaved_files.ToNative());
        CXTranslationUnitImpl arg7;
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_parseTranslationUnit2FullArgv(this, arg1, arg2, num_command_line_args, arg4, num_unsaved_files, options, out arg7);
        NativeUtils.Free(arg1);
        NativeUtils.Free(arg2);
        unsaved_files?.Dispose();
        NativeUtils.Free(arg4);
        out_TU = new QBTranslationUnit(arg7);
        return result;
    }

    public ref readonly CXIndexImpl GetPinnableReference() => ref __Instance;

    public static implicit operator QuantumBinding.Clang.Interop.CXIndexImpl(QBIndex q)
    {
        return q?.__Instance ?? new QuantumBinding.Clang.Interop.CXIndexImpl();
    }

    public static implicit operator QBIndex(QuantumBinding.Clang.Interop.CXIndexImpl q)
    {
        return new QBIndex(q);
    }

}



