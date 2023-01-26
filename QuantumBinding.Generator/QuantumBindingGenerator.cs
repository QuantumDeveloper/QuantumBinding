using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuantumBinding.Clang;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;
using QuantumBinding.Generator.Processors;

namespace QuantumBinding.Generator
{
    public abstract class QuantumBindingGenerator
    {
        private CodeGeneration.FileGenerator fileGenerator;
        public void Run()
        {
            var options = new BindingOptions();
            var processingCtx = new ProcessingContext(options);
            processingCtx.AddPreGeneratorPass(new CheckMacrosPass(), ExecutionPassKind.PerTranslationUnit);
            processingCtx.AddPreGeneratorPass(new NormalizeParametersPass(), ExecutionPassKind.PerTranslationUnit);
            OnSetup(options);
            if (File.Exists(options.PathToBindingsFile))
            {
                processingCtx.AddPreGeneratorPass(new LoadBindingsFromFilePass(options.PathToBindingsFile), ExecutionPassKind.PerTranslationUnit);
            }
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

                var clangIndex = clang.createIndex(0, 0);
                List<string> arguments = new List<string>
                {
                    "-std=c++14",                           // The input files should be compiled for C++ 11
                    "-xc++",                                // The input files are C++
                    "-Wno-pragma-once-outside-header",       // We are processing files which may be header files
                    "-v",
                    "-fparse-all-comments" //To make clang parse comments
                };

                foreach (var define in module.Defines)
                {
                    arguments.Add($"-D{define}");
                }

                foreach (var include in module.IncludeDirs)
                {
                    arguments.Add($"-I{include}");
                }

                List<TranslationUnit> translationUnits = new List<TranslationUnit>();

                try
                {
                    foreach (var file in module.Files)
                    {
                        TranslationUnit unit = new TranslationUnit(file, module);
                        unit.Parse(clangIndex, arguments);
                        if (unit.IsValid)
                        {
                            translationUnits.Add(unit);
                            module.TranslationUnits.Add(unit);
                        }
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
                                var units = unit.FindDeclarationsBySourceLocation(mapping.FileName, true);
                                tu.AddDeclarations(units);
                            }
                            tu.SetOwner(); // Update owner inside each declaration because now declarations belong to new TranslationUnit

                            tuList.Add(tu);
                        }

                        translationUnits.AddRange(tuList);
                    }

                    DeclarationUnit.TranslationUnitsPool = translationUnits;

                    ASTContext context = new ASTContext(translationUnits);
                    processingCtx.AstContext = context;
                    context.Module = module;

                    processingCtx.RunPreGeneratorPasses(module);

                    OnPostProcess(processingCtx);

                    processingCtx.RunCodeGenerationPasses(module);
                    
                    processingCtx.RunPostGeneratorPasses(module);
                    
                    RemoveFilesFromPreviousGeneration(processingCtx.AstContext);

                    SaveGeneratedCode(processingCtx.AstContext.GeneratorOutputs);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    clangIndex.disposeIndex();
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
                processingCtx.AddCodeGenerationPass(new BasicCodeGeneratorPass(), ExecutionPassKind.PerTranslationUnit, module);
                
                if (module.WrapInteropObjects)
                {
                    processingCtx.AddPreGeneratorPass(new WrappersCreationPass(specs), ExecutionPassKind.PerTranslationUnit, module);
                    processingCtx.AddPreGeneratorPass(new UpdateWrappedMethodParametersPass(specs), ExecutionPassKind.PerTranslationUnit, module);
                    processingCtx.AddPreGeneratorPass(new GlobalScopeToClassMethod(), ExecutionPassKind.PerTranslationUnit, module);

                    processingCtx.AddCodeGenerationPass(new WrappersGenerationPass(), ExecutionPassKind.PerTranslationUnit, module);
                }
            }
        }

        private List<GeneratorOutput> GenerateCode(ProcessingContext processingCtx, Module module)
        {
            fileGenerator = new CsFilesFileGenerator(processingCtx);
            var outputs = fileGenerator.GenerateOutputs(module);
            return outputs;
        }

        private void RemoveFilesFromPreviousGeneration(ASTContext context)
        {
            foreach (var unit in context.TranslationUnits)
            {
                if (!unit.Module.CleanPreviousGeneration) continue;

                if (Directory.Exists(unit.Module.OutputPath))
                {
                    Directory.Delete(unit.Module.OutputPath, true);
                }

                foreach (var mapping in context.Module.NamespaceMapping)
                {
                    if (Directory.Exists(mapping.OutputPath))
                    {
                        Directory.Delete(mapping.OutputPath, true);
                    }
                }
            }
        }

        private void SaveGeneratedCode(IEnumerable<GeneratorOutput> generatorOutputs)
        {
            foreach (var output in generatorOutputs)
            {
                var gen = output.Outputs.FirstOrDefault(x => x is FileExtensionGenerator);

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

                    if (!string.IsNullOrEmpty(finalPath))
                    {
                        Directory.CreateDirectory(finalPath);
                    }

                    File.WriteAllText(Path.Combine(finalPath, codeGenerator.GetFileName(output.TranslationUnit)),
                        codeGenerator.Generate());
                }
            }
        }

        private string GetFileName(TranslationUnit unit, CodeGenerator codeGenerator)
        {
            if (!unit.Module.EachTypeInSeparateFile)
            {
                return $"{unit.FullNamespace}.{codeGenerator.Category}.{fileGenerator.FileExtension}";
            }
            
            if (codeGenerator.Category is GeneratorCategory.Macros or GeneratorCategory.Functions
                or GeneratorCategory.StaticMethods)
            {
                return $"{unit.FullName}.{codeGenerator.Name}.{fileGenerator.FileExtension}";
            }

            return $"{codeGenerator.Name}.{fileGenerator.FileExtension}";
        }
    }
}
