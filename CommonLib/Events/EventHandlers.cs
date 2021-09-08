using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Events
{
    /// <summary>
    /// 实体类ID改变事件委托
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="eventArgs">事件数据类对象</param>
    public delegate void IdChangedEventHandler(object sender, IdChangedEventArgs eventArgs);

    /// <summary>
    /// 连接事件委托
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="eventArgs">事件数据类对象</param>
    public delegate void ConnectedEventHandler(object sender, EventArgs eventArgs);

    /// <summary>
    /// 断开事件委托
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="eventArgs">事件数据类对象</param>
    public delegate void DisconnectedEventHandler(object sender, EventArgs eventArgs);

    /// <summary>
    /// 数据维护状态改变事件（新增、更新、删除或无操作）委托
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="eventArgs">事件数据类对象</param>
    public delegate void RoutineStatusChangedEventHandler(object sender, RoutineStatusChangedEventArgs eventArgs);
    
    /// <summary>
    /// TcpClient重连成功次数改变事件委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="timerCount"></param>
    public delegate void ReconnTimerChangedEventHandler(object sender, int timerCount);

    /// <summary>
    /// 数据接收事件委托
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="eventArgs">事件数据类对象</param>
    public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs eventArgs);

    /// <summary>
    /// 数据发送事件委托
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="eventArgs">事件数据类对象</param>
    public delegate void DataSendEventHandler(object sender, DataSendEventArgs eventArgs);

    /// <summary>
    /// 发送数据事件(byte数组)委托
    /// </summary>
    /// <param name="data">待发送byte数组</param>
    /// <returns>返回操作结果</returns>
    public delegate bool DataSendEventHandler_ByteArray(byte[] data);

    /// <summary>
    /// 发送数据事件(字符串)委托
    /// </summary>
    /// <param name="content">待发送字符串</param>
    /// <returns>返回操作结果</returns>
    public delegate bool DataSendEventHandler_String(string content);

    /// <summary>
    /// 服务状态改变事件委托
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="eventArgs">事件数据类对象</param>
    public delegate void ServiceStateEventHandler(object sender, ServiceStateEventArgs eventArgs);
}
