
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

///<summary>
/// Describes a version number of the form major.minor.subminor.
///</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXVersion
{
    ///<summary>
    /// The major version number, e.g., the '10' in '10.7.3'. A negative value indicates that there is no version number at all.
    ///</summary>
    public int Major;
    ///<summary>
    /// The minor version number, e.g., the '7' in '10.7.3'. This value will be negative if no minor version number was provided, e.g., for version '10'.
    ///</summary>
    public int Minor;
    ///<summary>
    /// The subminor version number, e.g., the '3' in '10.7.3'. This value will be negative if no minor or subminor version number was provided, e.g., in version '10' or '10.7'.
    ///</summary>
    public int Subminor;
}



