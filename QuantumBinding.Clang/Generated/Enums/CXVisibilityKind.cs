
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

public enum CXVisibilityKind : uint
{
    ///<summary>
    /// This value indicates that no visibility information is available for a provided CXCursor.
    ///</summary>
    CXVisibility_Invalid = 0,

    ///<summary>
    /// Symbol not seen by the linker.
    ///</summary>
    CXVisibility_Hidden = 1,

    ///<summary>
    /// Symbol seen by the linker but resolves to a symbol inside this object.
    ///</summary>
    CXVisibility_Protected = 2,

    ///<summary>
    /// Symbol seen by the linker and acts like a normal symbol.
    ///</summary>
    CXVisibility_Default = 3,

}



