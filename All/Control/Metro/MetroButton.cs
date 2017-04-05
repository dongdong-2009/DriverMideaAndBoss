using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace All.Control.Metro
{
    [DefaultEvent("Click")]
    public partial class MetroButton : UserControl
    {
        [Description("背景色彩")]
        [Category("Shuai")]
        public Color Back
        {
            get { return this.BackColor; }
            set { this.BackColor = value; }
        }
        [Description("大标题文字")]
        [Category("Shuai")]
        public string Title
        {
            get { return this.lblTitle.Text; }
            set
            {
                this.lblTitle.Text = value;
                ChangeLocation();
            }
        }
        [Description("小标题文字")]
        [Category("Shuai")]
        public string Value
        {
            get { return this.lblUser.Text; }
            set
            {
                this.lblUser.Text = value;
                ChangeLocation();
            }
        }
        [Description("图片")]
        [Category("Shuai")]
        public Image Img
        {
            get { return Pic.Image; }
            set { this.Pic.Image = value; }
        }
        public MetroButton()
        {
            InitializeComponent();
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            ChangeLocation();
            base.OnSizeChanged(e);
        }
        private void ChangeLocation()
        {
            int totleWidth = Pic.Width + Math.Max(lblTitle.Width, lblUser.Width);
            Pic.Left = Width / 2 - totleWidth / 2;
            Pic.Top = Height / 2 - Pic.Height / 2;
            lblTitle.Left = Pic.Left + Pic.Width + 5;
            lblTitle.Top = Pic.Top + Pic.Height / 2 - lblTitle.Height;
            lblUser.Left = Pic.Left + Pic.Width + 5;
            lblUser.Top = Pic.Top + Pic.Height / 2;
        }
        protected override void OnClick(EventArgs e)
        {
            this.Invalidate();
            base.OnClick(e);
        }
        private void Pic_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            using (Graphics g = Graphics.FromHwnd(this.Handle))
            {
                g.DrawRectangle(new Pen(Color.White, 1), new Rectangle(0, 0, Width - 1, Height - 1));
            }
            base.OnMouseEnter(e);
        }
        private void Pic_MouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (!this.RectangleToScreen(this.ClientRectangle).Contains(MousePosition))
            {
                this.Invalidate();
            }
            base.OnMouseLeave(e);
        }
        private void Pic_MouseLeave(object sender, EventArgs e)
        {
            OnMouseLeave(e);
        }
    }
}
