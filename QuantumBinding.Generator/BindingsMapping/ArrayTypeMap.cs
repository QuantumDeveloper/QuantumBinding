using QuantumBinding.Generator.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace QuantumBinding.Generator.BindingsMapping
{
    public class ArrayTypeMap : BindingTypeMap
    {
        public BindingTypeMap ElementType { get; set; }

        [XmlAttribute]
        public ArraySizeType SizeType { get; set; }

        [XmlAttribute]
        public long Size { get; set; }

        [XmlAttribute]
        public long ElementSize { get; set; }

        [XmlAttribute]
        public string ArraySizeSource { get; set; }

        public override BindingType ToBindingType()
        {
            return new ArrayType()
            {
                ElementType = ElementType.ToBindingType(),
                SizeType = SizeType,
                Size = Size,
                ElementSize = ElementSize,
                ArraySizeSource = ArraySizeSource
            };
        }
    }
}
