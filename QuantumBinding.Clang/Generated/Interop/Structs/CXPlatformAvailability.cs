
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

///<summary>
/// Describes the availability of a given entity on a particular platform, e.g., a particular class might only be available on Mac OS 10.7 or newer.
///</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXPlatformAvailability
{
    ///<summary>
    /// A string that describes the platform for which this structure provides availability information.
    ///</summary>
    public CXString Platform;
    ///<summary>
    /// The version number in which this entity was introduced.
    ///</summary>
    public CXVersion Introduced;
    ///<summary>
    /// The version number in which this entity was deprecated (but is still available).
    ///</summary>
    public CXVersion Deprecated;
    ///<summary>
    /// The version number in which this entity was obsoleted, and therefore is no longer available.
    ///</summary>
    public CXVersion Obsoleted;
    ///<summary>
    /// Whether the entity is unconditionally unavailable on this platform.
    ///</summary>
    public int Unavailable;
    ///<summary>
    /// An optional message to provide to a user of this API, e.g., to suggest replacement APIs.
    ///</summary>
    public CXString Message;
}



