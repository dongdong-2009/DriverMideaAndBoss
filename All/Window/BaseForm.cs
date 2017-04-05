using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Microsoft.Win32;
namespace All.Window
{
    public partial class BaseForm : System.Windows.Forms.Form
    {
        bool isExitButton = true;

        public bool IsExitButton
        {
            get { return isExitButton; }
            set { isExitButton = value; this.Invalidate(); }
        }
        bool isMaxButton = true;

        public bool IsMaxButton
        {
            get { return isMaxButton; }
            set { isMaxButton = value; this.Invalidate(); }
        }

        bool isMinButton = true;

        public bool IsMinButton
        {
            get { return isMinButton; }
            set { isMinButton = value; this.Invalidate(); }
        }


        private static Bitmap imgCloseEnter;
        private static Bitmap imgMaxEnter;
        private static Bitmap imgMinEnter;
        private static Bitmap imgRestoreEnter;
        private static Bitmap imgCloseLevel;
        private static Bitmap imgMaxLevel;
        private static Bitmap imgMinLevel;
        private static Bitmap imgRestoreLevel;

        Rectangle rectLeftTop;
        Rectangle rectTop;
        Rectangle rectRightTop;
        Rectangle rectRight;
        Rectangle rectRightBottom;
        Rectangle rectBottom;
        Rectangle rectLeftBottom;
        Rectangle rectLeft;


        //窗体移动
        bool isMouseDown = false;
        Point oldPoint = new Point();
        Rectangle oldRect = new Rectangle();
        //ICON位置
        Rectangle recIcon = new Rectangle(6, 8, 23, 23);
        public BaseForm()
        {
            InitializeComponent();
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            this.SetStyle(ControlStyles.UserPaint, true);//自绘
            this.SetStyle(ControlStyles.DoubleBuffer, true);// 双缓冲
            this.SetStyle(ControlStyles.ResizeRedraw, true);//调整大小时重绘
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);// 双缓冲
       }

        private void BaseForm_Load(object sender, EventArgs e)
        {
            //this.BackColor = Color.DarkBlue;
            //this.BackgroundImage = ClassBySS.Source.BeautifulImage.GetImage(ClassBySS.Source.BeautifulImage.ImageList.Green);
        }
        private void PicExit_MouseMove(object sender, MouseEventArgs e)
        {
            PicExit.Image = imgCloseEnter;
        }
        private void PicExit_MouseLeave(object sender, EventArgs e)
        {
            PicExit.Image = imgCloseLevel;
        }
        private void PicMax_MouseMove(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                PicMax.Image = imgRestoreEnter;
            }
            else 
            {
                PicMax.Image = imgMaxEnter;
            }
        }
        private void PicMax_MouseLeave(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                PicMax.Image = imgRestoreLevel;
            }
            else
            {
                PicMax.Image = imgMaxLevel;
            }
        }

        private void PicMin_MouseMove(object sender, MouseEventArgs e)
        {
            PicMin.Image = imgMinEnter;
        }

        private void PicMin_MouseLeave(object sender, EventArgs e)
        {
            PicMin.Image = imgMinLevel;
        }
        private void DrawBtnImage(ref Bitmap image,PictureBox pic, Color color1, Color color2, string str, string font)
        {
            System.Drawing.StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            LinearGradientBrush lgb;
            if (image == null)
            {
                image = new Bitmap(pic.Width, pic.Height);
                using (Graphics g = Graphics.FromImage(image))
                {
                    lgb = new LinearGradientBrush(PicExit.ClientRectangle,color2,color1, LinearGradientMode.Vertical);
                    g.FillRectangle(lgb, pic.ClientRectangle);
                    g.DrawString(Convert.ToString(Convert.ToChar(Convert.ToByte(str, 16))),
                        new Font(font, 17, FontStyle.Regular),
                        new SolidBrush(Color.White), new PointF(22, 0), sf);
                }
            }
        }
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (recIcon.Contains(e.Location))
            {
                this.Close();
            }
            base.OnMouseDoubleClick(e);
        }
        protected override void OnTextChanged(EventArgs e)
        {
            this.Invalidate(new Rectangle(30, 6, this.Width, 25));
            base.OnTextChanged(e);
        }
        protected void SystemBtnSet()
        {
            DrawBtnImage(ref imgCloseLevel, PicExit, Color.FromArgb(255, 128, 128), Color.Red, "0x72", "Webdings");
            DrawBtnImage(ref imgCloseEnter, PicExit, Color.Red, Color.FromArgb(192, 0, 0), "0x72", "Webdings");
            DrawBtnImage(ref imgMaxLevel, PicMax, Color.FromArgb(128,128,255), Color.Blue, "0x31", "Webdings");
            DrawBtnImage(ref imgMaxEnter, PicMax, Color.Blue, Color.FromArgb(0, 0, 192), "0x31", "Webdings");
            DrawBtnImage(ref imgRestoreLevel, PicMax, Color.FromArgb(128, 128, 255), Color.Blue, "0x32", "Webdings");
            DrawBtnImage(ref imgRestoreEnter, PicMax, Color.Blue, Color.FromArgb(0, 0, 192), "0x32", "Webdings");
            DrawBtnImage(ref imgMinLevel, PicMin, Color.FromArgb(128, 128, 255), Color.Blue, "0x30", "Webdings");
            DrawBtnImage(ref  imgMinEnter, PicMin, Color.Blue, Color.FromArgb(0, 0, 192), "0x30", "Webdings");
            int leftPoint = this.Width - 6;
            if (isExitButton)
            {
                leftPoint = leftPoint - 1 - PicExit.Width;
                PicExit.Left = leftPoint;
                PicExit.Visible = true;
            }
            else
            {
                PicExit.Visible = false;
            }
            PicExit.Image = imgCloseLevel;


            if (isMaxButton)
            {
                leftPoint = leftPoint - 1 - PicMax.Width;
                PicMax.Left = leftPoint;
                PicMax.Visible = true;
            }
            else
            {
                PicMax.Visible = false;
            }
            if (this.WindowState == FormWindowState.Normal)
            {
                PicMax.Image = imgMaxLevel;
            }
            else
            {
                PicMax.Image = imgRestoreLevel;
            }

            if (isMinButton)
            {
                leftPoint = leftPoint - 1 - PicMin.Width;
                PicMin.Left = leftPoint;
                PicMin.Visible = true;
            }
            else
            {
                PicMin.Visible = false;
            }
            PicMin.Image = imgMinLevel;


            //此区域 改变鼠标形态,用于更改窗体大小
            rectLeftTop = new Rectangle(0, 0, 2, 2);
            rectTop = new Rectangle(3, 0, Width - 6, 2);
            rectRightTop = new Rectangle(Width - 2, 0, 2, 2);
            rectRight = new Rectangle(Width - 2, 3, 2, Height - 6);
            rectRightBottom = new Rectangle(Width - 2, Height - 2, 2, 2);
            rectBottom = new Rectangle(3, Height - 2, Width - 6, 2);
            rectLeftBottom = new Rectangle(0, Height - 2, 2, 2);
            rectLeft = new Rectangle(0, 3, 2, Height - 6);


        }
        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            SystemBtnSet();
            base.OnInvalidated(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {

            //System.Drawing.TextureBrush tb = new TextureBrush(ClassBySS.Source.BeautifulImage.GetImage(ClassBySS.Source.BeautifulImage.ImageList.Green));
            //e.Graphics.FillRectangle(tb, this.ClientRectangle);
            //Image tmp=ClassBySS.Source.BeautifulImage.GetImage(ClassBySS.Source.BeautifulImage.ImageList.Green);
            //e.Graphics.DrawImage(tmp, new Rectangle(0, 0, Width, Height), new Rectangle(0, 0, tmp.Width, tmp.Height),GraphicsUnit.Pixel);


            //画一个看上去立体的边框,不然界面像纸片一样
            e.Graphics.DrawLine(new Pen(new SolidBrush(SystemColors.ControlDarkDark)), 1, 1, 1, Height - 2);
            e.Graphics.DrawLine(new Pen(new SolidBrush(SystemColors.ControlDarkDark)), 1, 1, Width - 2, 1);

            e.Graphics.DrawLine(new Pen(new SolidBrush(SystemColors.ControlLightLight)), 1, Height - 2, Width - 2, Height - 2);
            e.Graphics.DrawLine(new Pen(new SolidBrush(SystemColors.ControlLightLight)), Width - 2, 1, Width - 2, Height - 2);

            e.Graphics.DrawLine(new Pen(new SolidBrush(SystemColors.ButtonShadow)), 0, 0, 0, Height - 1);
            e.Graphics.DrawLine(new Pen(new SolidBrush(SystemColors.ButtonShadow)), 0, 0, Width - 1, 0);

            e.Graphics.DrawLine(new Pen(new SolidBrush(SystemColors.ButtonShadow)), 0, Height - 1, Width - 1, Height - 1);
            e.Graphics.DrawLine(new Pen(new SolidBrush(SystemColors.ButtonShadow)), Width - 1, 0, Width - 1, Height - 1);



            if (this.Icon != null && this.ShowIcon)
            {
                e.Graphics.DrawImage(this.Icon.ToBitmap(), recIcon);
            }
            e.Graphics.DrawString(this.Text, new Font("微软雅黑", 14, FontStyle.Bold), new SolidBrush(Color.Black), new PointF(30, 6));
        }

        private void PicMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void PicMax_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                PicMax.Image = imgMaxEnter;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                PicMax.Image = imgRestoreEnter;
            }
        }

        private void PicExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Normal && !recIcon.Contains(e.Location))
            {
                isMouseDown = true;
                oldPoint = PointToScreen(e.Location);
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isMouseDown)
            {
                if (oldRect != Rectangle.Empty)
                {
                    ControlPaint.DrawReversibleFrame(oldRect, Color.Black, FrameStyle.Dashed);
                }
                Rectangle r = new Rectangle(PointToScreen(e.Location).X - oldPoint.X + Left - 1, PointToScreen(e.Location).Y - oldPoint.Y + Top - 1, this.Width + 2, this.Height + 2);
                ControlPaint.DrawReversibleFrame(r, Color.Black, FrameStyle.Dashed);
                oldRect = r;
            }
            else
            {
                if (rectLeftTop.Contains(e.Location))
                {
                    Cursor = Cursors.SizeNWSE;
                }
                else if (rectTop.Contains(e.Location))
                {
                    Cursor = Cursors.SizeNS;
                }
                else if (rectRightTop.Contains(e.Location))
                {
                    Cursor = Cursors.SizeNESW;
                }
                else if (rectRight.Contains(e.Location))
                {
                    Cursor = Cursors.SizeWE;
                }
                else if (rectRightBottom.Contains(e.Location))
                {
                    Cursor = Cursors.SizeNWSE;
                }
                else if (rectBottom.Contains(e.Location))
                {
                    Cursor = Cursors.SizeNS;
                }
                else if (rectLeftBottom.Contains(e.Location))
                {
                    Cursor = Cursors.SizeNESW;
                }
                else if (rectLeft.Contains(e.Location))
                {
                    Cursor = Cursors.SizeWE;
                }
                else
                {
                    Cursor = Cursors.Default;
                }
            }
            base.OnMouseMove(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (isMouseDown && !recIcon.Contains(e.Location))
            {
                isMouseDown = false;
                ControlPaint.DrawReversibleFrame(oldRect, Color.Black, FrameStyle.Dashed);
                oldRect = Rectangle.Empty;
                this.Location = new Point(PointToScreen(e.Location).X - (oldPoint.X - Left), PointToScreen(e.Location).Y - (oldPoint.Y - Top));
            }
            base.OnMouseUp(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            Cursor = Cursors.Default;
            base.OnMouseLeave(e);
        }
    }
}
