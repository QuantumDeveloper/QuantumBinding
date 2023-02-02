
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBTUResourceUsage : QBDisposableObject
{
    private NativeStruct<QuantumBinding.Clang.Interop.CXTUResourceUsageEntry> _entries;

    public QBTUResourceUsage()
    {
    }

    public QBTUResourceUsage(QuantumBinding.Clang.Interop.CXTUResourceUsage _internal)
    {
        Data = _internal.data;
        NumEntries = _internal.numEntries;
        Entries = new QBTUResourceUsageEntry(*_internal.entries);
        NativeUtils.Free(_internal.entries);
    }

    public void* Data { get; set; }
    public uint NumEntries { get; set; }
    public QBTUResourceUsageEntry Entries { get; set; }
    public void disposeCXTUResourceUsage()
    {
        QuantumBinding.Clang.Interop.ClangInterop.clang_disposeCXTUResourceUsage(ToNative());
    }


    public QuantumBinding.Clang.Interop.CXTUResourceUsage ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXTUResourceUsage();
        _internal.data = Data;
        _internal.numEntries = NumEntries;
        _entries.Dispose();
        if (Entries != null)
        {
            var struct0 = Entries.ToNative();
            _entries = new NativeStruct<QuantumBinding.Clang.Interop.CXTUResourceUsageEntry>(struct0);
            _internal.entries = _entries.Handle;
        }
        return _internal;
    }

    protected override void UnmanagedDisposeOverride()
    {
        _entries.Dispose();
    }


    public static implicit operator QBTUResourceUsage(QuantumBinding.Clang.Interop.CXTUResourceUsage q)
    {
        return new QBTUResourceUsage(q);
    }

}



