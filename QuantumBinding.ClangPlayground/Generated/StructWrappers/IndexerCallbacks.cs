
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class IndexerCallbacks : QBDisposableObject
{
    public IndexerCallbacks()
    {
    }

    public IndexerCallbacks(QuantumBinding.Clang.Interop.IndexerCallbacks _internal)
    {
        AbortQuery = _internal.abortQuery;
        Diagnostic = _internal.diagnostic;
        EnteredMainFile = _internal.enteredMainFile;
        PIncludedFile = _internal.ppIncludedFile;
        ImportedASTFile = _internal.importedASTFile;
        StartedTranslationUnit = _internal.startedTranslationUnit;
        IndexDeclaration = _internal.indexDeclaration;
        IndexEntityReference = _internal.indexEntityReference;
    }

    public void* AbortQuery { get; set; }
    public void* Diagnostic { get; set; }
    public void* EnteredMainFile { get; set; }
    public void* PIncludedFile { get; set; }
    public void* ImportedASTFile { get; set; }
    public void* StartedTranslationUnit { get; set; }
    public void* IndexDeclaration { get; set; }
    public void* IndexEntityReference { get; set; }

    public QuantumBinding.Clang.Interop.IndexerCallbacks ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.IndexerCallbacks();
        _internal.abortQuery = AbortQuery;
        _internal.diagnostic = Diagnostic;
        _internal.enteredMainFile = EnteredMainFile;
        _internal.ppIncludedFile = PIncludedFile;
        _internal.importedASTFile = ImportedASTFile;
        _internal.startedTranslationUnit = StartedTranslationUnit;
        _internal.indexDeclaration = IndexDeclaration;
        _internal.indexEntityReference = IndexEntityReference;
        return _internal;
    }

    public static implicit operator IndexerCallbacks(QuantumBinding.Clang.Interop.IndexerCallbacks i)
    {
        return new IndexerCallbacks(i);
    }

}



