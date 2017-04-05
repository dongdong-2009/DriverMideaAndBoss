using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
namespace All.Control.Metro
{
    public partial class ArrowTitle : System.Windows.Forms.Control
    {
        /// <summary>
        /// 标头式样
        /// </summary>
        public enum IconStyleList
        {
            三角形,
            箭头,
            圆形
        }
        bool tail = false;
        /// <summary>
        /// 是否有尾巴
        /// </summary>
        [Description("是否有尾巴")]
        [Category("Shuai")]
        public bool Tail
        {
            get { return tail; }
            set { tail = value; this.Invalidate(); }
        }

        IconStyleList iconStyle = IconStyleList.三角形;
        /// <summary>
        /// 标头式校正
        /// </summary>
        [Description("标头式校正")]
        [Category("Shuai")]
        public IconStyleList IconStyle
        {
            get { return iconStyle; }
            set { iconStyle = value; drawAll = true ; this.Invalidate(); }
        }
        Image icon = null;
        /// <summary>
        /// Item图标
        /// </summary>
        [Description("图标")]
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
        Color titleColor = Color.WhiteSmoke;

        /// <summary>
        /// 标题背景色
        /// </summary>
        [Description("标题背景色")]
        [Category("Shuai")]
        public Color TitleColor
        {
            get { return titleColor; }
            set { titleColor = value; this.Invalidate(); }
        }
        Color iconColor = Color.Red;

        /// <summary>
        /// 图标背景色
        /// </summary>
        [Description("图标背景色")]
        [Category("Shuai")]
        public Color IconColor
        {
            get { return iconColor; }
            set { iconColor = value; this.Invalidate(); }
        }
        string title = "这里填写显示标题";

        /// <summary>
        /// 标题
        /// </summary>
        [Description("标题")]
        [Category("Shuai")]
        public string Title
        {
            get { return title; }
            set { title = value; this.Invalidate(); }
        }
        Font titleFont = new Font("Segoe UI", 10, FontStyle.Bold);
        /// <summary>
        /// 标题字体
        /// </summary>
        [Description("标题字体")]
        [Category("Shuai")]
        public Font TitleFont
        {
            get { return titleFont; }
            set { titleFont = value; this.Invalidate(); }
        }
        
        Bitmap backImage;
        bool drawAll = false;

        GraphicsPath thisVisual = new GraphicsPath();
        GraphicsPath TitlePath = new GraphicsPath();
        GraphicsPath IconPath = new GraphicsPath();
        GraphicsPath TailPath = new GraphicsPath();
        public ArrowTitle()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();
        }
        protected override void OnResize(EventArgs e)
        {
            if (Width < 3 * Height)
            {
                Width = 3 * Height;
            }
            drawAll = true;
            base.OnResize(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            int LineBold = 4;
            if (backImage == null || drawAll)
            {
                drawAll = false;
                InitFrm();
            }
            using (Graphics g = Graphics.FromImage(backImage))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                Rectangle tmpRect;

                g.FillPath(new SolidBrush(TitleColor), TitlePath);

                g.FillPath(new SolidBrush(IconColor), IconPath);

                if (tail)
                {
                    g.FillPath(new SolidBrush(iconColor), TailPath);
                }

                if (icon != null)//画图标
                {
                    tmpRect = new Rectangle(LineBold , LineBold , Height - LineBold * 2, Height - LineBold * 2);
                    g.DrawImage(icon, tmpRect, new Rectangle(0, 0, icon.Width, icon.Height), GraphicsUnit.Pixel);
                }
                else
                {
                    tmpRect = new Rectangle(LineBold , LineBold , Height - LineBold * 2, Height - LineBold * 2);
                    g.DrawImage(All.Properties.Resources.Identity_Card, tmpRect, new Rectangle(0, 0, All.Properties.Resources.News.Width, All.Properties.Resources.News.Height), GraphicsUnit.Pixel);
                }
                StringFormat sf=new StringFormat();
                sf.Alignment=StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                g.DrawString(title, titleFont, new SolidBrush(ForeColor), new RectangleF(TitlePath.PathPoints[0], new SizeF(Width - Height, Height * 11f / 15)), sf);

                sf.Dispose();
            }
            e.Graphics.DrawImageUnscaled(backImage, 0, 0);
            base.OnPaint(e);
        }
        private void InitFrm()
        {
            backImage = new Bitmap(Width, Height);

            if (Width < 3 * Height)
            {
                return;
            }
            thisVisual = new GraphicsPath();

            IconPath = new GraphicsPath();
            TitlePath = new GraphicsPath();
            TailPath = new GraphicsPath();

            IconPath.FillMode = FillMode.Winding;
            TitlePath.FillMode = FillMode.Winding;
            TailPath.FillMode = FillMode.Winding;
            thisVisual.FillMode = FillMode.Winding;
            PointF[] tmpPoint = new PointF[5];


            switch (iconStyle)
            {
                case IconStyleList.箭头:
                    tmpPoint[0] = new PointF(0, 0);
                    tmpPoint[1] = new PointF(Height, 0);
                    tmpPoint[2] = new PointF(1.5f * Height, 0.5f * Height);
                    tmpPoint[3] = new PointF(Height, Height);
                    tmpPoint[4] = new PointF(0, Height);
                    IconPath.AddPolygon(tmpPoint);
                    break;
                case IconStyleList.三角形:
                    tmpPoint[0] = new PointF(0, 0);
                    tmpPoint[1] = new PointF(Height, 0);
                    tmpPoint[2] = new PointF(1.5f * Height, 0);
                    tmpPoint[3] = new PointF(Height, Height);
                    tmpPoint[4] = new PointF(0, Height);
                    IconPath.AddPolygon(tmpPoint);
                    break;
                case IconStyleList.圆形:
                    tmpPoint[0] = new PointF(0, 0);
                    tmpPoint[1] = new PointF(Height, 0);
                    tmpPoint[2] = new PointF(Height, Height);
                    tmpPoint[3] = new PointF(Height, Height);
                    tmpPoint[4] = new PointF(0, Height);
                    IconPath.AddPolygon(tmpPoint);
                    IconPath.AddEllipse(new Rectangle(Height / 2, 0, Height, Height));
                    break;
            }

            thisVisual.AddPath(IconPath, true);

            tmpPoint = new PointF[4];
            tmpPoint[0] = new PointF(Height, 2f * Height / 15);
            tmpPoint[1] = new PointF(Width, 2f * Height / 15);
            tmpPoint[2] = new PointF(Width, 13f * Height / 15);
            tmpPoint[3] = new PointF(Height, 13f * Height / 15);

            TitlePath.AddPolygon(tmpPoint);
            thisVisual.AddPath(TitlePath, true);

            tmpPoint = new PointF[5];
            switch (iconStyle)
            {
                case IconStyleList.箭头:
                    tmpPoint[0] = new PointF(Width - Height * 3 / 4, 0);
                    tmpPoint[1] = new PointF(Width, 0);
                    tmpPoint[2] = new PointF(Width, Height);
                    tmpPoint[3] = new PointF(Width - Height * 3 / 4, Height);
                    tmpPoint[4] = new PointF(Width - Height / 4, Height / 2);
                    TailPath.AddPolygon(tmpPoint);
                    break;
                case IconStyleList.三角形:
                    tmpPoint[0] = new PointF(Width - Height / 4, 0);
                    tmpPoint[1] = new PointF(Width, 0);
                    tmpPoint[2] = new PointF(Width, Height);
                    tmpPoint[3] = new PointF(Width - Height * 3 / 4, Height);
                    tmpPoint[4] = new PointF(Width - Height * 3 / 4, Height);
                    TailPath.AddPolygon(tmpPoint);
                    break;
                case IconStyleList.圆形:

                    tmpPoint[0] = new PointF(Width - Height * 3 / 4, 0);
                    tmpPoint[1] = new PointF(Width, 0);
                    tmpPoint[2] = new PointF(Width, Height);
                    tmpPoint[3] = new PointF(Width - Height * 3 / 4, Height);
                    tmpPoint[4] = new PointF(Width - Height * 3 / 4, Height);

                    TailPath.AddPolygon(tmpPoint);

                    TailPath.FillMode = FillMode.Alternate;
                    TailPath.AddPie(Width - Height - Height / 4, 0, Height, Height, 270, 180);
                    break;
            }
            //if (tail)
            //{
            //    thisVisual.AddPolygon(TailPoint);
            //}

            this.Region = new Region(thisVisual);
            thisVisual.Dispose();
        }
    }
}
