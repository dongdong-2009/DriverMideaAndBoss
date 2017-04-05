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
    public partial class MainWindow : Form, Class.ChangeSytle
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
      
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                if (lblTitle != null)
                {
                    lblTitle.Text = value;
                }
                base.Text = value;
            }
        }
        Size oldMainSize = Size.Empty;
        public MainWindow()
        {
            this.KeyPreview = true;
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
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
        }
        private void MainWindow_Load(object sender, EventArgs e)
        {
            oldMainSize = this.Size;
            this.Left = 0;
            this.Top = 0;
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height;
            if (this.Controls.Find("panMain", false).Length > 0)
            {
                Panel panMain = (Panel)this.Controls.Find("panMain", false)[0];
                panMain.Left = Screen.PrimaryScreen.WorkingArea.Width / 2 - panMain.Width / 2;
                panMain.Top = Screen.PrimaryScreen.WorkingArea.Height / 2 - panMain.Height / 2 +
                              (int)(Screen.PrimaryScreen.WorkingArea.Height * 0.1f);
            }
            if (this.Controls.Find("panSize", false).Length > 0)
            {
                Panel panSize = (Panel)this.Controls.Find("panSize", false)[0];
                ChangeSize(panSize,false);
            }
            if (this.Controls.Find("panAllSize", false).Length > 0)
            {
                Panel panSize = (Panel)this.Controls.Find("panAllSize", false)[0];
                ChangeSize(panSize,true);
            }
        }
        private void ChangeSize(System.Windows.Forms.Control cc,bool sizeSon)
        {
            if (sizeSon)
            {
                //this.Scale(new SizeF((float)Screen.PrimaryScreen.Bounds.Width / oldMainSize.Width, (float)Screen.PrimaryScreen.Bounds.Height / oldMainSize.Height));
                foreach (System.Windows.Forms.Control son in cc.Controls)
                {
                    ChangeSize(son, true);
                }
            }
            //else
            //{
                cc.Left = (int)(cc.Left * ((float)Screen.PrimaryScreen.Bounds.Width / oldMainSize.Width));
                cc.Top = (int)(cc.Top * ((float)Screen.PrimaryScreen.Bounds.Height / oldMainSize.Height));
                cc.Width = (int)(cc.Width * ((float)Screen.PrimaryScreen.Bounds.Width / oldMainSize.Width));
                cc.Height = (int)(cc.Height * ((float)Screen.PrimaryScreen.Bounds.Height / oldMainSize.Height));
            //}
        }

        private void MainWindow_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(boardColor, 4), 2, 2, Width - 4, Height - 4);
        }

    }
}
