
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBIdxIncludedFileInfo : QBDisposableObject
{
    private MarshaledString _filename;

    public QBIdxIncludedFileInfo()
    {
    }

    public QBIdxIncludedFileInfo(QuantumBinding.Clang.Interop.CXIdxIncludedFileInfo _internal)
    {
        HashLoc = new QBIdxLoc(_internal.hashLoc);
        Filename = new string(_internal.filename);
        File = new QBFile(_internal.file);
        IsImport = _internal.isImport;
        IsAngled = _internal.isAngled;
        IsModuleImport = _internal.isModuleImport;
    }

    public QBIdxLoc HashLoc { get; set; }
    public string Filename { get; set; }
    public QBFile File { get; set; }
    public int IsImport { get; set; }
    public int IsAngled { get; set; }
    public int IsModuleImport { get; set; }

    public QuantumBinding.Clang.Interop.CXIdxIncludedFileInfo ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXIdxIncludedFileInfo();
        if (HashLoc != null)
        {
            _internal.hashLoc = HashLoc.ToNative();
        }
        _filename.Dispose();
        if (Filename != null)
        {
            _filename = new MarshaledString(Filename, false);
            _internal.filename = (sbyte*)_filename;
        }
        _internal.file = File;
        _internal.isImport = IsImport;
        _internal.isAngled = IsAngled;
        _internal.isModuleImport = IsModuleImport;
        return _internal;
    }

    protected override void UnmanagedDisposeOverride()
    {
        _filename.Dispose();
    }


    public static implicit operator QBIdxIncludedFileInfo(QuantumBinding.Clang.Interop.CXIdxIncludedFileInfo q)
    {
        return new QBIdxIncludedFileInfo(q);
    }

}



