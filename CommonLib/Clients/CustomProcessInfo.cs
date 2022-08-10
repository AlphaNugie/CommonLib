using CommonLib.Helpers;
using CommonLib.UIControlUtil.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Clients
{
    /// <summary>
    /// 指定具体所在路径与名称的进程对象
    /// </summary>
    public class CustomProcessInfo
    {
        /// <summary>
        /// 自标识进程ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 进程别名
        /// </summary>
        public string Alias { get; set; }

        ///// <summary>
        ///// 进程监听服务的本地端口
        ///// </summary>
        //public int ServerPort { get; set; }

        ///// <summary>
        ///// 是否自动启动
        ///// </summary>
        //public bool AutoStart { get; set; }

        private string _filePath;
        /// <summary>
        /// 进程文件所在路径，假如为空则文件完整路径名称仅显示文件名称
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
        public string FileNameNoExt { get; set; }

        /// <summary>
        /// 进程文件完整名称（带路径、文件名以及后缀）
        /// </summary>
        //public string ProcessFullName { get { return FileSystemHelper.TrimFilePath(_procPath) + FileSystemHelper.DirSeparatorChar + _procName; } }
        public string FullName { get; private set; }

        /// <summary>
        /// 找出所有模块名称或完整路径与给定的进程文件完整名称相符的进程列表
        /// </summary>
        //public List<Process> ProcessList { get { return Process.GetProcesses().Where(process => process.MainModule.FileName.Equals(FullName) || process.MainModule.ModuleName.Equals(FullName)).ToList(); } }
        public List<Process> ProcessList
        {
            get
            {
                //return Process.GetProcesses().Where(process =>
                //{
                //    bool result = false;
                //    try { result = process.MainModule.FileName.Equals(FullName) || process.MainModule.ModuleName.Equals(FullName); }
                //    catch (Exception) { }
                //    return result;
                //}).ToList();
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
                //List<Process> processes = Process.GetProcesses().Where(process => process.MainModule.FileName.Equals(FullName) || process.MainModule.ModuleName.Equals(FullName)).ToList();
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
            set { _hMainWnd = (int)value != 0 ? value : _hMainWnd; }
        }

        ///// <summary>
        ///// 根据数据行对象进程初始化
        ///// </summary>
        ///// <param name="dataRow">数据行对象</param>
        //public CustomProcessInfo(DataRow dataRow = null)
        //{
        //    if (dataRow == null)
        //        return;

        //    Id = dataRow.Convert("proc_id", 0);
        //    Alias = dataRow.Convert("machine_name", string.Empty);
        //    ServerPort = dataRow.Convert("proc_serv_port", 5001);
        //    AutoStart = dataRow.Convert("auto_start", 0) == 1;
        //    FilePath = dataRow.Convert("proc_path", string.Empty);
        //    FileName = dataRow.Convert("proc_name", string.Empty);
        //}

        /// <summary>
        /// 根据给定参数进行初始化
        /// </summary>
        /// <param name="id">进程ID</param>
        /// <param name="alias">进程别名</param>
        /// <param name="processPath">进程文件所在路径</param>
        /// <param name="processName">进程文件名称（包括后缀）</param>
        public CustomProcessInfo(int id, string alias, /*int serverPort, bool autoStart, */string processPath, string processName)
        {
            Id = id;
            Alias = alias;
            //ServerPort = serverPort;
            //AutoStart = autoStart;
            FilePath = processPath;
            FileName = processName;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public CustomProcessInfo() { }

        /// <summary>
        /// 更新完整路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        private void UpdateProcessFullName(string path, string name)
        {
            path = string.IsNullOrWhiteSpace(path) ? string.Empty : path;
            name = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
            //if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(name))
            //    return;

            FullName = (!path.Equals(string.Empty) ? FileSystemHelper.TrimFilePath(path) + FileSystemHelper.DirSeparatorChar : string.Empty) + name;
            //List<Process> processes = Process.GetProcesses().Where(process => process.MainModule.FileName.Equals(ProcessFullName)).ToList();
            //Process = processes == null || processes.Count == 0 ? null : processes[0];
        }

        /// <summary>
        /// 是否有进程正在运行
        /// </summary>
        /// <returns></returns>
        public bool IsRunning()
        {
            //List<Process> processes = Process.GetProcesses().Where(process => process.MainModule.FileName.Equals(ProcessFullName)).ToList();
            //Process = processes == null || processes.Count == 0 ? null : processes[0];
            return Process != null;
        }

        /// <summary>
        /// 启动进程
        /// </summary>
        /// <param name="arguments">启动该进程时传递的命令行参数</param>
        public void Startup(string arguments)
        {
            if (!string.IsNullOrWhiteSpace(FullName))
            {
                //启动进程，附带命令行参数 
                try { Process.Start(FullName, arguments);}
                catch (Exception) { }
            }
        }

        /// <summary>
        /// 启动进程
        /// </summary>
        public void Startup()
        {
            if (!string.IsNullOrWhiteSpace(FullName))
            {
                try { Process.Start(FullName); }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// 关闭进程
        /// </summary>
        public void Shutdown()
        {
            //if (!IsRunning())
            //    return;
            var process = Process;
            if (process != null)
            {
                try { Process.Kill(); }
                catch (Exception) { }
            }
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
    }
}
