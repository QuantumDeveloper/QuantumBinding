using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;
using QuantumBinding.Generator.Processors;

namespace QuantumBinding.Generator;

public abstract class QuantumBindingGenerator
{
    public void Run()
    {
        var options = new BindingOptions();
        var processingCtx = new ProcessingContext(options);
        processingCtx.AddPreGeneratorPass(new CheckMacrosPass(), ExecutionPassKind.PerTranslationUnit);
        processingCtx.AddPreGeneratorPass(new NormalizeParametersPass(), ExecutionPassKind.PerTranslationUnit);
        OnSetup(options);
        OnBeforeSetupPasses(processingCtx);
        BeforeSetupPassesInternal(processingCtx);
        OnSetupPostProcessing(processingCtx);
        AfterSetupPassesInternal(processingCtx);
        OnSetupComplete(processingCtx);
        RunInternal(processingCtx);
    }

    public string GeneratorName => "QuantumBindingGenerator";

    private void RunInternal(ProcessingContext processingCtx)
    {
        foreach (var module in processingCtx.Options.Modules)
        {
            if (module == null) continue;
                
            if (string.IsNullOrEmpty(module.LibraryName))
            {
                throw new ArgumentNullException("LibraryName should not be empty");
            }

            if (string.IsNullOrEmpty(module.OutputFileName))
            {
                throw new ArgumentNullException("OutputFile should not be empty");
            }

            if (string.IsNullOrEmpty(module.OutputNamespace))
            {
                throw new ArgumentNullException("OutputNamespace should not be empty");
            }
                
            if (string.IsNullOrEmpty(module.InteropSubNamespace))
            {
                throw new ArgumentNullException("InteropSubNamespace should not be empty");
            }

            if (string.IsNullOrEmpty(module.MethodClassName))
            {
                throw new ArgumentNullException("MethodClassName should not be empty");
            }

            if (string.IsNullOrEmpty(module.InteropClassName))
            {
                throw new ArgumentNullException("InteropClassName should not be empty");
            }
            
            ArgumentNullException.ThrowIfNull(module.MetadataProvider, nameof(module.MetadataProvider));

            var translationUnits = new List<TranslationUnit>();

            try
            {
                foreach (var file in module.Files)
                {
                    var unit = new TranslationUnit(file, module);
                    unit.Parse();
                    if (!unit.IsValid)
                    {
                        var reason = File.Exists(file) ? unit.ParseResult.ToString() : "file not found";
                        throw new InvalidOperationException(
                            $"Module '{module.Name}': could not parse '{file}' ({reason}).");
                    }

                    translationUnits.Add(unit);
                    module.TranslationUnits.Add(unit);
                }

                if (translationUnits.Count > 1)
                {
                    var mainUnit = translationUnits[0];
                    for(int i= translationUnits.Count - 1; i >= 1 ; i--)
                    {
                        mainUnit.Merge(translationUnits[i]);
                        translationUnits.RemoveAt(i);
                    }
                }

                if (module.NamespaceMapping.Count > 0)
                {
                    var tuList = new List<TranslationUnit>();
                    foreach (var mapping in module.NamespaceMapping)
                    {
                        var tu = new TranslationUnit(mapping.FileName, module) {Name = module.OutputNamespace, NamespaceExtension = mapping.SubNamespace, OutputPath = mapping.OutputPath};
                        if (mapping.ReplaceBaseNameSpace)
                        {
                            tu.Name = mapping.SubNamespace;
                            tu.NamespaceExtension = string.Empty;
                        }

                        foreach (var unit in translationUnits)
                        {
                            var declarations = unit.FindDeclarationsBySourceLocation(mapping.FileName, true);
                            tu.AddDeclarations(declarations);
                        }
                        tu.SetOwner(); // Update an owner inside each declaration because now declarations belong to the new Translation Unit

                        tuList.Add(tu);
                    }

                    translationUnits.AddRange(tuList);
                }

                DeclarationUnit.TranslationUnitsPool = translationUnits;

                var context = new ASTContext(translationUnits);
                processingCtx.AstContext = context;
                context.Module = module;

                processingCtx.RunPreGeneratorPasses(module);

                OnPostProcess(processingCtx);

                processingCtx.RunCodeGenerationPasses(module);
                    
                processingCtx.RunPostGeneratorPasses(module);

                var files = RenderGeneratedCode(processingCtx.AstContext.GeneratorOutputs);
                WriteGeneratedCode(files);
                RemoveFilesFromPreviousGeneration(processingCtx.AstContext, files);
            }
            finally
            {
                module.MetadataProvider.Dispose();
            }
        }
    }

    public virtual void OnSetup(BindingOptions options) {}

    public virtual void OnSetupPostProcessing(ProcessingContext processingCtx) {}

    public virtual void OnPostProcess(ProcessingContext processingCtx) {}

    public virtual void OnSetupComplete(ProcessingContext processingCtx) { }

    public virtual void OnBeforeSetupPasses(ProcessingContext processingCtx)
    {
    }

    private void BeforeSetupPassesInternal(ProcessingContext processingCtx)
    {

    }

    private void AfterSetupPassesInternal(ProcessingContext processingCtx)
    {
        var specs = GeneratorSpecializations.StructWrappers | GeneratorSpecializations.UnionWrappers;
        foreach (var module in processingCtx.Options.Modules)
        {
            if (module == null) continue;
                
            processingCtx.AddCodeGenerationPass(new BasicCodeGeneratorPass(module.GeneratorSpecializations), ExecutionPassKind.PerTranslationUnit, module);

            processingCtx.AddPreGeneratorPass(new WrappersCreationPass(specs), ExecutionPassKind.PerTranslationUnit, module);
            processingCtx.AddPreGeneratorPass(new UpdateWrappedMethodParametersPass(specs), ExecutionPassKind.PerTranslationUnit, module);
            processingCtx.AddPreGeneratorPass(new GlobalScopeToClassMethod(), ExecutionPassKind.PerTranslationUnit, module);
            processingCtx.AddPreGeneratorPass(new ContextGenerationAnalyzerPass(), ExecutionPassKind.PerTranslationUnit, module);

            processingCtx.AddCodeGenerationPass(new WrappersCodeGenerationPass(), ExecutionPassKind.PerTranslationUnit, module);
        }
    }

    private static void RemoveFilesFromPreviousGeneration(ASTContext context, Dictionary<string, string> generated)
    {
        foreach (var unit in context.TranslationUnits)
        {
            if (!unit.Module.CleanPreviousGeneration) continue;

            RemoveStaleFiles(unit.Module.OutputPath, generated);

            foreach (var mapping in context.Module.NamespaceMapping)
            {
                RemoveStaleFiles(mapping.OutputPath, generated);
            }
        }
    }

    private static void RemoveStaleFiles(string root, Dictionary<string, string> generated)
    {
        if (string.IsNullOrEmpty(root) || !Directory.Exists(root))
        {
            return;
        }

        foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
        {
            if (!generated.ContainsKey(Path.GetFullPath(file)))
            {
                File.Delete(file);
            }
        }

        var directories = Directory.EnumerateDirectories(root, "*", SearchOption.AllDirectories)
            .OrderByDescending(directory => directory.Length);
        foreach (var directory in directories)
        {
            if (!Directory.EnumerateFileSystemEntries(directory).Any())
            {
                Directory.Delete(directory);
            }
        }
    }

    private static void WriteGeneratedCode(Dictionary<string, string> files)
    {
        foreach (var (path, code) in files)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, code);
        }
    }

    private static Dictionary<string, string> RenderGeneratedCode(IEnumerable<GeneratorOutput> generatorOutputs)
    {
        var files = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var output in generatorOutputs)
        {
            if (output.Outputs.Count == 0) continue;
                
            string path;
            if (!string.IsNullOrEmpty(output.TranslationUnit.OutputPath))
            {
                path = output.TranslationUnit.OutputPath ?? string.Empty;
            }
            else
            {
                path = output.TranslationUnit.Module.OutputPath ?? string.Empty;
            }

            foreach (var codeGenerator in output.Outputs)
            {
                if (codeGenerator.IsEmpty)
                {
                    continue;
                }

                var folderName = codeGenerator.FolderName;
                    
                string finalPath;
                if (codeGenerator.IsInteropGenerator)
                {
                    finalPath = Path.Combine(path, TranslationUnit.InteropNamespaceExtension.OutputPath, folderName);
                }
                else
                {
                    finalPath = Path.Combine(path, folderName);
                }

                var file = Path.GetFullPath(Path.Combine(finalPath, codeGenerator.GetFileName(output.TranslationUnit)));
                files[file] = codeGenerator.Generate();
            }
        }

        return files;
    }
}