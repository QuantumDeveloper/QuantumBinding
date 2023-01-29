
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// 'Qualifiers' written next to the return and parameter types in Objective-C method declarations.
///</summary>
[Flags]
public enum CXObjCDeclQualifierKind : uint
{
    CXObjCDeclQualifier_None = 0,

    CXObjCDeclQualifier_In = 1,

    CXObjCDeclQualifier_Inout = 2,

    CXObjCDeclQualifier_Out = 4,

    CXObjCDeclQualifier_Bycopy = 8,

    CXObjCDeclQualifier_Byref = 16,

    CXObjCDeclQualifier_Oneway = 32,

}



