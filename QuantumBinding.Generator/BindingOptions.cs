﻿using System.Collections.Generic;
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
            ConvertRules = new ConvertRules();
        }

        public bool GenerateSequentialLayout { get; set; }

        public bool DebugMode { get; set; }

        public string PathToBindingsFile { get; set; }

        public ConvertRules ConvertRules { get; }

        public List<string> ParserArguments { get; set; }

        public Module AddModule(string libraryName)
        {
            var module = new Module(libraryName);
            modulesInternal.Add(module);
            return module;
        }

        private List<Module> modulesInternal;

        public ReadOnlyCollection<Module> Modules { get; }
    }
}