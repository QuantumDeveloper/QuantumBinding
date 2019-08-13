using QuantumBinding.Generator;
using QuantumBinding.Generator.CodeGeneration;
using QuantumBinding.Generator.ProcessingFluentApi;
using QuantumBinding.Generator.Processors;
using QuantumBinding.Generator.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace QuantumBinding.ClangGenerator
{
    public class ClangBindingGenerator : QuantumBindingGenerator
    {
        public override void OnSetup(BindingOptions options)
        {
            var appRoot = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.LastIndexOf("bin"));
            string outputPath = Path.GetFullPath(Path.Combine(appRoot, "..\\", "QuantumBinding.Clang"));
            string library = "libclang";
            options.GenerateSequentialLayout = true;
            options.DebugMode = false;
            options.ConvertRules.PodTypesAsSimpleTypes = true;
            var clangModule = options.AddModule(library);
            clangModule.Defines.Add("_MSC_VER");
            clangModule.Defines.Add("_CINDEX_LIB_");
            clangModule.IncludeDirs.Add("./clang-c");
            clangModule.Files.Add(@"M:\GitHUB\LLVM\llvm-project\clang\include\Documentation.h");
            clangModule.ForceCallingConvention = true;
            clangModule.AllowConvertStructToClass = false;
            clangModule.CallingConvention = CallingConvention.Cdecl;
            clangModule.MethodClassName = "clang";
            clangModule.InteropClassName = "ClangInterop";
            clangModule.GeneratorSpecializations = GeneratorSpecializations.Classes |
                                                    GeneratorSpecializations.Enums |
                                                    GeneratorSpecializations.Functions |
                                                    GeneratorSpecializations.Delegates |
                                                    GeneratorSpecializations.Structs |
                                                    GeneratorSpecializations.Unions;
            clangModule.OutputPath = outputPath;
            clangModule.OutputFileName = "QuantumBinding.Clang";
            clangModule.OutputNamespace = "QuantumBinding.Clang";
            clangModule.SuppressUnmanagedCodeSecurity = false;
            clangModule.TreatOutputArraysAsPointers = true;
            clangModule.WrapInteropObjects = false;

            Module.UtilsOutputName = "Utils";
            Module.UtilsNamespace = clangModule.OutputNamespace;
        }
        public override void OnSetupPostProcessing(ProcessingContext context)
        {
            AddFunctionsToFix(context);
            context.AddPreGeneratorPass(new FunctionToInstanceMethodAction(), ExecutionPassKind.PerTranslationUnit);
            context.AddPreGeneratorPass(new ForceCallingConventionPass(CallingConvention.Cdecl), ExecutionPassKind.PerTranslationUnit);
            context.AddPreGeneratorPass(new CheckFlagEnumsPass(), ExecutionPassKind.PerTranslationUnit);
            context.AddPreGeneratorPass(new RegexRenamePass("^clang_", "", RenameTargets.Method, true), ExecutionPassKind.PerTranslationUnit);
        }

        private void AddFunctionsToFix(ProcessingContext ctx)
        {
            PostProcessingApi api = new PostProcessingApi();

            api.Functions("parseTranslationUnit", "clang_parseTranslationUnit2").
                WithParameterName("unsaved_files").
                TreatAsPointerToArray(new CustomType("CXUnsavedFile")).
                SetParameterKind(ParameterKind.In);

            api.Function("clang_getFileContents").
                WithParameterName("size").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_getExpansionLocation").
                WithParameterName("file").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("line").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("column").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("offset").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_getPresumedLocation").
                WithParameterName("filename").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("line").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("column").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_getInstantiationLocation").
                WithParameterName("file").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("line").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("column").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("offset").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_getSpellingLocation").
                WithParameterName("file").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("line").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("column").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("offset").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_getFileLocation").
                WithParameterName("file").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("line").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("column").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("offset").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_loadDiagnostics").
                WithParameterName("error").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("errorString").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_getDiagnosticOption").
                WithParameterName("Disable").
                TreatAsIs().
                SetParameterKind(ParameterKind.In);
                //SetNullable(true);

            api.Function("clang_getCursorPlatformAvailability").
                WithParameterName("always_deprecated").
                TreatAsIs().
                SetParameterKind(ParameterKind.In).
                //SetNullable(true).
                WithParameterName("deprecated_message").
                TreatAsIs().
                //SetNullable(true).
                WithParameterName("always_unavailable").
                TreatAsIs().
                //SetNullable(true).
                WithParameterName("unavailable_message").
                TreatAsIs().
                //SetNullable(true).
                WithParameterName("availability").
                TreatAsPointerToArray(new CustomType("CXPlatformAvailability"));

            api.Function("clang_getOverriddenCursors").
                WithParameterName("overridden").
                TreatAsPointerToArray(new CustomType("CXCursor"), true, "num_overridden").
                SetParameterKind(ParameterKind.Out).
                WithParameterName("num_overridden").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_Cursor_isExternalSymbol").
                WithParameterName("language").
                TreatAsIs().
                //SetNullable(true).
                WithParameterName("definedIn").
                TreatAsIs().
                //SetNullable(true).
                WithParameterName("isGenerated").
                TreatAsIs();
                //SetNullable(true);

            api.Function("clang_tokenize").
                WithParameterName("Tokens").
                TreatAsPointerToArray(new CustomType("CXToken"), true, "NumTokens").
                SetParameterKind(ParameterKind.Out).
                WithParameterName("NumTokens").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_annotateTokens").
                WithParameterName("Tokens").
                TreatAsPointerToArray(new CustomType("CXToken")).
                SetParameterKind(ParameterKind.In).
                WithParameterName("Cursors").
                TreatAsPointerToArray(new CustomType("CXCursor"), true, "NumTokens").
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_disposeTokens").
                WithParameterName("Tokens").
                TreatAsPointerType(new BuiltinType(PrimitiveType.IntPtr)).
                SetParameterKind(ParameterKind.In);

            api.Function("clang_executeOnThread").
                WithParameterName("fn").
                TreatAsPointerType(new BuiltinType(PrimitiveType.Void)).
                SetParameterKind(ParameterKind.In);

            var funcList = new List<string>
            {
                "clang_sortCodeCompletionResults",
                "clang_codeCompleteGetNumDiagnostics",
                "clang_codeCompleteGetDiagnostic",
                "clang_codeCompleteGetContexts",
                "clang_codeCompleteGetContainerKind",
                "clang_codeCompleteGetContainerUSR",
                "clang_codeCompleteGetObjCSelector"
            };

            api.Functions(funcList.ToArray()).
                WithParameterName("Results").
                TreatAsPointerToArray(new CustomType("CXCompletionResult")).
                SetParameterKind(ParameterKind.In);

            api.Function("clang_disposeCodeCompleteResults").
                WithParameterName("CXCompletionResult").
                TreatAsPointerType(new BuiltinType(PrimitiveType.IntPtr)).
                SetParameterKind(ParameterKind.In);

            api.Function("clang_codeCompleteGetContainerKind").
                WithParameterName("IsIncomplete").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_remap_getFilenames").
                WithParameterName("original").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("transformed").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_indexLoc_getFileLocation").
                WithParameterName("indexFile").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("file").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("line").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("column").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("offset").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_CompilationDatabase_fromDirectory").
                WithParameterName("ErrorCode").
                TreatAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Class("CXCursorAndRangeVisitor")
                .WithField("visit")
                .TreatAsPointerType(new BuiltinType(PrimitiveType.Void));

            api.Class("IndexerCallbacks")
                .WithField("abortQuery")
                .TreatAsPointerType(new BuiltinType(PrimitiveType.Void))
                .WithField("diagnostic")
                .TreatAsPointerType(new BuiltinType(PrimitiveType.Void))
                .WithField("enteredMainFile")
                .TreatAsPointerType(new BuiltinType(PrimitiveType.Void))
                .WithField("ppIncludedFile")
                .TreatAsPointerType(new BuiltinType(PrimitiveType.Void))
                .WithField("importedASTFile")
                .TreatAsPointerType(new BuiltinType(PrimitiveType.Void))
                .WithField("startedTranslationUnit")
                .TreatAsPointerType(new BuiltinType(PrimitiveType.Void))
                .WithField("indexDeclaration")
                .TreatAsPointerType(new BuiltinType(PrimitiveType.Void))
                .WithField("indexEntityReference")
                .TreatAsPointerType(new BuiltinType(PrimitiveType.Void)); 

             var fixingFunctionParameters = new FixIncorrectParametersPass(api);
            ctx.AddPreGeneratorPass(fixingFunctionParameters, ExecutionPassKind.PerTranslationUnit);
        }
    }
}
