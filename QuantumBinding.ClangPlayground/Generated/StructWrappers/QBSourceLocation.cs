
using System.Runtime.InteropServices;
using QuantumBinding.Utils;
using QuantumBinding.Clang.Interop;

namespace QuantumBinding.Clang;

public unsafe partial class QBSourceLocation
{
    public QBSourceLocation()
    {
    }

    public QBSourceLocation(QuantumBinding.Clang.Interop.CXSourceLocation _internal)
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
    /// Determine whether two source locations, which must refer into the same translation unit, refer to exactly the same point in the source code.
    ///</summary>
    public uint equalLocations(QBSourceLocation loc2)
    {
        var arg1 = ReferenceEquals(loc2, null) ? new QuantumBinding.Clang.Interop.CXSourceLocation() : loc2.ToNative();
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_equalLocations(ToNative(), arg1);
        return result;
    }

    ///<summary>
    /// Retrieve the file, line, column, and offset represented by the given source location.
    ///</summary>
    public void getExpansionLocation(out QBFile file, out uint line, out uint column, out uint offset)
    {
        CXFileImpl arg1;
        QuantumBinding.Clang.Interop.ClangInterop.clang_getExpansionLocation(ToNative(), out arg1, out line, out column, out offset);
        file = new QBFile(arg1);
    }

    ///<summary>
    /// Retrieve the file, line, column, and offset represented by the given source location.
    ///</summary>
    public void getFileLocation(out QBFile file, out uint line, out uint column, out uint offset)
    {
        CXFileImpl arg1;
        QuantumBinding.Clang.Interop.ClangInterop.clang_getFileLocation(ToNative(), out arg1, out line, out column, out offset);
        file = new QBFile(arg1);
    }

    ///<summary>
    /// Legacy API to retrieve the file, line, column, and offset represented by the given source location.
    ///</summary>
    public void getInstantiationLocation(out QBFile file, out uint line, out uint column, out uint offset)
    {
        CXFileImpl arg1;
        QuantumBinding.Clang.Interop.ClangInterop.clang_getInstantiationLocation(ToNative(), out arg1, out line, out column, out offset);
        file = new QBFile(arg1);
    }

    ///<summary>
    /// Retrieve the file, line and column represented by the given source location, as specified in a # line directive.
    ///</summary>
    public void getPresumedLocation(out QBString filename, out uint line, out uint column)
    {
        QuantumBinding.Clang.Interop.CXString arg1;
        QuantumBinding.Clang.Interop.ClangInterop.clang_getPresumedLocation(ToNative(), out arg1, out line, out column);
        filename = new QBString(arg1);
    }

    ///<summary>
    /// Retrieve a source range given the beginning and ending source locations.
    ///</summary>
    public QBSourceRange getRange(QBSourceLocation end)
    {
        var arg1 = ReferenceEquals(end, null) ? new QuantumBinding.Clang.Interop.CXSourceLocation() : end.ToNative();
        var result = QuantumBinding.Clang.Interop.ClangInterop.clang_getRange(ToNative(), arg1);
        return result;
    }

    ///<summary>
    /// Retrieve the file, line, column, and offset represented by the given source location.
    ///</summary>
    public void getSpellingLocation(out QBFile file, out uint line, out uint column, out uint offset)
    {
        CXFileImpl arg1;
        QuantumBinding.Clang.Interop.ClangInterop.clang_getSpellingLocation(ToNative(), out arg1, out line, out column, out offset);
        file = new QBFile(arg1);
    }

    ///<summary>
    /// Returns non-zero if the given source location is in the main file of the corresponding translation unit.
    ///</summary>
    public int Location_isFromMainFile()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Location_isFromMainFile(ToNative());
    }

    ///<summary>
    /// Returns non-zero if the given source location is in a system header.
    ///</summary>
    public int Location_isInSystemHeader()
    {
        return QuantumBinding.Clang.Interop.ClangInterop.clang_Location_isInSystemHeader(ToNative());
    }


    public QuantumBinding.Clang.Interop.CXSourceLocation ToNative()
    {
        var _internal = new QuantumBinding.Clang.Interop.CXSourceLocation();
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

    public static implicit operator QBSourceLocation(QuantumBinding.Clang.Interop.CXSourceLocation q)
    {
        return new QBSourceLocation(q);
    }

}


