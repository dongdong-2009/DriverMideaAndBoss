using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
namespace All.Control
{
    public partial class Light : System.Windows.Forms.Control
    {
        Color ledColor = Color.Red;

        /// <summary>
        /// 图形颜色
        /// </summary>
        [Description("图形颜色")]
        [Category("Shuai")]
        public Color LedColor
        {
            get { return ledColor; }
            set { ledColor = value; this.Invalidate(); }
        }
        public Light()
        {
            this.BackColor = Color.Green;
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint|ControlStyles.SupportsTransparentBackColor | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            this.Height = this.Width;
            base.OnSizeChanged(e);
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

            GraphicsPath gp = new GraphicsPath();
            PathGradientBrush pgb;

            gp.AddEllipse(0, 0, Width, Height);
            pgb = new PathGradientBrush(gp);
            pgb.CenterColor = Color.White;// System.Windows.Forms.ControlPaint.LightLight(BackColor);
            pgb.SurroundColors = new Color[] { ledColor};
            pgb.CenterPoint = new PointF(Width / 2.0f, Height /2.0f);
            e.Graphics.FillEllipse(pgb, 2, 2, Width - 4, Height - 4);


            //gp = new GraphicsPath();
            //gp.AddEllipse(0, 0, Width, Height);
            //pgb = new PathGradientBrush(gp);
            //pgb.CenterColor = Color.White;
            //pgb.SurroundColors = new Color[] { Color.FromArgb(220, ledColor.R, ledColor.G, ledColor.B) };
            //pgb.CenterPoint = new PointF(Width / 4.0f, Height / 4.0f);
            //e.Graphics.FillEllipse(pgb, Width / 8, Height / 8, Width * 7 / 10, Height * 7 / 10);

            //gp = new GraphicsPath();
            //gp.AddEllipse(4, 4, Width - 8, Height - 8);
            //pgb = new PathGradientBrush(gp);
            //pgb.CenterColor = Color.FromArgb(150, 255, 255, 255);// System.Windows.Forms.ControlPaint.LightLight(BackColor);
            //pgb.SurroundColors = new Color[] { Color.FromArgb(220, ledColor.R, ledColor.G, ledColor.B) };// System.Windows.Forms.ControlPaint.Dark(BackColor) };
            //pgb.CenterPoint = new PointF(Width / 3.0f, Height / 3.0f);
            //e.Graphics.FillEllipse(pgb, 4, 4, Width * 0.9f, Height * 0.9f);

            gp = new GraphicsPath();
            gp.AddEllipse(0, 0, Width, Height);
            pgb = new PathGradientBrush(gp);
            pgb.CenterColor = Color.FromArgb(150, ledColor.R, ledColor.G, ledColor.B);
            pgb.SurroundColors = new Color[] { Color.FromArgb(0, ledColor.R, ledColor.G, ledColor.B) };
            pgb.CenterPoint = new PointF(Width / 4.0f, Height / 4.0f);
            e.Graphics.FillEllipse(pgb, (int)(Width * 0.15), (int)(Height * 0.15), (int)(Width * 0.7), (int)(Height * 0.7));

            gp = new GraphicsPath();
            gp.AddEllipse(4, 4, Width - 8, Height - 8);
            pgb = new PathGradientBrush(gp);
            pgb.CenterColor = Color.FromArgb(100, ledColor.R, ledColor.G, ledColor.B);// System.Windows.Forms.ControlPaint.LightLight(BackColor);
            pgb.SurroundColors = new Color[] { Color.FromArgb(200, ledColor.R, ledColor.G, ledColor.B) };// System.Windows.Forms.ControlPaint.Dark(BackColor) };
            pgb.CenterPoint = new PointF(Width / 3.0f, Height / 3.0f);
            e.Graphics.FillEllipse(pgb, 4, 4, Width * 0.9f, Height * 0.9f);

            gp.Dispose();
            pgb.Dispose();

            //e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(255,255,0,0)), 0, 0, Width, Height);
            base.OnPaint(e);
        }
    }
}
