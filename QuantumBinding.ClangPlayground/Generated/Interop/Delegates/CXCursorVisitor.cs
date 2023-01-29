
using System.Security;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang;

namespace QuantumBinding.Clang.Interop;

///<summary>
/// Visitor invoked for each cursor found by a traversal.
///</summary>
public unsafe struct CXCursorVisitor
{
    public CXCursorVisitor(void* ptr)
    {
        NativePointer = ptr;
        InvokeFunc = (delegate* unmanaged<CXCursor, CXCursor, CXClientDataImpl, CXChildVisitResult>)ptr;
    }

    private delegate* unmanaged<CXCursor, CXCursor, CXClientDataImpl, CXChildVisitResult> InvokeFunc;

    public void* NativePointer { get; }

    public CXChildVisitResult Invoke(CXCursor cursor, CXCursor parent, CXClientDataImpl client_data)
    {
        return InvokeFunc(cursor, parent, client_data);
    }
    public static CXChildVisitResult Invoke(void* ptr, CXCursor cursor, CXCursor parent, CXClientDataImpl client_data)
    {
        return ((delegate* unmanaged<CXCursor, CXCursor, CXClientDataImpl, CXChildVisitResult>)ptr)(cursor, parent, client_data);
    }

    public static explicit operator CXCursorVisitor(void* ptr) => new(ptr);
}

