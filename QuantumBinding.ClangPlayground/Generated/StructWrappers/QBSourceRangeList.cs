
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBSourceRangeList : QBDisposableObject
{
    private NativeStruct<QuantumBinding.Clang.Interop.CXSourceRange> _ranges;

    public QBSourceRangeList()
    {
    }

    public QBSourceRangeList(QuantumBinding.Clang.Interop.CXSourceRangeList _internal)
    {
        Count = _internal.count;
        Ranges = new QBSourceRange(*_internal.ranges);
        NativeUtils.Free(_internal.ranges);
    }

    public uint Count { get; set; }
    public QBSourceRange Ranges { get; set; }
    ///<summary>
    /// Destroy the given CXSourceRangeList.
    ///</summary>
    public void disposeSourceRangeList()
    {
        var arg0 = NativeUtils.StructOrEnumToPointer(ToNative());
        QuantumBinding.Clang.Interop.ClangInterop.clang_disposeSourceRangeList(arg0);
        NativeUtils.Free(arg0);
    }


    public QuantumBinding.Clang.Interop.CXSourceRangeList ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXSourceRangeList();
        _internal.count = Count;
        _ranges.Dispose();
        if (Ranges != null)
        {
            var struct0 = Ranges.ToNative();
            _ranges = new NativeStruct<QuantumBinding.Clang.Interop.CXSourceRange>(struct0);
            _internal.ranges = _ranges.Handle;
        }
        return _internal;
    }

    protected override void UnmanagedDisposeOverride()
    {
        _ranges.Dispose();
    }


    public static implicit operator QBSourceRangeList(QuantumBinding.Clang.Interop.CXSourceRangeList q)
    {
        return new QBSourceRangeList(q);
    }

}


