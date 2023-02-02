
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Flags that control the reparsing of translation units.
///</summary>
[Flags]
public enum CXReparse_Flags : uint
{
    ///<summary>
    /// Used to indicate that no special reparsing options are needed.
    ///</summary>
    CXReparse_None = 0,

}



