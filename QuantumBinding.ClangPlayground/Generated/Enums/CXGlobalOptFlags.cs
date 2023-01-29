
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

public enum CXGlobalOptFlags : uint
{
    ///<summary>
    /// Used to indicate that no special CXIndex options are needed.
    ///</summary>
    CXGlobalOpt_None = 0,

    ///<summary>
    /// Used to indicate that threads that libclang creates for indexing purposes should use background priority.
    ///</summary>
    CXGlobalOpt_ThreadBackgroundPriorityForIndexing = 1,

    ///<summary>
    /// Used to indicate that threads that libclang creates for editing purposes should use background priority.
    ///</summary>
    CXGlobalOpt_ThreadBackgroundPriorityForEditing = 2,

    ///<summary>
    /// Used to indicate that all threads that libclang creates should use background priority.
    ///</summary>
    CXGlobalOpt_ThreadBackgroundPriorityForAll = 3,

}



