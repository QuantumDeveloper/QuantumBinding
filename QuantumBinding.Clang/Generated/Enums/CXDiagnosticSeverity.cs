
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Describes the severity of a particular diagnostic.
///</summary>
public enum CXDiagnosticSeverity : uint
{
    ///<summary>
    /// A diagnostic that has been suppressed, e.g., by a command-line option.
    ///</summary>
    CXDiagnostic_Ignored = 0,

    ///<summary>
    /// This diagnostic is a note that should be attached to the previous (non-note) diagnostic.
    ///</summary>
    CXDiagnostic_Note = 1,

    ///<summary>
    /// This diagnostic indicates suspicious code that may not be wrong.
    ///</summary>
    CXDiagnostic_Warning = 2,

    ///<summary>
    /// This diagnostic indicates that the code is ill-formed.
    ///</summary>
    CXDiagnostic_Error = 3,

    ///<summary>
    /// This diagnostic indicates that the code is ill-formed such that future parser recovery is unlikely to produce useful results.
    ///</summary>
    CXDiagnostic_Fatal = 4,

}



