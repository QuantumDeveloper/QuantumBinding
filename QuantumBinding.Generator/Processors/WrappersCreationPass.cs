using System;
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

            // Create wrappers only for structs and unions
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
            Class wrapper;
            if (!CurrentNamespace.IsWrapperPresent(@class.Name, out var wrapperDecl))
            {
                wrapper = (Class)@class.Clone();
                wrapper.Name = @class.Name[0].ToString().ToUpper() + @class.Name.Substring(1);
                if (@class.ClassType == ClassType.Struct)
                {
                    wrapper.ClassType = ClassType.StructWrapper;
                }
                else if (@class.ClassType == ClassType.Union)
                {
                    wrapper.ClassType = ClassType.UnionWrapper;
                }

                CurrentNamespace.AddDeclaration(wrapper);
            }
            else
            {
                wrapper = (Class)wrapperDecl;
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

            var op = new Operator();
            op.Class = wrapper;
            op.PassValueToConstructor = true;
            op.Type = new CustomType(@class.Name);
            op.Type.Declaration = @class;
            op.TransformationKind = TransformationKind.FromValueToClass;
            op.OperatorKind = OperatorKind.Implicit;
            wrapper.Operators.Add(op);

            int pointersCount = 0;
            foreach (var field in @class.Fields)
            {
                var property = new Property();
                var name = field.Name.Replace("@", "");
                if (name.ToLower().StartsWith("pp"))
                {
                    property.Name = name[1].ToString().ToUpper() + name.Substring(2);
                }
                else
                {
                    property.Name = name[0].ToString().ToUpper() + name.Substring(1);
                }
                
                property.Type = (BindingType)field.Type.Clone();
                
                if (field.Type.Declaration is Class declaration && !declaration.IsSimpleType)
                {
                    if ((declaration.ClassType == ClassType.Struct && declaration.ConnectedTo == null) || declaration.ClassType == ClassType.Union)
                    {
                        if (CurrentNamespace.IsWrapperPresent(declaration.Name, out wrapperDecl))
                        {
                            property.Type.Declaration = wrapperDecl;
                        }
                        else
                        {
                            var declarationCopy = (Class)declaration.Clone();
                            declarationCopy.ClassType = ClassType.StructWrapper;
                            if (declaration.ClassType == ClassType.Union)
                            {
                                declarationCopy.ClassType = ClassType.UnionWrapper;
                            }

                            declarationCopy.WrappedStruct = declaration;
                            property.Type.Declaration = declarationCopy;
                            CurrentNamespace.AddDeclaration(declarationCopy);
                        }
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
                    pointersCount++;
                }
                
                if (field.IsPointer && !field.Type.IsPointerToVoid())
                {
                    var pointerType = (PointerType)field.Type;
                    if (field.Name.StartsWith("@"))
                    {
                        name = field.Name.Substring(1);
                    }
                    name = $"_{name[0].ToString().ToLower()}{name.Substring(1)}";
                    property.PairedField = new Field($"{name}") { ShouldDispose = true };
                    if (field.Type.IsAnsiString() || field.Type.IsUnicodeString())
                    {
                        property.PairedField.Type = new CustomType("MarshaledString");
                    }
                    else if (field.Type.IsStringArray())
                    {
                        property.PairedField.Type = new CustomType("MarshaledStringArray");
                    }
                    else if (field.Type.IsPointerToArray() || field.Type.IsPointerToArrayOfEnums())
                    {
                        try
                        {
                            var typePrinter = new CSharpTypePrinter(ProcessingContext.Options);
                            typePrinter.PushModule(CurrentNamespace.Module);
                            typePrinter.PushMarshalType(MarshalTypes.NativeField);
                            var underlyingType = pointerType.Visit(typePrinter).Type;
                            typePrinter.PopMarshalType();
                            
                            if (pointerType.Declaration != null && !pointerType.Pointee.IsPrimitiveType(out var primitive))
                            {
                                if (pointerType.Declaration is Class class1)
                                {
                                    if (!class1.IsSimpleType || 
                                        (class1.IsSimpleType && 
                                         !ProcessingContext.Options.PodTypesAsSimpleTypes &&
                                         class1.UnderlyingNativeType != null))
                                    {
                                        underlyingType = $"{pointerType.Declaration.InteropNamespace}.{underlyingType}";
                                    }
                                }
                                else
                                {
                                    underlyingType = $"{pointerType.Declaration.Namespace}.{underlyingType}";
                                }
                            }

                            property.PairedField.Type = new CustomType($"NativeStructArray<{underlyingType}>");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }
                    else if (!field.Type.IsPointerToSystemType(out var customType) &&
                             (field.Type.IsPointerToStructOrUnion() || 
                              field.Type.IsPointerToCustomType(out var custom)||
                              field.Type.IsPointerToBuiltInType(out var primitive)))
                    {
                        try
                        {
                            var typePrinter = new CSharpTypePrinter(ProcessingContext.Options);
                            typePrinter.PushModule(CurrentNamespace.Module);
                            typePrinter.PushMarshalType(MarshalTypes.NativeField);
                            var underlyingType = pointerType.Visit(typePrinter).Type;

                            if (pointerType.Declaration != null)
                            {
                                if (pointerType.Declaration is Class class1)
                                {
                                    if (!class1.IsSimpleType || 
                                        (class1.IsSimpleType && 
                                        !ProcessingContext.Options.PodTypesAsSimpleTypes &&
                                        class1.UnderlyingNativeType != null))
                                    {
                                        underlyingType = $"{pointerType.Declaration.InteropNamespace}.{underlyingType}";
                                    }
                                }
                                else
                                {
                                    underlyingType = $"{pointerType.Declaration.Namespace}.{underlyingType}";
                                }
                            }

                            typePrinter.PopMarshalType();
                            property.PairedField.Type = new CustomType($"NativeStruct<{underlyingType}>");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }
                    else
                    {
                        if (!field.Type.IsPointerToSystemType(out var customT))
                        {
                            property.PairedField.Type = new CustomType("UnknownReference");
                        }
                    }

                    property.PairedField.AccessSpecifier = AccessSpecifier.Private;
                    if (!property.Type.IsPointerToIntPtr() && 
                        !field.Type.IsPointerToVoid() &&
                        !field.Type.IsPointerToSystemType(out var type))
                    {
                        wrapper.AddField(property.PairedField);
                    }

                    if ((property.Type.IsPointerToBuiltInType(out var primitiveType) || property.Type.IsPointerToStructOrUnion() && !property.Type.IsPointerToArray()) || 
                        (decl?.IsSimpleType == true))
                    {
                        if (pointerType != null && !property.Type.IsArray())
                            pointerType.IsNullable = true;
                    }
                }
                else if ((field.Type.IsArray() && !field.Type.IsSimpleType() && !field.Type.IsEnum() &&
                          !field.Type.IsDelegate()) ||
                         (field.Type.IsCustomType(out var custom) && !custom.IsSimpleType() && !custom.IsEnum() &&
                          !custom.IsDelegate()))
                {
                    property.PairedField = new Field(field.Name[0].ToString().ToLower() + field.Name.Substring(1))
                    {
                        Type = field.Type,
                        AccessSpecifier = AccessSpecifier.Private
                    };
                }

                if (field.CanGenerateGetter)
                {
                    property.Getter = new Method();
                }

                if (field.CanGenerateSetter)
                {
                    property.Setter = new Method();
                }

                wrapper.AddProperty(property);
            }

            if (pointersCount > 0)
            {
                wrapper.IsDisposable = true;
            }
            wrapper.DisposableBaseClass = FileExtensionGenerator.DisposableClassName;
            StringBuilder disposeBody = new StringBuilder();
            foreach (var field in wrapper.Fields)
            {
                if (field.ShouldDispose)
                {
                    disposeBody.AppendLine($"{field.Name}.Dispose();");
                }
            }
            wrapper.DisposeBody = disposeBody.ToString();
        }
    }
}
