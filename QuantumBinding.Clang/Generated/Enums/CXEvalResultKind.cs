
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

public enum CXEvalResultKind : uint
{
    CXEval_Int = 1,

    CXEval_Float = 2,

    CXEval_ObjCStrLiteral = 3,

    CXEval_StrLiteral = 4,

    CXEval_CFStr = 5,

    CXEval_Other = 6,

    CXEval_UnExposed = 0,

}



