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
    public partial class ComBoBox : System.Windows.Forms.ComboBox
    {
        Color[] itemColor = new Color[2] { Color.Red, Color.Blue };

        /// <summary>
        /// 子项背景色
        /// </summary>
        [Description("子项背景色")]
        [Category("Shuai")]
        public Color[] ItemColor
        {
            get { return itemColor; }
            set { itemColor = value; this.Invalidate(); }
        }
        /// <summary>
        /// 标头式样
        /// </summary>
        public enum IconStyleList
        {
            三角形,
            箭头,
            圆形,
            无
        }
        bool tail = true;
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
        /// 标头式样
        /// </summary>
        [Description("标头式样")]
        [Category("Shuai")]
        public IconStyleList IconStyle
        {
            get { return iconStyle; }
            set { iconStyle = value; drawAll = true; this.Invalidate(); }
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

        Font titleFont = new Font("Segoe UI", 10, FontStyle.Bold);
        /// <summary>
        /// 标题字体
        /// </summary>
        [Description("标题字体")]
        [Category("Shuai")]
        public Font TitleFont
        {
            get { return titleFont; }
            set
            {
                titleFont = value; 
                this.Font = value;
                switch (iconStyle)//改变项的高度，全其适应新画法中的图标高度
                {
                    case IconStyleList.无:
                        break;
                    default:
                        this.ItemHeight = (int)(All.Class.Num.GetFontHeight(titleFont) * 1.6f);
                        break;
                }
                this.Invalidate();
            }
        }
        string title = "";
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

        GraphicsPath TitlePath = new GraphicsPath();
        GraphicsPath IconPath = new GraphicsPath();
        GraphicsPath TailPath = new GraphicsPath();
        Bitmap backImage;
        bool drawAll = false;
        public ComBoBox()
        {
            InitializeComponent();
            base.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
        }
        protected override void OnResize(EventArgs e)
        {
            drawAll = true;
            this.Invalidate();
            base.OnResize(e);
        }
        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
        {
            //base.OnPaintBackground(pevent);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
        }
       
        protected override void OnDrawItem(System.Windows.Forms.DrawItemEventArgs e)
        {
            int LineBold = 4;
            if (backImage == null || drawAll)
            {
                drawAll = false;
                InitFrm(e.Bounds.Width, e.Bounds.Height);
            }
            using (Graphics g = Graphics.FromImage(backImage))
            {
                g.Clear(this.BackColor);
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                Rectangle tmpRect;

                //画选中项背景色
                if ((e.State & System.Windows.Forms.DrawItemState.Selected) == System.Windows.Forms.DrawItemState.Selected)
                {
                    g.FillRectangle(new SolidBrush(TitleColor),new Rectangle(new Point(0,0),e.Bounds.Size));
                }
                else
                {
                    g.FillPath(new SolidBrush(TitleColor), TitlePath);
                }
                //画前段颜色
                Color TmpColor = Color.Red;
                if (e.Index >= 0 && itemColor.Length > 0)
                {
                    TmpColor = itemColor[(e.Index % itemColor.Length)];
                }
                g.FillPath(new SolidBrush(TmpColor), IconPath);

                if (tail )//画尾巴
                {
                    g.FillPath(new SolidBrush(TmpColor), TailPath);
                }
                switch (iconStyle)//画图标
                {
                    case IconStyleList.无:
                        break;
                    default:
                        if (icon != null)
                        {
                            tmpRect = new Rectangle(LineBold, LineBold, e.Bounds.Height - LineBold * 2, e.Bounds.Height - LineBold * 2);
                            g.DrawImage(icon, tmpRect, new Rectangle(0, 0, icon.Width, icon.Height), GraphicsUnit.Pixel);
                        }
                        else
                        {
                            tmpRect = new Rectangle(LineBold, LineBold, e.Bounds.Height - LineBold * 2, e.Bounds.Height - LineBold * 2);
                            g.DrawImage(All.Properties.Resources.Identity_Card, tmpRect, new Rectangle(0, 0, All.Properties.Resources.News.Width, All.Properties.Resources.News.Height), GraphicsUnit.Pixel);
                        }
                        break;
                }
                //画上面颜色
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                if (e.Index >= 0 && e.Index < this.Items.Count)
                {
                    g.DrawString(this.Items[e.Index].ToString(), titleFont, new SolidBrush(ForeColor), new RectangleF(TitlePath.PathPoints[0], new SizeF(e.Bounds.Width - e.Bounds.Height/2, e.Bounds.Height * 11f / 15)), sf);
                }
                if (e.Index < 0)
                {
                    g.DrawString(this.title, titleFont, new SolidBrush(ForeColor), new RectangleF(TitlePath.PathPoints[0], new SizeF(e.Bounds.Width+e.Bounds.Height/2, e.Bounds.Height * 11f / 15)), sf);
                }
                sf.Dispose();
            }
            e.Graphics.DrawImageUnscaled(backImage, e.Bounds.Left, e.Bounds.Top);
            //base.OnDrawItem(e);
        }
        private void InitFrm(int width, int height)
        {
            backImage = new Bitmap(width, height);

            if (width < 3 * height)
            {
                return;
            }

            IconPath = new GraphicsPath();
            TitlePath = new GraphicsPath();
            TailPath = new GraphicsPath();

            IconPath.FillMode = FillMode.Winding;
            TitlePath.FillMode = FillMode.Winding;
            TailPath.FillMode = FillMode.Winding;
            PointF[] tmpPoint = new PointF[5];

            //头部
            switch (iconStyle)
            {
                case IconStyleList.箭头:
                    tmpPoint[0] = new PointF(0, 0);
                    tmpPoint[1] = new PointF(height, 0);
                    tmpPoint[2] = new PointF(1.5f * height, 0.5f * height);
                    tmpPoint[3] = new PointF(height, height);
                    tmpPoint[4] = new PointF(0, height);
                    IconPath.AddPolygon(tmpPoint);
                    break;
                case IconStyleList.三角形:
                    tmpPoint[0] = new PointF(0, 0);
                    tmpPoint[1] = new PointF(height, 0);
                    tmpPoint[2] = new PointF(1.5f * height, 0);
                    tmpPoint[3] = new PointF(height, height);
                    tmpPoint[4] = new PointF(0, height);
                    IconPath.AddPolygon(tmpPoint);
                    break;
                case IconStyleList.圆形:
                    tmpPoint[0] = new PointF(0, 0);
                    tmpPoint[1] = new PointF(height, 0);
                    tmpPoint[2] = new PointF(height, height);
                    tmpPoint[3] = new PointF(height, height);
                    tmpPoint[4] = new PointF(0, height);
                    IconPath.AddPolygon(tmpPoint);
                    IconPath.AddEllipse(new Rectangle(height / 2, 0, height, height));
                    break;
                default:
                    break;
            }

            //中间
            tmpPoint = new PointF[4];
            switch (iconStyle)
            {
                case IconStyleList.无:
                    tmpPoint[0] = new PointF(2f * height / 15, 2f * height / 15);
                    tmpPoint[1] = new PointF(width, 2f * height / 15);
                    tmpPoint[2] = new PointF(width, 13f * height / 15);
                    tmpPoint[3] = new PointF(2f * height / 15, 13f * height / 15);
                    break;
                default:
                    this.ItemHeight = (int)(All.Class.Num.GetFontHeight(titleFont) * 1.6f);
                    tmpPoint[0] = new PointF(height, 2f * height / 15);
                    tmpPoint[1] = new PointF(width, 2f * height / 15);
                    tmpPoint[2] = new PointF(width, 13f * height / 15);
                    tmpPoint[3] = new PointF(height, 13f * height / 15);
                    break;
            }

            TitlePath.AddPolygon(tmpPoint);

            //尾部
            tmpPoint = new PointF[5];
            switch (iconStyle)
            {
                case IconStyleList.箭头:
                    tmpPoint[0] = new PointF(width - height * 3 / 4, 0);
                    tmpPoint[1] = new PointF(width, 0);
                    tmpPoint[2] = new PointF(width, height);
                    tmpPoint[3] = new PointF(width - height * 3 / 4, height);
                    tmpPoint[4] = new PointF(width - height / 4, height / 2);
                    TailPath.AddPolygon(tmpPoint);
                    break;
                case IconStyleList.三角形:
                    tmpPoint[0] = new PointF(width - height / 4, 0);
                    tmpPoint[1] = new PointF(width, 0);
                    tmpPoint[2] = new PointF(width, height);
                    tmpPoint[3] = new PointF(width - height * 3 / 4, height);
                    tmpPoint[4] = new PointF(width - height * 3 / 4, height);
                    TailPath.AddPolygon(tmpPoint);
                    break;
                case IconStyleList.圆形:

                    tmpPoint[0] = new PointF(width - height * 3 / 4, 0);
                    tmpPoint[1] = new PointF(width, 0);
                    tmpPoint[2] = new PointF(width, height);
                    tmpPoint[3] = new PointF(width - height * 3 / 4, height);
                    tmpPoint[4] = new PointF(width - height * 3 / 4, height);

                    TailPath.AddPolygon(tmpPoint);

                    TailPath.FillMode = FillMode.Alternate;
                    TailPath.AddPie(width - height - height / 4, 0, height, height, 270, 180);
                    break;
                default:
                    break;
            }
            //if (tail)
            //{
            //    thisVisual.AddPolygon(TailPoint);
            //}

        }
    }
}
