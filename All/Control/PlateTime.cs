using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
namespace All.Control
{
    public partial class PlateTime : System.Windows.Forms.Control
    {
        public PlateTime()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();
            InitializeComponent();
        }

        private DateTime now = DateTime.Now;
        /// <summary>
        /// 显示数据
        /// </summary>
        [Category("Shuai")]
        [Description("显示数据")]
        public DateTime Now
        {
            get { return now; }
            set
            {
                now = value;
                this.Invalidate();
            }
        }
        string title = "";
        /// <summary>
        /// 显示标题
        /// </summary>
        [Description("显示标题")]
        [Category("Shuai")]
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                drawAll = true;
                this.Invalidate();
            }
        }
        Color plateColor = Color.Orange;
        /// <summary>
        /// 表盘颜色
        /// </summary>
        [Description("表盘颜色")]
        [Category("Shuai")]
        public Color PlateColor
        {
            get { return plateColor; }
            set
            {
                plateColor = value;
                drawAll = true;
                this.Invalidate();
            }
        }
        Color arrowColor = Color.Red;
        /// <summary>
        /// 指针颜色
        /// </summary>
        [Description("指针颜色")]
        [Category("Shuai")]
        public Color ArrowColor
        {
            get { return arrowColor; }
            set
            {
                arrowColor = value;
                this.Invalidate();
            }
        }
        /// <summary>
        /// 字体
        /// </summary>
        [Description("字体")]
        [Category("Shuai")]
        public override Font Font
        {
            get{ return base.Font;}
            set
            {
                base.Font = value;
                drawAll = true;
                this.Invalidate();
            }
        }
        /// <summary>
        /// 字体颜色
        /// </summary>
        [Description("字体颜色")]
        [Category("Shuai")]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set
            {
                base.ForeColor = value;
                drawAll = true;
                this.Invalidate();
            }
        }
        bool drawAll = false;
        Bitmap backImage;
        Bitmap tmpBackImage;
        protected override void OnCreateControl()
        {
            this.Font = new Font("黑体", 12);
            this.title = "北京时间";
            this.Now = DateTime.Now;
            base.OnCreateControl();
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            if (Width < 240)
            {
                this.Width = 240;
            }
            this.Height = this.Width;
            backImage = new Bitmap(this.Width, this.Height);
            tmpBackImage = new Bitmap(this.Width, this.Height);
            drawAll = true;
            DrawBack();
            base.OnSizeChanged(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            DrawBack();
            e.Graphics.DrawImageUnscaled(backImage, 0, 0);
            base.OnPaint(e);
        }
        /// <summary>
        /// 画表盘,改变此函数,可实际多表盘的风格
        /// </summary>
        private void DrawPlate()
        {
            if (tmpBackImage == null)
            {
                tmpBackImage = new Bitmap(this.Width, this.Height);
            }
            using (Graphics g = Graphics.FromImage(tmpBackImage))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.Clear(BackColor);


                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                //sf.LineAlignment = StringAlignment.Center;
                //外层彩圏
                GraphicsPath gp = new GraphicsPath();
                gp.AddEllipse(0, 0, Width - 1, Height - 1);
                PathGradientBrush pgb = new PathGradientBrush(gp);
                pgb.CenterColor = Color.White;
                pgb.SurroundColors = new Color[] { plateColor };
                pgb.CenterPoint = new PointF(30, 30);
                g.FillEllipse(pgb, 0, 0, Width - 1, Height - 1);
                //白框
                gp.ClearMarkers();
                gp.AddEllipse(10, 10, Width - 1 - 20, Height - 1 - 20);
                pgb = new PathGradientBrush(gp);
                pgb.CenterColor = Color.Black;
                pgb.SurroundColors = new Color[] { Color.White };
                g.FillEllipse(pgb, 10, 10, Width - 1 - 20, Height - 1 - 20);
                //黑圈
                g.FillEllipse(new SolidBrush(Color.Black), 20, 20, Width - 1 - 40, Height - 1 - 40);
                //灰背景
                gp.ClearMarkers();
                gp.AddEllipse(10, 10, Width - 1 - 20, Height - 1 - 20);
                pgb = new PathGradientBrush(gp);
                pgb.CenterColor = System.Windows.Forms.ControlPaint.LightLight(plateColor);
                pgb.SurroundColors = new Color[] { Color.White };
                pgb.CenterPoint = new PointF(30, 30);
                g.FillEllipse(pgb, 22, 22, Width - 1 - 44, Height - 1 - 44);


                float tmpJiaoDu = 360.00f / 12;
                float tmpR = Width / 2.00f - 30;
                //报警颜色段
                //g.FillPie(new SolidBrush(colorPart1), 30, 30, Width - 1 - 60, Height - 1 - 60, 135, tmpJiaoDu * 12);

                //刻度

                for (int i = 0; i < 60; i++)
                {
                    g.DrawLine(new Pen(Color.Black, 1), new PointF(Width / 2.0f, Height / 2.0f),
                        new PointF((float)(Width / 2.0 - Math.Sin(Math.PI * (i * 6) / 180) * tmpR),
                            (float)(Height / 2.0 + Math.Cos(Math.PI * (i * 6) / 180) * tmpR)));
                }
                //灰背景,把多余的报警颜色段覆盖
                g.FillEllipse(pgb, 35, 35, Width - 1 - 70, Height - 1 - 70);
                //刻度

                for (int i = 0; i < 12; i++)
                {
                    g.DrawLine(new Pen(Color.Black, 1), new PointF(Width / 2.0f, Height / 2.0f),
                        new PointF((float)(Width / 2.0 - Math.Sin(Math.PI * (i * 30) / 180) * tmpR),
                            (float)(Height / 2.0 + Math.Cos(Math.PI * (i * 30) / 180) * tmpR)));
                }
                ////灰背景,把多余的报警颜色段覆盖
                //g.FillEllipse(pgb, 40, 40, Width - 1 - 80, Height - 1 - 80);

                //刻度

                for (int i = 0; i < 4; i++)
                {
                    g.DrawLine(new Pen(Color.Black, 2), new PointF(Width / 2.0f, Height / 2.0f),
                        new PointF((float)(Width / 2.0 - Math.Sin(Math.PI * (i * 90) / 180) * tmpR),
                            (float)(Height / 2.0 + Math.Cos(Math.PI * (i * 90) / 180) * tmpR)));
                }
                //灰背景,把多余的报警颜色段覆盖
                g.FillEllipse(pgb, 40, 40, Width - 1 - 80, Height - 1 - 80);

                //画刻度上的文字
                string tmpStr = "";
                Font tmpFont = new Font("黑体", 10, FontStyle.Bold);
               // sf.LineAlignment = StringAlignment.Center;
                for (int i = 0; i < 12; i++)
                {
                    g.TranslateTransform(Width / 2.0f, Height / 2.0f);//把g的中心移动到图像中间
                    g.RotateTransform(i * tmpJiaoDu + 30);//把g旋转一定角度
                    tmpStr = string.Format("{0}", i + 1);
                    g.DrawString(tmpStr, tmpFont, new SolidBrush(Color.Black), new Rectangle(-Width/2, 45 - Height / 2, Width, All.Class.Num.GetFontHeight(tmpFont)), sf);
                    g.RotateTransform(360 - i * tmpJiaoDu - 30);
                    g.TranslateTransform(-Width / 2.0f, -Height / 2.0f);
                }
                //画下方数据区
                //gp = new GraphicsPath();
                //tmpR = Width / 2.00f - 21;
                //gp.AddArc(21, 21, Width - 1 - 42, Height - 1 - 42, 45, 90);
                //gp.AddLine((float)(Width / 2.0f - Math.Sin(45 * Math.PI / 180) * tmpR), (float)(Height / 2.0f + Math.Cos(45 * Math.PI / 180) * tmpR),
                //        (float)(Width / 2.0f + Math.Sin(45 * Math.PI / 180) * tmpR), (float)(Height / 2.0f + Math.Cos(45 * Math.PI / 180) * tmpR));
                //gp.CloseFigure();
                //pgb = new PathGradientBrush(gp);
                //pgb.CenterColor = Color.White;
                //pgb.SurroundColors = new Color[] { System.Windows.Forms.ControlPaint.Light(plateColor) };
                //g.FillPath(pgb, gp);

                //画标题
                tmpStr = title;
                //tmpFont = new Font("宋体", 12, FontStyle.Bold);
                g.DrawString(tmpStr, Font, new SolidBrush(ForeColor), new RectangleF(0, Height * 0.65f, Width, tmpFont.GetHeight()), sf);
                gp = new GraphicsPath();


                sf.Dispose();
                pgb.Dispose();
                gp.Dispose();
            }
        }
        void flush()
        {
            while (true)
            {
                this.Now = DateTime.Now;
                Thread.Sleep(1000);
            }
        }
        /// <summary>
        /// 画指针
        /// </summary>
        private void DrawBack()
        {
            if (backImage == null)
            {
                backImage = new Bitmap(this.Width, this.Height);
            }
            using (Graphics g = Graphics.FromImage(backImage))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                if (drawAll)
                {
                    DrawPlate();
                    drawAll = false;
                }
                g.DrawImageUnscaled(tmpBackImage, 0, 0);
                ////指针
                GraphicsPath gp = new GraphicsPath();
                float tmpR = 0;;
                float tmpJiaoDu =0;

                gp.Reset();
                tmpR = Width / 2.0f - 88;
                tmpJiaoDu = (float)(((now.Hour % 12) + now.Minute / 60f) * 30);
                gp.AddArc(88, 88, Width - 1 - 176, Height - 1 - 176, (int)(269 + tmpJiaoDu) % 360, 2);
                gp.AddArc(Width / 2 - 5, Height / 2 - 5, 10, 10, (int)(0 + tmpJiaoDu ) % 360, 180);
                gp.CloseFigure();
                g.FillPath(new SolidBrush(System.Windows.Forms.ControlPaint.Dark(plateColor)), gp);


                gp.Reset();
                tmpR = Width / 2.0f - 68;
                tmpJiaoDu = (float)(now.Minute * 6 + now.Second / 10f);
                gp.AddArc(68, 68, Width - 1 - 136, Height - 1 - 136, (int)(269 + tmpJiaoDu) % 360, 2);
                gp.AddArc(Width / 2 - 4, Height / 2 - 4, 8, 8, (int)(0 + tmpJiaoDu) % 360, 180);
                gp.CloseFigure();
                g.FillPath(new SolidBrush(System.Windows.Forms.ControlPaint.Dark(plateColor)), gp);


                gp.Reset();
                tmpR = Width / 2.0f - 58;
                tmpJiaoDu = (float)(now.Second * 6);
                gp.AddArc(58, 58, Width - 1 - 116, Height - 1 - 116, (int)(269 + tmpJiaoDu) % 360, 2);
                gp.AddArc(Width / 2 -2, Height / 2 - 2, 4, 4, (int)(0 + tmpJiaoDu) % 360, 180);
                gp.CloseFigure();
                g.FillPath(new SolidBrush(arrowColor), gp);




                //数字
                //tmpR = Width / 2.00f - 21;
                //RectangleF tmpRect = new RectangleF((float)(Width / 2.0f - Math.Sin(45 * Math.PI / 180) * tmpR), (float)(Height / 2.0f + Math.Cos(45 * Math.PI / 180) * tmpR),
                //        tmpR * 1.414f, tmpR - tmpR * 1.414f / 2.0f);
                //string tmpStr = value.ToString(format);
                //StringFormat sf = new StringFormat();
                //sf.LineAlignment = StringAlignment.Center;
                //sf.Alignment = StringAlignment.Center;
                //g.DrawString(tmpStr, Font, new SolidBrush(ForeColor), tmpRect, sf);


                //中心圆
                //g.DrawArc(new Pen(Color.Black, 4), Width / 2 - 13, Height / 2 - 13, 26, 26, 0, 360);

                //g.FillEllipse(new SolidBrush(Color.White), Width / 2 - 11, Height / 2 - 11, 22, 22);

                gp = new GraphicsPath();
                gp.AddEllipse(Width / 2 - 6, Height / 2 - 6, 12, 12);
                PathGradientBrush pgb = new PathGradientBrush(gp);
                pgb.CenterColor = Color.WhiteSmoke;
                pgb.SurroundColors = new Color[] { Color.Black };
                g.FillPath(pgb, gp);

                pgb.Dispose();
                gp.Dispose();
            }
        }
    }
}
