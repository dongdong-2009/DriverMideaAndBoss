using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
namespace All.Window
{
    public partial class KeyBoard : BaseWindow
    {
        int intMouse = 0;
        const string CapsValue = @"/.,MNBVCXZ';LKJHGFDSA\][POIUYTREWQ=-0987654321`";
        const string ShiftValue = @"?><MNBVCXZ"":LKJHGFDSA|}{POIUYTREWQ+_)(*&^%$#@!~";
        const string AllValue = @"?><mnbvcxz"":lkjhgfdsa|}{poiuytrewq+_)(*&^%$#@!~";
        const string NoValue = @"/.,mnbvcxz';lkjhgfdsa\][poiuytrewq=-0987654321`";
        string value = "";
        /// <summary>
        /// 返回值
        /// </summary>
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public KeyBoard(string value)
        {
            this.Value = value;
            InitializeComponent();
        }

        private void KeyBoard_Load(object sender, EventArgs e)
        {
            this.Left = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
            this.Top = (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2;
            txtValue.Text = this.Value;
            txtValue.SelectionStart = txtValue.Text.Length;
        }

        private void Button52_Click(object sender, EventArgs e)
        {
            if (Button41.BackColor == Color.Black)
            {
                Button41.BackColor = Color.Cyan;
                Button52.BackColor = Color.Cyan;
            }
            else
            {
                Button41.BackColor = Color.Black;
                Button52.BackColor = Color.Black;
            }
            SetButtonText();
            txtValue.Focus();
        }
        private void Button28_Click(object sender, EventArgs e)
        {
            if (Button28.BackColor == Color.Black)
            {
                Button28.BackColor = Color.Cyan;
            }
            else
            {
                Button28.BackColor = Color.Black;
            }
            SetButtonText();
            txtValue.Focus();
        }
        private void SetButtonText()
        {
            bool capsLock = Button28.BackColor == Color.Cyan;
            bool shift = Button41.BackColor == Color.Cyan;
            string NowValue = "";
            if (capsLock && shift)
            {
                NowValue = AllValue;
            }
            if (capsLock && !shift)
            {
                NowValue = CapsValue;
            }
            if (!capsLock && shift)
            {
                NowValue = ShiftValue;
            }
            if (!capsLock && !shift)
            {
                NowValue = NoValue;
            }
            IEnumerator KeyEnum = this.Controls.GetEnumerator();
            Button nowBtn;
            int i = 0;
            while (KeyEnum.MoveNext())
            {
                if (KeyEnum.Current is Button)
                {
                    nowBtn = (Button)KeyEnum.Current;
                    if (nowBtn.Tag != null && nowBtn.Tag.ToString() == "1")
                    {
                        nowBtn.Text = NowValue.Substring(i++, 1);
                    }
                }
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string temp = txtValue.Text;
            intMouse = txtValue.SelectionStart;
            Button nowBtn = (Button)sender;
           
            if (txtValue.SelectionLength > 0)
            {
                txtValue.Text = temp.Substring(0, intMouse) + nowBtn.Text + temp.Substring(intMouse + txtValue.SelectionLength);
                txtValue.SelectionLength = 0;
                txtValue.SelectionStart = intMouse;
            }
            else
            {
                txtValue.Text = temp.Substring(0, intMouse) + nowBtn.Text + temp.Substring(intMouse);
            }
            txtValue.SelectionStart = intMouse + 1;
            Button41.BackColor = Color.Black;
            Button52.BackColor = Color.Black;
            SetButtonText();
            txtValue.Focus();
        }

        private void Button14_Click(object sender, EventArgs e)
        {
            if (txtValue.Text != "")
            {
                if (txtValue.SelectedText != "")
                {
                    txtValue.SelectedText = "";
                    txtValue.Focus();
                }
                else
                {
                    string temp = txtValue.Text;
                    intMouse = txtValue.SelectionStart;
                    if (intMouse == 0)
                    {
                        txtValue.Focus();
                        return;
                    }
                    if (txtValue.SelectionLength > 0)
                    {
                        txtValue.Text = temp.Substring(0, intMouse) + temp.Substring(intMouse + txtValue.SelectionLength);
                        txtValue.SelectionLength = 0;
                        txtValue.SelectionStart = intMouse;
                    }
                    else
                    {
                        txtValue.Text = temp.Substring(0, intMouse - 1) + temp.Substring(intMouse);
                        txtValue.SelectionStart = intMouse - 1;
                    }
                    txtValue.Focus();
                }
            }
        }

        private void Button53_Click(object sender, EventArgs e)
        {

            string temp = txtValue.Text;
            intMouse = txtValue.SelectionStart;
            if (txtValue.SelectionLength > 0)
            {
                txtValue.Text = temp.Substring(0, intMouse) + " " + temp.Substring(intMouse + txtValue.SelectionLength);
                txtValue.SelectionLength = 0;
                txtValue.SelectionStart = intMouse;
            }
            else
            {
                txtValue.Text = temp.Substring(0, intMouse) + " " + temp.Substring(intMouse);
            }
            txtValue.SelectionStart = intMouse + 1;
            SetButtonText();
            txtValue.Focus();
        }

        private void Button54_Click(object sender, EventArgs e)
        {

            txtValue.Text = "";
        }

        private void Button40_Click(object sender, EventArgs e)
        {
            this.value = txtValue.Text;
            DialogResult = DialogResult.Yes;
        }

        private void Button55_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }

        private void Button56_Click(object sender, EventArgs e)
        {
            this.value = txtValue.Text;
            DialogResult = DialogResult.Yes;
        }

    }
}
