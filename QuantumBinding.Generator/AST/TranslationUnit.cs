using System;
using System.Collections.Generic;
using System.IO;
using QuantumBinding.Clang;
using QuantumBinding.Clang.Interop;
using QuantumBinding.Generator.Parser;

namespace QuantumBinding.Generator.AST
{
    public class TranslationUnit : Namespace
    {
        static TranslationUnit()
        {
            InteropNamespaceExtension = new NamespaceMapping() { FileName = "Marshal", NamespaceExtension = "Interop", OutputPath = "Interop" };
        }

        public TranslationUnit(string filePath, Module module)
        {
            FilePath = filePath;
            Module = module;
            Name = Module.OutputNamespace;
        }

        public static NamespaceMapping InteropNamespaceExtension { get; }

        public string OutputPath { get; set; }

        public string FilePath { get; }

        public bool IsValid { get; private set; }

        public ParseResult ParseResult { get; private set; }

        public string FileName => Path.GetFileNameWithoutExtension(FilePath);

        public Module Module { get; }

        public void Parse(QBIndex index, List<string> arguments)
        {
            var parser = new ClangParser(this);
            if (!File.Exists(FilePath))
            {
                Console.WriteLine($"File {FilePath} does not exist.");
                return;
            }
            
            ParseResult = parser.Parse(index, FilePath, arguments);
            if (ParseResult == ParseResult.Success)
            {
                IsValid = true;
            }
        }

        public void SetOwner()
        {
            foreach (var declaration in Declarations)
            {
                declaration.Owner = this;
            }
        }

        public void Merge(params TranslationUnit[] units)
        {
            foreach(var unit in units)
            {
                AddDeclarations(unit.Declarations);
            }
        }

        public override T Visit<T>(IDeclarationVisitor<T> visitor)
        {
            return visitor.VisitTranslationUnit(this);
        }
    }
}