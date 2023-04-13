using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibExample.PropertyMapper.Model
{
    public class Student
    {
        public TestEnums TestEnum { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public int Gender { get; set; }

        public School School { get; set; }

        public List<string> StrList { get; set; }

        public List<City> CityList { get; set; }

        public string[] Friends { get; set; }

        public Student()
        {
            StrList = new List<string>() { string.Empty, string.Empty, string.Empty, string.Empty };
            CityList = new List<City>() { new City() { Name = "London" }, new City() { Name = "New York" }, new City() { Name = "Peking" } };
            Friends = new string[5];
        }

        public Student(string name, int age, int gender = 0) : this()
        {
            Name = name;
            Age = age;
            Gender = gender;
        }
    }
}
