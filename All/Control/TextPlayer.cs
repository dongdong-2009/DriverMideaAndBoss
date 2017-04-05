using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Drawing2D;
namespace All.Control
{
    [DefaultProperty("Value")]
    public partial class TextPlayer : System.Windows.Forms.Control
    {
        string title = "标题";
        /// <summary>
        /// 标题
        /// </summary>
        [Description("标题")]
        [Category("Shuai")]
        public string Title
        {
            get { return title; }
            set { title = value; drawAll = true; Invalidate(); }
        }
        string value = "内容";
        /// <summary>
        /// 内容
        /// </summary>
        [Description("内容")]
        [Category("Shuai")]
        public string Value
        {
            get { return this.value; }
            set { this.value = value; drawAll = true; Invalidate(); }
        }
        string date = string.Format("{0:MM}月{0:dd}日", DateTime.Now);
        /// <summary>
        /// 日期
        /// </summary>
        [Description("日期")]
        [Category("Shuai")]
        public string Date
        {
            get { return date; }
            set { date = value; drawAll = true; Invalidate(); }
        }

        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value; drawAll = true; Invalidate(); 
            }
        }
        Bitmap backImage;
        bool drawAll = false;
        /// <summary>
        /// 标题高度比例
        /// </summary>
        const int TitleHeight = 2;
        /// <summary>
        /// 内容高度比例
        /// </summary>
        const int ValueHeight = 3;
        /// <summary>
        /// 日期高度比例
        /// </summary>
        const int DateHeight = 2;
        public TextPlayer()
        {
            this.ForeColor = Color.Red;
            InitializeComponent();
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            backImage = new Bitmap(this.Width, this.Height);
            drawAll = true;
            base.OnSizeChanged(e);
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            if (backImage == null || drawAll)
            {
                drawAll = false;
                InitFrm();
            }
            e.Graphics.DrawImageUnscaled(backImage, 0, 0);
            base.OnPaint(e);
        }
        private void InitFrm()
        {
            if (backImage == null)
            {
                backImage = new Bitmap(this.Width, this.Height);
            }
            using (StringFormat sf = new StringFormat())
            {
                using (Graphics g = Graphics.FromImage(backImage))
                {
                    g.Clear(BackColor);
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;


                    g.DrawString(title, new Font(this.Font.FontFamily, this.Font.Size*1.2f, FontStyle.Bold), new SolidBrush(ForeColor),
                        new RectangleF(0, 0, Width, Height * TitleHeight / (TitleHeight + ValueHeight + DateHeight)),
                        sf);

                    sf.Alignment = StringAlignment.Near;
                    g.DrawString(value, new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Regular), new SolidBrush(ForeColor),
                        new RectangleF(0, Height * TitleHeight / (TitleHeight + ValueHeight + DateHeight), Width, Height * ValueHeight / (TitleHeight + ValueHeight + DateHeight)),
                        sf);

                    sf.Alignment = StringAlignment.Far;
                    g.DrawString(date, new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Bold), new SolidBrush(ForeColor),
                        new RectangleF(0, Height * (TitleHeight + ValueHeight) / (TitleHeight + ValueHeight + DateHeight), Width*0.9f, Height * DateHeight / (TitleHeight + ValueHeight + DateHeight)),
                        sf);
                }
            }
        }
    }
}
