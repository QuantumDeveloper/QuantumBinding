
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Describes the kind of a template argument.
///</summary>
public enum CXTemplateArgumentKind : uint
{
    CXTemplateArgumentKind_Null = 0,

    CXTemplateArgumentKind_Type = 1,

    CXTemplateArgumentKind_Declaration = 2,

    CXTemplateArgumentKind_NullPtr = 3,

    CXTemplateArgumentKind_Integral = 4,

    CXTemplateArgumentKind_Template = 5,

    CXTemplateArgumentKind_TemplateExpansion = 6,

    CXTemplateArgumentKind_Expression = 7,

    CXTemplateArgumentKind_Pack = 8,

    ///<summary>
    /// Indicates an error case, preventing the kind from being deduced.
    ///</summary>
    CXTemplateArgumentKind_Invalid = 9,

}



