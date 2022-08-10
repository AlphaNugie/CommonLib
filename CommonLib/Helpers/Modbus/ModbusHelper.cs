using CommonLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function.Modbus
{
    /// <summary>
    /// MODBUS功能类
    /// </summary>
    public static class ModbusHelper
    {
        /// <summary>
        /// 获取MODBUS读命令
        /// </summary>
        /// <param name="meterAddress">表地址</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="address">线圈/寄存器地址</param>
        /// <param name="quantity">地址连续量</param>
        /// <returns>返回带校验码的MODBUS读命令</returns>
        public static string GetReadCommand(byte meterAddress, FunctionCode functionCode, ushort address, ushort quantity)
        {
            string command = string.Format("{0} {1} {2} {3} {4} {5}", meterAddress.ToString("X2"), ((byte)functionCode).ToString("X2"), (address / 256).ToString("X2"), (address % 256).ToString("X2"), (quantity / 256).ToString("X2"), (quantity % 256).ToString("X2"));
            return command + " " + HexHelper.GetCRC16_String(command);
        }

        /// <summary>
        /// 获取MODBUS返回信息的正则表达式
        /// </summary>
        /// <param name="meterAddress">表地址</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="quantity">地址连续量</param>
        /// <returns>返回MODBUS返回消息的正则表达式</returns>
        public static string GetReceiveRegexPattern(byte meterAddress, FunctionCode functionCode, ushort quantity)
        {
            //返回信息格式：[表地址16进制]<空格>[功能码16进制]<空格>[字节数量(quantity*2)16进制](<空格>[16进制][16进制])X重复次数：quantity*2+2
            string pattern = string.Format(@"{0}\s{1}\s{2}(\s[0-9a-fA-F]<2>)<{3}>", meterAddress.ToString("X2"), ((byte)functionCode).ToString("X2"), (quantity * 2).ToString("X2"), quantity * 2 + 2).Replace('<', '{').Replace('>', '}');
            return pattern;
        }
    }

    /// <summary>
    /// 解析后的读指令返回消息
    /// </summary>
    public class CommandResolved
    {
        private byte[] content = null;

        /// <summary>
        /// 表地址
        /// </summary>
        public byte MeterAddress { get; set; }

        /// <summary>
        /// 功能码
        /// </summary>
        public FunctionCode FunctionCode { get; set; }

        /// <summary>
        /// 错误码，没有错误则为None
        /// </summary>
        public ErrorCode ErrorCode { get; set; }

        /// <summary>
        /// 校验码验证是否通过
        /// </summary>
        public bool CheckPassed { get; set; }

        /// <summary>
        /// 数据部分长度（出现错误则为0）
        /// </summary>
        public int DataContentLength { get; set; }

        /// <summary>
        /// 数据部分（byte数组，出现错误则为空）
        /// </summary>
        public byte[] DataContent
        {
            get { return this.content; }
            set
            {
                this.content = value;
                if (this.content != null && this.content.Length > 0)
                    this.DataContent_HexString = HexHelper.ByteArray2HexString(this.content);
            }
        }

        /// <summary>
        /// 数据部分（16进制字符串）
        /// </summary>
        public string DataContent_HexString { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="input">输入数据</param>
        public CommandResolved(string input)
        {
            this.ResolveCommand(input);
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="input">输入数据</param>
        public CommandResolved(byte[] input)
        {
            this.ResolveCommand(input);
        }

        /// <summary>
        /// 解析MODBUS返回信息
        /// </summary>
        /// <param name="hex">返回的16进制字符串</param>
        public void ResolveCommand(string hex)
        {
            this.ResolveCommand(HexHelper.HexString2Bytes(hex));
        }

        /// <summary>
        /// 解析MODBUS返回信息
        /// </summary>
        /// <param name="array">返回的byte数组</param>
        public void ResolveCommand(byte[] array)
        {
            this.FunctionCode = FunctionCode.None;
            this.ErrorCode = ErrorCode.None;
            this.CheckPassed = false;

            if (array == null || array.Length < 5)
                return;

            this.MeterAddress = array[0]; //表地址
            bool no_error = array[1] <= 0x80; //是否发生错误
            this.FunctionCode = (FunctionCode) (no_error ? array[1] : array[1] - 0x80); //发生错误时，第二位为0x80+功能码
            this.ErrorCode = no_error ? ErrorCode.None : (ErrorCode)array[2]; //错误码
            this.DataContentLength = no_error ? array[2] : 0; //数据长度
            this.CheckPassed = HexHelper.ValidateCommandCRC16(array); //是否通过校验
            //假如发生错误、校验未通过或处于某种未知原因导致长度过短，则不再继续执行
            if (this.ErrorCode != ErrorCode.None || !this.CheckPassed || array.Length == 5)
                return;

            this.DataContent = array.Skip(3).Take(array.Length - 5).ToArray();
        }
    }

    /// <summary>
    /// MODBUS功能码
    /// </summary>
    public enum FunctionCode
    {
        /// <summary>
        /// 没有功能
        /// </summary>
        [EnumDescription("没有功能")]
        None = 0,

        /// <summary>
        /// 读线圈
        /// </summary>
        [EnumDescription("读线圈")]
        Read_Coils = 1,

        /// <summary>
        /// 读输入离散量
        /// </summary>
        [EnumDescription("读输入离散量")]
        Read_Discrete_Inputs = 2,

        /// <summary>
        /// 读多个寄存器
        /// </summary>
        [EnumDescription("读多个寄存器")]
        Read_Holding_Registers = 3,

        /// <summary>
        /// 读输入寄存器
        /// </summary>
        [EnumDescription("读输入寄存器")]
        Read_Input_Registers = 4,

        /// <summary>
        /// 写单个线圈
        /// </summary>
        [EnumDescription("写单个线圈")]
        Write_Single_Coil = 5,

        /// <summary>
        /// 写单个寄存器
        /// </summary>
        [EnumDescription("写单个寄存器")]
        Write_Single_Register = 6,

        /// <summary>
        /// 读取异常状态
        /// </summary>
        [EnumDescription("读取异常状态")]
        Read_Excpetion_Status = 7,

        /// <summary>
        /// 回送诊断校验
        /// </summary>
        [EnumDescription("回送诊断校验")]
        Diagnostic = 8,

        /// <summary>
        /// 读取事件计数
        /// </summary>
        [EnumDescription("读取事件计数")]
        Get_Comm_Event_Counter = 11,

        /// <summary>
        /// 读取事件记录
        /// </summary>
        [EnumDescription("读取事件记录")]
        Get_Comm_Event_Log = 12,

        /// <summary>
        /// 写多个线圈
        /// </summary>
        [EnumDescription("写多个线圈")]
        Write_Multiple_Coils = 15,

        /// <summary>
        /// 写多个寄存器
        /// </summary>
        [EnumDescription("写多个寄存器")]
        Write_Multiple_Registers = 16,

        /// <summary>
        /// 报告从机标识
        /// </summary>
        [EnumDescription("报告从机标识")]
        Report_Server_Id = 17,

        /// <summary>
        /// 读文件记录
        /// </summary>
        [EnumDescription("读文件记录")]
        Read_File_Record = 20,

        /// <summary>
        /// 写文件记录
        /// </summary>
        [EnumDescription("写文件记录")]
        Write_File_Record = 21,

        /// <summary>
        /// 屏蔽写寄存器
        /// </summary>
        [EnumDescription("屏蔽写寄存器")]
        Mask_Write_Register = 22,

        /// <summary>
        /// 读/写多个寄存器
        /// </summary>
        [EnumDescription("读/写多个寄存器")]
        Read_Write_Multiple_Registers = 23,

        /// <summary>
        /// 读取FIFO（First Input First Output）队列
        /// </summary>
        [EnumDescription("读取FIFO队列")]
        Read_FIFO_Queue = 24,

        /// <summary>
        /// 读设备识别码
        /// </summary>
        [EnumDescription("读设备识别码")]
        Read_Device_Identification = 43
    }

    /// <summary>
    /// 错误码
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// 没有错误
        /// </summary>
        [EnumDescription("没有错误")]
        None = 0,

        /// <summary>
        /// 非法的功能码（服务器不了解功能码）
        /// </summary>
        [EnumDescription("非法的功能码")]
        Illegal_Function_Code = 1,

        /// <summary>
        /// 非法的数据地址（与请求有关）
        /// </summary>
        [EnumDescription("非法的数据地址")]
        Illegal_Data_Address = 2,

        /// <summary>
        /// 非法的数据值（与请求有关）
        /// </summary>
        [EnumDescription("非法的数据值")]
        Illegal_Data_Value = 3,

        /// <summary>
        /// 服务器故障（在执行过程中，服务器故障）
        /// </summary>
        [EnumDescription("服务器故障")]
        Server_Error = 4,

        /// <summary>
        /// 服务已确认接收
        /// 服务器接受服务调用，但需要相对长的时间完成服务。因此，服务器仅返回一个服务调用接收的确认
        /// </summary>
        [EnumDescription("服务已确认接收")]
        Confirmed = 5,

        /// <summary>
        /// 服务器繁忙
        /// 服务器不能接受MODBUS请求PDU。客户应用由责任决定是否和何时重发请求
        /// </summary>
        [EnumDescription("服务器繁忙")]
        Server_Busy = 6,

        /// <summary>
        /// 存储奇偶性差错
        /// </summary>
        [EnumDescription("存储奇偶性差错")]
        Memory_Parity_Error = 8,

        /// <summary>
        /// 网关路径无效
        /// </summary>
        [EnumDescription("网关路径无效")]
        Gateway_Path_Invalid = 10,

        /// <summary>
        /// 网关目标设备响应失败（网关生成这个异常信息）
        /// </summary>
        [EnumDescription("网关目标设备响应失败")]
        Gateway_Target_Device_Failed_To_Respond = 11
    }
}
