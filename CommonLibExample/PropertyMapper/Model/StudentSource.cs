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
        [PropertyMapperFrom("TestEnum")]
        public int TestEnumValue { get; set; }

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

        //[PropertyMapperTo("StrList[0]")]
        [PropertyMapperFrom("StrList[0]")]
        public string ListSource1 { get; set; }

        //[PropertyMapperTo("StrList[1]")]
        [PropertyMapperFrom("StrList[1]")]
        public string ListSource2 { get; set; }

        //[PropertyMapperTo("StrList[3]")]
        //[PropertyMapperFrom("StrList[3]")]
        //[PropertyMapperFrom("StrList[3]", CopyFromOptions.Ignore)]
        [PropertyMapperFrom("StrList[3]", NullValueHandling.Skip)]
        public string ListSource3 { get; set; }

        [PropertyMapperFrom("CityList[0].Name")]
        public string CityName1 { get; set; }

        [PropertyMapperTo("Friends[0]")]
        public string Friend1 { get; set; }

        [PropertyMapperTo("Friends[1]")]
        public string Friend2 { get; set; }
    }
}
