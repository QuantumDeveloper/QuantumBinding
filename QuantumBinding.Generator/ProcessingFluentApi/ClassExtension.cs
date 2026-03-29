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
            Operators = new List<OperatorExtension>();
            Constructors = new List<ConstructorExtension>();
        }

        public ClassExtension(string name) : this()
        {
            Name = name;
        }

        static ClassExtension()
        {
            AllWrappers = new ClassExtension("AllWrappers");
        }
        
        public static ClassExtension AllWrappers { get; } 
        
        public string TranslationUnitFileName { get; set; }

        public string Name { get; set; }

        public List<FieldExtension> Fields { get; }

        public List<FieldExtension> FieldsToAdd { get; }

        public List<PropertyExtension> PropertiesToAdd { get; }

        public ClassType ClassType { get; set; }
        
        public string NativeStructName { get; set; }

        public BindingType UnderlyingNativeType { get; set; }

        public bool IsDisposable { get; set; }

        public string DisposeBody { get; set; }
        
        public bool IsIgnored { get; set; }
        
        public bool CopyFieldsFromLinkedObject { get; set; }
        
        public string LinkedClassName { get; set; }

        public bool CleanObject { get; set; }
        
        public List<OperatorExtension> Operators { get; set; }
        
        public List<ConstructorExtension> Constructors { get; set; }
    }
}