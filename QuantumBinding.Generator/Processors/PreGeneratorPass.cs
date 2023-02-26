using System.Collections.Generic;
using System.Linq;
using QuantumBinding.Generator.AST;

namespace QuantumBinding.Generator.Processors
{
    public abstract class PreGeneratorPass : IPreGeneratorPass
    {
        protected PreGeneratorPass()
        {
            Visited = new HashSet<Declaration>();
            Options = new PreGeneratorOptions();
        }

        private uint runIndex;
        private HashSet<Declaration> Visited { get; }
        public PreGeneratorOptions Options { get; }
        public ProcessingContext ProcessingContext { get; set; }
        public ASTContext AstContext => ProcessingContext?.AstContext;

        protected TranslationUnit CurrentNamespace { get; private set; }
        protected Declaration ParentDeclaration { get; private set; }

        public bool RunAgain { get; set; }

        public uint RunIndex => runIndex;

        public void Run()
        {
            OnInitialize();

            foreach (var unit in AstContext.TranslationUnits)
            {
                CurrentNamespace = unit;
                VisitTranslationUnit(unit);
                OnTranslationUnitPassCompleted();
            }

            OnComplete();
            runIndex++;
        }

        protected virtual bool IsVisited(Declaration declaration)
        {
            return !Visited.Add(declaration);
        }

        protected void CleanVisitedDeclarations()
        {
            Visited.Clear();
        }

        public virtual bool VisitTranslationUnit(TranslationUnit translationUnit)
        {
            VisitNamespace(translationUnit);

            return true;
        }

        public bool VisitNamespace(Namespace @namespace)
        {
            if (Options.VisitEnums)
            {
                foreach (var @enum in @namespace.Enums)
                {
                    ParentDeclaration = @enum;
                    @enum.Visit(this);
                }

                ParentDeclaration = null;
            }


            var classes = @namespace.AllClasses.ToArray();
            foreach (var @class in classes)
            {
                ParentDeclaration = @class;
                if (Options.VisitClasses)
                {
                    @class.Visit(this);
                }

                if (Options.VisitFields)
                {
                    foreach (var field in @class.Fields)
                    {
                        field.Visit(this);
                    }
                }

                if (Options.VisitProperties)
                {
                    foreach (var property in @class.Properties)
                    {
                        property.Visit(this);
                    }
                }

                foreach (var method in @class.AllMethods)
                {
                    ParentDeclaration = method;
                    if (Options.VisitMethods)
                    {
                        method.Visit(this);
                    }

                    if (Options.VisitParameters)
                    {
                        foreach (var argument in method.Parameters)
                        {
                            argument.Visit(this);
                        }
                    }
                }
            }

            ParentDeclaration = null;

            if (Options.VisitDelegates)
            {
                foreach (var @delegate in @namespace.Delegates)
                {
                    ParentDeclaration = @delegate;
                    @delegate.Visit(this);

                    if (Options.VisitParameters)
                    {
                        foreach (var argument in @delegate.Parameters)
                        {
                            argument.Visit(this);
                        }
                    }
                }

                ParentDeclaration = null;
            }


            foreach (var function in @namespace.Functions)
            {
                ParentDeclaration = function;
                if (Options.VisitFunctions)
                {
                    function.Visit(this);
                }

                if (Options.VisitParameters)
                {
                    foreach (var argument in function.Parameters)
                    {
                        argument.Visit(this);
                    }
                }

                ParentDeclaration = null;
            }


            foreach (var method in @namespace.Methods)
            {
                ParentDeclaration = method;
                if (Options.VisitMethods)
                {
                    method.Visit(this);
                }

                if (Options.VisitParameters)
                {
                    foreach (var argument in method.Parameters)
                    {
                        argument.Visit(this);
                    }
                }
                
                ParentDeclaration = null;
            }

            if (Options.VisitMacros)
            {
                foreach (var macro in @namespace.Macros)
                {
                    ParentDeclaration = macro;
                    macro.Visit(this);
                }

                ParentDeclaration = null;
            }

            return true;
        }

        public virtual bool VisitMacro(Macro macro)
        {
            return true;
        }

        public virtual bool VisitEnum(Enumeration enumeration)
        {
            if (Options.VisitEnumItems)
            {
                foreach (var item in enumeration.Items)
                {
                    if (IsVisited(item))
                        continue;

                    VisitEnumItem(item);
                }
            }

            return true;
        }

        public virtual bool VisitEnumItem(EnumerationItem item)
        {
            return true;
        }

        public virtual bool VisitFunction(Function function)
        {
            return true;
        }

        public virtual bool VisitDelegate(Delegate @delegate)
        {
            return true;
        }

        public virtual bool VisitParameter(Parameter parameter)
        {
            return true;
        }

        public virtual bool VisitClass(Class @class)
        {
            return true;
        }

        public virtual bool VisitField(Field field)
        {
            return true;
        }

        public virtual bool VisitProperty(Property property)
        {
            return true;
        }

        public virtual bool VisitMethod(Method method)
        {
            return true;
        }

        public bool VisitComment(Comment comment)
        {
            return true;
        }

        public virtual void OnInitialize()
        {
        }

        public virtual void OnTranslationUnitPassCompleted()
        {
        }

        public virtual void OnComplete()
        {
        }
    }
}