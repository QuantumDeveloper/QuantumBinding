
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

///<summary>
/// A group of callbacks used by #clang_indexSourceFile and #clang_indexTranslationUnit.
///</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct IndexerCallbacks
{
    ///<summary>
    /// Called periodically to check whether indexing should be aborted. Should return 0 to continue, and non-zero to abort.
    ///</summary>
    public void* abortQuery;
    ///<summary>
    /// Called at the end of indexing; passes the complete diagnostic set.
    ///</summary>
    public void* diagnostic;
    public void* enteredMainFile;
    ///<summary>
    /// Called when a file gets #included/#imported.
    ///</summary>
    public void* ppIncludedFile;
    ///<summary>
    /// Called when a AST file (PCH or module) gets imported.
    ///</summary>
    public void* importedASTFile;
    ///<summary>
    /// Called at the beginning of indexing a translation unit.
    ///</summary>
    public void* startedTranslationUnit;
    public void* indexDeclaration;
    ///<summary>
    /// Called to index a reference of an entity.
    ///</summary>
    public void* indexEntityReference;
}



