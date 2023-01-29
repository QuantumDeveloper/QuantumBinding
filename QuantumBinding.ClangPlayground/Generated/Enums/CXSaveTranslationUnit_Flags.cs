
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Flags that control how translation units are saved.
///</summary>
[Flags]
public enum CXSaveTranslationUnit_Flags : uint
{
    ///<summary>
    /// Used to indicate that no special saving options are needed.
    ///</summary>
    CXSaveTranslationUnit_None = 0,

}



