// ----------------------------------------------------------------------------------------------
// <auto-generated>
// This file was autogenerated by QuantumBindingGenerator.
// Do not edit this file manually, because you will lose all your changes after next generation.
// </auto-generated>
// ----------------------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Property attributes for a CXCursor_ObjCPropertyDecl.
///</summary>
[Flags]
public enum CXObjCPropertyAttrKind : uint
{
    CXObjCPropertyAttr_noattr = 0,

    CXObjCPropertyAttr_readonly = 1,

    CXObjCPropertyAttr_getter = 2,

    CXObjCPropertyAttr_assign = 4,

    CXObjCPropertyAttr_readwrite = 8,

    CXObjCPropertyAttr_retain = 16,

    CXObjCPropertyAttr_copy = 32,

    CXObjCPropertyAttr_nonatomic = 64,

    CXObjCPropertyAttr_setter = 128,

    CXObjCPropertyAttr_atomic = 256,

    CXObjCPropertyAttr_weak = 512,

    CXObjCPropertyAttr_strong = 1024,

    CXObjCPropertyAttr_unsafe_unretained = 2048,

    CXObjCPropertyAttr_class = 4096,

}



