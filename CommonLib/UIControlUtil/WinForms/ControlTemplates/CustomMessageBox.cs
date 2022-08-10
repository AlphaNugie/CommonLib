using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonLib.UIControlUtil.ControlTemplates
{
    public partial class CustomMessageBox : Form
    {
        private const int WIDTH = 75, HEIGHT = 25; //按钮默认宽度、高度

        public CustomMessageBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Here I am overriding Paint method of form object
        /// and set it's background color as gradient. Here I am
        /// using LinearGradientBrush class object to make gradient
        /// color which comes in System.Drawing.Drawing2D namespace.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = ClientRectangle;
            LinearGradientBrush brush = new LinearGradientBrush(rect, Color.SkyBlue, Color.AliceBlue, 60);
            e.Graphics.FillRectangle(brush, rect);
            base.OnPaint(e);
        }

        /// <summary>
        /// setMessage method is used to display message
        /// on form and it's height adjust automatically.
        /// I am displaying message in a Label control.
        /// </summary>
        /// <param name="messageText">Message which needs to be displayed to user.</param>
        public void SetMessage(string messageText)
        {
            int number = Math.Abs(messageText.Length / 30);
            if (number != 0)
                lblMessageText.Height = number * HEIGHT;
            lblMessageText.Text = messageText;
        }

        #region 将按钮添加到panel
        ///// <summary>
        ///// This method is used to add button on message box.
        ///// </summary>
        ///// <param name="MessageButton">MessageButton is type of enumMessageButton
        ///// through which I get type of button which needs to be displayed.</param>
        //public void AddButton(CustomMessageBoxButtons MessageButton)
        //{
        //    switch (MessageButton)
        //    {
        //        //If type of enumButton is OK then we add OK button only.
        //        case CustomMessageBoxButtons.OK:
        //            {
        //                Button btnOk = new Button
        //                {
        //                    //Text = "OK",  //Here we set text of Button.
        //                    Text = "OK",  //Here we set text of Button.
        //                    DialogResult = DialogResult.OK,  //Set DialogResult property of button.
        //                    FlatStyle = FlatStyle.Popup  //Set flat appearence of button.
        //                };  //Create object of Button.
        //                btnOk.FlatAppearance.BorderSize = 0;
        //                btnOk.SetBounds(pnlShowMessage.ClientSize.Width - 80, 5, 75, 25);  // Set bounds of button.
        //                pnlShowMessage.Controls.Add(btnOk);  //Finally Add button control on panel.
        //            }
        //            break;
        //        case CustomMessageBoxButtons.OKCancel:
        //            {
        //                Button btnOk = new Button
        //                {
        //                    //Text = "OK",
        //                    Text = "OK",
        //                    DialogResult = DialogResult.OK,
        //                    FlatStyle = FlatStyle.Popup
        //                };
        //                btnOk.FlatAppearance.BorderSize = 0;
        //                btnOk.SetBounds(pnlShowMessage.ClientSize.Width - 70, 5, 65, 25);
        //                pnlShowMessage.Controls.Add(btnOk);

        //                Button btnCancel = new Button
        //                {
        //                    //Text = "Cancel",
        //                    Text = "Cancel",
        //                    DialogResult = DialogResult.Cancel,
        //                    FlatStyle = FlatStyle.Popup
        //                };
        //                btnCancel.FlatAppearance.BorderSize = 0;
        //                btnCancel.SetBounds(pnlShowMessage.ClientSize.Width - (btnOk.ClientSize.Width + 5 + 80), 5, 75, 25);
        //                pnlShowMessage.Controls.Add(btnCancel);

        //            }
        //            break;
        //        case CustomMessageBoxButtons.YesNo:
        //            {
        //                Button btnNo = new Button
        //                {
        //                    //Text = "No",
        //                    Text = "No",
        //                    DialogResult = DialogResult.No,
        //                    FlatStyle = FlatStyle.Popup
        //                };
        //                btnNo.FlatAppearance.BorderSize = 0;
        //                btnNo.SetBounds((pnlShowMessage.ClientSize.Width - 70), 5, 65, 25);
        //                pnlShowMessage.Controls.Add(btnNo);

        //                Button btnYes = new Button
        //                {
        //                    //Text = "Yes",
        //                    Text = "Yes",
        //                    DialogResult = DialogResult.Yes,
        //                    FlatStyle = FlatStyle.Popup
        //                };
        //                btnYes.FlatAppearance.BorderSize = 0;
        //                btnYes.SetBounds((pnlShowMessage.ClientSize.Width - (btnNo.ClientSize.Width + 5 + 80)), 5, 75, 25);
        //                pnlShowMessage.Controls.Add(btnYes);
        //            }
        //            break;
        //        case CustomMessageBoxButtons.YesNoCancel:
        //            {
        //                Button btnCancel = new Button
        //                {
        //                    //Text = "Cancel",
        //                    Text = "Cancel",
        //                    DialogResult = DialogResult.Cancel,
        //                    FlatStyle = FlatStyle.Popup
        //                };
        //                btnCancel.FlatAppearance.BorderSize = 0;
        //                btnCancel.SetBounds((pnlShowMessage.ClientSize.Width - 70), 5, 65, 25);
        //                pnlShowMessage.Controls.Add(btnCancel);

        //                Button btnNo = new Button
        //                {
        //                    //Text = "No",
        //                    Text = "No",
        //                    DialogResult = DialogResult.No,
        //                    FlatStyle = FlatStyle.Popup
        //                };
        //                btnNo.FlatAppearance.BorderSize = 0;
        //                btnNo.SetBounds((pnlShowMessage.ClientSize.Width - (btnCancel.ClientSize.Width + 5 + 80)), 5, 75, 25);
        //                pnlShowMessage.Controls.Add(btnNo);

        //                Button btnYes = new Button
        //                {
        //                    Text = "Yes",
        //                    DialogResult = DialogResult.No,
        //                    FlatStyle = FlatStyle.Popup
        //                };
        //                btnYes.FlatAppearance.BorderSize = 0;
        //                btnYes.SetBounds((pnlShowMessage.ClientSize.Width - (btnCancel.ClientSize.Width + btnNo.ClientSize.Width + 10 + 80)), 5, 75, 25);
        //                pnlShowMessage.Controls.Add(btnYes);
        //            }
        //            break;
        //    }
        //}

        /// <summary>
        /// This method is used to add button on message box.
        /// </summary>
        /// <param name="MessageButton">MessageButton is type of enumMessageButton
        /// through which I get type of button which needs to be displayed.</param>
        public void AddButton(CustomMessageBoxButtons MessageButton)
        {
            switch (MessageButton)
            {
                case CustomMessageBoxButtons.OK:
                    AddButtonsToPanel(new Button[] { CreateOKButton() });
                    break;
                case CustomMessageBoxButtons.OKCancel:
                    AddButtonsToPanel(new Button[] { CreateOKButton(), CreateCancelButton() });
                    break;
                case CustomMessageBoxButtons.YesNo:
                    AddButtonsToPanel(new Button[] { CreateYesButton(), CreateNoButton() });
                    break;
                case CustomMessageBoxButtons.YesNoCancel:
                    AddButtonsToPanel(new Button[] { CreateYesButton(), CreateNoButton(), CreateCancelButton() });
                    break;
            }
        }

        /// <summary>
        /// 清空panel中已有的控件并按从左到右的显示顺序逐一添加列表中按钮
        /// </summary>
        /// <param name="buttons"></param>
        public void AddButtonsToPanel(IEnumerable<Button> buttons)
        {
            if (buttons == null || buttons.Count() == 0)
                return;
            //首先清空控件，再逐一添加按钮，添加前将列表反向以符合从左到右的直觉
            pnlShowMessage.Controls.Clear();
            buttons = buttons.Reverse();
            foreach (Button button in buttons)
                AddButtonToPanel(button);
        }

        /// <summary>
        /// 将按钮添加到panel中（靠右排列，后添加的按钮靠左）
        /// </summary>
        /// <param name="button">待添加的按钮</param>
        public void AddButtonToPanel(Button button)
        {
            if (button == null)
                return;

            //最后呈现的总宽度=所有panel中已存在控件的总宽度+当前按钮宽度+(已存在控件数量+1)*5
            int totalWidth = pnlShowMessage.Controls.Cast<Control>().Select(c => c.ClientSize.Width).Sum() + button.Width + (pnlShowMessage.Controls.Count + 1) * 5;
            //设置按钮的边界并将按钮添加到panel（靠右排列，后添加的按钮靠左）
            button.SetBounds(pnlShowMessage.ClientSize.Width - totalWidth, 5, button.Width, button.Height);
            pnlShowMessage.Controls.Add(button);
        }
        #endregion

        #region 生成按钮实例
        /// <summary>
        /// 初始化按钮，指定显示的文本、返回的结果，宽高默认为75x25
        /// </summary>
        /// <param name="text">按钮文本</param>
        /// <param name="dialogResult">按钮的返回结果</param>
        /// <returns></returns>
        public static Button CreateButton(string text, DialogResult dialogResult)
        {
            return CreateButton(text, dialogResult, WIDTH, HEIGHT);
        }

        /// <summary>
        /// 初始化按钮，指定显示的文本、返回的结果以及宽度、高度
        /// </summary>
        /// <param name="text">按钮文本</param>
        /// <param name="dialogResult">按钮的返回结果</param>
        /// <param name="width">按钮宽度</param>
        /// <param name="height">按钮高度</param>
        /// <returns></returns>
        public static Button CreateButton(string text, DialogResult dialogResult, int width, int height)
        {
            //Create object of Button.
            Button button = new Button
            {
                Text = text,  //Here we set text of Button.
                DialogResult = dialogResult,  //Set DialogResult property of button.
                FlatStyle = FlatStyle.Popup,  //Set flat appearence of button.
                Size = new Size(width, height), //Set size of button
            };
            button.FlatAppearance.BorderSize = 0;
            ////最后呈现的总宽度=所有panel中已存在控件的总宽度+当前按钮宽度+(已存在控件数量+1)*5
            //int totalWidth = pnlShowMessage.Controls.Cast<Control>().Select(con => con.ClientSize.Width).Sum() + width + (pnlShowMessage.Controls.Count + 1) * 5;
            //button.SetBounds(pnlShowMessage.ClientSize.Width - totalWidth, 5, width, height);
            return button;
        }

        /// <summary>
        /// 初始化“确认”按钮
        /// </summary>
        /// <returns></returns>
        public static Button CreateOKButton()
        {
            return CreateOKButton(WIDTH, HEIGHT);
        }

        /// <summary>
        /// 初始化“取消”按钮
        /// </summary>
        /// <returns></returns>
        public static Button CreateCancelButton()
        {
            return CreateCancelButton(WIDTH, HEIGHT);
        }

        /// <summary>
        /// 初始化“是”按钮
        /// </summary>
        /// <returns></returns>
        public static Button CreateYesButton()
        {
            return CreateYesButton(WIDTH, HEIGHT);
        }

        /// <summary>
        /// 初始化“否”按钮
        /// </summary>
        /// <returns></returns>
        public static Button CreateNoButton()
        {
            return CreateNoButton(WIDTH, HEIGHT);
        }

        /// <summary>
        /// 初始化“确认”按钮，设定给定的宽高
        /// </summary>
        /// <param name="width">按钮宽度</param>
        /// <param name="height">按钮高度</param>
        /// <returns></returns>
        public static Button CreateOKButton(int width, int height)
        {
            //return CreateButton("OK", DialogResult.OK, width, height);
            return CreateButton("确认", DialogResult.OK, width, height);
        }

        /// <summary>
        /// 初始化“取消”按钮，设定给定的宽高
        /// </summary>
        /// <param name="width">按钮宽度</param>
        /// <param name="height">按钮高度</param>
        /// <returns></returns>
        public static Button CreateCancelButton(int width, int height)
        {
            //return CreateButton("Cancel", DialogResult.Cancel, width, height);
            return CreateButton("取消", DialogResult.Cancel, width, height);
        }

        /// <summary>
        /// 初始化“是”按钮，设定给定的宽高
        /// </summary>
        /// <param name="width">按钮宽度</param>
        /// <param name="height">按钮高度</param>
        /// <returns></returns>
        public static Button CreateYesButton(int width, int height)
        {
            //return CreateButton("Yes", DialogResult.Yes, width, height);
            return CreateButton("是", DialogResult.Yes, width, height);
        }

        /// <summary>
        /// 初始化“否”按钮，设定给定的宽高
        /// </summary>
        /// <param name="width">按钮宽度</param>
        /// <param name="height">按钮高度</param>
        /// <returns></returns>
        public static Button CreateNoButton(int width, int height)
        {
            //return CreateButton("No", DialogResult.No, width, height);
            return CreateButton("否", DialogResult.No, width, height);
        }
        #endregion

        /// <summary>
        /// We can use this method to add image on message box.
        /// I had taken all images in ImageList control so that
        /// I can eaily add images. Image is displayed in 
        /// PictureBox control.
        /// </summary>
        /// <param name="messageIcon">Type of image to be displayed.</param>
        private void AddIconImage(CustomMessageBoxIcon messageIcon)
        {
            string iconTypeName = messageIcon.ToString();
            if (imageList1.Images.ContainsKey(iconTypeName))
                pictureBox1.Image = imageList1.Images[iconTypeName];
            //switch (messageIcon)
            //{
            //    case CustomMessageBoxIcon.Error:
            //        pictureBox1.Image = imageList1.Images["Error"];  //Error is key name in imagelist control which uniqly identified images in ImageList control.
            //        break;
            //    case CustomMessageBoxIcon.Information:
            //        pictureBox1.Image = imageList1.Images["Information"];
            //        break;
            //    case CustomMessageBoxIcon.Question:
            //        pictureBox1.Image = imageList1.Images["Question"];
            //        break;
            //    case CustomMessageBoxIcon.Warning:
            //        pictureBox1.Image = imageList1.Images["Warning"];
            //        break;
            //}
        }

        #region Overloaded Show message to display message box.

        /// <summary>
        /// Show method is overloaded which is used to display message
        /// and this is static method so that we don't need to create 
        /// object of this class to call this method.
        /// </summary>
        /// <param name="messageText"></param>
        public static DialogResult Show(string messageText)
        {
            return Show(messageText, string.Empty, CustomMessageBoxButtons.OK, CustomMessageBoxIcon.Information);
            //FormCustomMessageBox frmMessage = new FormCustomMessageBox();
            //frmMessage.SetMessage(messageText);
            //frmMessage.AddIconImage(CustomMessageBoxIcon.Information);
            //frmMessage.AddButton(CustomMessageBoxButton.OK);
            //frmMessage.ShowDialog();
        }

        public static DialogResult Show(string messageText, string messageTitle)
        {
            return Show(messageText, messageTitle, CustomMessageBoxButtons.OK, CustomMessageBoxIcon.Information);
            //FormCustomMessageBox frmMessage = new FormCustomMessageBox();
            //frmMessage.Text = messageTitle;
            //frmMessage.SetMessage(messageText);
            //frmMessage.AddIconImage(CustomMessageBoxIcon.Information);
            //frmMessage.AddButton(CustomMessageBoxButton.OK);
            //frmMessage.ShowDialog();
        }

        public static DialogResult Show(string messageText, string messageTitle, CustomMessageBoxButtons messageButton, CustomMessageBoxIcon messageIcon)
        {
            return Show(messageText, messageTitle, messageButton, messageIcon, null);
        }

        public static DialogResult Show(string messageText, string messageTitle, CustomMessageBoxButtons messageButton, CustomMessageBoxIcon messageIcon, Font font)
        {
            CustomMessageBox frmMessage = new CustomMessageBox { Text = messageTitle };
            if (font != null)
                frmMessage.Font = font;
            frmMessage.SetMessage(messageText);
            frmMessage.AddIconImage(messageIcon);
            frmMessage.AddButton(messageButton);
            return frmMessage.ShowDialog();
        }
        #endregion

        /// <summary>
        /// 当窗体字体改变后，修改信息label的字体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomMessageBox_FontChanged(object sender, EventArgs e)
        {
            lblMessageText.Font = Font;
        }
    }

    #region constant defiend in form of enumration which is used in showMessage class.
    public enum CustomMessageBoxIcon
    {
        Error,
        Warning,
        Information,
        Question,
    }

    public enum CustomMessageBoxButtons
    {
        OK,
        YesNo,
        YesNoCancel,
        OKCancel
    }

    #endregion
}
