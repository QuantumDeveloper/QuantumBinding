
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Clang;

namespace QuantumBinding.Clang.Interop;

///<summary>
/// Data for IndexerCallbacks#importedASTFile.
///</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXIdxImportedASTFileInfo
{
    ///<summary>
    /// Top level AST file containing the imported PCH, module or submodule.
    ///</summary>
    public CXFileImpl file;
    ///<summary>
    /// The imported module or NULL if the AST file is a PCH.
    ///</summary>
    public CXModuleImpl module;
    ///<summary>
    /// Location where the file is imported. Applicable only for modules.
    ///</summary>
    public CXIdxLoc loc;
    ///<summary>
    /// Non-zero if an inclusion directive was automatically turned into a module import. Applicable only for modules.
    ///</summary>
    public int isImplicit;
}


