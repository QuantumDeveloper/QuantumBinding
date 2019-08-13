using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;

namespace QuantumBinding.Generator.BindingsMapping
{
    public class DelegateMap : MapBase
    {
        [XmlAttribute]
        public CallingConventionMap CallingConvention { get; set; }

        [XmlAttribute]
        public bool SuppressUnmanagedCodeSecurity { get; set; }

        public BindingTypeMap ReturnType { get; set; }

        [XmlArray("Parameters")]
        [XmlArrayItem("Parameter", typeof(ParameterMap))]
        public List<ParameterMap> Parameters { get; set; }
    }
}
