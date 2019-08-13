using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;
using QuantumBinding.Generator.Types;
using System.Linq;
using System.Text;

namespace QuantumBinding.Generator.Processors
{
    public class WrappersCreationPass : PreGeneratorPass
    {
        public WrappersCreationPass(GeneratorSpecializations specializations)
        {
            Specializations = specializations;
            Options.VisitClasses = true;
        }

        public GeneratorSpecializations Specializations { get; }

        public override bool VisitClass(Class @class)
        {
            if (IsVisited(@class))
            {
                return false;
            }

            if (@class.Name == "VkBool32")
            {

            }

            if ((@class.ClassType != ClassType.Struct && @class.ClassType != ClassType.Union)
                || @class.IsSimpleType 
                || @class.ConnectedTo != null)
            {
                return false;
            }

            CreateStructWrapper(@class);

            return true;
        }

        private void CreateStructWrapper(Class @class)
        {
            var wrapper = (Class)@class.Clone();
            wrapper.Name = @class.Name[0].ToString().ToUpper()+@class.Name.Substring(1);
            if (@class.ClassType == ClassType.Struct)
            {
                wrapper.ClassType = ClassType.StructWrapper;
            }
            else if (@class.ClassType == ClassType.Union)
            {
                wrapper.ClassType = ClassType.UnionWrapper;
            }
            var innerWrapperField = new Field("_internal");
            innerWrapperField.AccessSpecifier = AccessSpecifier.Private;
            innerWrapperField.Type = new CustomType(@class.Name);
            innerWrapperField.Type.Declaration = @class;
            innerWrapperField.Class = @class;
            wrapper.WrappedStruct = @class;
            wrapper.WrappedStructFieldName = innerWrapperField.Name;
            wrapper.WrapperMethodAccessSpecifier = AccessSpecifier.Public;

            var ctor = new Constructor() { Class = wrapper, IsDefault = true };
            wrapper.Constructors.Add(ctor);
            ctor = new Constructor() { Class = wrapper };
            ctor.InputParameters.Add(innerWrapperField);
            wrapper.Constructors.Add(ctor);

            if (ctor.Class.Name == "VkFramebufferCreateInfo")
            {

            }

            foreach (var field in @class.Fields)
            {
                var property = new Property();
                var name = field.Name.Replace("@", "");
                property.Name = name[0].ToString().ToUpper() + name.Substring(1);
                property.Type = (BindingType)field.Type.Clone();
                if (field.Name == "pAttachments")
                {

                }

                if (field.Type.Declaration is Class declaration && (!declaration.IsSimpleType && ProcessingContext.Options.ConvertRules.PodTypesAsSimpleTypes))
                {
                    if ((declaration.ClassType == ClassType.Struct && declaration.ConnectedTo == null) || declaration.ClassType == ClassType.Union)
                    {
                        var declarationCopy = (Class) declaration.Clone();
                        declarationCopy.ClassType = ClassType.StructWrapper;
                        if (declaration.ClassType == ClassType.Union)
                        {
                            declarationCopy.ClassType = ClassType.UnionWrapper;
                        }

                        property.Type.Declaration = declarationCopy;
                    }
                    else
                    {
                        property.Type.Declaration = declaration;
                    }
                }

                property.Field = field;
                var decl = property.Type.Declaration as Class;
                if (field.IsPointer)
                {
                    property.PairedField = new Field($"ref{name}") { ShouldDispose = true };
                    if (field.Type.IsAnsiString() || field.Type.IsUnicodeString())
                    {
                        property.PairedField.Type = new CustomType("StringReference");
                    }
                    else if (field.Type.IsStringArray())
                    {
                        property.PairedField.Type = new CustomType("StringArrayReference");
                    }
                    else if (field.Type.IsPointerToArray() || field.Type.IsPointerToPointer())
                    {
                        property.PairedField.Type = new CustomType("GCHandleReference");
                    }
                    else
                    {
                        property.PairedField.Type = new CustomType("StructReference");
                    }

                    property.PairedField.AccessSpecifier = AccessSpecifier.Private;
                    if (!property.Type.IsPointerToIntPtr() && !field.Type.IsPointerToVoid())
                    {
                        wrapper.Fields.Add(property.PairedField);
                    }

                    if ((property.Type.IsPointerToPrimitiveType(out var primitiveType) || property.Type.IsPointerToStruct() && !property.Type.IsPointerToArray()) || 
                        (decl?.IsSimpleType == true))
                    {
                        var pointerType = property.Type as PointerType;
                        if (pointerType != null && !property.Type.IsArray())
                            pointerType.IsNullable = true;
                    }
                }
                else if ((field.Type.IsArray() && !field.Type.IsSimpleType() && !field.Type.IsEnum() && !field.Type.IsDelegate()) ||
                    (field.Type.IsCustomType(out var custom) && !custom.IsSimpleType() && !custom.IsEnum() && !custom.IsDelegate()))
                {
                    property.PairedField = new Field(field.Name[0].ToString().ToLower() + field.Name.Substring(1));
                    property.PairedField.Type = field.Type;
                    property.PairedField.AccessSpecifier = AccessSpecifier.Private;
                }

                if (field.CanGenerateGetter)
                {
                    property.Getter = new Method();
                }

                if (field.CanGenerateSetter)
                {
                    property.Setter = new Method();
                }

                wrapper.Properties.Add(property);
            }

            if (@class.Fields.Any(x => x.IsPointer))
            {
                wrapper.IsDisposable = true;
                wrapper.DisposableBaseClass = "DisposableObject";
                StringBuilder disposeBody = new StringBuilder();
                for (int i = 1; i < wrapper.Fields.Count; i++)
                {
                    if (wrapper.Fields[i].ShouldDispose)
                    {
                        disposeBody.AppendLine($"{wrapper.Fields[i].Name}?.Dispose();");
                    }
                }
                wrapper.DisposeBody = disposeBody.ToString();
            }

            CurrentNamespace.AddDeclaration(wrapper);
        }
    }
}
