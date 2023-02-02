
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Clang;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXIdxObjCContainerDeclInfo
{
    public CXIdxDeclInfo* declInfo;
    public CXIdxObjCContainerKind kind;
}


