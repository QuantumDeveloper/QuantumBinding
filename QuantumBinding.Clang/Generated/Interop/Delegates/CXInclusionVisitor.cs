// ----------------------------------------------------------------------------------------------
// <auto-generated>
// This file was autogenerated by QuantumBindingGenerator.
// Do not edit this file manually, because you will lose all your changes after next generation.
// </auto-generated>
// ----------------------------------------------------------------------------------------------

using System.Security;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang;

namespace QuantumBinding.Clang.Interop;

///<summary>
/// Visitor invoked for each file in a translation unit (used with clang_getInclusions()).
///</summary>
public unsafe struct CXInclusionVisitor
{
    public CXInclusionVisitor(void* ptr)
    {
        NativePointer = ptr;
        InvokeFunc = (delegate* unmanaged<CXFileImpl, CXSourceLocation*, uint, CXClientDataImpl, void>)ptr;
    }

    private delegate* unmanaged<CXFileImpl, CXSourceLocation*, uint, CXClientDataImpl, void> InvokeFunc;

    public void* NativePointer { get; }

    public void Invoke(CXFileImpl included_file, CXSourceLocation* inclusion_stack, uint include_len, CXClientDataImpl client_data)
    {
         InvokeFunc(included_file, inclusion_stack, include_len, client_data);
    }
    public static void Invoke(void* ptr, CXFileImpl included_file, CXSourceLocation* inclusion_stack, uint include_len, CXClientDataImpl client_data)
    {
         ((delegate* unmanaged<CXFileImpl, CXSourceLocation*, uint, CXClientDataImpl, void>)ptr)(included_file, inclusion_stack, include_len, client_data);
    }

    public static explicit operator CXInclusionVisitor(void* ptr) => new(ptr);
}


