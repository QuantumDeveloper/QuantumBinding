
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Error codes returned by libclang routines.
///</summary>
public enum CXErrorCode : uint
{
    ///<summary>
    /// No error.
    ///</summary>
    CXError_Success = 0,

    ///<summary>
    /// A generic error code, no further details are available.
    ///</summary>
    CXError_Failure = 1,

    ///<summary>
    /// libclang crashed while performing the requested operation.
    ///</summary>
    CXError_Crashed = 2,

    ///<summary>
    /// The function detected that the arguments violate the function contract.
    ///</summary>
    CXError_InvalidArguments = 3,

    ///<summary>
    /// An AST deserialization error has occurred.
    ///</summary>
    CXError_ASTReadError = 4,

}


