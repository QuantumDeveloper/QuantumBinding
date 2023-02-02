
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Describes the availability of a particular entity, which indicates whether the use of this entity will result in a warning or error due to it being deprecated or unavailable.
///</summary>
public enum CXAvailabilityKind : uint
{
    ///<summary>
    /// The entity is available.
    ///</summary>
    CXAvailability_Available = 0,

    ///<summary>
    /// The entity is available, but has been deprecated (and its use is not recommended).
    ///</summary>
    CXAvailability_Deprecated = 1,

    ///<summary>
    /// The entity is not available; any use of it will be an error.
    ///</summary>
    CXAvailability_NotAvailable = 2,

    ///<summary>
    /// The entity is available, but not accessible; any use of it will be an error.
    ///</summary>
    CXAvailability_NotAccessible = 3,

}



