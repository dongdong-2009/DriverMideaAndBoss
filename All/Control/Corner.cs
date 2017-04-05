using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace All.Control
{
    public partial class Corner : System.Windows.Forms.Control
    {
        /// <summary>
        /// 转角位置
        /// </summary>
        public enum SpaceList
        {
            /// <summary>
            /// 左上
            /// </summary>
            LeftTop,
            /// <summary>
            /// 右上
            /// </summary>
            RightTop,
            /// <summary>
            /// 左下
            /// </summary>
            LeftBottom,
            /// <summary>
            /// 右下
            /// </summary>
            RightBottom,
            /// <summary>
            /// 左到右
            /// </summary>
            Left2Right,
            /// <summary>
            /// 上到下
            /// </summary>
            Top2Bottom,
            /// <summary>
            /// 左上到右下
            /// </summary>
            LeftTop2RightBottom,
            /// <summary>
            /// 左下到右上
            /// </summary>
            LeftBottom2RightTop
        }
        SpaceList space = SpaceList.LeftTop;
        /// <summary>
        /// 转角位置
        /// </summary>
        [Description("转角位置")]
        [Category("Shuai")]
        public SpaceList Space
        {
            get { return space; }
            set { space = value; this.Invalidate(); }
        }
        int bold = 1;
        /// <summary>
        /// 线的粗细
        /// </summary>
        [Description("线的粗细")]
        [Category("Shuai")]
        public int Bold
        {
            get { return bold; }
            set { bold = value; }
        }
        public Corner()
        {
            InitializeComponent();
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            switch (space)
            {
                case SpaceList.LeftTop:
                    e.Graphics.DrawArc(new System.Drawing.Pen(ForeColor, bold),
                        new System.Drawing.RectangleF(Font.Size / 2, Font.Size / 2, Width * 2 - Font.Size, Height * 2 - Font.Size), 180, 90);
                    break;
                case SpaceList.RightTop:
                    e.Graphics.DrawArc(new System.Drawing.Pen(ForeColor, bold),
                        new System.Drawing.RectangleF(Font.Size / 2 - Width, Font.Size / 2, Width * 2 - Font.Size, Height * 2 - Font.Size), 270, 90);
                    break;
                case SpaceList.LeftBottom:
                    e.Graphics.DrawArc(new System.Drawing.Pen(ForeColor,bold),
                        new System.Drawing.RectangleF(Font.Size / 2, Font.Size / 2-Height, Width * 2 - Font.Size, Height * 2 - Font.Size), 90, 90);
                    break;
                case SpaceList.RightBottom:
                    e.Graphics.DrawArc(new System.Drawing.Pen(ForeColor, bold),
                        new System.Drawing.RectangleF(Font.Size / 2-Width, Font.Size / 2-Height, Width * 2 - Font.Size, Height * 2 - Font.Size), 0, 90);
                    break;
                case SpaceList.Left2Right:
                    e.Graphics.DrawLine(new System.Drawing.Pen(ForeColor, bold),
                        new System.Drawing.Point(0, Height / 2), new System.Drawing.Point(Width, Height / 2));
                    break;
                case SpaceList.Top2Bottom:
                    e.Graphics.DrawLine(new System.Drawing.Pen(ForeColor, bold),
                        new System.Drawing.Point(Width / 2, 0), new System.Drawing.Point(Width / 2, Height));
                    break;
                case SpaceList.LeftTop2RightBottom:
                    e.Graphics.DrawLine(new System.Drawing.Pen(ForeColor, bold),
                        new System.Drawing.Point(0, 0), new System.Drawing.Point(Width, Height));
                    break;
                case SpaceList.LeftBottom2RightTop:
                    e.Graphics.DrawLine(new System.Drawing.Pen(ForeColor, bold),
                        new System.Drawing.Point(Width, 0), new System.Drawing.Point(0, Height));
                    break;
            }
            base.OnPaint(e);
        }
    }
}
