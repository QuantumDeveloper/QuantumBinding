using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace QuantumBinding.Generator
{
    public class BindingOptions
    {
        public BindingOptions()
        {
            modulesInternal = new List<Module>();
            Modules = new ReadOnlyCollection<Module>(modulesInternal);
            ParserArguments = new List<string>();
        }
        
        public bool GenerateSequentialLayout { get; set; }

        public bool DebugMode { get; set; }

        public string PathToBindingsFile { get; set; }

        public bool PodTypesAsSimpleTypes { get; set; }

        public List<string> ParserArguments { get; set; }

        public void AddModule(Module module)
        {
            modulesInternal.Add(module);
        }

        private readonly List<Module> modulesInternal;

        public ReadOnlyCollection<Module> Modules { get; }
    }
}