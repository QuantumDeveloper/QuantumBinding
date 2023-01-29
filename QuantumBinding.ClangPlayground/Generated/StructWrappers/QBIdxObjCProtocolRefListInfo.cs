
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBIdxObjCProtocolRefListInfo : QBDisposableObject
{
    private NativeStructArray<QuantumBinding.Clang.Interop.CXIdxObjCProtocolRefInfo> _protocols;

    public QBIdxObjCProtocolRefListInfo()
    {
    }

    public QBIdxObjCProtocolRefListInfo(QuantumBinding.Clang.Interop.CXIdxObjCProtocolRefListInfo _internal)
    {
        Protocols = new QBIdxObjCProtocolRefInfo[_internal.numProtocols];
        var nativeTmpArray0 = NativeUtils.PointerToManagedArray(_internal.protocols, _internal.numProtocols);
        for (int i = 0; i < nativeTmpArray0.Length; ++i)
        {
            Protocols[i] = new QBIdxObjCProtocolRefInfo(nativeTmpArray0[i]);
        }
        NativeUtils.Free(_internal.protocols);
        NumProtocols = _internal.numProtocols;
    }

    public QBIdxObjCProtocolRefInfo[] Protocols { get; set; }
    public uint NumProtocols { get; set; }

    public QuantumBinding.Clang.Interop.CXIdxObjCProtocolRefListInfo ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXIdxObjCProtocolRefListInfo();
        _protocols.Dispose();
        if (Protocols != null)
        {
            var tmpArray0 = new QuantumBinding.Clang.Interop.CXIdxObjCProtocolRefInfo[Protocols.Length];
            for (int i = 0; i < Protocols.Length; ++i)
            {
                tmpArray0[i] = Protocols[i].ToNative();
            }
            _protocols = new NativeStructArray<QuantumBinding.Clang.Interop.CXIdxObjCProtocolRefInfo>(tmpArray0);
            _internal.protocols = _protocols.Handle;
        }
        _internal.numProtocols = NumProtocols;
        return _internal;
    }

    protected override void UnmanagedDisposeOverride()
    {
        _protocols.Dispose();
    }


    public static implicit operator QBIdxObjCProtocolRefListInfo(QuantumBinding.Clang.Interop.CXIdxObjCProtocolRefListInfo q)
    {
        return new QBIdxObjCProtocolRefListInfo(q);
    }

}



