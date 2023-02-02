
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXIdxObjCPropertyDeclInfo
{
    public CXIdxDeclInfo* declInfo;
    public CXIdxEntityInfo* getter;
    public CXIdxEntityInfo* setter;
}



