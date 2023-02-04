// ----------------------------------------------------------------------------------------------
// <auto-generated>
// This file was autogenerated by QuantumBindingGenerator.
// Do not edit this file manually, because you will lose all your changes after next generation.
// </auto-generated>
// ----------------------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Categorizes how memory is being used by a translation unit.
///</summary>
public enum CXTUResourceUsageKind : uint
{
    CXTUResourceUsage_AST = 1,

    CXTUResourceUsage_Identifiers = 2,

    CXTUResourceUsage_Selectors = 3,

    CXTUResourceUsage_GlobalCompletionResults = 4,

    CXTUResourceUsage_SourceManagerContentCache = 5,

    CXTUResourceUsage_AST_SideTables = 6,

    CXTUResourceUsage_SourceManager_Membuffer_Malloc = 7,

    CXTUResourceUsage_SourceManager_Membuffer_MMap = 8,

    CXTUResourceUsage_ExternalASTSource_Membuffer_Malloc = 9,

    CXTUResourceUsage_ExternalASTSource_Membuffer_MMap = 10,

    CXTUResourceUsage_Preprocessor = 11,

    CXTUResourceUsage_PreprocessingRecord = 12,

    CXTUResourceUsage_SourceManager_DataStructures = 13,

    CXTUResourceUsage_Preprocessor_HeaderSearch = 14,

    CXTUResourceUsage_MEMORY_IN_BYTES_BEGIN = 1,

    CXTUResourceUsage_MEMORY_IN_BYTES_END = 14,

    CXTUResourceUsage_First = 1,

    CXTUResourceUsage_Last = 14,

}



