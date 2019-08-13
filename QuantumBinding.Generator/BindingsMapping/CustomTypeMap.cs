using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.BindingsMapping
{
    [XmlRoot("CustomType")]
    public class CustomTypeMap : BindingTypeMap
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public bool IsInSystemHeader { get; set; }

        public override BindingType ToBindingType()
        {
            return new CustomType(Name) { IsInSystemHeader = IsInSystemHeader };
        }
    }
}
