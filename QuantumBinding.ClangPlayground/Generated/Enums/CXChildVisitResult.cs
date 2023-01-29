
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Describes how the traversal of the children of a particular cursor should proceed after visiting a particular child cursor.
///</summary>
[Flags]
public enum CXChildVisitResult : uint
{
    ///<summary>
    /// Terminates the cursor traversal.
    ///</summary>
    CXChildVisit_Break = 0,

    ///<summary>
    /// Continues the cursor traversal with the next sibling of the cursor just visited, without visiting its children.
    ///</summary>
    CXChildVisit_Continue = 1,

    ///<summary>
    /// Recursively traverse the children of this cursor, using the same visitor and client data.
    ///</summary>
    CXChildVisit_Recurse = 2,

}



