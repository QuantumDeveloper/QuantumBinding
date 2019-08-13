using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace QuantumBinding.Generator.BindingsMapping
{
    public abstract class MapBase
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public AccessSpecifier AccessSpecifier { get; set; }

        [XmlAttribute]
        public bool IsIgnored { get; set; }
    }
}
