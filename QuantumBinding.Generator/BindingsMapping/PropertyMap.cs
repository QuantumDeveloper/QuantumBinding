using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace QuantumBinding.Generator.BindingsMapping
{
    public class PropertyMap : MapBase
    {
        public BindingTypeMap Type { get; set; }

        [XmlAttribute]
        public bool Getter { get; set; }

        [XmlAttribute]
        public bool Setter { get; set; }

        [XmlAttribute]
        public bool IsAutoProperty { get; set; }

        public FieldMap Field { get; set; }
    }
}
