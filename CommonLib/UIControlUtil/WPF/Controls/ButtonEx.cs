using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommonLib.UIControlUtil.WPF.Controls
{
    /// <summary>
    /// 增强的按钮控件
    /// </summary>
    public class ButtonEx : Button
    {
        static ButtonEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonEx), new FrameworkPropertyMetadata(typeof(ButtonEx)));
        }


        /// <summary>
        /// 按钮类型
        /// </summary>
        public ButtonType ButtonType
        {
            get { return (ButtonType)GetValue(ButtonTypeProperty); }
            set { SetValue(ButtonTypeProperty, value); }
        }

        /// <summary>
        /// 按钮类型依赖属性
        /// </summary>
        public static readonly DependencyProperty ButtonTypeProperty =
            DependencyProperty.Register("ButtonType", typeof(ButtonType), typeof(ButtonEx), new PropertyMetadata(ButtonType.Normal));

        /// <summary>
        /// 按钮图标
        /// </summary>
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// 按钮图标依赖属性
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(ButtonEx), new PropertyMetadata(null));

        /// <summary>
        /// 按钮边角圆滑度
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// 按钮边角圆滑度依赖属性
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ButtonEx), new PropertyMetadata(new CornerRadius(0)));

        /// <summary>
        /// 鼠标悬停前景色
        /// </summary>
        public Brush MouseOverForeground
        {
            get { return (Brush)GetValue(MouseOverForegroundProperty); }
            set { SetValue(MouseOverForegroundProperty, value); }
        }

        /// <summary>
        /// 鼠标悬停前景色依赖属性
        /// </summary>
        public static readonly DependencyProperty MouseOverForegroundProperty =
            DependencyProperty.Register("MouseOverForeground", typeof(Brush), typeof(ButtonEx), new PropertyMetadata());

        /// <summary>
        /// 鼠标按下前景色
        /// </summary>
        public Brush MousePressedForeground
        {
            get { return (Brush)GetValue(MousePressedForegroundProperty); }
            set { SetValue(MousePressedForegroundProperty, value); }
        }

        /// <summary>
        /// 鼠标按下前景色依赖属性
        /// </summary>
        public static readonly DependencyProperty MousePressedForegroundProperty =
            DependencyProperty.Register("MousePressedForeground", typeof(Brush), typeof(ButtonEx), new PropertyMetadata());

        /// <summary>
        /// 鼠标悬停边框颜色
        /// </summary>
        public Brush MouseOverBorderbrush
        {
            get { return (Brush)GetValue(MouseOverBorderbrushProperty); }
            set { SetValue(MouseOverBorderbrushProperty, value); }
        }

        /// <summary>
        /// 鼠标悬停边框颜色依赖属性
        /// </summary>
        public static readonly DependencyProperty MouseOverBorderbrushProperty =
            DependencyProperty.Register("MouseOverBorderbrush", typeof(Brush), typeof(ButtonEx), new PropertyMetadata());

        /// <summary>
        /// 鼠标悬停背景色
        /// </summary>
        public Brush MouseOverBackground
        {
            get { return (Brush)GetValue(MouseOverBackgroundProperty); }
            set { SetValue(MouseOverBackgroundProperty, value); }
        }

        /// <summary>
        /// 鼠标悬停背景色依赖属性
        /// </summary>
        public static readonly DependencyProperty MouseOverBackgroundProperty =
            DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(ButtonEx), new PropertyMetadata());

        /// <summary>
        /// 鼠标按下背景色
        /// </summary>
        public Brush MousePressedBackground
        {
            get { return (Brush)GetValue(MousePressedBackgroundProperty); }
            set { SetValue(MousePressedBackgroundProperty, value); }
        }

        /// <summary>
        /// 鼠标按下背景色依赖属性
        /// </summary>
        public static readonly DependencyProperty MousePressedBackgroundProperty =
            DependencyProperty.Register("MousePressedBackground", typeof(Brush), typeof(ButtonEx), new PropertyMetadata());

        /// <summary>
        /// 重写的OnClick方法
        /// </summary>
        protected override void OnClick()
        {
            base.OnClick();

            if (!string.IsNullOrEmpty(Name) && Name == "PART_Close_TabItem")
            {
                TabItemClose itemclose = FindVisualParent<TabItemClose>(this);
                (itemclose.Parent as TabControl).Items.Remove(itemclose);
                RoutedEventArgs args = new RoutedEventArgs(TabItemClose.CloseItemEvent, itemclose);
                itemclose.RaiseEvent(args);
            }

        }

        /// <summary>
        /// 查找VisualParent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T FindVisualParent<T>(DependencyObject obj) where T : class
        {
            while (obj != null)
            {
                if (obj is T)
                    return obj as T;

                obj = VisualTreeHelper.GetParent(obj);
            }

            return null;
        }
    }

    /// <summary>
    /// 按钮类型枚举
    /// </summary>
    public enum ButtonType
    {
        /// <summary>
        /// 普通按钮
        /// </summary>
        Normal,

        /// <summary>
        /// 图标按钮
        /// </summary>
        Icon,

        /// <summary>
        /// 文本按钮
        /// </summary>
        Text,

        /// <summary>
        /// 图标+文本按钮
        /// </summary>
        IconText
    }
}
