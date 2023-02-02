
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuantumBinding.Clang;

///<summary>
/// Flags that control the creation of translation units.
///</summary>
[Flags]
public enum CXTranslationUnit_Flags : uint
{
    ///<summary>
    /// Used to indicate that no special translation-unit options are needed.
    ///</summary>
    CXTranslationUnit_None = 0,

    ///<summary>
    /// Used to indicate that the parser should construct a "detailed" preprocessing record, including all macro definitions and instantiations.
    ///</summary>
    CXTranslationUnit_DetailedPreprocessingRecord = 1,

    ///<summary>
    /// Used to indicate that the translation unit is incomplete.
    ///</summary>
    CXTranslationUnit_Incomplete = 2,

    ///<summary>
    /// Used to indicate that the translation unit should be built with an implicit precompiled header for the preamble.
    ///</summary>
    CXTranslationUnit_PrecompiledPreamble = 4,

    ///<summary>
    /// Used to indicate that the translation unit should cache some code-completion results with each reparse of the source file.
    ///</summary>
    CXTranslationUnit_CacheCompletionResults = 8,

    ///<summary>
    /// Used to indicate that the translation unit will be serialized with clang_saveTranslationUnit.
    ///</summary>
    CXTranslationUnit_ForSerialization = 16,

    ///<summary>
    /// DEPRECATED: Enabled chained precompiled preambles in C++.
    ///</summary>
    CXTranslationUnit_CXXChainedPCH = 32,

    ///<summary>
    /// Used to indicate that function/method bodies should be skipped while parsing.
    ///</summary>
    CXTranslationUnit_SkipFunctionBodies = 64,

    ///<summary>
    /// Used to indicate that brief documentation comments should be included into the set of code completions returned from this translation unit.
    ///</summary>
    CXTranslationUnit_IncludeBriefCommentsInCodeCompletion = 128,

    ///<summary>
    /// Used to indicate that the precompiled preamble should be created on the first parse. Otherwise it will be created on the first reparse. This trades runtime on the first parse (serializing the preamble takes time) for reduced runtime on the second parse (can now reuse the preamble).
    ///</summary>
    CXTranslationUnit_CreatePreambleOnFirstParse = 256,

    ///<summary>
    /// Do not stop processing when fatal errors are encountered.
    ///</summary>
    CXTranslationUnit_KeepGoing = 512,

    ///<summary>
    /// Sets the preprocessor in a mode for parsing a single file only.
    ///</summary>
    CXTranslationUnit_SingleFileParse = 1024,

    ///<summary>
    /// Used in combination with CXTranslationUnit_SkipFunctionBodies to constrain the skipping of function bodies to the preamble.
    ///</summary>
    CXTranslationUnit_LimitSkipFunctionBodiesToPreamble = 2048,

    ///<summary>
    /// Used to indicate that attributed types should be included in CXType.
    ///</summary>
    CXTranslationUnit_IncludeAttributedTypes = 4096,

    ///<summary>
    /// Used to indicate that implicit attributes should be visited.
    ///</summary>
    CXTranslationUnit_VisitImplicitAttributes = 8192,

    ///<summary>
    /// Used to indicate that non-errors from included files should be ignored.
    ///</summary>
    CXTranslationUnit_IgnoreNonErrorsFromIncludedFiles = 16384,

    ///<summary>
    /// Tells the preprocessor not to skip excluded conditional blocks.
    ///</summary>
    CXTranslationUnit_RetainExcludedConditionalBlocks = 32768,

}



