using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using QuantumBinding.Generator.Types;

namespace QuantumBinding.Generator.BindingsMapping
{
    public class PointerTypeMap : BindingTypeMap
    {
        public BindingTypeMap Pointee { get; set; }

        [XmlAttribute]
        public bool IsNullable { get; set; } //Add "?" to the type

        [XmlAttribute]
        public bool IsNullableForDelegate { get; set; }

        public override BindingType ToBindingType()
        {
            return new PointerType()
            {
                Pointee = Pointee.ToBindingType(),
                IsNullable = IsNullable,
                IsNullableForDelegate = IsNullableForDelegate
            };
        }
    }
}
