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
    public partial class PicButton : System.Windows.Forms.Control,All.Class.ChangeSytle
    {
        /// <summary>
        /// 样式
        /// </summary>
        public enum Styles
        {
            Button,
            Tabpage
        }
        Styles style = Styles.Button;
        [Category("Shuai")]
        [Description("样式")]
        /// <summary>
        /// 样式
        /// </summary>
        public Styles Style
        {
            get { return style; }
            set { style = value; this.Invalidate(); }
        }

        PictureBox tmpPicture = new PictureBox();
        [Category("Shuai")]
        [Description("绘制图片")]
        /// <summary>
        /// 绘制图片
        /// </summary>
        public Image BackImage
        {
            get { return tmpPicture.Image; }
            set { tmpPicture.Image = value; this.Invalidate(); }
        }
        [Category("Shuai")]
        [Description("图片绘制位置")]
        /// <summary>
        /// 图片绘制位置
        /// </summary>
        public override  RightToLeft RightToLeft
        {
            get { return base.RightToLeft; }
            set { base.RightToLeft = value; this.Invalidate(); }
        }
        Color picBackColor = Color.Black;
        [Category("Shuai")]
        [Description("图片位置背景色")]
        /// <summary>
        /// 图片位置背景色
        /// </summary>
        public Color PicBackColor
        {
            get { return picBackColor; }
            set { picBackColor = value; this.Invalidate(); }
        }

        bool border = false;
        [Category("Shuai")]
        [Description("显示边框")]
        /// <summary>
        /// 显示边框
        /// </summary>
        public bool Border
        {
            get { return border; }
            set { border = value; this.Invalidate(); }
        }
        [Category("Shuai")]
        [Description("按钮文本")]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                this.Invalidate();
            }
        }
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                this.Invalidate();
            }
        }
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                this.Invalidate();
            }
        }
        Color boardColor = All.Class.Style.Color;
        public void ChangeColor(Color color)
        {
            boardColor = color;
            this.Invalidate();
        }
        public void ChangeTheme(All.Class.Style.Themes theme)
        { }
        StringFormat sf = new StringFormat();
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            if (border)
            {
                e.Graphics.DrawRectangle(new Pen(boardColor, 2), new Rectangle(1, 1, Width - 2, Height - 2));
            }

            switch (RightToLeft)
            {
                case System.Windows.Forms.RightToLeft.No:
                    if (tmpPicture.Image != null && Width > Height)//有图片，先绘文字 ，后绘图
                    {
                        e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), new Rectangle(Height, 1, Width - Height, Height), sf);
                        e.Graphics.FillRectangle(new SolidBrush(picBackColor), new Rectangle(-1, -1, Height+1, Height+1));
                        e.Graphics.DrawImage(tmpPicture.Image, new Rectangle(0, 0, Height+1, Height), new Rectangle(0, 0, tmpPicture.Image.Width, tmpPicture.Image.Height), GraphicsUnit.Pixel);
                    }
                    else//无图片，直接居中绘制文字
                    {
                        e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), new Rectangle(0, 0, Width, Height), sf);
                    }
                    break;
                default:
                    if (tmpPicture.Image != null && Width > Height)//有图片，先绘文字 ，后绘图
                    {
                        e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), new Rectangle(0, 1, Width - Height, Height), sf);
                        e.Graphics.FillRectangle(new SolidBrush(picBackColor), new Rectangle(Width - Height, 0, Height, Height));
                        e.Graphics.DrawImage(tmpPicture.Image, new Rectangle(Width - Height, 0, Height, Height), new Rectangle(0, 0, tmpPicture.Image.Width, tmpPicture.Image.Height), GraphicsUnit.Pixel);
                    }
                    else//无图片，直接居中绘制文字
                    {
                        e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), new Rectangle(0, 0, Width, Height), sf);
                    }
                    break;
            }
            base.OnPaint(e);
        }
        bool isMouseDown = false;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (style == Styles.Button)
            {
                this.Location = new Point(this.Left + 1, this.Top + 1);
            }
            base.OnMouseDown(e);
            isMouseDown = true;
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (isMouseDown)
            {
                isMouseDown = false;
                if (style == Styles.Button)
                {
                    this.Location = new Point(this.Left - 1, this.Top - 1);
                }
            }
            base.OnMouseUp(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            if (isMouseDown)
            {
                isMouseDown = false;
                if (style == Styles.Button)
                {
                    this.Location = new Point(this.Left - 1, this.Top - 1);
                }
            }
            base.OnMouseLeave(e);
        }
        public PicButton()
        {
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            BackColor = SystemColors.Control;
            Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Regular);
            InitializeComponent();
            Class.Style.Add(this);
        }
        ~PicButton()
        {
            Class.Style.Remove(this);
        }

    }
}
