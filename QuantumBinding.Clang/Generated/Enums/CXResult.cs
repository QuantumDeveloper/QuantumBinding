
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

[Flags]
public enum CXResult : uint
{
    ///<summary>
    /// Function returned successfully.
    ///</summary>
    CXResult_Success = 0,

    ///<summary>
    /// One of the parameters was invalid for the function.
    ///</summary>
    CXResult_Invalid = 1,

    ///<summary>
    /// The function was terminated by a callback (e.g. it returned CXVisit_Break)
    ///</summary>
    CXResult_VisitBreak = 2,

}


