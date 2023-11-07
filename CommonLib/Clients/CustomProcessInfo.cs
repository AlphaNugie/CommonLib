using CommonLib.Function;
using CommonLib.Helpers;
using CommonLib.UIControlUtil.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Clients
{
    /// <summary>
    /// 指定具体所在路径与名称的进程对象
    /// </summary>
    public class CustomProcessInfo
    {
        #region 公共属性
        /// <summary>
        /// 自标识进程ID
        /// 数据源字段为“proc_id”
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 进程别名
        /// 数据源字段为“machine_name”
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 是否自动启动（假如进程关闭或停止响应则重新启动）
        /// 数据源字段为“auto_start”
        /// </summary>
        public bool AutoStart { get; set; }

        private string _filePath;
        /// <summary>
        /// 进程文件所在路径，假如为空则文件完整路径名称仅显示文件名称
        /// 数据源字段为“proc_path”
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                UpdateProcessFullName(_filePath, _fileName);
            }
        }

        private string _fileName;
        /// <summary>
        /// 进程文件名称（包括后缀）
        /// 数据源字段为“proc_name”
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                int index = _fileName.LastIndexOf('.');
                FileNameNoExt = index >= 0 ? _fileName.Substring(0, index) : _fileName;
                UpdateProcessFullName(_filePath, _fileName);
            }
        }

        /// <summary>
        /// 不带类型后缀的文件名
        /// </summary>
        public string FileNameNoExt { get; private set; }

        /// <summary>
        /// 进程文件完整名称（带路径、文件名以及后缀）
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// 找出所有模块名称或完整路径与给定的进程文件完整名称相符的进程列表
        /// </summary>
        public List<Process> ProcessList
        {
            get
            {
                return Process.GetProcessesByName(FileNameNoExt).Where(process =>
                {
                    bool result = false;
                    try { result = process.MainModule.FileName.Equals(FullName) || process.MainModule.ModuleName.Equals(FullName); }
                    catch (Exception) { }
                    return result;
                }).ToList();
            }
        }

        /// <summary>
        /// 进程列表中的第一个对象，假如不存在任何进程则为空
        /// </summary>
        public Process Process
        {
            get
            {
                var list = ProcessList;
                var process = list == null || list.Count == 0 ? null : list[0];
                if (process != null)
                    MainWindowHandle = process.MainWindowHandle;
                return process;
            }
        }

        private IntPtr _hMainWnd;
        /// <summary>
        /// 上一个不为空的关联进程主窗口的窗口句柄
        /// </summary>
        public IntPtr MainWindowHandle
        {
            get { return _hMainWnd; }
            private set { _hMainWnd = (int)value != 0 ? value : _hMainWnd; }
        }

        /// <summary>
        /// 用于读取应用程序错误输出的流
        /// </summary>
        public StreamReader StandardErrorReader { get; set; }
        #endregion

        #region 构造器
        /// <summary>
        /// 根据数据行对象进程初始化
        /// </summary>
        /// <param name="dataRow">数据行对象</param>
        public CustomProcessInfo(DataRow dataRow = null) : base()
        {
            if (dataRow == null)
                return;

            Id = dataRow.Convert("proc_id", 0);
            Alias = dataRow.Convert("machine_name", string.Empty);
            AutoStart = dataRow.Convert("auto_start", 0) == 1;
            FilePath = dataRow.Convert("proc_path", string.Empty);
            FileName = dataRow.Convert("proc_name", string.Empty);
        }

        /// <summary>
        /// 根据给定参数进行初始化
        /// </summary>
        /// <param name="id">进程ID</param>
        /// <param name="alias">进程别名</param>
        /// <param name="processPath">进程文件所在路径</param>
        /// <param name="processName">进程文件名称（包括后缀）</param>
        /// <param name="autoStart">进程是否自动启动或在异常时重新启动</param>
        public CustomProcessInfo(int id, string alias, string processPath, string processName, bool autoStart = false)
        {
            Id = id;
            Alias = alias;
            FilePath = processPath;
            FileName = processName;
            AutoStart = autoStart;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public CustomProcessInfo() { }
        #endregion

        #region 功能
        /// <summary>
        /// 更新进程的完整路径
        /// </summary>
        /// <param name="fullName">进程文件完整名称</param>
        public void UpdateProcessFullName(string fullName)
        {
            //string path = null, name = null;
            if (string.IsNullOrWhiteSpace(fullName))
                goto END;
            try
            {
                FileInfo fileInfo = new FileInfo(fullName);
                FilePath = fileInfo.DirectoryName;
                FileName = fileInfo.Name;
            }
            catch (Exception) { }
        END:
            //UpdateProcessFullName(path, name);
            UpdateProcessFullName(FilePath, FileName);
        }

        /// <summary>
        /// 更新进程的完整路径
        /// </summary>
        /// <param name="path">进程文件所在路径</param>
        /// <param name="name">进程文件名称</param>
        public void UpdateProcessFullName(string path, string name)
        {
            path = string.IsNullOrWhiteSpace(path) ? string.Empty : path;
            name = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
            FullName = (!path.Equals(string.Empty) ? FileSystemHelper.TrimFilePath(path) + FileSystemHelper.DirSeparatorChar : string.Empty) + name;
        }

        /// <summary>
        /// 检查进程的响应状态
        /// </summary>
        /// <returns></returns>
        public bool CheckIfResponding()
        {
            return CheckIfResponding(out _);
        }

        /// <summary>
        /// 检查进程的响应状态
        /// </summary>
        /// <param name="state">输出的当前响应状态的类型</param>
        /// <returns>假如正常响应则返回true</returns>
        public bool CheckIfResponding(out ResponseState state)
        {
            //state = ResponseState.Responding;
            //尝试获取进程对象，判断是否存在以及是否响应
            var process = Process;
            if (process == null)
                state = ResponseState.NotExisted;
            else
            {
                //var streamReader = StandardErrorReader;
                //if (streamReader != null)
                //{
                //    var error = streamReader.ReadLine();
                //    //var error = streamReader.ReadToEnd();
                //}
                state = process.Responding ? ResponseState.Responding : ResponseState.NotResponding;
            }
            return state == ResponseState.Responding;
        }

        /// <summary>
        /// 是否有进程正在运行并正常响应
        /// </summary>
        /// <returns>假如正常响应则返回true</returns>
        public bool IsRunning()
        {
            //return Process != null;
            return CheckIfResponding();
        }

        /// <summary>
        /// 启动进程
        /// </summary>
        /// <param name="arguments">启动该进程时传递的命令行参数</param>
        /// <param name="killOthers">启动时是否关闭进程的其它实例</param>
        /// <returns>假如启动成功，则返回true，否则返回false</returns>
        public bool Startup(string arguments, bool killOthers = false)
        {
            bool result = false;
            //if (!string.IsNullOrWhiteSpace(FullName))
            //    try { Process.Start(FullName, arguments); } catch (Exception) { } //启动进程，附带命令行参数 
            if (string.IsNullOrWhiteSpace(FullName))
                goto END;
            if (killOthers)
                Shutdown();
            ////启动进程，附带命令行参数（假如不为空）
            //try { var process = !string.IsNullOrWhiteSpace(arguments) ? Process.Start(FullName, arguments) : Process.Start(FullName); }
            //进程启动的参数，附带命令行参数（假如不为空）
            var startInfo = !string.IsNullOrWhiteSpace(arguments) ? new ProcessStartInfo(FullName, arguments) : new ProcessStartInfo(FullName);
            #region 假如需要重新定向输出的错误信息，则取消注释
            //startInfo.UseShellExecute = false;
            ////重定向StandardError流，以准备读取程序输出的错误
            //startInfo.RedirectStandardError = true;
            #endregion
            try
            {
                var process = Process.Start(startInfo);
                if (process == null)
                    goto END;
                process.ErrorDataReceived += new DataReceivedEventHandler(Process_ErrorDataReceived);
                result = true;
            }
            catch (Exception) { }
            END:
            return result;
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            var data = e.Data;
        }

        /// <summary>
        /// 启动进程
        /// </summary>
        /// <param name="killOthers">启动时是否关闭进程的其它实例</param>
        /// <returns>假如启动成功，则返回true，否则返回false</returns>
        public bool Startup(bool killOthers = false)
        {
            return Startup(null, killOthers);
        }

        /// <summary>
        /// 关闭所有相关进程
        /// </summary>
        public void Shutdown()
        {
            //var process = Process;
            //if (process != null)
            //{
            //    try { process.Kill(); }
            //    catch (Exception) { }
            //}
            var list = ProcessList;
            if (list == null || list.Count == 0)
                return;
            list.ForEach(process =>
            {
                if (process != null)
                    try { process.Kill(); } catch (Exception) { }
            });
        }

        /// <summary>
        /// 显示或隐藏进程的窗体
        /// </summary>
        /// <param name="show">false 隐藏（不关闭），true 显示</param>
        /// <returns></returns>
        public bool ShowWindow(bool show)
        {
            var process = Process;
            //return process != null && process.ShowWindow(show ? 1u : 0u);
            return process != null && WindowUtil.ShowWindow(MainWindowHandle, show ? 1u : 0u);
        }
        #endregion
    }

    /// <summary>
    /// 进程的响应状态
    /// </summary>
    public enum ResponseState
    {
        /// <summary>
        /// 正常响应
        /// </summary>
        Responding = 0,

        /// <summary>
        /// 无响应
        /// </summary>
        NotResponding = 1,

        /// <summary>
        /// 进程不存在
        /// </summary>
        NotExisted = 2,
    }
}
