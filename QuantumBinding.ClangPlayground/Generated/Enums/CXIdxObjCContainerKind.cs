
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

[Flags]
public enum CXIdxObjCContainerKind : uint
{
    CXIdxObjCContainer_ForwardRef = 0,

    CXIdxObjCContainer_Interface = 1,

    CXIdxObjCContainer_Implementation = 2,

}



