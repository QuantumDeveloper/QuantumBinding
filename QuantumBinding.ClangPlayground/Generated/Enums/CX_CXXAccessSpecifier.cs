
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Represents the C++ access control level to a base class for a cursor with kind CX_CXXBaseSpecifier.
///</summary>
public enum CX_CXXAccessSpecifier : uint
{
    CX_CXXInvalidAccessSpecifier = 0,

    CX_CXXPublic = 1,

    CX_CXXProtected = 2,

    CX_CXXPrivate = 3,

}



