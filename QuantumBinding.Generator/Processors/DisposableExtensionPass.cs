using System.Linq;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;
using QuantumBinding.Generator.ProcessingFluentApi;

namespace QuantumBinding.Generator.Processors
{
    public class DisposableExtensionPass : CodeGeneratorPass
    {
        DisposableExtension[] disposableClasses;

        public DisposableExtensionPass(params DisposableExtension[] classes)
        {
            disposableClasses = classes;
            CodeGeneratorPassKind = ExecutionPassKind.PerTranslationUnit;
        }

        protected override CodeGenerator OnCreateGenerator(ProcessingContext context, GeneratorSpecializations specializations, params TranslationUnit[] units)
        {
            foreach (var unit in units)
            {
                var classes = unit.Classes.Where(x => !x.IsIgnored).ToList();
                foreach (var @class in classes)
                {
                    var disposable = disposableClasses.FirstOrDefault(x => x.Name == @class.Name);
                    if (disposable != null)
                    {
                        @class.IsDisposable = true;
                        @class.DisposeBody = disposable.DisposableContent;
                    }
                }
            }

            return new FileExtensionGenerator(context, units, FileExtensionKind.Disposable);
        }
    }
}
