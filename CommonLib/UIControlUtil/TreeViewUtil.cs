﻿using System;
using System.Collections.Generic;
using System.Data;
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
        /// 为TreeView绑定数据源
        /// </summary>
        /// <param name="treeView">欲扩展方法的TreeView对象</param>
        /// <param name="dataSource">数据表对象，TreeView的数据源</param>
        /// <param name="parentField">绑定父节点的字段</param>
        /// <param name="keyField">绑定节点的字段</param>
        /// <param name="displayField">显示为文本的字段</param>
        public static void BindTreeViewDataSource(this TreeView treeView, DataTable dataSource, string parentField, string keyField, string displayField)
        {
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

            if (treeView.TopNode != null)
                treeView.SelectedNode = treeView.TopNode;
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
