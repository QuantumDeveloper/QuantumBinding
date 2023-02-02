
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Describes the calling convention of a function type
///</summary>
public enum CXCallingConv : uint
{
    CXCallingConv_Default = 0,

    CXCallingConv_C = 1,

    CXCallingConv_X86StdCall = 2,

    CXCallingConv_X86FastCall = 3,

    CXCallingConv_X86ThisCall = 4,

    CXCallingConv_X86Pascal = 5,

    CXCallingConv_AAPCS = 6,

    CXCallingConv_AAPCS_VFP = 7,

    CXCallingConv_X86RegCall = 8,

    CXCallingConv_IntelOclBicc = 9,

    CXCallingConv_Win64 = 10,

    ///<summary>
    /// Alias for compatibility with older versions of API.
    ///</summary>
    CXCallingConv_X86_64Win64 = 10,

    ///<summary>
    /// Alias for compatibility with older versions of API.
    ///</summary>
    CXCallingConv_X86_64SysV = 11,

    ///<summary>
    /// Alias for compatibility with older versions of API.
    ///</summary>
    CXCallingConv_X86VectorCall = 12,

    ///<summary>
    /// Alias for compatibility with older versions of API.
    ///</summary>
    CXCallingConv_Swift = 13,

    ///<summary>
    /// Alias for compatibility with older versions of API.
    ///</summary>
    CXCallingConv_PreserveMost = 14,

    ///<summary>
    /// Alias for compatibility with older versions of API.
    ///</summary>
    CXCallingConv_PreserveAll = 15,

    ///<summary>
    /// Alias for compatibility with older versions of API.
    ///</summary>
    CXCallingConv_AArch64VectorCall = 16,

    ///<summary>
    /// Alias for compatibility with older versions of API.
    ///</summary>
    CXCallingConv_SwiftAsync = 17,

    ///<summary>
    /// Alias for compatibility with older versions of API.
    ///</summary>
    CXCallingConv_AArch64SVEPCS = 18,

    ///<summary>
    /// Alias for compatibility with older versions of API.
    ///</summary>
    CXCallingConv_Invalid = 100,

    ///<summary>
    /// Alias for compatibility with older versions of API.
    ///</summary>
    CXCallingConv_Unexposed = 200,

}



