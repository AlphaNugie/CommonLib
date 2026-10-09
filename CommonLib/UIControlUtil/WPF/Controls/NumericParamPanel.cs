using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using CommonLib.UIControlUtil.NumericParamEditor;

namespace CommonLib.UIControlUtil.WPF.Controls
{
    /// <summary>
    /// 数值参数编辑面板（纯代码构建，无 XAML）。
    /// 传入一个 <see cref="NumericParamTable{T}"/> 与宿主对象后，自动生成 N 组
    /// 「标签 + 输入框 + −/+ 按钮」，支持单击步进、长按连续调整、手动输入、一键重置。
    /// 与具体业务实体和框架版本解耦（net45 与 net9.0-windows 通用）。
    /// </summary>
    public class NumericParamPanel : UserControl
    {
        #region 对外属性

        /// <summary>面板标题（显示在左侧）</summary>
        public string Title
        {
            get { return _txtTitle.Text; }
            set { _txtTitle.Text = value; }
        }

        /// <summary>参数网格列数（默认 4）</summary>
        public int Columns
        {
            get { return _grid.Columns; }
            set { _grid.Columns = value; }
        }

        /// <summary>长按连续调整的重复间隔（毫秒，默认 50）</summary>
        public int RepeatIntervalMs { get; set; }

        /// <summary>参数值被修改后触发（参数名 + 新值）。宿主可订阅以感知变化。</summary>
        public event Action<string, double> ValueChanged;

        #endregion

        #region 内部字段

        /// <summary>长按重复定时器（所有按钮共用一个实例）</summary>
        private readonly DispatcherTimer _repeatTimer;

        /// <summary>参数网格容器</summary>
        private readonly UniformGrid _grid =
#if NET45_OR_GREATER
            new UniformGrid
#elif NET9_0_OR_GREATER
            new()
#endif
         { Columns = 4, Margin = new Thickness(0, 0, 10, 0) };

    /// <summary>标题文本</summary>
    private readonly TextBlock _txtTitle;

        /// <summary>重置按钮</summary>
        private readonly Button _btnReset;

        /// <summary>参数名 → 输入框 映射</summary>
        private readonly Dictionary<string, TextBox> _boxes =
#if NET45_OR_GREATER
            new Dictionary<string, TextBox>();
#elif NET9_0_OR_GREATER
            [];
#endif

        /// <summary>参数名 → 读取闭包</summary>
        private readonly Dictionary<string, Func<double>> _getters = 
#if NET45_OR_GREATER
            new Dictionary<string, Func<double>>();
#elif NET9_0_OR_GREATER
            [];
#endif

        /// <summary>参数名 → 写入闭包</summary>
        private readonly Dictionary<string, Action<double>> _setters = 
#if NET45_OR_GREATER
            new Dictionary<string, Action<double>>();
#elif NET9_0_OR_GREATER
            [];
#endif

        /// <summary>参数名 → 步进逻辑闭包（含方向）</summary>
        private readonly Dictionary<string, Action<double>> _steppers = 
#if NET45_OR_GREATER
            new Dictionary<string, Action<double>>();
#elif NET9_0_OR_GREATER
        [];
#endif

        /// <summary>参数名 → 格式化解</summary>
        private readonly Dictionary<string, Func<double, string>> _formatters = 
#if NET45_OR_GREATER
            new Dictionary<string, Func<double, string>>();
#elif NET9_0_OR_GREATER
        [];
#endif

        /// <summary>参数名 → 钳制函数</summary>
        private readonly Dictionary<string, Func<double, double>> _clampers = 
#if NET45_OR_GREATER
            new Dictionary<string, Func<double, double>>();
#elif NET9_0_OR_GREATER
        [];
#endif

        /// <summary>参数名 → 初始值（供重置）</summary>
        private readonly Dictionary<string, double> _initialValues = 
#if NET45_OR_GREATER
            new Dictionary<string, double>();
#elif NET9_0_OR_GREATER
        [];
#endif

        /// <summary>当前长按的参数名 + 方向（null 表示无长按）</summary>
        private string _activeName;
        private double _activeDir;

        #endregion

        #region 构造

        /// <summary>
        /// 构造函数：构建面板骨架（标题 + 重置按钮 + 参数网格 + 长按定时器）。
        /// </summary>
        public NumericParamPanel()
        {
            RepeatIntervalMs = 50;

            // 长按重复定时器
            _repeatTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(RepeatIntervalMs) };
            _repeatTimer.Tick += RepeatTimer_Tick;

            // 外层边框（浅蓝底，与主界面风格一致）
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0xF0, 0xF8, 0xFF)),
                Padding = new Thickness(8, 5, 8, 5),
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0, 1, 0, 1)
            };

            var rootGrid = new Grid();
            rootGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            rootGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            rootGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // 左：标题 + 重置按钮
            var leftPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0)
            };

            _txtTitle = new TextBlock
            {
                Text = "参数",
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33)),
                VerticalAlignment = VerticalAlignment.Center
            };

            _btnReset = new Button
            {
                Content = "重置",
                Width = 42,
                Height = 21,
                FontSize = 10,
                Margin = new Thickness(8, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            _btnReset.Click += BtnReset_Click;

            leftPanel.Children.Add(_txtTitle);
            leftPanel.Children.Add(_btnReset);

            // 右：步长说明
            var hint = new TextBlock
            {
                FontSize = 10,
                Foreground = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x88)),
                VerticalAlignment = VerticalAlignment.Center,
                Text = "步长：角度 0.05° / 偏移 0.005 m"
            };

            Grid.SetColumn(leftPanel, 0);
            Grid.SetColumn(_grid, 1);
            Grid.SetColumn(hint, 2);

            rootGrid.Children.Add(leftPanel);
            rootGrid.Children.Add(_grid);
            rootGrid.Children.Add(hint);

            border.Child = rootGrid;
            Content = border;
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化面板：遍历参数表，为每个参数生成一组控件并回填初始值。
        /// </summary>
        /// <typeparam name="T">宿主对象类型</typeparam>
        /// <param name="table">参数元数据表</param>
        /// <param name="host">宿主对象（参数读写目标）</param>
        /// <param name="initialValues">初始值字典（可选，供重置按钮恢复；缺失时回退到 host 当前值）</param>
        public void Initialize<T>(
            NumericParamTable<T> table,
            T host,
            IReadOnlyDictionary<string, double> initialValues = null)
        {
#if NET45_OR_GREATER
            if (table == null) throw new ArgumentNullException(nameof(table));
#elif NET9_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(table);
#endif
            if (host == null) throw new ArgumentNullException(nameof(host));

            Clear();

            foreach (var p in table)
            {
                // 捕获每个参数的闭包（避免泛型 T 泄漏到非泛型控件路径）
                var param = p;
                //Func<double> getter = () => param.Get(host);
                //Action<double> setter = v => param.Set(host, v);
                //Func<double, string> formatter = v => param.Format(v);
                //Func<double, double> clamper = v => param.Clamp(v);
                //Action<double> stepper = dir =>
                //{
                //    double newVal = param.StepValue(host, dir);
                //    param.Set(host, newVal);
                //};
                double getter() => param.Get(host);
                void setter(double v) => param.Set(host, v);
                string formatter(double v) => param.Format(v);
                double clamper(double v) => param.Clamp(v);
                void stepper(double dir)
                {
                    double newVal = param.StepValue(host, dir);
                    param.Set(host, newVal);
                }

                _getters[param.Name] = getter;
                _setters[param.Name] = setter;
                _formatters[param.Name] = formatter;
                _clampers[param.Name] = clamper;
                _steppers[param.Name] = stepper;

                // 初始值：优先用传入字典，否则用 host 当前值
                double initVal = getter();
                if (initialValues != null && initialValues.TryGetValue(param.Name, out double iv))
                    initVal = iv;
                _initialValues[param.Name] = initVal;

                // 生成一行控件
                _grid.Children.Add(BuildParamRow(param.Name, formatter(initVal)));
            }
        }

        /// <summary>清空面板所有参数（供重复 Initialize 时重置）</summary>
        public void Clear()
        {
            _grid.Children.Clear();
            _boxes.Clear();
            _getters.Clear();
            _setters.Clear();
            _steppers.Clear();
            _formatters.Clear();
            _clampers.Clear();
            _initialValues.Clear();
            _activeName = null;
        }

        /// <summary>
        /// 生成一组参数控件：「标签 + 输入框 + −按钮 + +按钮」。
        /// </summary>
        private StackPanel BuildParamRow(string name, string initText)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(2),
                VerticalAlignment = VerticalAlignment.Center
            };

            // 标签
            var label = new TextBlock
            {
                Text = name + ":",
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(0x55, 0x55, 0x55)),
                VerticalAlignment = VerticalAlignment.Center
            };

            // 输入框
            var box = new TextBox
            {
                Width = 56,
                FontSize = 11,
                Margin = new Thickness(3, 0, 0, 0),
                VerticalContentAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Right,
                Tag = name,
                Text = initText
            };
            box.LostFocus += Box_LostFocus;
            box.KeyDown += Box_KeyDown;

            // − 按钮
            var decBtn = BuildStepButton("−", name + ":-1", 12, new Thickness(2, 0, 0, 0));
            // + 按钮
            var incBtn = BuildStepButton("+", name + ":1", 11, new Thickness(1, 0, 0, 0));

            panel.Children.Add(label);
            panel.Children.Add(box);
            panel.Children.Add(decBtn);
            panel.Children.Add(incBtn);

            _boxes[name] = box;
            return panel;
        }

        /// <summary>
        /// 生成一个步进按钮，绑定单击/按下/松开事件（Tag 格式「参数名:方向」）。
        /// </summary>
        private Button BuildStepButton(string content, string tag, double fontSize, Thickness margin)
        {
            var btn = new Button
            {
                Content = content,
                Width = 20,
                Height = 20,
                FontSize = fontSize,
                Margin = margin,
                Tag = tag
            };
            btn.Click += BtnStep_Click;
            btn.PreviewMouseLeftButtonDown += BtnStep_Pressed;
            btn.PreviewMouseLeftButtonUp += BtnStep_Released;
            return btn;
        }

        #endregion

        #region 步进逻辑

        /// <summary>应用一次步进（写入宿主 + 刷新输入框 + 触发事件）</summary>
        private void ApplyStep(string name, double dir)
        {
            if (name == null || !_steppers.TryGetValue(name, out var stepper)) return;

            stepper(dir);
            RefreshBox(name);
            RaiseValueChanged(name);
        }

        /// <summary>刷新指定参数的输入框显示</summary>
        private void RefreshBox(string name)
        {
            if (_boxes.TryGetValue(name, out var box)
                && _getters.TryGetValue(name, out var getter)
                && _formatters.TryGetValue(name, out var formatter))
            {
                box.Text = formatter(getter());
            }
        }

        private void RaiseValueChanged(string name)
        {
            if (_getters.TryGetValue(name, out var getter))
            {
                var handler = ValueChanged;
                if (handler != null) handler(name, getter());
            }
        }

        #endregion

        #region 事件处理器

        /// <summary>± 按钮单击：步进一次</summary>
        private void BtnStep_Click(object sender, RoutedEventArgs e)
        {
            var tag = (sender as Button)?.Tag as string;
            if (tag == null) return;
            string name = ParseName(tag);
            double dir = ParseDirection(tag);
            ApplyStep(name, dir);
        }

        /// <summary>± 按钮按下：立即步进一次 + 启动长按定时器</summary>
        private void BtnStep_Pressed(object sender, MouseButtonEventArgs e)
        {
            var tag = (sender as Button)?.Tag as string;
            if (tag == null) return;

            _activeName = ParseName(tag);
            _activeDir = ParseDirection(tag);
            ApplyStep(_activeName, _activeDir);   // 立即响应一次
            _repeatTimer.Start();
        }

        /// <summary>± 按钮松开：停止长按定时器</summary>
        private void BtnStep_Released(object sender, MouseButtonEventArgs e)
        {
            _repeatTimer.Stop();
            _activeName = null;
        }

        /// <summary>长按定时器 Tick：持续步进</summary>
        private void RepeatTimer_Tick(object sender, EventArgs e)
        {
            if (_activeName != null)
                ApplyStep(_activeName, _activeDir);
        }

        /// <summary>输入框失焦：提交</summary>
        private void Box_LostFocus(object sender, RoutedEventArgs e)
        {
            CommitBox(sender as TextBox);
        }

        /// <summary>输入框回车：提交</summary>
        private void Box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CommitBox(sender as TextBox);
                e.Handled = true;
            }
        }

        /// <summary>提交输入框：解析 → 钳制 → 写回 → 刷新</summary>
        private void CommitBox(TextBox box)
        {
            if (box == null) return;
            string name = box.Tag as string;
            if (name == null) return;

            if (!_setters.TryGetValue(name, out var setter)
                || !_clampers.TryGetValue(name, out var clamper)
                || !_formatters.TryGetValue(name, out var formatter))
                return;

            if (!double.TryParse(box.Text, out double val))
            {
                // 解析失败：恢复原值
                RefreshBox(name);
                return;
            }

            double clamped = clamper(val);
            setter(clamped);
            box.Text = formatter(clamped);
            RaiseValueChanged(name);
        }

        /// <summary>重置按钮：恢复所有参数到初始值</summary>
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            foreach (var kv in _initialValues)
            {
                string name = kv.Key;
                if (_setters.TryGetValue(name, out var setter))
                {
                    setter(kv.Value);
                    RefreshBox(name);
                    RaiseValueChanged(name);
                }
            }
        }

        #endregion

        #region Tag 解析辅助

        /// <summary>从 Tag 中提取参数名（取冒号前部分）</summary>
        private static string ParseName(string tag)
        {
            int idx = tag.IndexOf(':');
            return idx >= 0 ? tag.Substring(0, idx) : tag;
        }

        /// <summary>从 Tag 中解析方向（冒号后 "1" 返回 +1，否则 -1）</summary>
        private static double ParseDirection(string tag)
        {
            int idx = tag.IndexOf(':');
            if (idx < 0 || idx + 1 >= tag.Length) return 1.0;
            string dir = tag.Substring(idx + 1);
            if (double.TryParse(dir, out double d))
                return d > 0 ? 1.0 : -1.0;
            return -1.0;
        }

        #endregion
    }
}
