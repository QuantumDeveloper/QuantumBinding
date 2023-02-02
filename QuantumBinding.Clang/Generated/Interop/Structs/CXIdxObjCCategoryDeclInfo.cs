
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXIdxObjCCategoryDeclInfo
{
    public CXIdxObjCContainerDeclInfo* containerInfo;
    public CXIdxEntityInfo* objcClass;
    public CXCursor classCursor;
    public CXIdxLoc classLoc;
    public CXIdxObjCProtocolRefListInfo* protocols;
}



