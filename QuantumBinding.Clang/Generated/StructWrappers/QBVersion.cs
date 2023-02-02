
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBVersion
{
    public QBVersion()
    {
    }

    public QBVersion(QuantumBinding.Clang.Interop.CXVersion _internal)
    {
        Major = _internal.Major;
        Minor = _internal.Minor;
        Subminor = _internal.Subminor;
    }

    public int Major { get; set; }
    public int Minor { get; set; }
    public int Subminor { get; set; }

    public QuantumBinding.Clang.Interop.CXVersion ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXVersion();
        _internal.Major = Major;
        _internal.Minor = Minor;
        _internal.Subminor = Subminor;
        return _internal;
    }

    public static implicit operator QBVersion(QuantumBinding.Clang.Interop.CXVersion q)
    {
        return new QBVersion(q);
    }

}



