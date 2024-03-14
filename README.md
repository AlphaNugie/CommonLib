# CommonLib.DataUtil.SqliteProviderLibAnyCpu
## 生成位置
bin\Debug-AnyCpu
## 引用注意事项
下载sqlite时选择非混合编译（不带“bundle”字样）的包，在项目中引用非混合编译的System.Data.SQLite.dll
同时将32位和64位的SQLite.Interop.dll复制到项目根目录下的x86和x64文件夹内，SQLite.Interop.dll的属性设置为“一直复制”或“较新时复制”

# OpcLibraryAnyCpu
引用CommonLib.DataUtil.SqliteProviderLibAnyCpu的版本，框架调整为.net framework 4.5，将OpcLibrary与OpcLibrary.Controls合并

# CommonLib.DataUtil.OracleProvider
## 调用存储过程返回结果集
包头
```sql
create or replace package pag_example is
 type mycursor is ref cursor;

 Procedure p_example (var_flowid Varchar2,
            var_fremark Varchar2,
            myresult out mycursor);

end pag_example;
```
包体
```sql
create or replace package body pag_example is

 Procedure p_example (var_flowid Varchar2,
            var_fremark Varchar2,
            myresult out mycursor) is
 begin

  open myresult for

   select * from table_example

 end p_example;
end pag_example;
```
调用过程
```cs
OracleProvider provider = new ("localhost", "orcl", "user_name", "user_pass");
IDataParameter[] parameters = new IDataParameter[3]
{
    new OracleParameter("var_flowid", OracleDbType.Varchar2, ParameterDirection.Input) { Value = "1" },
    new OracleParameter("var_fremark", OracleDbType.Varchar2) { Value = "Q" },
    new OracleParameter("myresult", OracleDbType.RefCursor, ParameterDirection.Output)
};
DataTable dataTable = this.provider.RunProcedureQuery("pag_get_camera.p_camera", parameters).Tables[0];
```

# CommonLib.Extensions.Property
## PropertyMapperToAttribute
为快捷地将一个实体的属性值赋给另外一个属性而设计，配合扩展方法CopyPropertyValueTo<T>(ref T target)使用
### 属性使用
此特性只有对应用此特性的属性起作用，未使用PropertyMapperTo特性的属性或将PropertyMapperTo特性给字段使用都将不产生效果
### 可为子属性赋值
当PropertyMapperTo特性值形似"aaa.bbb"时，将可向下寻找属性中的子属性并为其赋值，假如过程中遇到为null的引用类型则将对其初始化
此过程将一直持续，直到匹配到最后一个子属性，或遇到值类型、类型不具有含有0个参数的构造器时为止
### 示例
目标
```cs
    public class Student
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public School School { get; set; }
    }

    public class School
    {
        public string Name { get; set; }

        public List<double> Position { get; set; }

        public City City { get; set; }
    }
    
    public class City
    {
        public string Name { get; set; }
    }
```
源
```cs
    public class StudentSource
    {
        [PropertyMapperTo("Name")]
        public string StudentName { get; set; } = string.Empty;

        [PropertyMapperTo("Age")]
        public int StudentAge { get; set; }

        [PropertyMapperTo("School.Name")]
        public string SchoolName { get; set; } = string.Empty;

        [PropertyMapperTo("School.Position")]
        public List<double> SchoolPosition { get; set; } = new List<double>() { 0, 0 };

        [PropertyMapperTo("School.City.Name")]
        public string SchoolCityName { get; set; } = string.Empty;
    }
```
```cs
StudentSource studentSource = new StudentSource() { StudentName = "Molly Shannon", StudentAge = 17, SchoolName = "Grandville High School", SchoolPosition = new List<double>() { 119, 911 }, SchoolCityName = "Gotham" };
Student student = null;
studentSource.CopyPropertyValueTo(ref student);
```
## PropertyMapperFromAttribute
为快捷地将一个实体的属性值从另外一个属性处获得而设计，配合扩展方法CopyPropertyValueFrom<T>(T target)使用
### 属性使用
此特性只有对应用此特性的属性起作用，未使用PropertyMapperFrom特性的属性或将PropertyMapperFrom特性给字段使用都将不产生效果
### 可为子属性赋值
当PropertyMapperFrom特性值形似"aaa.bbb"时，将可向下寻找属性中的子属性并从此属性获取值，假如过程中遇到为null的引用类型则停止寻找，返回默认值
此过程将一直持续，直到匹配到最后一个子属性，或遇到为null时为止
### 示例
源
```cs
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
```
```cs
StudentSource studentSource = new StudentSource();
Student student = new Student() { Name = "Molly Shannon", Age = 17, School = new School() { Name = "Grandville High School", Position = new List<double>() { 119, 911 }, City = new City() { Name = "Gotham" } } };
studentSource.CopyPropertyValueFrom(student);
```
