
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBIdxLoc
{
    public QBIdxLoc()
    {
    }

    public QBIdxLoc(QuantumBinding.Clang.Interop.CXIdxLoc _internal)
    {
        Ptr_data = new void*[2];
        for (int i = 0; i < 2; ++i)
        {
            Ptr_data[i] = _internal.ptr_data[i];
        }
        Int_data = _internal.int_data;
    }

    public void*[] Ptr_data { get; set; }
    public uint Int_data { get; set; }
    ///<summary>
    /// Retrieve the CXSourceLocation represented by the given CXIdxLoc.
    ///</summary>
    public QBSourceLocation indexLoc_getCXSourceLocation()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_indexLoc_getCXSourceLocation(ToNative());
    }

    ///<summary>
    /// Retrieve the CXIdxFile, file, line, column, and offset represented by the given CXIdxLoc.
    ///</summary>
    public void indexLoc_getFileLocation(out QBIdxClientFile indexFile, out QBFile file, out uint line, out uint column, out uint offset)
    {
        CXIdxClientFileImpl arg1;
        CXFileImpl arg2;
        QuantumBinding.Clang.Interop.ClangInterop.clang_indexLoc_getFileLocation(ToNative(), out arg1, out arg2, out line, out column, out offset);
        indexFile = new QBIdxClientFile(arg1);
        file = new QBFile(arg2);
    }


    public QuantumBinding.Clang.Interop.CXIdxLoc ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXIdxLoc();
        if(Ptr_data != null)
        {
            if (Ptr_data.Length > 2)
                throw new System.ArgumentOutOfRangeException(nameof(Ptr_data), "Array is out of bounds. Size should not be more than 2");

            for (int i = 0; i < 2; ++i)
            {
                _internal.ptr_data[i] = Ptr_data[i];
            }
        }
        _internal.int_data = Int_data;
        return _internal;
    }

    public static implicit operator QBIdxLoc(QuantumBinding.Clang.Interop.CXIdxLoc q)
    {
        return new QBIdxLoc(q);
    }

}



