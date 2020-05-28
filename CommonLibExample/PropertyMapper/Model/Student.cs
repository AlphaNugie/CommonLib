using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibExample.PropertyMapper.Model
{
    public class Student
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public School School { get; set; }
    }
}
