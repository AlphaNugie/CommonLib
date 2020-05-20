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
