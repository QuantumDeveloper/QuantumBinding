
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Clang;

namespace QuantumBinding.Clang.Interop;

///<summary>
/// Data for ppIncludedFile callback.
///</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXIdxIncludedFileInfo
{
    ///<summary>
    /// Location of '#' in the #include/#import directive.
    ///</summary>
    public CXIdxLoc hashLoc;
    ///<summary>
    /// Filename as written in the #include/#import directive.
    ///</summary>
    public sbyte* filename;
    ///<summary>
    /// The actual file that the #include/#import directive resolved to.
    ///</summary>
    public CXFileImpl file;
    public int isImport;
    public int isAngled;
    ///<summary>
    /// Non-zero if the directive was automatically turned into a module import.
    ///</summary>
    public int isModuleImport;
}



