using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace All.Window
{
    public partial class MetroMessageBoxOld : Form
    {
        public enum Button
        {
            OK,
            OKCancel,
            YesNo
        }
        public string LblText
        {
            get { return lblStr.Text; }
            set { lblStr.Text = value; }
        }
        public static DialogResult Show(string text)
        {
            return Show(text, "");
        }
        public static DialogResult Show(string text, string title)
        {
            return Show(text, title, Button.OK);
        }
        public static DialogResult Show(string text, string title, Button button)
        {
            using (MetroMessageBoxOld MM = new MetroMessageBoxOld())
            {
                MM.LblText = text;
                MM.Text = title;
                switch (button)
                {
                    case Button.OK:
                        MM.btn1.Text = "确定";
                        MM.btn1.Left = MM.Width / 2 - MM.btn1.Width / 2;
                        MM.btn1.Visible = true;
                        break;
                    case Button.OKCancel:
                        MM.btn1.Text = "确定";
                        MM.btn1.Left = MM.Width / 2 - MM.btn1.Width - 10;
                        MM.btn1.Visible = true;
                        MM.btn2.Text = "取消";
                        MM.btn2.Left = MM.Width / 2 + 10;
                        MM.btn2.Visible = true;
                        break;
                    case Button.YesNo:
                        MM.btn1.Text = "是";
                        MM.btn1.Left = MM.Width / 2 - MM.btn1.Width - 10;
                        MM.btn1.Visible = true;
                        MM.btn2.Text = "否";
                        MM.btn2.Left = MM.Width / 2 + 10;
                        MM.btn2.Visible = true;
                        break;
                }
                return MM.ShowDialog();
            }
        }
        StringFormat sf = new StringFormat();
        public MetroMessageBoxOld()
        {
            InitializeComponent();
        }

        private void MetroMessageBox_Paint(object sender, PaintEventArgs e)
        {

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.DrawRectangle(new Pen(Color.DodgerBlue, 5), new Rectangle(2, 2, Width - 5, Height - 5));
            e.Graphics.FillRectangle(new SolidBrush(Color.DodgerBlue), new Rectangle(0, 0, Width, 32));

            e.Graphics.DrawLine(new Pen(Color.White, 3), new Point(Width - 40, 8), new Point(Width - 30, 18));
            e.Graphics.DrawLine(new Pen(Color.White, 3), new Point(Width - 40, 18), new Point(Width - 30, 8));

            e.Graphics.DrawString(this.Text, new Font("Segoe UI", 10, FontStyle.Bold), new SolidBrush(Color.White), new RectangleF(15, 0, Width - 50, 32), sf);
        }
        protected override void OnTextChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnTextChanged(e);
        }


        bool isMouseDown = false;
        Point oldMousePoint = Point.Empty;
        Point oldWindowPoint = Point.Empty;
        private void MetroMessageBox_Load(object sender, EventArgs e)
        {
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {

            if (CloseRect == Rectangle.Empty)
            {
                CloseRect = new Rectangle(Width - 60, 0, 50, 25);
            }
            if (!CloseRect.Contains(e.Location))
            {
                isMouseDown = true;
                oldMousePoint = this.PointToScreen(e.Location);
                oldWindowPoint = this.Location;
                this.Cursor = Cursors.SizeAll;
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            isMouseDown = false;
            oldMousePoint = Point.Empty;
            oldWindowPoint = Point.Empty;
            this.Cursor = Cursors.Default;
            base.OnMouseUp(e);
        }


        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (CloseRect == Rectangle.Empty)
            {
                CloseRect = new Rectangle(Width - 70, 0, 50, 25);
            }
            if (CloseRect.Contains(e.Location))
            {
                this.Close();
            }
            base.OnMouseClick(e);
        }

        bool CloseIn = false;
        Rectangle CloseRect = Rectangle.Empty;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point nowMousePoint = this.PointToScreen(e.Location);
                int x = oldWindowPoint.X + nowMousePoint.X - oldMousePoint.X;
                int y = oldWindowPoint.Y + nowMousePoint.Y - oldMousePoint.Y;
                this.Location = new Point(x, y);
            }
            if (CloseRect == Rectangle.Empty)
            {
                CloseRect = new Rectangle(Width - 60, 0, 50, 25);
            }
            if (CloseRect.Contains(e.Location))
            {
                if (!CloseIn)
                {
                    using (Graphics g = Graphics.FromHwnd(this.Handle))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.FillRectangle(new SolidBrush(Color.Red), CloseRect);

                        g.DrawLine(new Pen(Color.White, 3), new Point(Width - 40, 8), new Point(Width - 30, 18));
                        g.DrawLine(new Pen(Color.White, 3), new Point(Width - 40, 18), new Point(Width - 30, 8));
                    }
                }
                CloseIn = true;
            }
            else
            {
                if (CloseIn)
                {
                    using (Graphics g = Graphics.FromHwnd(this.Handle))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.FillRectangle(new SolidBrush(Color.DodgerBlue), new Rectangle(CloseRect.Left - 1, CloseRect.Top - 1, CloseRect.Width + 2, CloseRect.Height + 2));

                        g.DrawLine(new Pen(Color.White, 3), new Point(Width - 40, 8), new Point(Width - 30, 18));
                        g.DrawLine(new Pen(Color.White, 3), new Point(Width - 40, 18), new Point(Width - 30, 8));
                    }
                }
                CloseIn = false;
            }
            base.OnMouseMove(e);
        }

        private void btn1_Click(object sender, EventArgs e)
        {

            if (btn1.Text.IndexOf("确定") >= 0)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            if (btn1.Text.IndexOf("是") >= 0)
            {
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }
        }

        private void btn2_Click(object sender, EventArgs e)
        {

            if (btn2.Text.IndexOf("取消") >= 0)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            if (btn2.Text.IndexOf("否") >= 0)
            {
                this.DialogResult = DialogResult.No;
                this.Close();
            }
        }
    }
}
