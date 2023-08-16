using CommonLib.Extensions.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibExample.PropertyMapper.Model
{
    public class Source
    {
        [PropertyMapperTo("Name")]
        public string Name { get; set; }

        [PropertyMapperTo("Value")]
        public string Value { get; set; }

        [PropertyMapperTo("Number")]
        public int Number { get; set; }

        public Source() { }
    }
}
