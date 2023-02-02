
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Describe the "language" of the entity referred to by a cursor.
///</summary>
public enum CXLanguageKind : uint
{
    CXLanguage_Invalid = 0,

    CXLanguage_C = 1,

    CXLanguage_ObjC = 2,

    CXLanguage_CPlusPlus = 3,

}



