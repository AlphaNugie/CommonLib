using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OpcLibrary.Model
{
    /// <summary>
    /// OPC项对象
    /// </summary>
    public class OpcItem
    {
        /// <summary>
        /// 记录的唯一ID，0为新增标志，-1为导入失败标志
        /// </summary>
        public int RecordId { get; set; }

        /// <summary>
        /// OPC项名称
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// OPC组的ID
        /// </summary>
        public int OpcGroupId { get; set; }

        /// <summary>
        /// 对应数据源类的字段名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 值的系数，默认为0
        /// </summary>
        public double Coeff { get; set; }

        /// <summary>
        /// 值的偏移量，默认为0
        /// </summary>
        public double Offset { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public OpcItem() { }

        /// <summary>
        /// 构造器，从公共变量获取属性值，再用给定的DataRow对象覆盖各属性的值
        /// </summary>
        /// <param name="row"></param>
        public OpcItem(DataRow row) : this()
        {
            if (row == null)
                return;

            RecordId = int.Parse(row["record_id"].ToString());
            ItemId = row["item_id"].ToString();
            OpcGroupId = int.Parse(row["opcgroup_id"].ToString());
            FieldName = row["field_name"].ToString();
            Enabled = row["enabled"].ToString().Equals("1");
        }

        /// <summary>
        /// 构造器，从导入字符串分析各项值，各项值分别为条目级别(仅使用1,0为标题)、标签名称、opc组编号、数据源实体类字段、是否启用(0/1)、系数、偏移量，各项用半角逗号分隔
        /// </summary>
        /// <param name="import">导入字符串</param>
        public OpcItem(string import) : this()
        {
            if (string.IsNullOrWhiteSpace(import))
                return;

            //假如RecordId最终为-1则导入失败
            RecordId = -1;
            try
            {
                string[] splits = import.Split(',');
                int level = int.Parse(splits[0]);
                if (level != 1)
                    return;
                ItemId = splits[1];
                OpcGroupId = int.Parse(splits[2]);
                FieldName = splits[3];
                Enabled = splits[4].Equals("1");
                Coeff = double.Parse(splits[5]);
                Offset = double.Parse(splits[6]);
                RecordId = 0;
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 转换为CSV导出文件格式的字符串
        /// </summary>
        /// <returns></returns>
        public string ToExportString()
        {
            return string.Format("1,{0},{1},{2},{3},{4},{5}", ItemId, OpcGroupId, FieldName, Enabled ? 1 : 0, Coeff, Offset);
        }
    }
}
