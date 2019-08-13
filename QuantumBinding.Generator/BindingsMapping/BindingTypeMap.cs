using QuantumBinding.Generator.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace QuantumBinding.Generator.BindingsMapping
{
    [XmlInclude(typeof(ArrayTypeMap))]
    [XmlInclude(typeof(PointerTypeMap))]
    [XmlInclude(typeof(CustomTypeMap))]
    [XmlInclude(typeof(BuiltinTypeMap))]
    public abstract class BindingTypeMap
    {
        [XmlAttribute]
        public bool IsConst { get; set; }

        [XmlAttribute]
        public bool IsVolatile { get; set; }

        [XmlAttribute]
        public bool IsRestrict { get; set; }

        [XmlIgnore]
        public virtual bool IsPrimitiveType => false;

        public abstract BindingType ToBindingType();
    }
}
