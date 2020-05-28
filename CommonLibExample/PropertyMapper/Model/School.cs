using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibExample.PropertyMapper.Model
{
    public class School
    {
        public string Name { get; set; }

        public List<double> Position { get; set; }

        public City City { get; set; }
    }
}
