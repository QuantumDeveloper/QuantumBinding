﻿using System.Collections.Generic;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.ProcessingFluentApi
{
    public class FunctionExtension
    {
        public FunctionExtension()
        {
            Parameters = new List<ParameterExtension>();
        }
        
        public string EntryPointName { get; set; }

        public string DecoratedName { get; set; }

        public BindingType ReturnType { get; set; }

        public List<ParameterExtension> Parameters { get; set; }
    }
}