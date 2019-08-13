using System;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;
using QuantumBinding.Generator.Types;
using System.Linq;
using System.Text;

namespace QuantumBinding.Generator.Processors
{
    public class UpdateWrappedMethodParametersPass : PreGeneratorPass
    {
        public UpdateWrappedMethodParametersPass(GeneratorSpecializations specializations)
        {
            Specializations = specializations;
            Options.VisitClasses = true;
            Options.VisitMethods = true;
        }

        public GeneratorSpecializations Specializations { get; }

        public override bool VisitMethod(Method method)
        {
            if (IsVisited(method))
            {
                return false;
            }

            UpdateMethodParameters(method);

            return true;
        }


        private void UpdateMethodParameters(Method method)
        {
            if (method.Name == "VkCreateWin32SurfaceKHR")
            {

            }

            foreach (var parameter in method.Parameters)
            {
                var decl = parameter.Type.Declaration as Class;
                if (decl == null || (decl.IsSimpleType && ProcessingContext.Options.ConvertRules.PodTypesAsSimpleTypes)) continue;
                if (decl.ClassType != ClassType.Struct && decl.ClassType != ClassType.Union) continue;

                if (decl.Name == "ImageView")
                {

                }

                Class declaration = null;
                foreach (var unit in AstContext.TranslationUnits)
                {
                    if (decl.ClassType == ClassType.Struct)
                    {
                        declaration = unit.StructsWrappers.FirstOrDefault(x => x.Id == decl.Id);
                    }
                    else if (decl.ClassType == ClassType.Union)
                    {
                        declaration = unit.UnionWrappers.FirstOrDefault(x => x.Id == decl.Id);
                    }

                    if (declaration != null) break;
                }

                if (declaration == null) continue;

                var wrappedType = parameter.Type;
                parameter.WrappedType = wrappedType;
                parameter.Type = (BindingType) wrappedType.Clone();
                parameter.Type.Declaration = declaration;

            }
        }
    }
}
