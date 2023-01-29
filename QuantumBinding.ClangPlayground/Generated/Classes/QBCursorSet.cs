
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

///<summary>
/// A fast container representing a set of CXCursors.
///</summary>
public unsafe partial class QBCursorSet
{
    internal CXCursorSetImpl __Instance;
    public QBCursorSet()
    {
    }

    public QBCursorSet(QuantumBinding.Clang.Interop.CXCursorSetImpl __Instance)
    {
        this.__Instance = __Instance;
    }

    ///<summary>
    /// Creates an empty CXCursorSet.
    ///</summary>
    public static QBCursorSet createCXCursorSet()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_createCXCursorSet();
    }

    ///<summary>
    /// Queries a CXCursorSet to see if it contains a specific CXCursor.
    ///</summary>
    public uint CXCursorSet_contains(QBCursor cursor)
    {
        var arg1 = ReferenceEquals(cursor, null) ? new QuantumBinding.Clang.Interop.CXCursor() : cursor.ToNative();
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_CXCursorSet_contains(this, arg1);
        return result;
    }

    ///<summary>
    /// Inserts a CXCursor into a CXCursorSet.
    ///</summary>
    public uint CXCursorSet_insert(QBCursor cursor)
    {
        var arg1 = ReferenceEquals(cursor, null) ? new QuantumBinding.Clang.Interop.CXCursor() : cursor.ToNative();
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_CXCursorSet_insert(this, arg1);
        return result;
    }

    ///<summary>
    /// Disposes a CXCursorSet and releases its associated memory.
    ///</summary>
    public void disposeCXCursorSet()
    {
        QuantumBinding.Clang.Interop.ClangInterop.clang_disposeCXCursorSet(this);
    }

    public ref readonly CXCursorSetImpl GetPinnableReference() => ref __Instance;

    public static implicit operator QuantumBinding.Clang.Interop.CXCursorSetImpl(QBCursorSet q)
    {
        return q?.__Instance ?? new QuantumBinding.Clang.Interop.CXCursorSetImpl();
    }

    public static implicit operator QBCursorSet(QuantumBinding.Clang.Interop.CXCursorSetImpl q)
    {
        return new QBCursorSet(q);
    }

}


