using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
namespace All.Control.Metro
{
    public partial class MoveButton : System.Windows.Forms.UserControl
    {
        int delayTime = 3000;//停留时间

        Color foreColor1 = Color.Black;
        Color foreColor2 = Color.Blue;

        Color backColor1 = Color.Pink;
        Color backColor2 = Color.LightCyan;

        string title1 = "测试文字";
        string title2 = "点击此处";

        string value1 = "测试轮播按钮";
        string value2 = "点击此处有惊喜";

        Font font1 = new Font("Segoe UI", 16, FontStyle.Bold);
        Font font2 = new Font("Segoe UI", 12, FontStyle.Bold);

        Bitmap backImage1;
        Bitmap backImage2;


        public MoveButton()
        {
            this.SetStyle(System.Windows.Forms.ControlStyles.UserPaint | System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer | System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
            this.Update();

            InitializeComponent();
        }
        
        private void DrawBackImage()
        {
            if (backImage1 == null)
            {
                backImage1 = new Bitmap(Width, Height);
            }
            if (backImage2 == null)
            {
                backImage2 = new Bitmap(Width, Height);
            }

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            using (Graphics g = Graphics.FromImage(backImage1))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.Clear(backColor1);
                g.DrawString(title1, font1, new SolidBrush(foreColor1), new Rectangle(0, 0, Width, Height * 2 / 3), sf);

                g.DrawString(value1, font2, new SolidBrush(foreColor2), new Rectangle(0, Height * 2 / 3, Width, Height / 3), sf);
            }
            using (Graphics g = Graphics.FromImage(backImage2))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.Clear(backColor2);
                g.DrawString(title2, font1, new SolidBrush(foreColor1), new Rectangle(0, 0, Width, Height * 2 / 3), sf);

                g.DrawString(value2, font2, new SolidBrush(foreColor2), new Rectangle(0, Height * 2 / 3, Width, Height / 3), sf);
            }
            sf.Dispose();
            pictureBox1.Image = backImage1;
            pictureBox2.Image = backImage2;
        }
        protected override void OnResize(EventArgs e)
        {
            stop = true;
            t1.Stop();
            t2.Stop();
            step = 0;
            backImage1 = new Bitmap(Width, Height);
            backImage2 = new Bitmap(Width, Height);
            DrawBackImage();
            pictureBox1.Size = this.Size;
            pictureBox2.Size = this.Size;
            pictureBox1.Location = new Point(0, Height);
            pictureBox2.Location = new Point(0, 0);
            stop = false;
            t1.Enabled = true;
            base.OnResize(e);
        }
        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }


        private void MoveButton_Load(object sender, EventArgs e)
        {
            t1.Enabled = true;
        }
        bool stop = false;
        int start = 0;
        int step = 0;
        void t1_Tick(object sender, EventArgs e)
        {
            if (stop)
                t1.Stop();
            if ((Environment.TickCount - start) > delayTime)
            {
                step = 0;
                t1.Enabled = false;
                t2.Enabled = true;
            }
        }
        void t2_Tick(object sender, EventArgs e)
        {
            if (stop)
                t2.Stop();
            step++;
            MoveControl();
            if (step >= Height)
            {
                start = Environment.TickCount;
                t1.Enabled = true;
                t2.Enabled = false;
            }            
        }

        private void MoveControl()
        {
            pictureBox1.Top = pictureBox1.Top - 1;
            pictureBox2.Top = pictureBox2.Top - 1;

            if ((pictureBox1.Top + pictureBox1.Height) <= 0)
            {
                pictureBox1.Top = Height;
                pictureBox2.Top = 0;
            }
            if ((pictureBox2.Top + pictureBox2.Height) <= 0)
            {
                pictureBox1.Top = 0;
                pictureBox2.Top = Height;
            }
        }
    }
}
