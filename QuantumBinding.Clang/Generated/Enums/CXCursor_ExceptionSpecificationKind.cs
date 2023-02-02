
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Describes the exception specification of a cursor.
///</summary>
public enum CXCursor_ExceptionSpecificationKind : uint
{
    ///<summary>
    /// The cursor has no exception specification.
    ///</summary>
    CXCursor_ExceptionSpecificationKind_None = 0,

    ///<summary>
    /// The cursor has exception specification throw()
    ///</summary>
    CXCursor_ExceptionSpecificationKind_DynamicNone = 1,

    ///<summary>
    /// The cursor has exception specification throw(T1, T2)
    ///</summary>
    CXCursor_ExceptionSpecificationKind_Dynamic = 2,

    ///<summary>
    /// The cursor has exception specification throw(...).
    ///</summary>
    CXCursor_ExceptionSpecificationKind_MSAny = 3,

    ///<summary>
    /// The cursor has exception specification basic noexcept.
    ///</summary>
    CXCursor_ExceptionSpecificationKind_BasicNoexcept = 4,

    ///<summary>
    /// The cursor has exception specification computed noexcept.
    ///</summary>
    CXCursor_ExceptionSpecificationKind_ComputedNoexcept = 5,

    ///<summary>
    /// The exception specification has not yet been evaluated.
    ///</summary>
    CXCursor_ExceptionSpecificationKind_Unevaluated = 6,

    ///<summary>
    /// The exception specification has not yet been instantiated.
    ///</summary>
    CXCursor_ExceptionSpecificationKind_Uninstantiated = 7,

    ///<summary>
    /// The exception specification has not been parsed yet.
    ///</summary>
    CXCursor_ExceptionSpecificationKind_Unparsed = 8,

    ///<summary>
    /// The cursor has a __declspec(nothrow) exception specification.
    ///</summary>
    CXCursor_ExceptionSpecificationKind_NoThrow = 9,

}



