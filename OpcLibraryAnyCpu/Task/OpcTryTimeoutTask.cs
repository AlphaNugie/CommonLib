#if DA
using OpcLibrary.Core;
#elif UA
using OpcLibrary.Ua.Core;
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if DA
namespace OpcLibrary.Task
#elif UA
namespace OpcLibrary.Ua.Task
#endif
{
    /// <summary>
    /// OPC连接超时检测
    /// </summary>
    public class OpcTryTimeoutTask
    {
        /// <summary>
        /// OPC功能包装类的实体，进行具体的OPC操作
        /// </summary>
        private static readonly OpcUtilHelper opcHelper = new OpcUtilHelper(/*1000, */true);

        /// <summary>
        /// 检测与OPC服务的连接以及超时情况
        /// <para/>创建 CancellationTokenSource
        /// <para/>_cancellationTokenSource = new CancellationTokenSource();
        /// <para/>// 启动异步监控
        /// <para/>// 使用下划线表示这个任务不需要被等待
        /// <para/>_ = MonitorOpcConnRecurAsync(_cancellationTokenSource.Token);
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="restartComputer">是否重启计算机</param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task MonitorOpcConnRecurAsync(CancellationToken cancellationToken, bool restartComputer)
        {
            //检查取消请求
            while (!cancellationToken.IsCancellationRequested)
            {
                await System.Threading.Tasks.Task.Delay(10000); // 每10秒尝试一次连接

#if DA
                var result = await opcHelper.ConnectRemoteServerAsync(OpcConst.OpcServerIp, OpcConst.OpcServerName, 10000);
#elif UA
                var result = await opcHelper.ConnectRemoteServerAsync(OpcConst.OpcServerIp, OpcConst.OpcServerPort, OpcConst.OpcServerName, OpcConst.UserName, OpcConst.Password);
#endif
                if (!result.Success)
                {
                    Console.WriteLine($"OPC连接失败：{result.Message}，是否超时：{result.OperationCancelled}");
                    if (restartComputer && result.OperationCancelled)
                        RestartComputer();
                }
            }
        }

        /// <summary>
        /// 重启计算机
        /// </summary>
        /// <exception cref="PlatformNotSupportedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static void RestartComputer()
        {
            if (!Environment.OSVersion.Platform.ToString().Contains("Win"))
                throw new PlatformNotSupportedException("仅支持Windows系统");

            using (var process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = "/C shutdown /r /f /t 0";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.Verb = "runas"; // 请求管理员权限

                try
                {
                    process.Start();
                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("重启失败，请以管理员权限运行程序", ex);
                }
            }
        }
    }
}
