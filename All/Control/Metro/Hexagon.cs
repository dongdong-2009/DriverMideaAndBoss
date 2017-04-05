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
    [DefaultEvent("Click")]
    [DefaultProperty("Icon")]
    public partial class Hexagon : System.Windows.Forms.Control
    {
        Image icon = null;
        /// <summary>
        /// 图标
        /// </summary>
        [Description("图标")]
        [Category("Shuai")]
        public Image Icon
        {
            get { return icon; }
            set { icon = value; this.Invalidate(); }
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
        string value = "";
        /// <summary>
        /// 内容
        /// </summary>
        [Description("内容")]
        [Category("Shuai")]
        public string Value
        {
            get { return this.value; }
            set
            {
                this.value = value;
                this.Invalidate();
            }
        }


        Font titleFont = new Font("Segoe UI", 16, FontStyle.Bold);
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
                this.Invalidate();
            }
        }
        Font valueFont = new Font("Segoe UI", 12, FontStyle.Bold);
        /// <summary>
        /// 小标题字体
        /// </summary>
        [Description("小标题字体")]
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
        Color back = Color.DarkCyan;
        /// <summary>
        /// 背景色
        /// </summary>
        [Description("背景色")]
        [Category("Shuai")]
        public Color Back
        {
            get { return back; }
            set { back = value;this.Invalidate(); }
        }
        bool login = false;
        /// <summary>
        /// 登陆后可用
        /// </summary>
        [Description("登陆后可用")]
        [Category("Shuai")]
        public bool Login
        {
            get { return login; }
            set { login = value; this.Invalidate(); }
        }
        /// <summary>
        /// 按钮外形列表
        /// </summary>
        public enum ButtonList
        {
            Hexagon,
            Box,
            ItemBox
        }
        ButtonList buttonValue = ButtonList.Hexagon;
        /// <summary>
        /// 按钮外形
        /// </summary>
        [Description("按钮外形")]
        [Category("Shuai")]
        public ButtonList ButtonValue
        {
            get { return buttonValue; }
            set { buttonValue = value; drawAll = true; this.Invalidate(); }
        }
        Bitmap backImage;
        bool drawAll = false;
        bool drawAfterMouseIn = false;
        GraphicsPath thisVisual;
        public Hexagon()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();
        }
        protected override void OnResize(EventArgs e)
        {
            switch (buttonValue)
            {
                case ButtonList.Hexagon:
                    if (Height < Width / 1.732f)
                    {
                        Height = (int)(Width / 1.732f);
                    }
                    break;
                case ButtonList.Box:
                    if (Height * 8.732f != Width * 8)
                    {
                        Height = (int)(Width * 8f / 8.732f);
                    }
                    break;
                case ButtonList.ItemBox:
                    if (Height < 45)
                    {
                        Height = 45;
                    }
                    if (Width < 60)
                    {
                        Width = 60;
                    }
                    if (Height > Width)
                    {
                        Height = Width;
                    }
                    break;
            }
            drawAll = true;
            base.OnResize(e);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            switch(buttonValue)
            {
                case ButtonList.ItemBox:
                    drawAfterMouseIn = true;
                    this.Invalidate();
                    break;
            }
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            switch (buttonValue)
            {
                case ButtonList.ItemBox:
                    drawAfterMouseIn = false;
                    this.Invalidate();
                    break;
            }
            base.OnMouseLeave(e);
        }
        protected override void OnClick(EventArgs e)
        {
            if (login)
            {
                base.OnClick(e);
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            PointF[] tmpF;
           // GraphicsPath tmpGP;
            int tmpWidth;
            if (backImage == null || drawAll)
            {
                drawAll = false;
                InitFrm();
            }
            using (Graphics g = Graphics.FromImage(backImage))
            {
                if (login)
                {
                    g.Clear(back);
                }
                else 
                {
                    g.Clear(Color.DimGray);
                }
                switch (buttonValue)
                {
                    case ButtonList.ItemBox:
                        Size IconSize = new System.Drawing.Size(All.Class.Num.GetFontHeight(titleFont) + All.Class.Num.GetFontHeight(valueFont),
                                                                All.Class.Num.GetFontHeight(titleFont) + All.Class.Num.GetFontHeight(valueFont));
                            
                        tmpWidth = IconSize.Width -5 + Math.Max(All.Class.Num.GetFontWidth(titleFont, title), All.Class.Num.GetFontWidth(valueFont, value));
                        if (icon == null)
                        {
                            g.DrawImage(All.Properties.Resources.Identity_Card,
                                new Rectangle(Width / 2 - tmpWidth / 2, Height / 2 - IconSize.Height / 2, IconSize.Width, IconSize.Height),
                                new Rectangle(new Point(0, 0), All.Properties.Resources.Identity_Card.Size), GraphicsUnit.Pixel);
                        }
                        else
                        {
                            g.DrawImage(icon,
                                new Rectangle(Width / 2 - tmpWidth / 2, Height / 2 - IconSize.Height / 2, IconSize.Width, IconSize.Height),
                                new Rectangle(new Point(0, 0), icon.Size), GraphicsUnit.Pixel);
                        }
                        using (StringFormat sf = new StringFormat())
                        {
                            sf.Alignment = StringAlignment.Near;
                            sf.LineAlignment = StringAlignment.Center;

                            g.DrawString(title, titleFont, new SolidBrush(ForeColor), new RectangleF(Width / 2 - tmpWidth / 2 + IconSize.Width , Height / 2 - IconSize.Height / 2,tmpWidth-IconSize.Width,IconSize.Height/2), sf);
                            g.DrawString(value, valueFont, new SolidBrush(ForeColor), new RectangleF(Width / 2 - tmpWidth / 2 + IconSize.Width , Height / 2, tmpWidth - IconSize.Width, IconSize.Height / 2), sf);
                        }
                        if (drawAfterMouseIn)
                        {
                            g.DrawRectangle(new Pen(Color.White), new Rectangle(0, 0, Width - 1, Height - 1));
                        }
                        break;
                    case ButtonList.Hexagon:
                        g.DrawLine(new Pen(ControlPaint.LightLight(back), 3), 5, Height / 1.9f, Width - 5, Height / 1.9f);
                        if (icon == null)
                        {
                            g.DrawImage(All.Properties.Resources.Identity_Card,
                                new Rectangle(Width / 2 - (int)(Width / 2 / (1 + 2 * 1.732f)), (int)(Width / 2 / (1 + 2 * 1.732f)), (int)(Width / (1 + 2 * 1.732f)), (int)(Width / (1 + 2 * 1.732F))),
                                new Rectangle(new Point(0, 0), All.Properties.Resources.Identity_Card.Size), GraphicsUnit.Pixel);
                        }
                        else
                        {
                            g.DrawImage(icon,
                                new Rectangle(Width / 2 - (int)(Width / 2 / (1 + 2 * 1.732f)), (int)(Width / 2 / (1 + 2 * 1.732f)), (int)(Width / (1 + 2 * 1.732f)), (int)(Width / (1 + 2 * 1.732F))),
                                new Rectangle(new Point(0, 0), icon.Size), GraphicsUnit.Pixel);
                        }
                        using (StringFormat sf = new StringFormat())
                        {
                            sf.Alignment = StringAlignment.Center;
                            sf.LineAlignment = StringAlignment.Center;

                            g.DrawString(title, titleFont, new SolidBrush(ForeColor), new RectangleF(0, Width / 1.732F / 2, Width, Height / 1.8f - Width / 1.732f / 2), sf);
                            g.DrawString(value, valueFont, new SolidBrush(ForeColor), new RectangleF(0, Height / 1.8f, Width, Height / 2 - Width / 1.732f / 2), sf);
                        }
                        break;
                    case ButtonList.Box:
                        if (icon == null)
                        {
                            g.DrawImage(All.Properties.Resources.Identity_Card,
                                new RectangleF(12.196f * Height / 24f, 7f / 48f * Height, 7f / 24f * Height, 7f / 24f * Height),
                                new RectangleF(new Point(0, 0), All.Properties.Resources.Identity_Card.Size), GraphicsUnit.Pixel);
                        }
                        else
                        {
                            g.DrawImage(icon,
                                new RectangleF(12.196f * Height / 24f, 7f / 48f * Height, 7f / 24f * Height, 7f / 24f * Height),
                                new RectangleF(new Point(0, 0), icon.Size), GraphicsUnit.Pixel);
                        }
                        using (LinearGradientBrush lgb = new LinearGradientBrush(new Point(Width, 0), new Point(0, Height), ControlPaint.Light(back), ControlPaint.Dark(back)))
                        {
                            tmpF = new PointF[6];
                            tmpF[0] = new PointF(0, Height / 8.0f);
                            tmpF[1] = new PointF(Height / 8.0f * 1.732f, 0);
                            tmpF[2] = new PointF(Height / 8.0f * 1.732f, 7f / 8f * Height);
                            tmpF[3] = new PointF(Width, 7f / 8f * Height);
                            tmpF[4] = new PointF(Width - Height / 8.0f * 1.732f, Height);
                            tmpF[5] = new PointF(0, Height);

                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                gp.AddPolygon(tmpF);
                                g.FillPath(lgb, gp);
                            }
                        }
                        break;
                }
            }
            e.Graphics.DrawImageUnscaled(backImage, new Point(0, 0));
            base.OnPaint(e);
        }
        private void InitFrm()
        {
            backImage = new Bitmap(Width, Height);
            PointF[] tmpF;
            switch (buttonValue)
            {
                case ButtonList.Hexagon:
                    if (Height < Width / 1.732f)
                    {
                        Height = (int)(Width / 1.732f);
                    }
                    thisVisual = new GraphicsPath();
                    tmpF = new PointF[6];
                    tmpF[0] = new PointF(Width / 2, 0);
                    tmpF[1] = new PointF(Width, Width / 1.732f / 2);
                    tmpF[2] = new PointF(Width, Height - Width / 1.732f / 2);
                    tmpF[3] = new PointF(Width / 2, Height);
                    tmpF[4] = new PointF(0, Height - Width / 1.732f / 2);
                    tmpF[5] = new PointF(0, Width / 1.732f / 2);
                    thisVisual.AddPolygon(tmpF);
                    this.Region = new Region(thisVisual);
                    break;
                case ButtonList.Box:
                    if (Height * 8.732f != Width * 8)
                    {
                        Height = (int)(Width * 8f / 8.732f);
                    }
                    thisVisual = new GraphicsPath();
                    tmpF = new PointF[6];
                    tmpF[0] = new PointF(0, Height / 8.0f);
                    tmpF[1] = new PointF(Height / 8.0f * 1.732f, 0);
                    tmpF[2] = new PointF(Width, 0);
                    tmpF[3] = new PointF(Width, Height * 7.0f / 8.0f);
                    tmpF[4] = new PointF(Width - Height / 8.0f * 1.732f, Height);
                    tmpF[5] = new PointF(0, Height);
                    thisVisual.AddPolygon(tmpF);
                    this.Region = new Region(thisVisual);
                    break;
                case ButtonList.ItemBox:
                    if (Height < 45)
                    {
                        Height = 45;
                    }
                    if (Width < 60)
                    {
                        Width = 60;
                    }
                    if (Height > Width)
                    {
                        Height = Width;
                    }
                    thisVisual = new GraphicsPath();
                    tmpF = new PointF[4];
                    tmpF[0] = new PointF(0, 0);
                    tmpF[1] = new PointF(Width, 0);
                    tmpF[2] = new PointF(Width, Height);
                    tmpF[3] = new PointF(0, Height);
                    thisVisual.AddPolygon(tmpF);
                    this.Region = new Region(thisVisual);
                    break;
            }
        }
    }
}
