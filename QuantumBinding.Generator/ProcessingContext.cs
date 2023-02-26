using System.Collections.Generic;
using System.Linq;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Processors;

namespace QuantumBinding.Generator
{
    class ExecutionInfo
    {
        public ExecutionPassKind ExecutionKind { get; set; }

        public Module[] ExecuteFor { get; set; }
    }


    public class ProcessingContext
    {
        Dictionary<IPreGeneratorPass, ExecutionInfo> preGeneratorPassesDict;
        Dictionary<IPostGeneratorPass, ExecutionInfo> postGeneratorPassesDict;
        Dictionary<ICodeGenerationPass, ExecutionInfo> codeGenerationPassesDict;
        private List<IPreGeneratorPass> preGeneratorPassesList;

        public ProcessingContext(BindingOptions options)
        {
            preGeneratorPassesDict = new Dictionary<IPreGeneratorPass, ExecutionInfo>();
            postGeneratorPassesDict = new Dictionary<IPostGeneratorPass, ExecutionInfo>();
            codeGenerationPassesDict = new Dictionary<ICodeGenerationPass, ExecutionInfo>();

            preGeneratorPassesList = new List<IPreGeneratorPass>();
            Options = options;
        }

        public IReadOnlyCollection<IPreGeneratorPass> PreGeneratorPasses => preGeneratorPassesDict.Keys;

        public IReadOnlyCollection<IPostGeneratorPass> PostGeneratorPasses => postGeneratorPassesDict.Keys;

        public IReadOnlyCollection<ICodeGenerationPass> CodeGenerationPasses => codeGenerationPassesDict.Keys;

        public ASTContext AstContext { get; set; }

        public BindingOptions Options { get; }

        private bool CanExecute(ExecutionInfo info, Module module)
        {
            return info.ExecuteFor == null || info.ExecuteFor.Length == 0 || info.ExecuteFor.Contains(module);
        }

        internal void RunPreGeneratorPasses(Module module)
        {
            for (var index = 0; index < preGeneratorPassesList.Count; index++)
            {
                var pass = preGeneratorPassesList[index];
                if (CanExecute(preGeneratorPassesDict[pass], module))
                {
                    do
                    {
                        pass.ProcessingContext = this;
                        pass.Run();
                    } while (pass.RunAgain);
                }
            }

            var dict = preGeneratorPassesDict.Where(x => x.Value.ExecutionKind == ExecutionPassKind.Once && CanExecute(x.Value, module));

            foreach (var pass in dict)
            {
                preGeneratorPassesDict.Remove(pass.Key);
                preGeneratorPassesList.Remove(pass.Key);
            }
        }

        internal void RunPostGeneratorPasses(Module module)
        {
            foreach (var pass in postGeneratorPassesDict)
            {
                if (CanExecute(pass.Value, module))
                {
                    pass.Key.ProcessingContext = this;
                    pass.Key.Run();
                }
            }

            var dict = postGeneratorPassesDict.Where(x => x.Value.ExecutionKind == ExecutionPassKind.Once && CanExecute(x.Value, module));

            foreach (var pass in dict)
            {
                postGeneratorPassesDict.Remove(pass.Key);
            }
        }

        internal void RunCodeGenerationPasses(Module module)
        {
            foreach (var pass in codeGenerationPassesDict)
            {
                if (CanExecute(pass.Value, module))
                {
                    pass.Key.ProcessingContext = this;
                    var outputs = pass.Key.Generate();
                    if (outputs.Count == 0) continue;
                    
                    AstContext.GeneratorOutputs.AddRange(outputs);
                }
            }

            var dict = codeGenerationPassesDict.Where(x => x.Value.ExecutionKind == ExecutionPassKind.Once && CanExecute(x.Value, module));

            foreach (var pass in dict)
            {
                codeGenerationPassesDict.Remove(pass.Key);
            }
        }

        public void AddPreGeneratorPass(IPreGeneratorPass pass, ExecutionPassKind passKind, params Module[] executeFor)
        {
            if (!preGeneratorPassesDict.ContainsKey(pass))
            {
                preGeneratorPassesDict.Add(pass, new ExecutionInfo() { ExecutionKind = passKind, ExecuteFor = executeFor });
                preGeneratorPassesList.Add(pass);
            }
        }

        public void AddPostGeneratorPass(IPostGeneratorPass pass, ExecutionPassKind passKind, params Module[] executeFor)
        {
            if (!postGeneratorPassesDict.ContainsKey(pass))
            {
                postGeneratorPassesDict.Add(pass, new ExecutionInfo() { ExecutionKind = passKind, ExecuteFor = executeFor });
            }
        }

        public void AddCodeGenerationPass(ICodeGenerationPass pass, ExecutionPassKind passKind, params Module[] executeFor)
        {
            if (!codeGenerationPassesDict.ContainsKey(pass))
            {
                codeGenerationPassesDict.Add(pass, new ExecutionInfo() { ExecutionKind = passKind, ExecuteFor = executeFor });
            }
        }
    }
}