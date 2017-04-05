using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;
using System.Security;
namespace All.Window
{
    public partial class BaseWindow : Form, Class.ChangeSytle
    {

        static Color boardColor = All.Class.Style.Color;


        [Browsable(false)]
        [ReadOnly(true)]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set 
            {
            }
        }
      
        public BaseWindow()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.UserPaint, true);
            ChangeTheme(All.Class.Style.Theme);
        }
        protected override void OnLoad(EventArgs e)
        {
            All.Class.Style.Add(this);
            base.OnLoad(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            All.Class.Style.Remove(this);
            base.OnClosed(e);
        }
        public void ChangeColor(Color color)
        {
            boardColor = color;
            this.Invalidate();
        }
        public void ChangeTheme(All.Class.Style.Themes theme)
        {
            switch (theme)
            {
                case Class.Style.Themes.Light:
                    base.BackColor = Color.White;
                    break;
                default:
                    base.BackColor = Color.Black;
                    break;
            }
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.Close();
            }
            base.OnKeyPress(e);
        }
        private void BaseWindow_Load(object sender, EventArgs e)
        {
        }
        private void BaseWindow_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(boardColor, 4), 2, 2, Width - 4, Height - 4);
        }



    }
}
