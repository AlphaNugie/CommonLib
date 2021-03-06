﻿using CommonLib.Extensions.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibExample.PropertyMapper.Model
{
    public class StudentSource
    {
        [PropertyMapperTo("Name")]
        [PropertyMapperFrom("Name")]
        public string StudentName { get; set; } = string.Empty;

        [PropertyMapperTo("Age")]
        [PropertyMapperFrom("Age")]
        public int StudentAge { get; set; }

        [PropertyMapperTo("School.Name")]
        [PropertyMapperFrom("School.Name")]
        public string SchoolName { get; set; } = string.Empty;

        [PropertyMapperTo("School.Position")]
        [PropertyMapperFrom("School.Position")]
        public List<double> SchoolPosition { get; set; } = new List<double>() { 0, 0 };

        [PropertyMapperTo("School.City.Name")]
        [PropertyMapperFrom("School.City.Name")]
        public string SchoolCityName { get; set; } = string.Empty;
    }
}
