
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

public enum CXTypeNullabilityKind : uint
{
    ///<summary>
    /// Values of this type can never be null.
    ///</summary>
    CXTypeNullability_NonNull = 0,

    ///<summary>
    /// Values of this type can be null.
    ///</summary>
    CXTypeNullability_Nullable = 1,

    ///<summary>
    /// Whether values of this type can be null is (explicitly) unspecified. This captures a (fairly rare) case where we can't conclude anything about the nullability of the type even though it has been considered.
    ///</summary>
    CXTypeNullability_Unspecified = 2,

    ///<summary>
    /// Nullability is not applicable to this type.
    ///</summary>
    CXTypeNullability_Invalid = 3,

    ///<summary>
    /// Generally behaves like Nullable, except when used in a block parameter that was imported into a swift async method. There, swift will assume that the parameter can get null even if no error occurred. _Nullable parameters are assumed to only get null on error.
    ///</summary>
    CXTypeNullability_NullableResult = 4,

}



