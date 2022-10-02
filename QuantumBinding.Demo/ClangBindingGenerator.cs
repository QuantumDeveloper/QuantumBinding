using QuantumBinding.Generator;
using QuantumBinding.Generator.CodeGeneration;
using QuantumBinding.Generator.ProcessingFluentApi;
using QuantumBinding.Generator.Processors;
using QuantumBinding.Generator.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using QuantumBinding.Generator.Utils;

namespace QuantumBinding.ClangGenerator
{
    public class ClangBindingGenerator : QuantumBindingGenerator
    {
        public override void OnSetup(BindingOptions options)
        {
            var appRoot = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.LastIndexOf("bin"));
            string outputPath = Path.GetFullPath(Path.Combine(appRoot, "..", "QuantumBinding.Clang", "Generated"));
            string library = "libclang";
            options.GenerateSequentialLayout = true;
            options.DebugMode = false;
            options.PodTypesAsSimpleTypes = true;
            var clangModule = options.AddModule(library);
            clangModule.Defines.Add("_MSC_VER");
            clangModule.Defines.Add("_CINDEX_LIB_");
            clangModule.IncludeDirs.Add(@"C:\Source\llvm-project-master\clang\include\clang-c");
            clangModule.Files.Add(@"C:\Source\llvm-project-master\clang\include\clang-c\Documentation.h");
            clangModule.ForceCallingConvention = true;
            clangModule.AllowConvertStructToClass = true;
            clangModule.CallingConvention = CallingConvention.Cdecl;
            clangModule.MethodClassName = "clang";
            clangModule.InteropClassName = "ClangInterop";
            clangModule.GeneratorSpecializations = GeneratorSpecializationUtils.AllExcept(GeneratorSpecializations.Constants); 
            clangModule.OutputPath = outputPath;
            clangModule.OutputFileName = "QuantumBinding.Clang";
            clangModule.OutputNamespace = "QuantumBinding.Clang";
            clangModule.SuppressUnmanagedCodeSecurity = false;
            clangModule.WrapInteropObjects = true;
        }
        public override void OnSetupPostProcessing(ProcessingContext context)
        {
            AddFunctionsToFix(context);
            context.AddPreGeneratorPass(new FunctionToInstanceMethodPass(), ExecutionPassKind.PerTranslationUnit);
            context.AddPreGeneratorPass(new ForceCallingConventionPass(CallingConvention.Cdecl), ExecutionPassKind.PerTranslationUnit);
            context.AddPreGeneratorPass(new CheckFlagEnumsPass(), ExecutionPassKind.PerTranslationUnit);
        }

        public override void OnSetupComplete(ProcessingContext context)
        {
            base.OnSetupComplete(context);
            context.AddPreGeneratorPass(new RegexRenamePass("^clang_", "", RenameTargets.Method, true), ExecutionPassKind.PerTranslationUnit);
            context.AddPreGeneratorPass(new RegexRenamePass("^CX", "QB", RenameTargets.Class | RenameTargets.StructWrapper | RenameTargets.UnionWrapper, true), ExecutionPassKind.PerTranslationUnit);
        }

        private void AddFunctionsToFix(ProcessingContext ctx)
        {
            PostProcessingApi api = new PostProcessingApi();

            api.Functions("parseTranslationUnit", "clang_parseTranslationUnit2").
                WithParameterName("unsaved_files").
                InterpretAsPointerToArray(new CustomType("CXUnsavedFile")).
                SetParameterKind(ParameterKind.In);

            api.Function("clang_getFileContents").
                WithParameterName("size").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_getExpansionLocation").
                WithParameterName("file").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("line").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("column").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("offset").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_getPresumedLocation").
                WithParameterName("filename").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("line").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("column").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_getInstantiationLocation").
                WithParameterName("file").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("line").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("column").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("offset").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_getSpellingLocation").
                WithParameterName("file").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("line").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("column").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("offset").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_getFileLocation").
                WithParameterName("file").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("line").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("column").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("offset").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_loadDiagnostics").
                WithParameterName("error").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("errorString").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_getDiagnosticOption").
                WithParameterName("Disable").
                InterpretAsIs().
                SetParameterKind(ParameterKind.In);

            api.Function("clang_getCursorPlatformAvailability").
                WithParameterName("always_deprecated").
                InterpretAsIs().
                SetParameterKind(ParameterKind.In).
                WithParameterName("deprecated_message").
                InterpretAsIs().
                WithParameterName("always_unavailable").
                InterpretAsIs().
                WithParameterName("unavailable_message").
                InterpretAsIs().
                WithParameterName("availability").
                InterpretAsPointerToArray(new CustomType("CXPlatformAvailability"));

            api.Function("clang_getOverriddenCursors").
                WithParameterName("overridden").
                InterpretAsPointerToArray(new CustomType("CXCursor"), true, "num_overridden").
                SetParameterKind(ParameterKind.Out).
                WithParameterName("num_overridden").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_Cursor_isExternalSymbol").
                WithParameterName("language").
                InterpretAsIs().
                WithParameterName("definedIn").
                InterpretAsIs().
                WithParameterName("isGenerated").
                InterpretAsIs();

            api.Function("clang_tokenize").
                WithParameterName("Tokens").
                InterpretAsPointerToArray(new CustomType("CXToken"), true, "NumTokens").
                SetParameterKind(ParameterKind.Out).
                WithParameterName("NumTokens").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_annotateTokens").
                WithParameterName("Tokens").
                InterpretAsPointerToArray(new CustomType("CXToken")).
                SetParameterKind(ParameterKind.In).
                WithParameterName("Cursors").
                InterpretAsPointerToArray(new CustomType("CXCursor"), true, "NumTokens").
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_disposeTokens").
                WithParameterName("Tokens").
                InterpretAsPointerType(new BuiltinType(PrimitiveType.IntPtr)).
                SetParameterKind(ParameterKind.In);

            api.Function("clang_executeOnThread").
                WithParameterName("fn").
                InterpretAsPointerType(new BuiltinType(PrimitiveType.Void)).
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
                InterpretAsPointerToArray(new CustomType("CXCompletionResult")).
                SetParameterKind(ParameterKind.In);

            api.Function("clang_disposeCodeCompleteResults").
                WithParameterName("CXCompletionResult").
                InterpretAsPointerType(new BuiltinType(PrimitiveType.IntPtr)).
                SetParameterKind(ParameterKind.In);

            api.Function("clang_codeCompleteGetContainerKind").
                WithParameterName("IsIncomplete").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_remap_getFilenames").
                WithParameterName("original").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("transformed").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_indexLoc_getFileLocation").
                WithParameterName("indexFile").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("file").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("line").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("column").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out).
                WithParameterName("offset").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Function("clang_CompilationDatabase_fromDirectory").
                WithParameterName("ErrorCode").
                InterpretAsIs().
                SetParameterKind(ParameterKind.Out);

            api.Class("CXCursorAndRangeVisitor")
                .WithField("visit")
                .InterpretAsPointerType(new BuiltinType(PrimitiveType.Void));

            api.Class("IndexerCallbacks")
                .WithField("abortQuery")
                .InterpretAsPointerType(new BuiltinType(PrimitiveType.Void))
                .WithField("diagnostic")
                .InterpretAsPointerType(new BuiltinType(PrimitiveType.Void))
                .WithField("enteredMainFile")
                .InterpretAsPointerType(new BuiltinType(PrimitiveType.Void))
                .WithField("ppIncludedFile")
                .InterpretAsPointerType(new BuiltinType(PrimitiveType.Void))
                .WithField("importedASTFile")
                .InterpretAsPointerType(new BuiltinType(PrimitiveType.Void))
                .WithField("startedTranslationUnit")
                .InterpretAsPointerType(new BuiltinType(PrimitiveType.Void))
                .WithField("indexDeclaration")
                .InterpretAsPointerType(new BuiltinType(PrimitiveType.Void))
                .WithField("indexEntityReference")
                .InterpretAsPointerType(new BuiltinType(PrimitiveType.Void));

            api.Class("CXIdxEntityInfo")
                .WithField("attributes")
                .InterpretAsPointerToArray(new CustomType("CXIdxAttrInfo"), true, "numAttributes");

            api.Class("CXIdxDeclInfo")
                .WithField("attributes")
                .InterpretAsPointerToArray(new CustomType("CXIdxAttrInfo"), true, "numAttributes");

            api.Class("CXIdxObjCProtocolRefListInfo")
                .WithField("protocols")
                .InterpretAsPointerToArray(new CustomType("CXIdxObjCProtocolRefInfo"), true, "numProtocols");

            api.Class("CXIdxCXXClassDeclInfo")
                .WithField("bases")
                .InterpretAsPointerToArray(new CustomType("CXIdxBaseClassInfo"), true, "numBases");

            var fixingFunctionParameters = new PostProcessingApiPass(api);
            ctx.AddPreGeneratorPass(fixingFunctionParameters, ExecutionPassKind.PerTranslationUnit);
        }
    }
}
