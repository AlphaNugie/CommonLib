using CommonLib.Extensions.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibExample.PropertyMapper.Model
{
    public class SourceInherit : Source
    {
        [PropertyMapperTo("NameNew")]
        public string NameNew { get; set; }

        [PropertyMapperTo("NameOld")]
        public string NameOld { get; set; }

        public SourceInherit() { }
    }
}
