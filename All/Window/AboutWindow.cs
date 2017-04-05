using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace All.Window
{
    public partial class AboutWindow : Form
    {
        public AboutWindow(string title,string code,string company,string englishCompany,string address)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            SetStyle(ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();
            lblTitle.Text = title;
            lblCode.Text = code;
            lblCompany.Text = company;
            lblCompanyEnglish.Text = englishCompany;
            lblAddress.Text = address;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AboutWindow_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.Close();
            }
        }

        private void btnClose_Enter(object sender, EventArgs e)
        {
        }

        private void btnClose_Leave(object sender, EventArgs e)
        {
        }

        private void btnClose_MouseEnter(object sender, EventArgs e)
        {
            btnClose.BackColor = Color.Red;
            btnClose.ForeColor = Color.White;
        }

        private void btnClose_MouseLeave(object sender, EventArgs e)
        {
            btnClose.BackColor = lblFormName.BackColor;
            btnClose.ForeColor = Color.Black;
        }

        bool down = false;
        Point startMouse = Point.Empty;
        Point startWindow = Point.Empty;
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!down)
            {
                down = true;
                startMouse = this.PointToScreen(e.Location);
                startWindow = this.Location;
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (down)
            {
                Point now = this.PointToScreen(e.Location);
                int x = now.X - startMouse.X;
                int y = now.Y - startMouse.Y;
                this.Location = new Point(startWindow.X + x, startWindow.Y + y);
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            down = false;
        }
    }
}
