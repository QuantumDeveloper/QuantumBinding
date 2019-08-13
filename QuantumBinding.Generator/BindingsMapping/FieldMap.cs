using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace QuantumBinding.Generator.BindingsMapping
{
    [XmlRoot("Field")]
    public class FieldMap : MapBase
    {
        public BindingTypeMap Type { get; set; }

        [XmlArray("Attributes")]
        [XmlArrayItem("Attribute", typeof(AttributeMap))]
        public List<AttributeMap> Attributes { get; set; }

        [XmlAttribute]
        public bool RemoveExistingAttributes { get; set; }
    }
}
