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

[Flags]
public enum CXRefQualifierKind : uint
{
    ///<summary>
    /// No ref-qualifier was provided.
    ///</summary>
    CXRefQualifier_None = 0,

    ///<summary>
    /// An lvalue ref-qualifier was provided ( &).
    ///</summary>
    CXRefQualifier_LValue = 1,

    ///<summary>
    /// An rvalue ref-qualifier was provided ( &&).
    ///</summary>
    CXRefQualifier_RValue = 2,

}



