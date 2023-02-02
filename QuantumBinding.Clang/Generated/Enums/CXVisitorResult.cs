
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// @{
///</summary>
[Flags]
public enum CXVisitorResult : uint
{
    CXVisit_Break = 0,

    CXVisit_Continue = 1,

}



