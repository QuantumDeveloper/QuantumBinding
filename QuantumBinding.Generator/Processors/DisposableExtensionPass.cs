using System.Collections.Generic;
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

        protected override CodeGenerator OnCreateGenerator(GeneratorCategory category, params TranslationUnit[] units)
        {
            return new FileExtensionGenerator(ProcessingContext, units);
        }

        protected override List<CodeGenerator> ProcessPerTypeCodeGeneration(TranslationUnit unit, GeneratorSpecializations specs)
        {
            var classes = unit.Classes.Where(x => !x.IsIgnored).ToList();
            foreach (var @class in classes)
            {
                var disposable = disposableClasses.FirstOrDefault(x => x.Name == @class.Name);
                if (disposable != null)
                {
                    @class.IsExtension = true;
                    @class.IsDisposable = true;
                    @class.DisposeBody = disposable.DisposableContent;
                }
            }

            classes = unit.Classes.Where(x => x.IsDisposable).ToList();
            
            return ProcessDeclarations(classes, unit);
        }
    }
}
