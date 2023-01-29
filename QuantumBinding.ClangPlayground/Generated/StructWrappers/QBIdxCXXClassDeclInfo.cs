
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBIdxCXXClassDeclInfo : QBDisposableObject
{
    private NativeStruct<QuantumBinding.Clang.Interop.CXIdxDeclInfo> _declInfo;

    private NativeStructArray<QuantumBinding.Clang.Interop.CXIdxBaseClassInfo> _bases;

    public QBIdxCXXClassDeclInfo()
    {
    }

    public QBIdxCXXClassDeclInfo(QuantumBinding.Clang.Interop.CXIdxCXXClassDeclInfo _internal)
    {
        DeclInfo = new QBIdxDeclInfo(*_internal.declInfo);
        NativeUtils.Free(_internal.declInfo);
        Bases = new QBIdxBaseClassInfo[_internal.numBases];
        var nativeTmpArray0 = NativeUtils.PointerToManagedArray(_internal.bases, _internal.numBases);
        for (int i = 0; i < nativeTmpArray0.Length; ++i)
        {
            Bases[i] = new QBIdxBaseClassInfo(nativeTmpArray0[i]);
        }
        NativeUtils.Free(_internal.bases);
        NumBases = _internal.numBases;
    }

    public QBIdxDeclInfo DeclInfo { get; set; }
    public QBIdxBaseClassInfo[] Bases { get; set; }
    public uint NumBases { get; set; }

    public QuantumBinding.Clang.Interop.CXIdxCXXClassDeclInfo ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXIdxCXXClassDeclInfo();
        _declInfo.Dispose();
        if (DeclInfo != null)
        {
            var struct0 = DeclInfo.ToNative();
            _declInfo = new NativeStruct<QuantumBinding.Clang.Interop.CXIdxDeclInfo>(struct0);
            _internal.declInfo = _declInfo.Handle;
        }
        _bases.Dispose();
        if (Bases != null)
        {
            var tmpArray0 = new QuantumBinding.Clang.Interop.CXIdxBaseClassInfo[Bases.Length];
            for (int i = 0; i < Bases.Length; ++i)
            {
                tmpArray0[i] = Bases[i].ToNative();
            }
            _bases = new NativeStructArray<QuantumBinding.Clang.Interop.CXIdxBaseClassInfo>(tmpArray0);
            _internal.bases = _bases.Handle;
        }
        _internal.numBases = NumBases;
        return _internal;
    }

    protected override void UnmanagedDisposeOverride()
    {
        _declInfo.Dispose();
        _bases.Dispose();
    }


    public static implicit operator QBIdxCXXClassDeclInfo(QuantumBinding.Clang.Interop.CXIdxCXXClassDeclInfo q)
    {
        return new QBIdxCXXClassDeclInfo(q);
    }

}


