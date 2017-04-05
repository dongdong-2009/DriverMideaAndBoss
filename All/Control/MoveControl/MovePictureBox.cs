using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace All.Control.MoveControl
{
    public partial class MovePictureBox : System.Windows.Forms.PictureBox
    {
        MoveControl moveControl;

        public MoveControl MoveControl
        {
            get { return moveControl; }
            set { moveControl = value; }
        }
        public MovePictureBox()
        {
            InitializeComponent();
            moveControl = new MoveControl(this);
            this.MouseDown += moveControl._control_MouseDown;
            this.MouseMove += moveControl._control_MouseMove;
            this.Paint += moveControl._control_Paint;
            this.MouseClick += moveControl._control_MouseClick;
            this.MouseUp += moveControl._control_MouseUp;
            this.SizeChanged += moveControl._control_SizeChanged;
            this.Disposed += MovePictureBox_Disposed;
        }

        void MovePictureBox_Disposed(object sender, EventArgs e)
        {
            moveControl.Remove(moveControl);
        }
    }
}
