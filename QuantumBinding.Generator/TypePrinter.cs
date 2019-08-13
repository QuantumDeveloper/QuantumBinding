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

        public virtual TypePrinterResult VisitArrayType(ArrayType array)
        {
            throw new System.NotImplementedException();
        }

        public virtual TypePrinterResult VisitPointerType(PointerType pointer)
        {
            throw new System.NotImplementedException();
        }

        public virtual TypePrinterResult VisitBuiltinType(BuiltinType builtin)
        {
            throw new System.NotImplementedException();
        }

        public virtual TypePrinterResult VisitCustomType(CustomType customType)
        {
            throw new System.NotImplementedException();
        }

        public virtual TypePrinterResult VisitDependentNameType(DependentNameType dependentNameType)
        {
            throw new System.NotImplementedException();
        }

        public virtual TypePrinterResult VisitEnum(Enumeration enumeration)
        {
            throw new System.NotImplementedException();
        }

        public virtual TypePrinterResult VisitEnumItem(EnumerationItem item)
        {
            throw new System.NotImplementedException();
        }

        public virtual TypePrinterResult VisitFunction(Function function)
        {
            throw new System.NotImplementedException();
        }

        public virtual TypePrinterResult VisitDelegate(Delegate @delegate)
        {
            throw new System.NotImplementedException();
        }

        public virtual TypePrinterResult VisitParameter(Parameter parameter)
        {
            throw new System.NotImplementedException();
        }

        public virtual TypePrinterResult VisitClass(Class @class)
        {
            throw new System.NotImplementedException();
        }

        public virtual TypePrinterResult VisitField(Field field)
        {
            throw new System.NotImplementedException();
        }

        public virtual TypePrinterResult VisitProperty(Property property)
        {
            throw new System.NotImplementedException();
        }

        public virtual TypePrinterResult VisitMethod(Method method)
        {
            throw new System.NotImplementedException();
        }

        public virtual TypePrinterResult VisitMacro(Macro macros)
        {
            throw new System.NotImplementedException();
        }

        public virtual TypePrinterResult VisitNamespace(Namespace @namespace)
        {
            throw new System.NotImplementedException();
        }

        public virtual TypePrinterResult VisitTranslationUnit(TranslationUnit translationUnit)
        {
            throw new System.NotImplementedException();
        }

        public virtual TypePrinterResult VisitParameters(IEnumerable<Parameter> @params, MarshalTypes marshalType, bool isExtensionMethod = false)
        {
            throw new System.NotImplementedException();
        }

        public virtual TypePrinterResult VisitComment(Comment comment)
        {
            throw new System.NotImplementedException();
        }
    }
}