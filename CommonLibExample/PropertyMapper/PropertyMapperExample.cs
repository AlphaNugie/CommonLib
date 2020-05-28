using CommonLib.Extensions.Property;
using CommonLibExample.PropertyMapper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibExample.PropertyMapper
{
    public static class PropertyMapperExample
    {
        public static void Test()
        {
            StudentSource studentSource = new StudentSource() { StudentName = "Molly Shannon", StudentAge = 17, SchoolName = "Grandville High School", SchoolPosition = new List<double>() { 119, 911 }, SchoolCityName = "Gotham" };
            Student student = null;
            studentSource.CopyPropertyValueTo(ref student);
        }
    }
}
