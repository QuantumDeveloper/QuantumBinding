
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBCursorAndRangeVisitor : QBDisposableObject
{
    public QBCursorAndRangeVisitor()
    {
    }

    public QBCursorAndRangeVisitor(QuantumBinding.Clang.Interop.CXCursorAndRangeVisitor _internal)
    {
        Context = _internal.context;
        Visit = _internal.visit;
    }

    public void* Context { get; set; }
    public void* Visit { get; set; }

    public QuantumBinding.Clang.Interop.CXCursorAndRangeVisitor ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXCursorAndRangeVisitor();
        _internal.context = Context;
        _internal.visit = Visit;
        return _internal;
    }

    public static implicit operator QBCursorAndRangeVisitor(QuantumBinding.Clang.Interop.CXCursorAndRangeVisitor q)
    {
        return new QBCursorAndRangeVisitor(q);
    }

}



