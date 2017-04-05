using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
namespace All.Control.Metro
{
    [DefaultProperty("Icon")]
    [ToolboxBitmap("\\ItemBox.bmp")]
    public class ItemBox : System.Windows.Forms.Control
    {
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
        string value = "this is ItemBox";
        /// <summary>
        /// Item内容
        /// </summary>
        [Description("Item内容")]
        [Category("Shuai")]
        public string Value
        {
            get { return value; }
            set
            {
                this.value = value;
                this.Invalidate();
            }
        }
        bool check = true;
        /// <summary>
        /// Item是否选中
        /// </summary>
        [Description("Item是否选中")]
        [Category("Shuai")]
        public bool Check
        {
            get { return check; }
            set
            {
                check = value;
                this.Invalidate();
            }
        }
        Color checkColor = Color.Red;
        /// <summary>
        /// Item选中颜色
        /// </summary>
        [Description("Item选中颜色")]
        [Category("Shuai")]
        public Color CheckColor
        {
            get { return checkColor; }
            set { checkColor = value; this.Invalidate(); }
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
        Font valueFont = new Font("Segoe UI", 12, FontStyle.Bold);
        /// <summary>
        /// Item小标题字体
        /// </summary>
        [Description("Item小标题字体")]
        [Category("Shuai")]
        public Font ValueFont
        {
            get { return valueFont; }
            set
            {
                valueFont = value;
                this.Invalidate();
            }
        }
        bool canFouce = false;
        /// <summary>
        /// Item能否有焦点
        /// </summary>
        [Description("Item能否有焦点")]
        [Category("Shuai")]
        public bool CanFouce
        {
            get { return canFouce; }
            set { canFouce = value; }
        }
        /// <summary>
        /// 线体长度
        /// </summary>
        public int LineBold = 4;
        bool isGetFocus = false;
        bool isMouseDown = false;

        Bitmap backImage;
        StringFormat sf = new StringFormat();
        public ItemBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();

            this.BackColor = Color.White;
            sf.LineAlignment = StringAlignment.Near;
        }
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
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
        protected override void OnSizeChanged(EventArgs e)
        {
            backImage = new Bitmap(this.Width, this.Height);
            this.Invalidate();
            base.OnSizeChanged(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            isMouseDown = false;
            this.Invalidate();
            base.OnMouseUp(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            isMouseDown = true;
            this.Invalidate();
            base.OnMouseDown(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Draw();
            e.Graphics.DrawImageUnscaled(backImage, 0, 0);
            base.OnPaint(e);
        }
        private void Draw()
        {
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
                    tmpRect = new Rectangle(LineBold * 2, LineBold * 2, Height - LineBold * 4, Height - LineBold * 4);
                    g.DrawImage(icon, tmpRect, new Rectangle(0, 0, icon.Width, icon.Height), GraphicsUnit.Pixel);
                }
                else
                {
                    if (Height < LineBold * 4 || Width < LineBold * 4)
                    {
                        return;
                    }
                    tmpRect = new Rectangle(LineBold * 2, LineBold * 2, Height - LineBold * 4, Height - LineBold * 4);
                    g.DrawImage(All.Properties.Resources.News, tmpRect, new Rectangle(0, 0, All.Properties.Resources.News.Width, All.Properties.Resources.News.Height), GraphicsUnit.Pixel);
                }
                if (Width < Height)
                {
                    return;
                }
                //画文字
                tmpRect = new Rectangle(Height - LineBold, 2 * LineBold + (Height - 4 * LineBold - Class.Num.GetFontHeight(titleFont) - Class.Num.GetFontHeight(valueFont)) / 3,
                    Width - Height + LineBold, Class.Num.GetFontHeight(titleFont));
                g.DrawString(title, titleFont, new SolidBrush(Color.Black), tmpRect, sf);
                tmpRect = new Rectangle(Height - LineBold, Class.Num.GetFontHeight(titleFont) + 2 * LineBold + 2 * (Height - 4 * LineBold - Class.Num.GetFontHeight(titleFont) - Class.Num.GetFontHeight(valueFont)) / 3,
                    Width - Height + LineBold, Class.Num.GetFontHeight(valueFont));
                g.DrawString(value, valueFont, new SolidBrush(Color.Black), tmpRect, sf);

                //画选中的"勾"
                if (check)
                {
                    tmpRect = new Rectangle(LineBold / 2, LineBold / 2, Width - LineBold, Height - LineBold);
                    g.DrawRectangle(new Pen(checkColor, LineBold), tmpRect);
                    GraphicsPath path = new GraphicsPath();
                    Point[] SanJiaoXing = new Point[3];
                    SanJiaoXing[0] = new Point(Width, 0);
                    SanJiaoXing[1] = new Point(Width, 10 * LineBold);
                    SanJiaoXing[2] = new Point(Width -10 * LineBold, 0);
                    path.AddPolygon(SanJiaoXing);
                    g.FillPath(new SolidBrush(checkColor), path);
                    path.Dispose();

                    g.DrawLine(new Pen(Color.White, LineBold), Width - 8 * LineBold / 2 - 0.25f * LineBold, 8 * LineBold / 2 + 0.25f * LineBold, Width - LineBold, LineBold);
                    g.DrawLine(new Pen(Color.White, LineBold), Width - 8 * LineBold * 3 / 4, 8 * LineBold * 1 / 4, Width - 8 * LineBold / 2 + 0.25f * LineBold, 8 * LineBold / 2 + 0.25f * LineBold);
                }
                //画有焦点时的框
                if (isGetFocus && canFouce)
                {
                    tmpRect = new Rectangle(LineBold / 2, LineBold / 2, Width - LineBold, Height - LineBold);
                    g.DrawRectangle(new Pen(Color.Silver, LineBold), tmpRect);
                }
                //画按下时的框
                if (isMouseDown && canFouce)
                {
                    tmpRect = new Rectangle(LineBold / 2, LineBold / 2, Width - LineBold, Height - LineBold);
                    g.DrawRectangle(new Pen(Color.Black, LineBold), tmpRect);
                }
            }
        }
    }
}
