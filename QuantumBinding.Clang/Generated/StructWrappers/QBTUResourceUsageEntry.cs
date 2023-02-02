
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBTUResourceUsageEntry
{
    public QBTUResourceUsageEntry()
    {
    }

    public QBTUResourceUsageEntry(QuantumBinding.Clang.Interop.CXTUResourceUsageEntry _internal)
    {
        Kind = _internal.kind;
        Amount = _internal.amount;
    }

    public CXTUResourceUsageKind Kind { get; set; }
    public uint Amount { get; set; }

    public QuantumBinding.Clang.Interop.CXTUResourceUsageEntry ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXTUResourceUsageEntry();
        _internal.kind = Kind;
        _internal.amount = Amount;
        return _internal;
    }

    public static implicit operator QBTUResourceUsageEntry(QuantumBinding.Clang.Interop.CXTUResourceUsageEntry q)
    {
        return new QBTUResourceUsageEntry(q);
    }

}



