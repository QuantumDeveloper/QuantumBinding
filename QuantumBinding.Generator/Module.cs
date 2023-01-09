using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;

namespace QuantumBinding.Generator
{
    public class Module
    {
        public Module() : this(string.Empty)
        {
        }

        public Module(string libraryName)
        {
            LibraryName = libraryName;
            Files = new List<string>();
            IncludeDirs = new List<string>();
            Defines = new List<string>();
            namespaceMapping = new List<NamespaceMapping>();
            TranslationUnits = new List<TranslationUnit>();
            GeneratorSpecializations = GeneratorSpecializations.All;
            CleanPreviousGeneration = true;
        }

        private readonly List<NamespaceMapping> namespaceMapping;

        internal List<TranslationUnit> TranslationUnits { get; }

        public string Name { get; set; }

        public List<string> Files { get; set; }

        public List<string> IncludeDirs { get; set; }
        
        public GeneratorMode GeneratorMode { get; set; }
        
        public bool EachTypeInSeparateFile { get; set; }
        
        public bool CleanPreviousGeneration { get; set; }
        
        public string FileHeader { get; set; }

        public string OutputPath { get; set; }

        public string OutputFileName { get; set; }

        public string OutputNamespace { get; set; }
        
        public string InteropSubNamespace { get; set; } 

        public string LibraryName { get; set; }

        public string MethodClassName { get; set; }

        public string InteropClassName { get; set; }

        public bool ForceCallingConvention { get; set; }

        // TODO: should be removed!!!
        public bool SkipTypedefsGeneration { get; set; }

        public bool AllowConvertStructToClass { get; set; }
        
        public AccessSpecifier InteropClassAccessSpecifier { get; set; }

        public CallingConvention CallingConvention { get; set; }

        public GeneratorSpecializations GeneratorSpecializations { get; set; }

        public bool SuppressUnmanagedCodeSecurity { get; set; }

        public bool WrapInteropObjects { get; set; }

        public bool CharAsBoolForMethods { get; set; }

        /// <summary>
        /// Create overloads for functions, which contains input arrays
        /// </summary>
        public bool GenerateOverloadsForArrayParams { get; set; }

        public static string UtilsNamespace => "QuantumBinding.Utils";

        public List<string> Defines { get; set; }

        public ReadOnlyCollection<NamespaceMapping> NamespaceMapping => namespaceMapping.AsReadOnly();

        public ReadOnlyCollection<TranslationUnit> Units => TranslationUnits.AsReadOnly();

        public void AddNamespaceMapping(string fileName, string subNamespace, string outputFilePath, bool replaceBaseNamespace = false)
        {
            var @namespace = new NamespaceMapping() { FileName = fileName, SubNamespace = subNamespace, OutputPath = outputFilePath, ReplaceBaseNameSpace = replaceBaseNamespace};
            namespaceMapping.Add(@namespace);
        }
    }
}