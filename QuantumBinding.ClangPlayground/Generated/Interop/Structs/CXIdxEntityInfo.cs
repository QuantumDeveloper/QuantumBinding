
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Clang;

namespace QuantumBinding.Clang.Interop;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CXIdxEntityInfo
{
    public CXIdxEntityKind kind;
    public CXIdxEntityCXXTemplateKind templateKind;
    public CXIdxEntityLanguage lang;
    public sbyte* name;
    public sbyte* USR;
    public CXCursor cursor;
    public CXIdxAttrInfo* attributes;
    public uint numAttributes;
}



