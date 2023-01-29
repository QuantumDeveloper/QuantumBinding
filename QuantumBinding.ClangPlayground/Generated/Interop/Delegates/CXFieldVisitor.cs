
using System.Security;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang;

namespace QuantumBinding.Clang.Interop;

///<summary>
/// Visitor invoked for each field found by a traversal.
///</summary>
public unsafe struct CXFieldVisitor
{
    public CXFieldVisitor(void* ptr)
    {
        NativePointer = ptr;
        InvokeFunc = (delegate* unmanaged<CXCursor, CXClientDataImpl, CXVisitorResult>)ptr;
    }

    private delegate* unmanaged<CXCursor, CXClientDataImpl, CXVisitorResult> InvokeFunc;

    public void* NativePointer { get; }

    public CXVisitorResult Invoke(CXCursor C, CXClientDataImpl client_data)
    {
        return InvokeFunc(C, client_data);
    }
    public static CXVisitorResult Invoke(void* ptr, CXCursor C, CXClientDataImpl client_data)
    {
        return ((delegate* unmanaged<CXCursor, CXClientDataImpl, CXVisitorResult>)ptr)(C, client_data);
    }

    public static explicit operator CXFieldVisitor(void* ptr) => new(ptr);
}


