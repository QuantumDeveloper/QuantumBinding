
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using QuantumBinding.Clang;

namespace QuantumBinding.Clang.Interop;

public static unsafe class Delegates
{
    ///<summary>
    /// Visitor invoked for each cursor found by a traversal.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate CXChildVisitResult CXCursorVisitor(CXCursor cursor, CXCursor parent, CXClientDataImpl client_data);
    ///<summary>
    /// Visitor invoked for each file in a translation unit (used with clang_getInclusions()).
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void CXInclusionVisitor(CXFileImpl included_file, CXSourceLocation* inclusion_stack, uint include_len, CXClientDataImpl client_data);
    ///<summary>
    /// Visitor invoked for each field found by a traversal.
    ///</summary>
    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate CXVisitorResult CXFieldVisitor(CXCursor C, CXClientDataImpl client_data);
}


