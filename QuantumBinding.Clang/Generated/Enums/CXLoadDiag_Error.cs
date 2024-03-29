// ----------------------------------------------------------------------------------------------
// <auto-generated>
// This file was autogenerated by QuantumBindingGenerator.
// Do not edit this file manually, because you will lose all your changes after next generation.
// </auto-generated>
// ----------------------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Describes the kind of error that occurred (if any) in a call to clang_loadDiagnostics.
///</summary>
public enum CXLoadDiag_Error : uint
{
    ///<summary>
    /// Indicates that no error occurred.
    ///</summary>
    CXLoadDiag_None = 0,

    ///<summary>
    /// Indicates that an unknown error occurred while attempting to deserialize diagnostics.
    ///</summary>
    CXLoadDiag_Unknown = 1,

    ///<summary>
    /// Indicates that the file containing the serialized diagnostics could not be opened.
    ///</summary>
    CXLoadDiag_CannotLoad = 2,

    ///<summary>
    /// Indicates that the serialized diagnostics file is invalid or corrupt.
    ///</summary>
    CXLoadDiag_InvalidFile = 3,

}



