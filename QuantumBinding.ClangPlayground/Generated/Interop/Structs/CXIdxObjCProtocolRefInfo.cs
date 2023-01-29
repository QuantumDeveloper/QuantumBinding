
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXIdxObjCProtocolRefInfo
{
    public CXIdxEntityInfo* protocol;
    public CXCursor cursor;
    public CXIdxLoc loc;
}



