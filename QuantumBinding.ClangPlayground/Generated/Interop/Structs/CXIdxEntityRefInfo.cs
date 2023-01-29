
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Clang;

namespace QuantumBinding.Clang.Interop;

///<summary>
/// Data for IndexerCallbacks#indexEntityReference.
///</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXIdxEntityRefInfo
{
    public CXIdxEntityRefKind kind;
    ///<summary>
    /// Reference cursor.
    ///</summary>
    public CXCursor cursor;
    public CXIdxLoc loc;
    ///<summary>
    /// The entity that gets referenced.
    ///</summary>
    public CXIdxEntityInfo* referencedEntity;
    ///<summary>
    /// Immediate "parent" of the reference. For example:
    ///</summary>
    public CXIdxEntityInfo* parentEntity;
    ///<summary>
    /// Lexical container context of the reference.
    ///</summary>
    public CXIdxContainerInfo* container;
    ///<summary>
    /// Sets of symbol roles of the reference.
    ///</summary>
    public CXSymbolRole role;
}



