using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Media;
namespace All.Window
{
    public partial class MessageBox : Form
    {
        private Color _defaultColor = Color.FromArgb(57, 179, 215);
        private Color _errorColor = Color.FromArgb(210, 50, 45);
        private Color _warningColor = Color.FromArgb(237, 156, 40);
        private Color _success = Color.FromArgb(0, 170, 173);
        private Color _question = Color.FromArgb(71, 164, 71);

        public MessageBox()
        {
            InitializeComponent();

            this.metroButton1.Cursor = Cursors.Hand;
            this.metroButton2.Cursor = Cursors.Hand;
            this.metroButton3.Cursor = Cursors.Hand;
        }

        public void SetValue()
        {
            lblTitle.Text = Title;
            lblMessage.Text = Message;


            switch (messageBoxIcon)
            {
                case MessageBoxIcon.Error:
                    SystemSounds.Hand.Play(); 
                    tlpBody.BackColor = _errorColor;
                    break;
                case MessageBoxIcon.Exclamation:
                    tlpBody.BackColor = _warningColor;
                    SystemSounds.Exclamation.Play(); 
                    break;
                case MessageBoxIcon.Question:
                    SystemSounds.Beep.Play();
                    tlpBody.BackColor = _question;
                    break;
                default:
                    SystemSounds.Asterisk.Play();
                    tlpBody.BackColor = _defaultColor;
                    break;
            }
            switch (messageBoxButtons)
            {
                case System.Windows.Forms.MessageBoxButtons.OK:
                    metroButton1.Location = metroButton3.Location;
                    metroButton2.Visible = false;
                    metroButton3.Visible = false;
                    metroButton1.Tag = DialogResult.OK;
                    metroButton1.Text = "确定";
                    break;
                case System.Windows.Forms.MessageBoxButtons.AbortRetryIgnore:
                    metroButton1.Tag = DialogResult.Abort;
                    metroButton2.Tag = DialogResult.Retry;
                    metroButton3.Tag = DialogResult.Ignore;
                    metroButton1.Text = "取消";
                    metroButton2.Text = "重试";
                    metroButton3.Text = "忽略";
                    break;
                case System.Windows.Forms.MessageBoxButtons.OKCancel:
                    metroButton1.Tag = DialogResult.OK;
                    metroButton2.Tag = DialogResult.Cancel;
                    metroButton3.Visible = false;
                    metroButton1.Location = metroButton2.Location;
                    metroButton2.Location = metroButton3.Location;
                    metroButton1.Text = "确定";
                    metroButton2.Text = "取消";
                    break;
                case System.Windows.Forms.MessageBoxButtons.RetryCancel:
                    metroButton1.Tag = DialogResult.Retry;
                    metroButton2.Tag = DialogResult.Cancel;
                    metroButton3.Visible = false;
                    metroButton1.Location = metroButton2.Location;
                    metroButton2.Location = metroButton3.Location;
                    metroButton1.Text = "重试";
                    metroButton2.Text = "取消";
                    break;
                case System.Windows.Forms.MessageBoxButtons.YesNo:
                    metroButton1.Tag = DialogResult.Yes;
                    metroButton2.Tag = DialogResult.No;
                    metroButton3.Visible = false;
                    metroButton1.Location = metroButton2.Location;
                    metroButton2.Location = metroButton3.Location;
                    metroButton1.Text = "是";
                    metroButton2.Text = "否";
                    break;
                case System.Windows.Forms.MessageBoxButtons.YesNoCancel:
                    metroButton1.Tag = DialogResult.Yes;
                    metroButton2.Tag = DialogResult.No;
                    metroButton3.Tag = DialogResult.Cancel;
                    metroButton1.Text = "是";
                    metroButton2.Text = "否";
                    metroButton3.Text = "取消";
                    break;
            }
            metroButton1.Click += button_Click;
            metroButton2.Click += button_Click;
            metroButton3.Click += button_Click;
        }

        private void button_Click(object sender, EventArgs e)
        {
            All.Control.Metro.PicButton btn = (All.Control.Metro.PicButton)sender;
            this.DialogResult = (DialogResult)btn.Tag;
            this.Close();
        }
        string title = "";

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        string message = "";

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        MessageBoxButtons messageBoxButtons = MessageBoxButtons.OK;

        public MessageBoxButtons MessageBoxButtons
        {
            get { return messageBoxButtons; }
            set { messageBoxButtons = value; }
        }
        MessageBoxIcon messageBoxIcon = MessageBoxIcon.Information;

        public MessageBoxIcon MessageBoxIcon
        {
            get { return messageBoxIcon; }
            set { messageBoxIcon = value; }
        }
        MessageBoxDefaultButton messageBoxDefaultButton = MessageBoxDefaultButton.Button1;

        public MessageBoxDefaultButton MessageBoxDefaultButton
        {
            get { return messageBoxDefaultButton; }
            set { messageBoxDefaultButton = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd">窗体</param>
        /// <param name="message">显示信息</param>
        /// <returns></returns>
        public static DialogResult Show(IWin32Window hwnd, string message)
        {
            return Show(hwnd, message, "");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd">窗体</param>
        /// <param name="message">显示信息</param>
        /// <param name="title">显示标题</param>
        /// <returns></returns>
        public static DialogResult Show(IWin32Window hwnd, string message, string title)
        {
            return Show(hwnd, message, title, MessageBoxButtons.OK);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd">窗体</param>
        /// <param name="message">显示信息</param>
        /// <param name="title">显示标题</param>
        /// <param name="button">按钮</param>
        /// <returns></returns>
        public static DialogResult Show(IWin32Window hwnd, string message, string title, MessageBoxButtons button)
        {
            return Show(hwnd, message, title, button, MessageBoxIcon.Information);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd">窗体</param>
        /// <param name="message">显示信息</param>
        /// <param name="title">显示标题</param>
        /// <param name="button">按钮</param>
        /// <param name="icon">图标</param>
        /// <returns></returns>
        public static DialogResult Show(IWin32Window hwnd, string message, string title, MessageBoxButtons button, MessageBoxIcon icon)
        {
            return Show(hwnd, message, title, button, icon, MessageBoxDefaultButton.Button1);
        }
        public static DialogResult Show(IWin32Window hwnd, string message, string title, MessageBoxButtons button, MessageBoxIcon icon, MessageBoxDefaultButton defautButton)
        {
            DialogResult result = DialogResult.None;
            using (MetroMessageBox mmbox = new MetroMessageBox())
            {
                Form parent = (Form)hwnd;
                mmbox.Title = title;
                mmbox.Message = message;
                mmbox.MessageBoxButtons = button;
                mmbox.MessageBoxIcon = icon;
                mmbox.MessageBoxDefaultButton = defautButton;
                mmbox.Width = parent.Size.Width;
                mmbox.SetValue();
                mmbox.Location = new Point(parent.Location.X, parent.Location.Y + parent.Height / 2 - mmbox.Height / 2);
                result = mmbox.ShowDialog();
                mmbox.BringToFront();
            }
            return result;
        }

        private void MetroMessageBox_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        private void MetroMessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                switch (messageBoxDefaultButton)
                {
                    case System.Windows.Forms.MessageBoxDefaultButton.Button1:
                        button_Click(metroButton1, new EventArgs());
                        break;
                    case System.Windows.Forms.MessageBoxDefaultButton.Button2:
                        button_Click(metroButton2, new EventArgs());
                        break;
                    case System.Windows.Forms.MessageBoxDefaultButton.Button3:
                        button_Click(metroButton3, new EventArgs());
                        break;
                }
            }
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {

        }
    }
}
