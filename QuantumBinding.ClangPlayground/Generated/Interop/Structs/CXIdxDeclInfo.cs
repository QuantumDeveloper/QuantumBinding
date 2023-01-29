
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXIdxDeclInfo
{
    public CXIdxEntityInfo* entityInfo;
    public CXCursor cursor;
    public CXIdxLoc loc;
    public CXIdxContainerInfo* semanticContainer;
    ///<summary>
    /// Generally same as #semanticContainer but can be different in cases like out-of-line C++ member functions.
    ///</summary>
    public CXIdxContainerInfo* lexicalContainer;
    public int isRedeclaration;
    public int isDefinition;
    public int isContainer;
    public CXIdxContainerInfo* declAsContainer;
    ///<summary>
    /// Whether the declaration exists in code or was created implicitly by the compiler, e.g. implicit Objective-C methods for properties.
    ///</summary>
    public int isImplicit;
    public CXIdxAttrInfo* attributes;
    public uint numAttributes;
    public uint flags;
}


