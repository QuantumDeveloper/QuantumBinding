using System.Collections.Generic;
using System.Linq;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator
{
    public abstract class TypePrinter : ITypePrinter<TypePrinterResult>, IDeclarationVisitor<TypePrinterResult>
    {
        protected TypePrinter(BindingOptions options)
        {
            Options = options;
            marshalTypes = new Stack<MarshalTypes>();
            parametersStack = new Stack<Parameter>();
            fieldsStack = new Stack<Field>();
            indentation = new Stack<uint>();
        }

        protected BindingOptions Options { get; }

        public MarshalTypes MarshalType => marshalTypes.Peek();

        public int Indent => (int)indentation.Sum(x => x);

        public Module Module { get; private set; }

        private readonly Stack<MarshalTypes> marshalTypes;

        private readonly Stack<uint> indentation;

        private readonly Stack<Parameter> parametersStack;

        private readonly Stack<Field> fieldsStack;

        public Parameter Parameter
        {
            get 
            {
                if (parametersStack.Count > 0)
                    return parametersStack.Peek();

                return null;
            }
        }

        public Field Field
        {
            get
            {
                if (fieldsStack.Count > 0)
                    return fieldsStack.Peek();

                return null;
            }
        }

        public void PushParameter(Parameter parameter)
        {
            parametersStack.Push(parameter);
        }

        public void PopParameter()
        {
            if (parametersStack.Count > 0)
            {
                parametersStack.Pop();
            }
        }

        public void PushField(Field field)
        {
            fieldsStack.Push(field);
        }

        public void PopField()
        {
            if (fieldsStack.Count > 0)
            {
                fieldsStack.Pop();
            }
        }

        public void PushModule(Module module)
        {
            Module = module;
        }
        public void PushMarshalType(MarshalTypes marshalType)
        {
            marshalTypes.Push(marshalType);
        }

        public void PopMarshalType()
        {
            if (marshalTypes.Count > 0)
            {
                marshalTypes.Pop();
            }
        }

        public void PushIndent(uint indent)
        {
            indentation.Push(indent);
        }

        public void PopIndent()
        {
            if (indentation.Count > 0)
            {
                indentation.Pop();
            }
        }

        public abstract TypePrinterResult VisitArrayType(ArrayType array);

        public abstract TypePrinterResult VisitPointerType(PointerType pointer);

        public abstract TypePrinterResult VisitBuiltinType(BuiltinType builtin);

        public abstract TypePrinterResult VisitCustomType(CustomType customType);
        public abstract TypePrinterResult VisitDelegateType(DelegateType delegateType);

        public abstract TypePrinterResult VisitDependentNameType(DependentNameType dependentNameType);

        public abstract TypePrinterResult VisitEnum(Enumeration enumeration);

        public abstract TypePrinterResult VisitEnumItem(EnumerationItem item);

        public abstract TypePrinterResult VisitFunction(Function function);

        public abstract TypePrinterResult VisitDelegate(Delegate @delegate);

        public abstract TypePrinterResult VisitParameter(Parameter parameter);

        public abstract TypePrinterResult VisitClass(Class @class);

        public abstract TypePrinterResult VisitField(Field field);

        public abstract TypePrinterResult VisitProperty(Property property);

        public abstract TypePrinterResult VisitMethod(Method method);
        
        public abstract TypePrinterResult VisitParameters(
            IEnumerable<Parameter> @params,
            MarshalTypes marshalType,
            bool isExtensionMethod = false);

        public virtual TypePrinterResult VisitMacro(Macro macros) => string.Empty;

        public virtual TypePrinterResult VisitNamespace(Namespace @namespace) => string.Empty;

        public virtual TypePrinterResult VisitTranslationUnit(TranslationUnit translationUnit) => string.Empty;

        public virtual TypePrinterResult VisitComment(Comment comment) => string.Empty;
    }
}