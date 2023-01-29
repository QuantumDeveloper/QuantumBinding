
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Options to control the display of diagnostics.
///</summary>
[Flags]
public enum CXDiagnosticDisplayOptions : uint
{
    ///<summary>
    /// Display the source-location information where the diagnostic was located.
    ///</summary>
    CXDiagnostic_DisplaySourceLocation = 1,

    ///<summary>
    /// If displaying the source-location information of the diagnostic, also include the column number.
    ///</summary>
    CXDiagnostic_DisplayColumn = 2,

    ///<summary>
    /// If displaying the source-location information of the diagnostic, also include information about source ranges in a machine-parsable format.
    ///</summary>
    CXDiagnostic_DisplaySourceRanges = 4,

    ///<summary>
    /// Display the option name associated with this diagnostic, if any.
    ///</summary>
    CXDiagnostic_DisplayOption = 8,

    ///<summary>
    /// Display the category number associated with this diagnostic, if any.
    ///</summary>
    CXDiagnostic_DisplayCategoryId = 16,

    ///<summary>
    /// Display the category name associated with this diagnostic, if any.
    ///</summary>
    CXDiagnostic_DisplayCategoryName = 32,

}



