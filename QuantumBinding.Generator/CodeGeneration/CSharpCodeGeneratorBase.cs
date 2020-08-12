using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.CodeGeneration
{
    public abstract class CSharpCodeGeneratorBase : CodeGenerator
    {
        protected CSharpCodeGeneratorBase(ProcessingContext context, TranslationUnit unit, GeneratorSpecializations specializations) :
            this(context, new List<TranslationUnit>() { unit }, specializations)
        {
            CurrentTranslationUnit = unit;
        }

        protected CSharpCodeGeneratorBase(ProcessingContext context, IEnumerable<TranslationUnit> units, GeneratorSpecializations specializations) :
            base(context, units, specializations)
        {
            TypePrinter = new CSharpTypePrinter(context.Options);
            UsedUsings = new List<string>();
        }

        protected string IntPtrZero => "System.IntPtr.Zero";

        protected TranslationUnit CurrentTranslationUnit { get; set; }
        protected CSharpTypePrinter TypePrinter { get; set; }
        protected CodeBlock UsingsBlock { get; set; }
        protected List<string> UsedUsings { get; }
        protected string CurrentNamespace { get; set; }
        protected string InteropNamespace => $"{CurrentTranslationUnit.FullNamespace}.{TranslationUnit.InteropNamespaceExtension.NamespaceExtension}";
        protected string InteropNamespaceExtension => TranslationUnit.InteropNamespaceExtension.NamespaceExtension;
        protected string ConversionMethodName => "ToInternal()";


        protected virtual void WriteCurrentNamespace(Namespace @namespace)
        {
            if (IsInteropGenerator)
            {
                CurrentNamespace = $"{@namespace.FullNamespace}.{TranslationUnit.InteropNamespaceExtension.NamespaceExtension}";
                WriteLine($"namespace {CurrentNamespace}");
            }
            else
            {
                CurrentNamespace = @namespace.FullNamespace;
                WriteLine($"namespace {CurrentNamespace}");
            }
        }

        protected virtual void GenerateUsings()
        {
            PushBlock(CodeBlockKind.Usings);
            if (Specializations.HasFlag(GeneratorSpecializations.Functions) ||
                Specializations.HasFlag(GeneratorSpecializations.Delegates))
            {
                WriteLine("using System.Security;");
            }
            WriteLine("using System;");
            WriteLine("using System.Runtime.InteropServices;");

            UsingsBlock = PopBlock();
        }

        protected void SetAlternativeNamespace(TranslationUnit unit)
        {
            var classes = unit.AllClasses.Where(
                    x => 
                        x.ClassType == ClassType.Struct || x.ClassType == ClassType.Union ||
                        x.ClassType == ClassType.StructWrapper || x.ClassType == ClassType.UnionWrapper).
                ToList();
            foreach (var @class in classes)
            {
                if (!string.IsNullOrEmpty(@class.AlternativeNamespace))
                {
                    continue;
                }

                @class.AlternativeNamespace = GetAlternativeNamespace(@class);
            }

            foreach (var @class in classes)
            {
                if (@class.InnerStruct != null)
                {
                    if (!string.IsNullOrEmpty(@class.InnerStruct.AlternativeNamespace))
                    {
                        continue;
                    }

                    @class.AlternativeNamespace = GetAlternativeNamespace(@class.InnerStruct);
                }
            }

            foreach (var function in unit.Functions)
            {
                if (!string.IsNullOrEmpty(function.AlternativeNamespace))
                {
                    continue;
                }

                function.AlternativeNamespace = GetAlternativeNamespace(function);
            }

            foreach (var function in unit.Delegates)
            {
                if (!string.IsNullOrEmpty(function.AlternativeNamespace))
                {
                    continue;
                }

                function.AlternativeNamespace = GetAlternativeNamespace(function);
            }
        }

        private string GetAlternativeNamespace(DeclarationUnit decl)
        {
            return $"{decl.Owner.FullNamespace}.{TranslationUnit.InteropNamespaceExtension.NamespaceExtension}";
        }

        protected virtual void AddUsingIfNeeded(BindingType type)
        {
            if (CurrentNamespace == type.Declaration?.AlternativeNamespace)
            {
                return;
            }

            if (type.Declaration != null && type.Declaration.Owner != null && 
                (type.Declaration.Owner != CurrentTranslationUnit || CurrentNamespace != type.Declaration.AlternativeNamespace))
            {
                var @namespace = type.Declaration.AlternativeNamespace;
                if (string.IsNullOrEmpty(@namespace))
                {
                    @namespace = type.Declaration.Owner.FullNamespace;
                }

                if (UsedUsings.Contains(@namespace) || CurrentNamespace == @namespace)
                {
                    return;
                }

                UsingsBlock.WriteLine($"using {@namespace};");
                UsedUsings.Add(@namespace);
            }
        }

        private void AddUsingIfNeeded(string type)
        {
            AddUsingIfNeeded(new CustomType(type));
        }

        protected virtual void GenerateFields(Class @class)
        {
            if (@class.Name == "VkTransformMatrixKHR")
            {

            }
            TypePrinter.PushMarshalType(MarshalTypes.NativeField);
            foreach (var field in @class.Fields)
            {
                PushBlock(CodeBlockKind.Field, field);

                GenerateCommentIfNotEmpty(field.Comment);

                foreach (var attribute in field.Attributes)
                {
                    PushBlock(CodeBlockKind.Attribute, attribute);
                    WriteLine(attribute);
                    PopBlock();
                }

                var fieldStr = TypePrinter.VisitField(field);
                if (!string.IsNullOrEmpty(fieldStr.Attribute) && @class.ClassType != ClassType.Union)
                {
                    PushBlock(CodeBlockKind.Attribute, fieldStr.Attribute);
                    WriteLine(fieldStr.Attribute);
                    PopBlock();
                }

                AddUsingIfNeeded(field.Type);

                PushBlock(CodeBlockKind.FieldDefinition, field);
                WriteLine($"{fieldStr.Type};");
                PopBlock();

                NewLine();

                PopBlock();
            }
            TypePrinter.PopMarshalType();
        }

        protected virtual void GenerateConstructors(Class @class)
        {
            foreach (var ctor in @class.Constructors)
            {
                GenerateConstructor(ctor);
            }
        }

        protected virtual void GenerateConstructor(Constructor ctor)
        {
            PushBlock(CodeBlockKind.Constructor, ctor);
            if (ctor.IsDefault)
            {
                WriteLine($"{ctor.AccessSpecifier.ToString().ToLower()} {ctor.Class.Name}()");
                WriteOpenBraceAndIndent();
                UnindentAndWriteCloseBrace();
            }
            else
            {
                Write($"{ctor.AccessSpecifier.ToString().ToLower()} {ctor.Class.Name}(");

                int index = 0;
                TypePrinter.PushMarshalType(MarshalTypes.MethodParameter);
                foreach (var param in ctor.InputParameters)
                {
                    index++;
                    var visitResult = TypePrinter.VisitField(param);
                    if (param.Type.Declaration is Class decl && !string.IsNullOrEmpty(decl.AlternativeNamespace))
                    {
                        visitResult.Type = $"{decl.AlternativeNamespace}.{visitResult.Type}";
                    }
                    Write($"{visitResult}");
                    if (index < ctor.InputParameters.Count)
                    {
                        Write(",");
                    }
                }
                TypePrinter.PopMarshalType();
                Write(")");
                NewLine();
                WriteOpenBraceAndIndent();
                foreach (var param in ctor.InputParameters)
                {
                    WriteLine($"this.{param.Name} = {param.Name};");
                }
                UnindentAndWriteCloseBrace();
            }

            NewLine();

            PopBlock();
        }

        protected virtual void GenerateOverloads(Class @class)
        {
            if (@class.Operators.Count > 0)
            {
                NewLine();
            }

            // Temporary disable Char to Bool convertion during generating an overloads to keep in sync with fields 
            // in case we really want use byte values
            var prev = CurrentTranslationUnit.Module.CharAsBoolForMethods;
            CurrentTranslationUnit.Module.CharAsBoolForMethods = false;

            foreach (var op in @class.Operators)
            {
                GenerateOperator(op);
            }

            CurrentTranslationUnit.Module.CharAsBoolForMethods = prev;
        }

        protected virtual void GenerateOperator(Operator @operator)
        {
            if (@operator.OperatorKind == OperatorKind.None)
                return;

            PushBlock(CodeBlockKind.Operator, @operator);
            var variableName = @operator.Class.Name[0].ToString().ToLower();
            string @namespace = @operator.Type.Declaration?.AlternativeNamespace;
            if (string.IsNullOrEmpty(@namespace) && !@operator.Type.IsSimpleType())
            {
                @namespace = $"{@operator.Type.Declaration?.Owner.FullNamespace}";
            }

            var decl = @operator.Type.Declaration as Class;
            if (@operator.TransformationKind == TransformationKind.FromClassToValue)
            {
                if (decl != null && !decl.IsSimpleType)
                {
                    WriteLine(
                        $"public static {@operator.OperatorKind.ToString().ToLower()} operator {@namespace}.{@operator.Type.Visit(TypePrinter)}({@operator.Class.Name} {variableName})");
                }
                else
                {
                    WriteLine(
                        $"public static {@operator.OperatorKind.ToString().ToLower()} operator {@operator.Type.Visit(TypePrinter)}({@operator.Class.Name} {variableName})");
                }
                WriteOpenBraceAndIndent();
                OperatorOverloadOverride(@operator, variableName);
                if (decl != null && @operator.Class.ClassType == ClassType.Class)
                {
                    WriteLine($"return {variableName}?.{@operator.FieldName} ?? new {@namespace}.{@operator.Type.Visit(TypePrinter)}();");
                }
                else
                {
                    WriteLine($"return {variableName}.{@operator.FieldName};");
                }

                UnindentAndWriteCloseBrace();
            }
            else
            {
                if (@operator.Class.ClassType == ClassType.Class)
                {
                    TypePrinter.PushMarshalType(MarshalTypes.Property);
                }
                else if (@operator.Class.ClassType == ClassType.StructWrapper || @operator.Class.ClassType == ClassType.UnionWrapper)
                {
                    TypePrinter.PushMarshalType(MarshalTypes.WrappedProperty);
                }

                if (decl != null && !decl.IsSimpleType)
                {
                    WriteLine($"public static {@operator.OperatorKind.ToString().ToLower()} operator {@operator.Class.Name}({@namespace}.{@operator.Type.Visit(TypePrinter)} {variableName})");
                }
                else
                {
                    WriteLine($"public static {@operator.OperatorKind.ToString().ToLower()} operator {@operator.Class.Name}({@operator.Type.Visit(TypePrinter)} {variableName})");
                }

                if (@operator.Class.ClassType == ClassType.Class || @operator.Class.ClassType == ClassType.StructWrapper || @operator.Class.ClassType == ClassType.UnionWrapper)
                {
                    TypePrinter.PopMarshalType();
                }

                WriteOpenBraceAndIndent();

                OperatorOverloadOverride(@operator, variableName);

                if (@operator.PassValueToConstructor)
                {
                    WriteLine($"return new {@operator.Class.Name}({variableName});");
                }
                else
                {
                    WriteLine($"return new {@operator.Class.Name}(){{{@operator.Class.Fields[0].Name} = {variableName}}};");
                }
                UnindentAndWriteCloseBrace();
            }

            NewLine();
            PopBlock();
        }

        protected virtual void OperatorOverloadOverride(Operator @operator, string variableName)
        {
        }

    }
}
