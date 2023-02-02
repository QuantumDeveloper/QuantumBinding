
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXIdxIBOutletCollectionAttrInfo
{
    public CXIdxAttrInfo* attrInfo;
    public CXIdxEntityInfo* objcClass;
    public CXCursor classCursor;
    public CXIdxLoc classLoc;
}



