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
/// Describes parameter passing direction for \param or \arg command.
///</summary>
[Flags]
public enum CXCommentParamPassDirection : uint
{
    ///<summary>
    /// The parameter is an input parameter.
    ///</summary>
    CXCommentParamPassDirection_In = 0,

    ///<summary>
    /// The parameter is an output parameter.
    ///</summary>
    CXCommentParamPassDirection_Out = 1,

    ///<summary>
    /// The parameter is an input and output parameter.
    ///</summary>
    CXCommentParamPassDirection_InOut = 2,

}



