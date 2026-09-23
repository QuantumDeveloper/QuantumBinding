using System;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.CodeGeneration;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.Processors;

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
            || @class.LinkedTo is { IsIgnored: false })
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

        var innerWrapperField = new Parameter("native");
        innerWrapperField.ParameterKind = ParameterKind.Readonly;
        innerWrapperField.Type = new CustomType(@class.Name);
        innerWrapperField.Type.Declaration = @class;
        wrapper.NativeStruct = @class;
        wrapper.NativeStructFieldName = innerWrapperField.Name;
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

            try
            {
                property.Type = (BindingType)field.Type.Clone();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (field.Type.Declaration is Class declaration && !declaration.IsSimpleType)
            {
                if ((declaration.ClassType == ClassType.Struct && declaration.LinkedTo == null) || declaration.ClassType == ClassType.Union)
                {
                    if (CurrentNamespace.IsWrapperPresent(declaration.Name, out wrapperDecl))
                    {
                        property.Type.Declaration = wrapperDecl;
                    }
                    else
                    {
                        var declarationCopy = (Class)declaration.Clone();
                        declarationCopy.Name = declaration.Name[0].ToString().ToUpper() + declaration.Name.Substring(1);
                        declarationCopy.ClassType = ClassType.StructWrapper;
                        if (declaration.ClassType == ClassType.Union)
                        {
                            declarationCopy.ClassType = ClassType.UnionWrapper;
                        }

                        declarationCopy.NativeStruct = declaration;
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

            // A pointer property is NULLABLE - that is all this branch decides now. It used to give every pointer a
            // companion field owning its native memory, back when a wrapper allocated per property and disposed it;
            // marshalling writes into one call-scoped buffer instead, so there is nothing to own and nothing to free.
            if (field.IsPointer && !field.Type.IsPointerToVoid(out _))
            {
                var pointerType = (PointerType)field.Type;
                if ((property.Type.IsPointerToBuiltInType(out var primitiveType) || property.Type.IsPointerToStructOrUnion() && !property.Type.IsPointerToArray()) ||
                    (decl?.IsSimpleType == true))
                {
                    if (pointerType != null && !property.Type.IsArray())
                        pointerType.IsNullable = true;
                }
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
    }
}
