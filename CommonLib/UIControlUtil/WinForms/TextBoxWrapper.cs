using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonLib.UIControlUtil
{
    /// <summary>
    /// 文本框包裹类
    /// </summary>
    public class TextBoxWrapper
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 被包裹的控件
        /// </summary>
        public TextBox Control { get; } = new TextBox() { Multiline = true, TabStop = false };

        /// <summary>
        /// 控件名称
        /// </summary>
        public string ControlName
        {
            get { return Control.Name; }
            set { Control.Name = value; }
        }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 控件宽度
        /// </summary>
        public int Width
        {
            get { return Control.Width; }
            set { Control.Width = value; }
        }

        /// <summary>
        /// 控件高度
        /// </summary>
        public int Height
        {
            get { return Control.Height; }
            set { Control.Height = value; }
        }

        public string Text
        {
            get { return Control.Text; }
            set { Control.Text = value; }
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="id">控件ID</param>
        /// <param name="name">控件名称</param>
        /// <param name="width">控件宽度</param>
        /// <param name="height">控件高度</param>
        public TextBoxWrapper(int id, string name, int width, int height)
        {
            Id = id;
            ControlName = name;
            Width = width;
            Height = height;
        }
    }
}
