using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.BindingsMapping
{
    [XmlRoot(ElementName = "Bindings")]
    public class BindingsMap
    {
        [XmlArray("Classes")]
        [XmlArrayItem("Class", typeof(ClassMap))]
        public List<ClassMap> Classes { get; set; }

        [XmlArray("Functions")]
        [XmlArrayItem("Function", typeof(FunctionMap))]
        public List<FunctionMap> Functions { get; set; }

        [XmlArray("Delegates")]
        [XmlArrayItem("Delegate", typeof(DelegateMap))]
        public List<DelegateMap> Delegates { get; set; }

        [XmlArray("SharedParameters")]
        [XmlArrayItem("Parameter", typeof(ParameterMap))]
        public List<ParameterMap> SharedParameters { get; set; }

        public bool TryGetCommonParameterType(BindingType inputType, out ParameterMap parameter)
        {
            parameter = null;
            if (inputType == null)
            {
                return false;
            }

            foreach (var param in SharedParameters)
            {
                var paramType = param.Type.ToBindingType();

                if (inputType.IsPrimitiveType && paramType.IsPrimitiveType)
                {
                    parameter = param;
                }
                else if (inputType.IsPointerToCustomType(out var customType) && paramType.IsPointerToCustomType(out var paramCustomType))
                {
                    if (customType.Name == paramCustomType.Name)
                    {
                        parameter = param;
                    }
                }
                else if (inputType.IsPointerToArray() && paramType.IsPointerToArray())
                {
                    var inputArrayType = (ArrayType)((PointerType)(inputType)).Pointee;
                    var paramArrayType = (ArrayType)((PointerType)(paramType)).Pointee;

                    if (inputArrayType.ElementType == paramArrayType.ElementType)
                    {
                        parameter = param;
                    }
                }
                else if (inputType.IsArray() && paramType.IsArray())
                {
                    var inputArrayType = (ArrayType)inputType;
                    var paramArrayType = (ArrayType)paramType;

                    if (inputArrayType.ElementType == paramArrayType.ElementType)
                    {
                        parameter = param;
                    }
                }
            }

            return parameter != null;
        }
    }
}
