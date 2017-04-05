using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace All.Control
{
    public partial class Shape : System.Windows.Forms.Control
    {
        public enum ShapeList
        {
            方形,
            圆形
        }
        ShapeList shapeValue = ShapeList.圆形;

        public ShapeList ShapeValue
        {
            get { return shapeValue; }
            set { shapeValue = value; ChangeValue(); }
        }
        public Shape()
        {
            InitializeComponent();
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            ChangeValue();
            base.OnSizeChanged(e);
        }
        private void ChangeValue()
        {
            if (Width > 0 && Height > 0)
            {
                switch (shapeValue)
                {
                    case ShapeList.圆形:
                        System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
                        gp.AddEllipse(0, 0, Width, Width);
                        this.Region = new System.Drawing.Region(gp);
                        break;
                    case ShapeList.方形:
                        this.Region = null;
                        break;
                }
            }
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {

            base.OnPaint(e);
        }
    }
}
