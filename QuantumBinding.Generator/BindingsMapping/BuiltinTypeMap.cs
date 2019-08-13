using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.BindingsMapping
{
    public class BuiltinTypeMap : BindingTypeMap
    {
        [XmlAttribute]
        public PrimitiveType Type { get; set; }

        public override bool IsPrimitiveType => Type != PrimitiveType.None && Type != PrimitiveType.Unknown;

        public override BindingType ToBindingType()
        {
            return new BuiltinType(Type);
        }
    }
}
