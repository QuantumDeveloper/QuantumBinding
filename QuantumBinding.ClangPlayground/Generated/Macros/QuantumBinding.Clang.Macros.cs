
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

public static class Constants
{
    public static uint _MSC_VER => 1;

    public static uint CINDEX_VERSION_MAJOR => 0;

    public static uint CINDEX_VERSION_MINOR => 62;

    public static uint CINDEX_VERSION_ENCODE(uint major, uint minor)
    {
        var version = ((0)*10000) + ((62)*1);
        return (uint)version;
    }

    public static uint CINDEX_VERSION => CINDEX_VERSION_ENCODE(CINDEX_VERSION_MAJOR,CINDEX_VERSION_MINOR);

    public static string CINDEX_VERSION_STRINGIZE(uint major, uint minor)
    {
        return "0.62";
    }

    public static string CINDEX_VERSION_STRING => CINDEX_VERSION_STRINGIZE(CINDEX_VERSION_MAJOR,CINDEX_VERSION_MINOR);

}


