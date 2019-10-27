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
        }

        private List<NamespaceMapping> namespaceMapping;

        internal List<TranslationUnit> TranslationUnits { get; }

        public string Name { get; set; }

        public List<string> Files { get; set; }

        public List<string> IncludeDirs { get; set; }

        public string OutputPath { get; set; }

        public string OutputFileName { get; set; }

        public string OutputNamespace { get; set; }

        public string LibraryName { get; set; }

        public string MethodClassName { get; set; }

        public string InteropClassName { get; set; }

        public bool ForceCallingConvention { get; set; }

        public bool SkipPodTypesGeneration { get; set; }

        public bool AllowConvertStructToClass { get; set; }

        public CallingConvention CallingConvention { get; set; }

        public GeneratorSpecializations GeneratorSpecializations { get; set; }

        public bool SuppressUnmanagedCodeSecurity { get; set; }

        public bool WrapInteropObjects { get; set; }

        public bool CharAsBoolForMethods { get; set; }

        /// <summary>
        /// Create overloads for functions, which contains input arrays
        /// </summary>
        public bool GenerateOverloadsForArrayParams { get; set; }

        public static string UtilsOutputPath { get; set; }

        public static string UtilsOutputName { get; set; }

        public static string UtilsNamespace { get; set; }

        public static Module GenerateUtilsForModule { get; set; }

        public List<string> Defines { get; set; }

        public ReadOnlyCollection<NamespaceMapping> NamespaceMapping => namespaceMapping.AsReadOnly();

        public ReadOnlyCollection<TranslationUnit> Units => TranslationUnits.AsReadOnly();

        public void AddNamespaceMapping(string fileName, string subNamespace, string outputFilePath, bool repcaseBaseNamespace = false)
        {
            var @namespace = new NamespaceMapping() { FileName = fileName, NamespaceExtension = subNamespace, OutputPath = outputFilePath, ReplaceBaseNameSpace = repcaseBaseNamespace};
            namespaceMapping.Add(@namespace);
        }
    }
}