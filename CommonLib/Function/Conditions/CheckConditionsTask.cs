using CommonLib.Clients.Tasks;
using CommonLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib.Function.Conditions
{
    /// <summary>
    /// 检查各条件是否符合的任务
    /// </summary>
#if NET45_OR_GREATER
    public abstract class CheckConditionsTask : Task
#elif NET9_0_OR_GREATER
    public abstract class CheckConditionsTask : Clients.Tasks.Task
#endif
    {
        private List<ConditionSet> _conditions;

        /// <inheritdoc/>
#if NET45_OR_GREATER
        protected override Task GetNewInstance()
#elif NET9_0_OR_GREATER
        protected override Clients.Tasks.Task GetNewInstance()
#endif
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override void Init() { }

        /// <summary>
        /// 返回条件描述字符串，格式类似于“Front2Back = 1, base1.db; Back2Front != 1, base2.db”
        /// </summary>
        /// <returns></returns>
        protected abstract string GetConditionsDescrption();

        /// <summary>
        /// 返回数据源对象，要在此数据源中寻找各属性的值并与条件进行比较
        /// </summary>
        /// <returns></returns>
        protected abstract object GetDataSource();

        /// <summary>
        /// 没有任何条件命中时做的事
        /// </summary>
        protected abstract void DoThingsWhenMissed();

        /// <summary>
        /// 有任一条件命中后做的事
        /// </summary>
        /// <param name="conditionHitDescription">命中后获得的描述字符串</param>
        /// <param name="condition">命中的条件</param>
        protected abstract void DoThingsWhenHit(string conditionHitDescription, ConditionSet condition);

        /// <inheritdoc/>
        protected override void LoopContent()
        {
            //var conditions = BaseConst.SqliteFileNameConditions;
            var conditions = GetConditionsDescrption();
            if (string.IsNullOrWhiteSpace(conditions))
                return;
            //_conditions = conditions.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => new ConditionSet(x, BaseConst.OpcDataSource)).ToList();
            var dataSource = GetDataSource();
            _conditions = conditions.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => new ConditionSet(x, dataSource)).ToList();
            if (_conditions == null || _conditions.Count == 0)
                return;

            //var opcSource = BaseConst.OpcDataSource;
            string fileName = string.Empty;
            ConditionSet cond = null;
            foreach (var c in _conditions)
            {
                //Type targetType;
                //object targetValue = opcSource.GetPropertyValue(c.FieldName, out targetType);
                //if (c.IsMatch(targetValue))
                //{
                //    fileName = c.SqliteFileName;
                //    cond = c;
                //    break;
                //}

                if (c.IsMatch())
                {
                    fileName = c.ConditionHitDescription;
                    cond = c;
                    break;
                }
            }
            //string log;
            //var opcSource = BaseConst.OpcDataSource;
            //假如没有命中任何条件，则不进行修改
            if (cond == null)
            {
                //log = string.Format("没有任何条件命中，堆料模式BaseConst.OpcDataSource.StackDir_Back2Front的值为{0}", opcSource.StackDir_Back2Front);
                //if (!logStr.Equals(log))
                //    BaseConst.Log.WriteLogsToFile(log);
                //logStr = log;
                DoThingsWhenMissed();
                return;
            }
            //string newFilePath, fileDir = BaseConst.SqliteFileDir;
            //FileSystemHelper.UpdateFilePath(ref fileDir, fileName, out newFilePath);
            //// 若文件不存在，则不进行修改
            //if (string.IsNullOrWhiteSpace(newFilePath) || !File.Exists(newFilePath))
            //    return;
            //// 修改文件名
            //string prevFileName = BaseConst.SqliteFileName;
            //// 若文件名发生变化，则退出程序
            //if (!prevFileName.Equals(fileName, StringComparison.OrdinalIgnoreCase))
            //{
            //    BaseConst.IniHelper.WriteData("Sqlite", "FileName", fileName);
            //    log = string.Format("条件“{0}”命中，堆料模式BaseConst.OpcDataSource.StackDir_Back2Front的值为{1}，将修改Sqlite数据库文件名为{2}（假如文件存在）", cond.ToString(), opcSource.StackDir_Back2Front, fileName);
            //    if (!logStr.Equals(log))
            //        BaseConst.Log.WriteLogsToFile(log);
            //    logStr = log;
            //    BaseConst.Log.WriteLogsToFile(string.Format("Sqlite config file name changed from {0} to {1}.", prevFileName, fileName));
            //    Environment.Exit(0);
            //}
            DoThingsWhenHit(fileName, cond);
        }
    }
}
