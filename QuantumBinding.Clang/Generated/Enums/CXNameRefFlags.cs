
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

[Flags]
public enum CXNameRefFlags : uint
{
    ///<summary>
    /// Include the nested-name-specifier, e.g. Foo:: in x.Foo::y, in the range.
    ///</summary>
    CXNameRange_WantQualifier = 1,

    ///<summary>
    /// Include the explicit template arguments, e.g. <int> in x.f<int>, in the range.
    ///</summary>
    CXNameRange_WantTemplateArgs = 2,

    ///<summary>
    /// If the name is non-contiguous, return the full spanning range.
    ///</summary>
    CXNameRange_WantSinglePiece = 4,

}



