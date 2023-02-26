using System.Linq;
using System.Text.RegularExpressions;
using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;
using QuantumBinding.Generator.Utils;

namespace QuantumBinding.Generator.Processors
{
    public class RegexRenamePass : PreGeneratorPass
    {
        protected Regex regex;
        protected string pattern;
        protected string replaceWith;
        protected RenameTargets renameTargets;
        protected RegexOptions regexOpts;

        protected RegexRenamePass()
        {

        }

        public RegexRenamePass(string pattern, string replaceWith, RenameTargets targets, bool ignoreCase)
        {
            InitializeLocalValues(pattern, replaceWith, targets, ignoreCase);
        }

        protected void InitializeLocalValues(string pattern, string replaceWith, RenameTargets targets, bool ignoreCase)
        {
            this.pattern = pattern;
            this.replaceWith = replaceWith;
            regexOpts = RegexOptions.None;
            if (ignoreCase)
            {
                regexOpts |= RegexOptions.IgnoreCase;
            }
            regex = new Regex(pattern, regexOpts);

            if (targets != RenameTargets.Any)
            {
                if (targets.HasFlag(RenameTargets.Enum))
                {
                    Options.VisitEnums = true;
                }

                if (targets.HasFlag(RenameTargets.EnumItem))
                {
                    Options.VisitEnumItems = true;
                }

                if (targets.HasFlag(RenameTargets.Class)
                || targets.HasFlag(RenameTargets.Struct)
                || targets.HasFlag(RenameTargets.StructWrapper))
                {
                    Options.VisitClasses = true;
                }

                if (targets.HasFlag(RenameTargets.Field))
                {
                    Options.VisitFields = true;
                }

                if (targets.HasFlag(RenameTargets.Property))
                {
                    Options.VisitProperties = true;
                }

                if (targets.HasFlag(RenameTargets.Method))
                {
                    Options.VisitMethods = true;
                    Options.VisitClasses = true;
                }

                if (targets.HasFlag(RenameTargets.Function))
                {
                    Options.VisitFunctions = true;
                }

                if (targets.HasFlag(RenameTargets.Delegate))
                {
                    Options.VisitDelegates = true;
                }

                if (targets.HasFlag(RenameTargets.Parameter))
                {
                    Options.VisitParameters = true;
                    Options.VisitFunctions = true;
                    Options.VisitDelegates = true;
                    Options.VisitMethods = true;
                }
            }
            else
            {
                Options.VisitEnums = true;
                Options.VisitClasses = true;
                Options.VisitFields = true;
                Options.VisitProperties = true;
                Options.VisitMethods = true;
                Options.VisitFunctions = true;
                Options.VisitDelegates = true;
                Options.VisitParameters = true;
            }

            this.renameTargets = targets;
        }

        public override bool VisitEnum(Enumeration enumeration)
        {
            if (IsVisited(enumeration))
                return false;

            enumeration.Name = Replace(enumeration.Name);

            return true;
        }

        public override bool VisitEnumItem(EnumerationItem item)
        {
            if (IsVisited(item))
                return false;

            var result = regex.Match(item.Name);

            if (!result.Success)
            {
                return false;
            }

            item.Name = Replace(item.Name);

            return true;
        }

        public override bool VisitClass(Class @class)
        {
            if (IsVisited(@class))
                return false;

            if (renameTargets.HasFlag(RenameTargetsUtils.ClassTypeToRenameTargets(@class.ClassType)))
            {
                @class.Name = Replace(@class.Name);
            }

            return true;
        }

        public override bool VisitField(Field field)
        {
            if (IsVisited(field))
            {
                return false;
            }

            var type = field.Type;
            RenameType(type);

            return true;
        }

        public override bool VisitProperty(Property property)
        {
            if (IsVisited(property))
            {
                return false;
            }

            var type = property.Type;
            RenameType(type);

            return true;
        }

        public override bool VisitFunction(Function function)
        {
            if (IsVisited(function))
            {
                return false;
            }

            var returnType = function.ReturnType;
            RenameType(returnType);
            if (renameTargets.HasFlag(RenameTargets.Function))
            {
                function.Name = Replace(function.Name);
            }

            return true;
        }

        public override bool VisitDelegate(Delegate @delegate)
        {
            if (IsVisited(@delegate))
            {
                return false;
            }

            var returnType = @delegate.ReturnType;
            RenameType(returnType);
            if (renameTargets.HasFlag(RenameTargets.Delegate))
            {
                @delegate.Name = Replace(@delegate.Name);
            }

            return true;
        }

        public override bool VisitMethod(Method method)
        {
            if (IsVisited(method))
            {
                return false;
            }

            var returnType = method.ReturnType;

            RenameType(returnType);

            if (renameTargets.HasFlag(RenameTargets.Method))
            {
                method.Name = Replace(method.Name);
            }

            return true;
        }

        public override bool VisitParameter(Parameter parameter)
        {
            if (IsVisited(parameter))
            {
                return false;
            }

            RenameType(parameter.Type);

            return true;
        }

        private void RenameType(BindingType type)
        {
            if (type == null) return;

            var decl = type.Declaration;
            if (decl == null)
            {
                if (type.IsCustomType(out var custom) && !custom.IsInSystemHeader)
                {
                    custom.Name = Replace(custom.Name);
                }
                return;
            }
            
            if (decl is Class @class && renameTargets.HasFlag(RenameTargetsUtils.ClassTypeToRenameTargets(@class.ClassType)) ||
                decl is Enumeration && renameTargets.HasFlag(RenameTargets.Enum) ||
                decl is Delegate && renameTargets.HasFlag(RenameTargets.Delegate))
            {
                if (type.IsCustomType(out var custom))
                {
                    custom.Name = Replace(custom.Name);
                }

                decl.Name = Replace(decl.Name);
            }
        }

        private string Replace(string input)
        {
            var result = regex.Match(input);

            if (!result.Success)
            {
                return input;
            }

            return Regex.Replace(input, pattern, replaceWith, regexOpts);
        }
    }
}