
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBUnsavedFile : QBDisposableObject
{
    private MarshaledString _filename;

    private MarshaledString _contents;

    public QBUnsavedFile()
    {
    }

    public QBUnsavedFile(QuantumBinding.Clang.Interop.CXUnsavedFile _internal)
    {
        Filename = new string(_internal.Filename);
        Contents = new string(_internal.Contents);
        Length = _internal.Length;
    }

    public string Filename { get; set; }
    public string Contents { get; set; }
    public uint Length { get; set; }

    public QuantumBinding.Clang.Interop.CXUnsavedFile ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXUnsavedFile();
        _filename.Dispose();
        if (Filename != null)
        {
            _filename = new MarshaledString(Filename, false);
            _internal.Filename = (sbyte*)_filename;
        }
        _contents.Dispose();
        if (Contents != null)
        {
            _contents = new MarshaledString(Contents, false);
            _internal.Contents = (sbyte*)_contents;
        }
        _internal.Length = Length;
        return _internal;
    }

    protected override void UnmanagedDisposeOverride()
    {
        _filename.Dispose();
        _contents.Dispose();
    }


    public static implicit operator QBUnsavedFile(QuantumBinding.Clang.Interop.CXUnsavedFile q)
    {
        return new QBUnsavedFile(q);
    }

}



