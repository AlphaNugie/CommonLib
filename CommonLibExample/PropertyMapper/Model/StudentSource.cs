using CommonLib.Extensions.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibExample.PropertyMapper.Model
{
    public class StudentSource
    {
        [PropertyMapper("Name")]
        public string StudentName { get; set; } = string.Empty;

        [PropertyMapper("Age")]
        public int StudentAge { get; set; }

        [PropertyMapper("School.Name")]
        public string SchoolName { get; set; } = string.Empty;

        [PropertyMapper("School.Position")]
        public List<double> SchoolPosition { get; set; } = new List<double>() { 0, 0 };

        [PropertyMapper("School.City.Name")]
        public string SchoolCityName { get; set; } = string.Empty;
    }
}
