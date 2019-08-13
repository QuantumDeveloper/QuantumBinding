using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace QuantumBinding.Generator.BindingsMapping
{
    [XmlRoot("Parameter")]
    public class ParameterMap : MapBase
    {
        [XmlAttribute]
        public ParameterKind ParameterKind { get; set; }

        public BindingTypeMap Type { get; set; }

        public bool HasDefaultValue => !string.IsNullOrEmpty(DefaultValue);

        [XmlAttribute]
        public string DefaultValue { get; set; }
    }
}
