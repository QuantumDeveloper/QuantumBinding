using QuantumBinding.Generator;
using QuantumBinding.Generator.ProcessingFluentApi;
using QuantumBinding.Generator.Processors;
using QuantumBinding.Generator.Utils;

namespace QuickStart;

/// <summary>The generator of the README's quick start: bindings for native/include/mylib.h into ../MyLib.</summary>
public class MyLibGenerator : QuantumBindingGenerator
{
    private Module module;

    public override void OnSetup(BindingOptions options)
    {
        module = Module.Create("mylib");
        module.Name = "MyLib";
        module.Files.Add("native/include/mylib.h");
        module.IncludeDirs.Add("native/include");
        module.OutputPath = "../MyLib/Generated";
        module.OutputFileName = "MyLib";
        module.OutputNamespace = "MyLib";
        module.InteropSubNamespace = "Interop";
        module.InteropClassName = "MyLibInterop";
        module.MethodClassName = "MyLibNative";
        module.EachTypeInSeparateFile = true;

        options.AddModule(module);
    }

    public override void OnBeforeSetupPasses(ProcessingContext context)
    {
        var api = new PostProcessingApi();
        api.Function("mylib_watch")
            .WithParameterName("userData")
            .InterpretAsPointerToVoid()
            .SetParameterKind(ParameterKind.In);
        context.AddPreGeneratorPass(new PostProcessingApiPass(api), ExecutionPassKind.PerTranslationUnit);
    }

    public override void OnSetupPostProcessing(ProcessingContext context)
    {
        context.AddPreGeneratorPass(new FunctionToInstanceMethodPass(), ExecutionPassKind.PerTranslationUnit, module);
        context.AddPreGeneratorPass(
            new RegexRenamePass("^mylib_", "", RenameTargets.Method, true),
            ExecutionPassKind.PerTranslationUnit,
            module);
        context.AddPreGeneratorPass(
            new CaseRenamePass(RenameTargetsUtils.AnyExcept(RenameTargets.Function), CasePattern.PascalCase),
            ExecutionPassKind.PerTranslationUnit,
            module);
    }
}
