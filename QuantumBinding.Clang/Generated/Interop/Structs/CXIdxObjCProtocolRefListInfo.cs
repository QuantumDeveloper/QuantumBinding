
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXIdxObjCProtocolRefListInfo
{
    public CXIdxObjCProtocolRefInfo* protocols;
    public uint numProtocols;
}



