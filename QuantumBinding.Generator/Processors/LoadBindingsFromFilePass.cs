using QuantumBinding.Generator.AST;
using QuantumBinding.Generator.BindingsMapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Delegate = QuantumBinding.Generator.AST.Delegate;

namespace QuantumBinding.Generator.Processors
{
    internal class LoadBindingsFromFilePass : PreGeneratorPass
    {
        private BindingsMap map;
        Dictionary<string, FunctionMap> functions;
        Dictionary<string, DelegateMap> delegates;
        Dictionary<string, ClassMap> classes;
        public LoadBindingsFromFilePass(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BindingsMap));
            using (Stream reader = new FileStream(path, FileMode.Open))
            {
                map = (BindingsMap)serializer.Deserialize(reader);
                Options.VisitParameters = true;
                if (map.Functions.Count > 0)
                {
                    Options.VisitFunctions = true;
                    functions = map.Functions.ToDictionary(x => x.Name);
                }
                if (map.Delegates.Count > 0)
                {
                    Options.VisitDelegates = true;
                    delegates = map.Delegates.ToDictionary(x => x.Name);
                }
                if (map.Classes.Count > 0)
                {
                    Options.VisitClasses = true;
                    classes = map.Classes.ToDictionary(x => x.Name);
                }
            }
        }

        public override bool VisitFunction(Function function)
        {
            if (IsVisited(function))
            {
                return false;
            }

            if (!functions.TryGetValue(function.Name, out var func))
            {
                return false;
            }

            if (func.ReturnType != null)
            {
                function.ReturnType = func.ReturnType.ToBindingType();
            }

            foreach (var parameter in func.Parameters)
            {
                var param = function.Parameters.FirstOrDefault(x => x.Name == parameter.Name);
                if (param == null)
                {
                    continue;
                }

                if (parameter.Type != null)
                {
                    var decl = param.Type.Declaration;
                    param.Type = parameter.Type.ToBindingType();
                    param.Type.Declaration = decl;
                }

                if (parameter.HasDefaultValue)
                {
                    param.DefaultValue = parameter.DefaultValue;
                }

                if (parameter.ParameterKind != ParameterKind.Unknown)
                {
                    param.ParameterKind = parameter.ParameterKind;
                }
            }

            return true;
        }

        public override bool VisitParameter(Parameter parameter)
        {
            if (!map.TryGetCommonParameterType(parameter.Type, out var parameterToApply))
            {
                return false;
            }

            if (parameterToApply.ParameterKind != ParameterKind.Unknown)
            {
                parameter.ParameterKind = parameterToApply.ParameterKind;
            }

            if (parameterToApply.HasDefaultValue)
            {
                parameter.DefaultValue = parameterToApply.DefaultValue;
            }

            if (parameterToApply.Type != null)
            {
                var decl = parameter.Type.Declaration;
                parameter.Type = parameterToApply.Type.ToBindingType();
                parameter.Type.Declaration = decl;
            }

            return true;
        }

        public override bool VisitDelegate(Delegate @delegate)
        {
            if (IsVisited(@delegate))
            {
                return false;
            }

            if (!delegates.TryGetValue(@delegate.Name, out var delegateFix))
            {
                return false;
            }

            if (delegateFix.ReturnType != null)
            {
                @delegate.ReturnType = delegateFix.ReturnType.ToBindingType();
            }

            foreach (var parameter in delegateFix.Parameters)
            {
                var param = @delegate.Parameters.FirstOrDefault(x => x.Name == parameter.Name);
                if (param == null)
                {
                    continue;
                }

                if (parameter.Type != null)
                {
                    var decl = param.Type.Declaration;
                    param.Type = parameter.Type.ToBindingType();
                    param.Type.Declaration = decl;
                }

                if (parameter.ParameterKind != ParameterKind.Unknown)
                {
                    param.ParameterKind = parameter.ParameterKind;
                }
            }

            return true;
        }

        public override bool VisitClass(Class @class)
        {
            if (IsVisited(@class))
            {
                return false;
            }

            if (!classes.TryGetValue(@class.Name, out var classFix))
            {
                return false;
            }

            if (classFix.ClassType != ClassTypeMap.Undefined)
            {
                classFix.ClassType = classFix.ClassType;
            }

            if (classFix.UnderlyingNativeType != null)
            {
                @class.UnderlyingNativeType = classFix.UnderlyingNativeType.ToBindingType();
            }

            foreach (var fieldFix in classFix.Fields)
            {
                var field = @class.Fields.FirstOrDefault(x => x.Name == fieldFix.Name);
                if (field == null)
                {
                    continue;
                }

                if (fieldFix.RemoveExistingAttributes)
                {
                    field.Attributes.Clear();
                }

                foreach (var attribute in fieldFix.Attributes)
                {
                    field.AddAttribute(attribute.Name);
                }

                if (field.Type != null)
                {
                    var decl = field.Type.Declaration;
                    field.Type = fieldFix.Type.ToBindingType();
                    field.Type.Declaration = decl;
                }
            }

            foreach (var fieldFix in classFix.FieldsToAdd)
            {
                var field = @class.Fields.FirstOrDefault(x => x.Name == fieldFix.Name);
                if (field != null)
                {
                    continue;
                }

                field = new Field();
                field.Name = fieldFix.Name;
                foreach (var attribute in fieldFix.Attributes)
                {
                    field.AddAttribute(attribute.Name);
                }

                field.Type = fieldFix.Type.ToBindingType();

                @class.AddField(field);
            }

            foreach (var propertyFix in classFix.PropertiesToAdd)
            {
                var property = @class.Properties.FirstOrDefault(x => x.Name == propertyFix.Name);
                if (property != null)
                {
                    continue;
                }

                property = new Property();
                property.Name = propertyFix.Name;
                property.Type = propertyFix.Type.ToBindingType();
                property.Getter = propertyFix.Getter ? new Method() : null;
                property.Setter = propertyFix.Setter ? new Method() : null;
                property.Field = new Field(propertyFix.Field.Name);

                @class.AddProperty(property);
            }

            return true;
        }
    }
}
