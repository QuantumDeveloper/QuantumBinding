
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

///<summary>
/// A particular source file that is part of a translation unit.
///</summary>
public unsafe partial class QBFile
{
    internal CXFileImpl __Instance;
    public QBFile()
    {
    }

    public QBFile(QuantumBinding.Clang.Interop.CXFileImpl __Instance)
    {
        this.__Instance = __Instance;
    }

    ///<summary>
    /// Returns non-zero if the file1 and file2 point to the same file, or they are both NULL.
    ///</summary>
    public int File_isEqual(QBFile file2)
    {
        var arg1 = ReferenceEquals(file2, null) ? new CXFileImpl() : (CXFileImpl)file2;
        return QuantumBinding.Clang.Interop.ClangInterop.clang_File_isEqual(this, arg1);
    }

    ///<summary>
    /// Returns the real path name of file.
    ///</summary>
    public QBString File_tryGetRealPathName()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_File_tryGetRealPathName(this);
    }

    ///<summary>
    /// Retrieve the complete file and path name of the given file.
    ///</summary>
    public QBString getFileName()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getFileName(this);
    }

    ///<summary>
    /// Retrieve the last modification time of the given file.
    ///</summary>
    public long getFileTime()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_getFileTime(this);
    }

    ///<summary>
    /// Retrieve the unique ID for the given file.
    ///</summary>
    public int getFileUniqueID(out QBFileUniqueID outID)
    {
        QuantumBinding.Clang.Interop.CXFileUniqueID arg1;
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getFileUniqueID(this, out arg1);
        outID = new QBFileUniqueID(arg1);
        return result;
    }

    public ref readonly CXFileImpl GetPinnableReference() => ref __Instance;

    public static implicit operator QuantumBinding.Clang.Interop.CXFileImpl(QBFile q)
    {
        return q?.__Instance ?? new QuantumBinding.Clang.Interop.CXFileImpl();
    }

    public static implicit operator QBFile(QuantumBinding.Clang.Interop.CXFileImpl q)
    {
        return new QBFile(q);
    }

}



