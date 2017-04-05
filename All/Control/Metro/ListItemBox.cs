using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
namespace All.Control.Metro
{
    public class ListItemBox:System.Windows.Forms.Control
    {
        Color mouseInColor = Color.Red;
        /// <summary>
        /// 鼠标进入时颜色
        /// </summary>
        [Description("鼠标进入时颜色")]
        [Category("Shuai")]
        public Color MouseInColor
        {
            get { return mouseInColor; }
            set { mouseInColor = value; }
        }
        Image icon = null;
        /// <summary>
        /// Item图标
        /// </summary>
        [Description("Item图标")]
        [Category("Shuai")]
        public Image Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                this.Invalidate();
            }
        }
        string title = "ItemBox";
        /// <summary>
        /// Item标题
        /// </summary>
        [Description("Item标题")]
        [Category("Shuai")]
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                this.Invalidate();
            }
        }
        Font titleFont = new Font("Segoe UI", 16, FontStyle.Bold);
        /// <summary>
        /// Item标题字体
        /// </summary>
        [Description("Item标题字体")]
        [Category("Shuai")]
        public Font TitleFont
        {
            get { return titleFont; }
            set
            {
                titleFont = value;
                this.Invalidate();
            }
        }

        bool isGetFocus = false;
        Bitmap backImage;
        StringFormat sf = new StringFormat();
        public ListItemBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();

            this.BackColor = Color.White;
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Near;
        }
        protected override void OnCreateControl()
        {
            this.title = this.Name;
            base.OnCreateControl();
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            backImage = new Bitmap(this.Width, this.Height);
            base.OnSizeChanged(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Draw();
            e.Graphics.DrawImageUnscaled(backImage, 0, 0);
            base.OnPaint(e);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            isGetFocus = true;
            this.Invalidate();
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            isGetFocus = false;
            this.Invalidate();
            base.OnMouseLeave(e);
        }
        private void Draw()
        {
            int LineBold = 4;

            Rectangle tmpRect = Rectangle.Empty;
            if (backImage == null)
            {
                backImage = new Bitmap(this.Width, this.Height);
            }
            using (Graphics g = Graphics.FromImage(backImage))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.Clear(BackColor);

                if (icon != null)//画图票
                {
                    if (Height < LineBold * 4 || Width < LineBold * 4)
                    {
                        return;
                    }
                    tmpRect = new Rectangle(LineBold * 3, LineBold * 2, Height - LineBold * 4, Height - LineBold * 4);
                    g.DrawImage(icon, tmpRect, new Rectangle(0, 0, icon.Width, icon.Height), GraphicsUnit.Pixel);
                }
                else
                {
                    if (Height < LineBold * 4 || Width < LineBold * 4)
                    {
                        return;
                    }
                    tmpRect = new Rectangle(LineBold * 3, LineBold * 2, Height - LineBold * 4, Height - LineBold * 4);
                    g.DrawImage(All.Properties.Resources.News, tmpRect, new Rectangle(0, 0, All.Properties.Resources.News.Width, All.Properties.Resources.News.Height), GraphicsUnit.Pixel);
                }
                if (Width < Height)
                {
                    return;
                }
                //画文字
                tmpRect = new Rectangle(Height, 2 * LineBold,
                    Width - Height, Height - 4 * LineBold);
                g.DrawString(title, titleFont, new SolidBrush(Color.Black), tmpRect, sf);

                //画有焦点时的框
                if (isGetFocus )
                {
                    tmpRect = new Rectangle(0,LineBold*2,LineBold, Height-4*LineBold);
                    g.DrawRectangle(new Pen(mouseInColor, LineBold), tmpRect);
                }
            }
        }
    }
}
