using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.Types;
using System;
using System.Linq;
using Delegate = QuantumBinding.Generator.AST.Delegate;

namespace QuantumBinding.Generator.Processors
{
    public class CaseRenamePass : PreGeneratorPass
    {
        private CasePattern casePattern;
        private RenameTargets renameTargets;

        public CaseRenamePass(RenameTargets targets, CasePattern pattern)
        {
            this.renameTargets = targets;
            this.casePattern = pattern;

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

                if (targets.HasFlag(RenameTargets.Class))
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
        }

        public override bool VisitEnum(Enumeration enumeration)
        {
            if (IsVisited(enumeration))
                return false;

            enumeration.Name = SplitAndRename(enumeration.Name);

            return true;
        }

        public override bool VisitClass(Class @class)
        {
            if (IsVisited(@class))
            {
                return false;
            }

            @class.Name = SplitAndRename(@class.Name);
            if (@class.UnderlyingNativeType != null)
            {
                foreach (var unit in AstContext.TranslationUnits)
                {
                    var @struct = unit.Declarations.FirstOrDefault(x =>
                        x.OriginalName == @class.UnderlyingNativeType.Declaration.OriginalName);

                    if (@struct != null)
                    {
                        @class.UnderlyingNativeType.Declaration = @struct;
                        break;
                    }
                }
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
                function.Name = SplitAndRename(function.Name);
            }

            return true;
        }

        public override bool VisitDelegate(AST.Delegate @delegate)
        {
            if (IsVisited(@delegate))
            {
                return false;
            }

            var returnType = @delegate.ReturnType;
            RenameType(returnType);
            if (renameTargets.HasFlag(RenameTargets.Delegate))
            {
                @delegate.Name = SplitAndRename(@delegate.Name);
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
                method.Name = SplitAndRename(method.Name);
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
            var decl = type.Declaration;
            if (decl == null)
            {
                if (type.IsCustomType(out var custom) && !custom.IsInSystemHeader)
                {
                    custom.Name = SplitAndRename(custom.Name);
                }
                return;
            }

            if ((decl is Class @class &&
                (@class.ClassType == ClassType.Class && renameTargets.HasFlag(RenameTargets.Class) ||
                @class.ClassType != ClassType.Class && renameTargets.HasFlag(RenameTargets.Class))) ||
                decl is Enumeration && renameTargets.HasFlag(RenameTargets.Enum) ||
                decl is Delegate && renameTargets.HasFlag(RenameTargets.Delegate))
            {
                if (type.IsCustomType(out var custom))
                {
                    custom.Name = SplitAndRename(custom.Name);
                }
            }
        }

        private string SplitAndRename(string name)
        {
            var splittted = name.Split('_', StringSplitOptions.RemoveEmptyEntries);
            if (splittted.Length == 0)
            {
                return name;
            }

            string result = "";

            for (int i = 0; i < splittted.Length; i++)
            {
                string part = splittted[i];
                if (i == 0)
                {
                    if (casePattern == CasePattern.CamelCase)
                    {
                        result = part[0].ToString().ToLower()+part.Substring(1);
                    }
                    else
                    {
                        result = part[0].ToString().ToUpper() + part.Substring(1);
                    }
                }
                else
                {
                    result += part[0].ToString().ToUpper() + part.Substring(1);
                }
            }

            return result;
        }
    }
}
