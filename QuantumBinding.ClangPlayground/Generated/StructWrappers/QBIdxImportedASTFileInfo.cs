
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBIdxImportedASTFileInfo
{
    public QBIdxImportedASTFileInfo()
    {
    }

    public QBIdxImportedASTFileInfo(QuantumBinding.Clang.Interop.CXIdxImportedASTFileInfo _internal)
    {
        File = new QBFile(_internal.file);
        Module = new QBModule(_internal.module);
        Loc = new QBIdxLoc(_internal.loc);
        IsImplicit = _internal.isImplicit;
    }

    public QBFile File { get; set; }
    public QBModule Module { get; set; }
    public QBIdxLoc Loc { get; set; }
    public int IsImplicit { get; set; }

    public QuantumBinding.Clang.Interop.CXIdxImportedASTFileInfo ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXIdxImportedASTFileInfo();
        _internal.file = File;
        _internal.module = Module;
        if (Loc != null)
        {
            _internal.loc = Loc.ToNative();
        }
        _internal.isImplicit = IsImplicit;
        return _internal;
    }

    public static implicit operator QBIdxImportedASTFileInfo(QuantumBinding.Clang.Interop.CXIdxImportedASTFileInfo q)
    {
        return new QBIdxImportedASTFileInfo(q);
    }

}


