using System;
using System.Collections.Generic;
using System.IO;
using QuantumBinding.Clang;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;
using QuantumBinding.Generator.Processors;

namespace QuantumBinding.Generator
{
    public abstract class QuantumBindingGenerator
    {
        public void Run()
        {
            var options = new BindingOptions();
            var processingCtx = new ProcessingContext(options);
            processingCtx.AddPreGeneratorPass(new CheckMacrosPass(), ExecutionPassKind.PerTranslationUnit);
            processingCtx.AddPreGeneratorPass(new NormalizeParametersPass(), ExecutionPassKind.PerTranslationUnit);
            OnSetup(options);
            if (!string.IsNullOrEmpty(options.PathToBindingsFile) && File.Exists(options.PathToBindingsFile))
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
                            var tu = new TranslationUnit(mapping.FileName, module) {Name = module.OutputNamespace, NamespaceExtension = mapping.NamespaceExtension, OutputPath = mapping.OutputPath};
                            if (mapping.ReplaceBaseNameSpace)
                            {
                                tu.Name = mapping.NamespaceExtension;
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

                    var generatorOutputs = GenerateCode(processingCtx, module);
                    context.GeneratorOutputs.AddRange(generatorOutputs);

                    processingCtx.RunPostGeneratorPasses(module);

                    processingCtx.RunCodeGenerationPasses(module);

                    SaveGeneratedCode(generatorOutputs);

                    SaveGeneratedCode(context.CodeGeneratorPathOutputs);
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
            foreach (var module in processingCtx.Options.Modules)
            {
                if (module.GenerateOverloadsForArrayParams)
                {
                    processingCtx.AddPreGeneratorPass(new GenerateFunctionOverloadsPass(), ExecutionPassKind.PerTranslationUnit, module);
                }
            }
        }

        private void AfterSetupPassesInternal(ProcessingContext processingCtx)
        {
            var utils = new UtilsExtensionPass() { OutputPath = Module.UtilsOutputPath, OutputFileName = Module.UtilsOutputName, Namespace = Module.UtilsNamespace };
            processingCtx.AddCodeGenerationPass(utils, ExecutionPassKind.Once, Module.GenerateUtilsForModule);
            var specs = GeneratorSpecializations.StructWrappers | GeneratorSpecializations.UnionWrappers;
            foreach (var module in processingCtx.Options.Modules)
            {
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
            generator = new CsFilesGenerator(processingCtx);
            var outputs = generator.GenerateOutputs(module);
            return outputs;
        }

        private void SaveGeneratedCode(IEnumerable<GeneratorOutput> generatorOutputs)
        {
            foreach (var output in generatorOutputs)
            {
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
                    if (codeGenerator.IsGeneratorEmpty)
                    {
                        continue;
                    }

                    string finalPath = path;
                    if (codeGenerator.IsInteropGenerator)
                    {
                        finalPath = Path.Combine(path, TranslationUnit.InteropNamespaceExtension.OutputPath);
                    }

                    if (!string.IsNullOrEmpty(finalPath))
                    {
                        Directory.CreateDirectory(finalPath);
                    }

                    using (var sr = new StreamWriter(Path.Combine(finalPath, GetFileName(output.TranslationUnit, codeGenerator))))
                    {
                        sr.Write(codeGenerator.Generate());
                    }
                }
            }
        }

        private CodeGeneration.Generator generator;

        private string GetFileName(TranslationUnit unit, CodeGenerator codeGenerator)
        {
            if (codeGenerator.Specializations == GeneratorSpecializations.All || codeGenerator.Specializations == GeneratorSpecializations.None)
            {
                return $"{unit.FullNamespace}.{generator.FileExtension}";
            }

            if (codeGenerator.IsInteropGenerator)
            {
                return $"{unit.FullNamespace}.{TranslationUnit.InteropNamespaceExtension.FileName}{codeGenerator.Specializations.ToString()}.{generator.FileExtension}";
            }

            return $"{unit.FullNamespace}.{codeGenerator.Specializations.ToString()}.{generator.FileExtension}";
        }
    }
}
