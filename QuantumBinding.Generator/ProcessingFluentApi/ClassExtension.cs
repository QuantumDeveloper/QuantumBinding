using System.Collections.Generic;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.ProcessingFluentApi
{
    public class ClassExtension
    {
        public ClassExtension()
        {
            Fields = new List<FieldExtension>();
            FieldsToAdd = new List<FieldExtension>();
            PropertiesToAdd = new List<PropertyExtension>();
        }

        public ClassExtension(string name) : this()
        {
            Name = name;
        }

        public string Name { get; set; }

        public List<FieldExtension> Fields { get; }

        public List<FieldExtension> FieldsToAdd { get; }

        public List<PropertyExtension> PropertiesToAdd { get; }

        public ClassType ClassType { get; set; }

        public BindingType UnderlyingNativeType { get; set; }

        public bool IsDisposable { get; set; }

        public string DisposeBody { get; set; }
        
        public bool IsIgnored { get; set; }
        
        public bool CopyFieldsFromLinkedObject { get; set; }

        public bool CleanObject { get; set; }
    }
}