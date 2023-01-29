
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Data for IndexerCallbacks#indexEntityReference.
///</summary>
[Flags]
public enum CXIdxEntityRefKind : uint
{
    ///<summary>
    /// The entity is referenced directly in user's code.
    ///</summary>
    CXIdxEntityRef_Direct = 1,

    ///<summary>
    /// An implicit reference, e.g. a reference of an Objective-C method via the dot syntax.
    ///</summary>
    CXIdxEntityRef_Implicit = 2,

}



