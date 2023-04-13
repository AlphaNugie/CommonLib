using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonLib.UIControlUtil
{
    /// <summary>
    /// 扩展方法类
    /// </summary>
    public static class TreeViewUtil
    {
        /// <summary>
        /// 遍历TreeView下的所有节点，可选择是否包括子节点
        /// </summary>
        /// <param name="treeView">当前等待遍历的TreeView</param>
        /// <param name="childIncl">是否算入子节点</param>
        public static List<TreeNode> GetAllNodes(this TreeView treeView, bool childIncl = true)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            if (treeView == null || treeView.Nodes == null || treeView.Nodes.Count == 0)
                goto END;
            var childNodes = treeView.Nodes.Cast<TreeNode>().ToList();
            nodes.AddRange(childNodes);
            if (childIncl)
                childNodes.ForEach(child => nodes.AddRange(child.GetAllNodes()));
        END:
            return nodes;
        }

        /// <summary>
        /// 遍历TreeNode下的所有节点，可选择是否包括子节点
        /// </summary>
        /// <param name="treeNode">当前等待遍历的TreeNode</param>
        /// <param name="childIncl">是否算入子节点</param>
        public static List<TreeNode> GetAllNodes(this TreeNode treeNode, bool childIncl = true)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            if (treeNode == null || treeNode.Nodes == null || treeNode.Nodes.Count == 0)
                goto END;
            var childNodes = treeNode.Nodes.Cast<TreeNode>().ToList();
            nodes.AddRange(childNodes);
            if (childIncl)
                childNodes.ForEach(child => nodes.AddRange(child.GetAllNodes()));
            END:
            return nodes;
        }

        /// <summary>
        /// 为TreeView绑定数据源
        /// </summary>
        /// <param name="treeView">欲扩展方法的TreeView对象</param>
        /// <param name="dataSource">数据表对象，TreeView的数据源</param>
        /// <param name="parentField">绑定父节点的字段</param>
        /// <param name="keyField">绑定节点的字段</param>
        /// <param name="displayField">显示为文本的字段</param>
        /// <param name="stayOnCurrNode">重绑定数据源后，仍尝试停留在上个已选节点上</param>
        public static void BindTreeViewDataSource(this TreeView treeView, DataTable dataSource, string parentField, string keyField, string displayField, bool stayOnCurrNode = true)
        {
            if (treeView == null)
                return;

            var currNode = treeView.SelectedNode; //记录当前所选节点
            treeView.Nodes.Clear(); //清除所有节点
            if (dataSource == null || dataSource.Rows.Count == 0)
                return;

            //选出根节点并遍历
            DataRow[] dataRows = dataSource.Select(string.Format("{0} is null or {0} = 'ROOT'", parentField));
            foreach (DataRow dataRow in dataRows)
            {
                //决定当前节点的文本与值
                TreeNode treeNode = new TreeNode
                {
                    Text = dataRow[displayField].ToString(),
                    Tag = dataRow[keyField].ToString()
                };

                treeNode.FillTree(dataSource, parentField, keyField, displayField); //填充当前节点的子节点
                treeView.Nodes.Add(treeNode); //向TreeView添加当前节点
            }

            //假如之前所选节点不为空，并设置为重回上一个所选节点，则尝试重新选定，否则选择根节点
            if (currNode != null && stayOnCurrNode)
                treeView.SelectedNode = treeView.GetAllNodes().FirstOrDefault(node => node.Tag.Equals(currNode.Tag));
            else if (treeView.TopNode != null)
                treeView.SelectedNode = treeView.TopNode;
            //重新使TreeView获取焦点以使选中节点高亮
            treeView.Select();
        }

        /// <summary>
        /// 为TreeNode填充子节点
        /// </summary>
        /// <param name="treeNode">欲填充子节点的TreeNode对象</param>
        /// <param name="dataSource">数据表对象，TreeView与TreeNode的数据源</param>
        /// <param name="parentField">绑定父节点的字段</param>
        /// <param name="keyField">绑定节点的字段</param>
        /// <param name="displayMember">显示为文本的字段</param>
        public static void FillTree(this TreeNode treeNode, DataTable dataSource, string parentField, string keyField, string displayMember)
        {
            treeNode.Nodes.Clear(); //清除所有子节点
            if (dataSource == null || dataSource.Rows.Count == 0)
                return;

            //选出当前节点的子节点并遍历
            DataRow[] dataRows = dataSource.Select(string.Format("{0} = '{1}'", parentField, treeNode.Tag.ToString()));
            if (dataRows == null || dataRows.Length <= 0)
                return;

            foreach (DataRow dataRow in dataRows)
            {
                TreeNode childNode = new TreeNode
                {
                    Text = dataRow[displayMember].ToString(),
                    Tag = dataRow[keyField].ToString()
                };
                childNode.FillTree(dataSource, parentField, keyField, displayMember);
                treeNode.Nodes.Add(childNode);
            }
        }

        /// <summary>
        /// 寻找某一节点的根节点
        /// </summary>
        /// <param name="treeNode"></param>
        /// <returns></returns>
        public static TreeNode FindRootNode(this TreeNode treeNode)
        {
            if (treeNode.Parent == null)
                return treeNode;
            else
                return FindRootNode(treeNode.Parent);
        }
    }
}
