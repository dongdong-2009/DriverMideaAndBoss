using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

using System.Drawing.Drawing2D;
using System.Drawing;
namespace All.Control.Metro
{
    public partial class CheckBox : System.Windows.Forms.Control
    {
        string checkText = "";
        /// <summary>
        /// 选中文本
        /// </summary>
        [Description("选中文本")]
        [Category("Shuai")]
        public string CheckText
        {
            get { return checkText; }
            set { checkText = value; this.Invalidate(); }
        }
        string unCheckText = "";
        /// <summary>
        /// 未选中文本
        /// </summary>
        [Description("未选中文本")]
        [Category("Shuai")]
        public string UnCheckText
        {
            get { return unCheckText; }
            set { unCheckText = value; this.Invalidate(); }
        }
        Color unCheckColor = Color.DimGray;
        /// <summary>
        /// 未选中颜色
        /// </summary>
        [Description("未选中颜色")]
        [Category("Shuai")]
        public Color UnCheckColor
        {
            get { return unCheckColor; }
            set { unCheckColor = value; this.Invalidate(); }
        }
        Color checkColor = Color.DarkGreen;
        /// <summary>
        /// 选中颜色
        /// </summary>
        [Description("选中颜色")]
        [Category("Shuai")]
        public Color CheckColor
        {
            get { return checkColor; }
            set { checkColor = value; this.Invalidate(); }
        }
        bool check = true;
        /// <summary>
        /// 是否选中
        /// </summary>
        [Category("Shuai")]
        [Description("是否选中")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DefaultValue(true)]
        public bool Checked
        {
            get { return check; }
            set { check = value; this.Invalidate(); }
        }
        StringFormat sf = new StringFormat();
        public CheckBox()
        {
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            InitializeComponent();
        }
        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }
        private void CheckBox_Load(object sender, EventArgs e)
        {
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            //e.Graphics.DrawRectangle(new Pen(unCheckColor, 2), new Rectangle(1, 1, Width - 2, Height - 2));
            e.Graphics.DrawRectangle(new Pen(Color.White, 3), new Rectangle(1, 1, Width - 3, Height - 3));
            if (check)
            {
                e.Graphics.FillRectangle(new SolidBrush(checkColor), new Rectangle(3, 3, Width - 6, Height - 6));
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(32, 32, 32)), new Rectangle(Width - Height * 2 / 3 - 2, 1, Height * 2 / 3 - 1, Height - 3));
                e.Graphics.DrawString(checkText, Font, new SolidBrush(ForeColor), new RectangleF(4, 4, Width - 4 - Height * 2 / 3, Height-4), sf);
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(unCheckColor), new Rectangle(3, 3, Width - 6, Height - 6));
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(32, 32, 32)), new Rectangle(1, 1, Height * 2 / 3-1, Height-3));
                e.Graphics.DrawString(unCheckText, Font, new SolidBrush(ForeColor), new RectangleF(Height * 2 / 3, 4, Width - 4 - Height * 2 / 3, Height -4), sf);
            }
            base.OnPaint(e);
        }
        protected override void OnClick(EventArgs e)
        {
            if (this.Enabled)
            {
                Checked = !check;
            }
            base.OnClick(e);
        }
    }
}
