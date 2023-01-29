
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

public enum CXIdxEntityLanguage : uint
{
    CXIdxEntityLang_None = 0,

    CXIdxEntityLang_C = 1,

    CXIdxEntityLang_ObjC = 2,

    CXIdxEntityLang_CXX = 3,

    CXIdxEntityLang_Swift = 4,

}



