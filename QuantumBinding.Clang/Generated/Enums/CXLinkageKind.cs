
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Describe the linkage of the entity referred to by a cursor.
///</summary>
public enum CXLinkageKind : uint
{
    ///<summary>
    /// This value indicates that no linkage information is available for a provided CXCursor.
    ///</summary>
    CXLinkage_Invalid = 0,

    ///<summary>
    /// This is the linkage for variables, parameters, and so on that have automatic storage. This covers normal (non-extern) local variables.
    ///</summary>
    CXLinkage_NoLinkage = 1,

    ///<summary>
    /// This is the linkage for static variables and static functions.
    ///</summary>
    CXLinkage_Internal = 2,

    ///<summary>
    /// This is the linkage for entities with external linkage that live in C++ anonymous namespaces.
    ///</summary>
    CXLinkage_UniqueExternal = 3,

    ///<summary>
    /// This is the linkage for entities with true, external linkage.
    ///</summary>
    CXLinkage_External = 4,

}



