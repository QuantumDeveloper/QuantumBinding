
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBPlatformAvailability
{
    public QBPlatformAvailability()
    {
    }

    public QBPlatformAvailability(QuantumBinding.Clang.Interop.CXPlatformAvailability _internal)
    {
        Platform = new QBString(_internal.Platform);
        Introduced = new QBVersion(_internal.Introduced);
        Deprecated = new QBVersion(_internal.Deprecated);
        Obsoleted = new QBVersion(_internal.Obsoleted);
        Unavailable = _internal.Unavailable;
        Message = new QBString(_internal.Message);
    }

    public QBString Platform { get; set; }
    public QBVersion Introduced { get; set; }
    public QBVersion Deprecated { get; set; }
    public QBVersion Obsoleted { get; set; }
    public int Unavailable { get; set; }
    public QBString Message { get; set; }
    ///<summary>
    /// Free the memory associated with a CXPlatformAvailability structure.
    ///</summary>
    public void disposeCXPlatformAvailability()
    {
        var arg0 = NativeUtils.StructOrEnumToPointer(ToNative());
        QuantumBinding.Clang.Interop.ClangInterop.clang_disposeCXPlatformAvailability(arg0);
        NativeUtils.Free(arg0);
    }


    public QuantumBinding.Clang.Interop.CXPlatformAvailability ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXPlatformAvailability();
        if (Platform != null)
        {
            _internal.Platform = Platform.ToNative();
        }
        if (Introduced != null)
        {
            _internal.Introduced = Introduced.ToNative();
        }
        if (Deprecated != null)
        {
            _internal.Deprecated = Deprecated.ToNative();
        }
        if (Obsoleted != null)
        {
            _internal.Obsoleted = Obsoleted.ToNative();
        }
        _internal.Unavailable = Unavailable;
        if (Message != null)
        {
            _internal.Message = Message.ToNative();
        }
        return _internal;
    }

    public static implicit operator QBPlatformAvailability(QuantumBinding.Clang.Interop.CXPlatformAvailability q)
    {
        return new QBPlatformAvailability(q);
    }

}



