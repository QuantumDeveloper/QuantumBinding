using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace QuantumBinding.Generator.BindingsMapping
{
    [XmlRoot("Class")]
    public class ClassMap : MapBase
    {
        [XmlAttribute]
        public ClassTypeMap ClassType { get; set; }

        public BindingTypeMap UnderlyingNativeType { get; set; }

        [XmlAttribute]
        public string ConnectedTo { get; set; }

        [XmlAttribute]
        public bool IsSimpleType { get; set; }

        [XmlAttribute]
        public bool IsTypedef { get; set; }

        [XmlAttribute]
        public bool IsPointer { get; set; }

        [XmlAttribute]
        public bool IsUnsafe { get; set; }

        [XmlAttribute]
        public string InnerStructName { get; set; }

        [XmlArray("Fields")]
        [XmlArrayItem("Field", typeof(FieldMap))]
        public List<FieldMap> Fields { get; set; }

        [XmlArray("FieldsToAdd")]
        [XmlArrayItem("Field", typeof(FieldMap))]
        public List<FieldMap> FieldsToAdd { get; set; }

        [XmlArray("PropertiesToAdd")]
        [XmlArrayItem("Property", typeof(PropertyMap))]
        public List<PropertyMap> PropertiesToAdd { get; set; }
    }
}
