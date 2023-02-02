
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXIdxCXXClassDeclInfo
{
    public CXIdxDeclInfo* declInfo;
    public CXIdxBaseClassInfo* bases;
    public uint numBases;
}



