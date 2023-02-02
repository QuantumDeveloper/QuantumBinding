
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

///<summary>
/// An indexing action/session, to be applied to one or multiple translation units.
///</summary>
public unsafe partial class QBIndexAction
{
    internal CXIndexActionImpl __Instance;
    public QBIndexAction()
    {
    }

    public QBIndexAction(QuantumBinding.Clang.Interop.CXIndexActionImpl __Instance)
    {
        this.__Instance = __Instance;
    }

    ///<summary>
    /// Destroy the given index action.
    ///</summary>
    public void IndexAction_dispose()
    {
        QuantumBinding.Clang.Interop.ClangInterop.clang_IndexAction_dispose(this);
    }

    ///<summary>
    /// Index the given source file and the translation unit corresponding to that file via callbacks implemented through #IndexerCallbacks.
    ///</summary>
    public int indexSourceFile(QBClientData client_data, IndexerCallbacks index_callbacks, uint index_callbacks_size, uint index_options, string source_filename, in string[] command_line_args, int num_command_line_args, QBUnsavedFile unsaved_files, uint num_unsaved_files, out QBTranslationUnit out_TU, uint TU_options)
    {
        var arg1 = ReferenceEquals(client_data, null) ? new CXClientDataImpl() : (CXClientDataImpl)client_data;
        var arg2 = ReferenceEquals(index_callbacks, null) ? null : NativeUtils.StructOrEnumToPointer(index_callbacks.ToNative());
        var arg5 = (sbyte*)NativeUtils.StringToPointer(source_filename, false);
        var arg6 = (sbyte**)NativeUtils.StringArrayToPointer(command_line_args, false);
        var arg8 = ReferenceEquals(unsaved_files, null) ? null : NativeUtils.StructOrEnumToPointer(unsaved_files.ToNative());
        CXTranslationUnitImpl arg10;
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_indexSourceFile(this, arg1, arg2, index_callbacks_size, index_options, arg5, arg6, num_command_line_args, arg8, num_unsaved_files, out arg10, TU_options);
        index_callbacks?.Dispose();
        NativeUtils.Free(arg2);
        NativeUtils.Free(arg5);
        NativeUtils.Free(arg6);
        unsaved_files?.Dispose();
        NativeUtils.Free(arg8);
        out_TU = new QBTranslationUnit(arg10);
        return result;
    }

    ///<summary>
    /// Same as clang_indexSourceFile but requires a full command line for command_line_args including argv[0]. This is useful if the standard library paths are relative to the binary.
    ///</summary>
    public int indexSourceFileFullArgv(QBClientData client_data, IndexerCallbacks index_callbacks, uint index_callbacks_size, uint index_options, string source_filename, in string[] command_line_args, int num_command_line_args, QBUnsavedFile unsaved_files, uint num_unsaved_files, out QBTranslationUnit out_TU, uint TU_options)
    {
        var arg1 = ReferenceEquals(client_data, null) ? new CXClientDataImpl() : (CXClientDataImpl)client_data;
        var arg2 = ReferenceEquals(index_callbacks, null) ? null : NativeUtils.StructOrEnumToPointer(index_callbacks.ToNative());
        var arg5 = (sbyte*)NativeUtils.StringToPointer(source_filename, false);
        var arg6 = (sbyte**)NativeUtils.StringArrayToPointer(command_line_args, false);
        var arg8 = ReferenceEquals(unsaved_files, null) ? null : NativeUtils.StructOrEnumToPointer(unsaved_files.ToNative());
        CXTranslationUnitImpl arg10;
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_indexSourceFileFullArgv(this, arg1, arg2, index_callbacks_size, index_options, arg5, arg6, num_command_line_args, arg8, num_unsaved_files, out arg10, TU_options);
        index_callbacks?.Dispose();
        NativeUtils.Free(arg2);
        NativeUtils.Free(arg5);
        NativeUtils.Free(arg6);
        unsaved_files?.Dispose();
        NativeUtils.Free(arg8);
        out_TU = new QBTranslationUnit(arg10);
        return result;
    }

    ///<summary>
    /// Index the given translation unit via callbacks implemented through #IndexerCallbacks.
    ///</summary>
    public int indexTranslationUnit(QBClientData client_data, IndexerCallbacks index_callbacks, uint index_callbacks_size, uint index_options, QBTranslationUnit param5)
    {
        var arg1 = ReferenceEquals(client_data, null) ? new CXClientDataImpl() : (CXClientDataImpl)client_data;
        var arg2 = ReferenceEquals(index_callbacks, null) ? null : NativeUtils.StructOrEnumToPointer(index_callbacks.ToNative());
        var arg5 = ReferenceEquals(param5, null) ? new CXTranslationUnitImpl() : (CXTranslationUnitImpl)param5;
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_indexTranslationUnit(this, arg1, arg2, index_callbacks_size, index_options, arg5);
        index_callbacks?.Dispose();
        NativeUtils.Free(arg2);
        return result;
    }

    public ref readonly CXIndexActionImpl GetPinnableReference() => ref __Instance;

    public static implicit operator QuantumBinding.Clang.Interop.CXIndexActionImpl(QBIndexAction q)
    {
        return q?.__Instance ?? new QuantumBinding.Clang.Interop.CXIndexActionImpl();
    }

    public static implicit operator QBIndexAction(QuantumBinding.Clang.Interop.CXIndexActionImpl q)
    {
        return new QBIndexAction(q);
    }

}



