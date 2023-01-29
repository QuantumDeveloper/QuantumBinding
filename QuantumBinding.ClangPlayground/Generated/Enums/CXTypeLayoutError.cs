
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// List the possible error codes for clang_Type_getSizeOf, clang_Type_getAlignOf, clang_Type_getOffsetOf and clang_Cursor_getOffsetOf.
///</summary>
public enum CXTypeLayoutError : int
{
    ///<summary>
    /// Type is of kind CXType_Invalid.
    ///</summary>
    CXTypeLayoutError_Invalid = -1,

    ///<summary>
    /// The type is an incomplete Type.
    ///</summary>
    CXTypeLayoutError_Incomplete = -2,

    ///<summary>
    /// The type is a dependent Type.
    ///</summary>
    CXTypeLayoutError_Dependent = -3,

    ///<summary>
    /// The type is not a constant size type.
    ///</summary>
    CXTypeLayoutError_NotConstantSize = -4,

    ///<summary>
    /// The Field name is not valid for this record.
    ///</summary>
    CXTypeLayoutError_InvalidFieldName = -5,

    ///<summary>
    /// The type is undeduced.
    ///</summary>
    CXTypeLayoutError_Undeduced = -6,

}


