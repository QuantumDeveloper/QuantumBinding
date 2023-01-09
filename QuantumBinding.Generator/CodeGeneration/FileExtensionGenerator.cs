using QuantumBinding.Generator.AST;
using System.Collections.Generic;
using System.Linq;

namespace QuantumBinding.Generator.CodeGeneration
{
    public class FileExtensionGenerator : CSharpCodeGeneratorBase
    {
        private readonly FileExtensionKind extensionKind;
        public static string DisposableClassName => "QBDisposableObject";
        
        public FileExtensionGenerator(ProcessingContext context, TranslationUnit unit, FileExtensionKind extensionKind = FileExtensionKind.Disposable)
            : this(context, new List<TranslationUnit> { unit }, extensionKind)
        {
        }

        public FileExtensionGenerator(ProcessingContext context, IEnumerable<TranslationUnit> units, FileExtensionKind extensionKind = FileExtensionKind.Disposable)
            : base(context, units, GeneratorCategory.Extensions)
        {
            this.extensionKind = extensionKind;
        }

        public override string FolderName => $"{Category}.{extensionKind}";

        public override void Run()
        {
            Name = extensionKind.ToString();
            PushBlock(CodeBlockKind.Root);
            GenerateFileHeader();
            GenerateNamespace();
            PopBlock();
        }

        public override void Run(Declaration declaration)
        {
            Name = declaration.Name;
            PushBlock(CodeBlockKind.Root);
            GenerateFileHeader();
            GenerateNamespace(declaration);
            PopBlock();
        }

        protected virtual void GenerateNamespace()
        {
            switch (extensionKind)
            {
                case FileExtensionKind.Disposable:
                    GenerateDisposeExtension();
                    break;
            }
        }
        
        protected virtual void GenerateNamespace(Declaration declaration)
        {
            switch (extensionKind)
            {
                case FileExtensionKind.Disposable:
                    GenerateDisposeExtension(declaration);
                    break;
            }
        }

        private void GenerateDisposeExtension()
        {
            int classesCount = 0;
            foreach (var unit in TranslationUnits)
            {
                CurrentTranslationUnit = unit;
                var classes = unit.Classes.Where(x => !x.IsIgnored && x.ClassType == ClassType.Class && x.IsDisposable && !x.IsExtension).ToList();
                if (classes.Count == 0)
                {
                    continue;
                }

                classesCount += classes.Count;
                
                GenerateUsings();
                
                NewLine();

                PushBlock(CodeBlockKind.Namespace);
                WriteCurrentNamespace(unit);

                NewLine();

                TypePrinter.PushModule(unit.Module);
                GenerateDisposedClasses(classes);
                
                PopBlock();
            }

            if (classesCount == 0)
            {
                IsEmpty = true;
            }
        }

        private void GenerateDisposeExtension(Declaration declaration)
        {
            if (declaration is not Class || declaration.IsIgnored)
            {
                IsEmpty = true;
                return;
            }

            var @class = (Class)declaration;
            if (@class.ClassType != ClassType.Class || !@class.IsDisposable || !@class.IsExtension)
            {
                IsEmpty = true;
                return;
            }

            CurrentTranslationUnit = declaration.Owner;

            GenerateUsings();
            
            NewLine();
            
            PushBlock(CodeBlockKind.Namespace);
            WriteCurrentNamespace(CurrentTranslationUnit);

            NewLine();

            TypePrinter.PushModule(CurrentTranslationUnit.Module);

            GenerateClass(@class);
            
            PopBlock();
        }

        private void GenerateDisposedClasses(List<Class> classes)
        {
            foreach(var @class in classes)
            {
                GenerateClass(@class);
                NewLine();
            }
        }

        protected override void GenerateClass(Class @class)
        {
            PushBlock(CodeBlockKind.Class, @class);

            WriteLine($"{TypePrinter.VisitClass(@class)} : {DisposableClassName}");

            WriteOpenBraceAndIndent();

            GenerateUnmanagedDisposePattern(@class.DisposeBody);

            UnindentAndWriteCloseBrace();

            PopBlock();
        }

        private void GenerateUnmanagedDisposePattern(string disposeContent)
        {
            PushBlock(CodeBlockKind.Disposable);
            WriteLine("protected override void UnmanagedDisposeOverride()");
            WriteOpenBraceAndIndent();
            WriteLine(disposeContent);
            UnindentAndWriteCloseBrace();
            PopBlock();
        }
    }
}
