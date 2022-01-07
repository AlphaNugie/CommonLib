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
        public static StudentSource StudentSource;
        public static Student Student;

        public static void CopyToMethodTest()
        {
            //StudentSource studentSource = new StudentSource() { StudentName = "Molly Shannon", StudentAge = 17, SchoolName = "Grandville High School", SchoolPosition = new List<double>() { 119, 911 }, SchoolCityName = "Gotham" };
            //Student student = null;
            //studentSource.CopyPropertyValueTo(ref student);
            StudentSource = new StudentSource() { StudentName = "Molly Shannon", StudentAge = 17, SchoolName = "Grandville High School", SchoolPosition = new List<double>() { 119, 911 }, SchoolCityName = "Gotham" };
            Student = null;
            StudentSource.CopyPropertyValueTo(ref Student);
        }

        public static void CopyFromMethodTest()
        {
            //StudentSource studentSource = new StudentSource();
            //Student student = new Student() { Name = "Molly Shannon", Age = 17, School = new School() { Name = "Grandville High School", Position = new List<double>() { 119, 911 }, City = new City() { Name = "Gotham" } } };
            //studentSource.CopyPropertyValueFrom(student);
            StudentSource = new StudentSource();
            Student = new Student() { Name = "Molly Shannon", Age = 17, School = new School() { Name = "Grandville High School", Position = new List<double>() { 119, 911 }, City = new City() { Name = "Gotham" } } };
            StudentSource.CopyPropertyValueFrom(Student);
        }
    }
}
