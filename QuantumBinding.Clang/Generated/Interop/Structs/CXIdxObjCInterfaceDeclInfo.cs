
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXIdxObjCInterfaceDeclInfo
{
    public CXIdxObjCContainerDeclInfo* containerInfo;
    public CXIdxBaseClassInfo* superInfo;
    public CXIdxObjCProtocolRefListInfo* protocols;
}



