using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace All.Control
{
    public partial class Delay : System.Windows.Forms.Control
    {
        public Delay()
        {
            InitializeComponent();
        }
        int index = 0;
        public void Flush()
        {
            index++;
            this.Invalidate();
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            index = (index % 8);
            System.Drawing.Image tmp = new System.Drawing.Bitmap(1, 1);
            switch (index)
            {
                case 0:
                    tmp = All.Properties.Resources._1;
                    break;
                case 1:
                    tmp = All.Properties.Resources._2;
                    break;
                case 2:
                    tmp = All.Properties.Resources._3;
                    break;
                case 3:
                    tmp = All.Properties.Resources._4;
                    break;
                case 4:
                    tmp = All.Properties.Resources._5;
                    break;
                case 5:
                    tmp = All.Properties.Resources._6;
                    break;
                case 6:
                    tmp = All.Properties.Resources._7;
                    break;
                case 7:
                    tmp = All.Properties.Resources._8;
                    break;
            }
            e.Graphics.DrawImage(tmp, new System.Drawing.Rectangle(0, 0, this.Width, this.Height), new System.Drawing.Rectangle(0, 0, tmp.Width, tmp.Height), System.Drawing.GraphicsUnit.Pixel);
            base.OnPaint(e);
        }
    }
}
