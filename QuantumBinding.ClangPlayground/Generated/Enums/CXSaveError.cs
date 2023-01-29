
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Describes the kind of error that occurred (if any) in a call to clang_saveTranslationUnit().
///</summary>
public enum CXSaveError : uint
{
    ///<summary>
    /// Indicates that no error occurred while saving a translation unit.
    ///</summary>
    CXSaveError_None = 0,

    ///<summary>
    /// Indicates that an unknown error occurred while attempting to save the file.
    ///</summary>
    CXSaveError_Unknown = 1,

    ///<summary>
    /// Indicates that errors during translation prevented this attempt to save the translation unit.
    ///</summary>
    CXSaveError_TranslationErrors = 2,

    ///<summary>
    /// Indicates that the translation unit to be saved was somehow invalid (e.g., NULL).
    ///</summary>
    CXSaveError_InvalidTU = 3,

}


