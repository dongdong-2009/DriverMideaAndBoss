using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Drawing;
using System.Windows.Forms;
namespace All.Control.MoveControl
{
    public class MoveControl
    {
        static List<MoveControl> allControl = new List<MoveControl>();

        bool isGetFocus = false;
        /// <summary>
        /// 是否获取焦点
        /// </summary>
        public bool IsGetFocus
        {
            get { return isGetFocus; }
            set { isGetFocus = value; }
        }
        System.Windows.Forms.Control backControl = new System.Windows.Forms.Control();

        public System.Windows.Forms.Control BackControl
        {
            get { return backControl; }
            set { backControl = value; }
        }

        bool isMouseDown = false;
        /// <summary>
        /// 是否鼠标左键按下
        /// </summary>
        public bool IsMouseDown
        {
            get { return isMouseDown; }
            set { isMouseDown = value; }
        }

        bool isTop = false;
        /// <summary>
        /// 最后一个选中
        /// </summary>
        public bool IsTop
        {
            get { return isTop; }
            set { isTop = value; }
        }
        static int count = 0;
        /// <summary>
        /// 当前选中个数
        /// </summary>
        public static int Count
        {
            get { return MoveControl.count; }
            set { MoveControl.count = value; }
        }

        /// <summary>
        /// 鼠标位置
        /// </summary>
        enum MouseLocalList
        {
            /// <summary>
            /// 东
            /// </summary>
            E,
            /// <summary>
            /// 南
            /// </summary>
            S,
            /// <summary>
            /// 西
            /// </summary>
            W,
            /// <summary>
            /// 北
            /// </summary>
            N,
            /// <summary>
            /// 东南
            /// </summary>
            ES,
            /// <summary>
            /// 西南
            /// </summary>
            WS,
            /// <summary>
            /// 西北
            /// </summary>
            WN,
            /// <summary>
            /// 东北
            /// </summary>
            EN,
            /// <summary>
            /// 中间
            /// </summary>
            M
        }

        Point startPoint = Point.Empty;
        Point currentPoint = Point.Empty;
        Rectangle startRect = Rectangle.Empty;

        Size littleRect = new Size(5, 5);
        RectangleF[] mRectangle = new RectangleF[9];
        MouseLocalList mouseLocalList = MouseLocalList.M;
        Size MinSize = new Size(20, 20);
        public MoveControl(System.Windows.Forms.Control control)
        {
            backControl = control;
            allControl.Add(this);
            initSize();
        }
        public void Remove(MoveControl control)
        {
            if (allControl.Contains(control))
            {
                allControl.Remove(control);
            }
        }
        private void initSize()
        {
            mRectangle[0] = new RectangleF(new PointF(0, 0), littleRect);
            mRectangle[1] = new RectangleF(new PointF((backControl.Width - littleRect.Width) / 2.0f, 0), littleRect);
            mRectangle[2] = new RectangleF(new PointF(backControl.Width - littleRect.Width - 1, 0), littleRect);
            mRectangle[3] = new RectangleF(new PointF(backControl.Width - littleRect.Width - 1, (backControl.Height - littleRect.Height) / 2.0f), littleRect);
            mRectangle[4] = new RectangleF(new PointF(backControl.Width - littleRect.Width - 1, backControl.Height - littleRect.Height - 1), littleRect);
            mRectangle[5] = new RectangleF(new PointF((backControl.Width - littleRect.Width) / 2.0f, backControl.Height - littleRect.Height - 1), littleRect);
            mRectangle[6] = new RectangleF(new PointF(0, backControl.Height - littleRect.Height - 1), littleRect);
            mRectangle[7] = new RectangleF(new PointF(0, (backControl.Height - littleRect.Height) / 2.0f), littleRect);
            mRectangle[8] = new RectangleF(0, 0, backControl.Width - 1, backControl.Height - 1);
        }
        public void _control_SizeChanged(object sender, EventArgs e)
        {
            initSize();
            backControl.Refresh();
        }
        public void _control_MouseClick(object sender, MouseEventArgs e)
        {
        }
        public void _control_MouseMove(object sender, MouseEventArgs e)
        {
            if (isGetFocus && !isMouseDown)//鼠标指针开关
            {
                if (mRectangle[0].Contains(e.Location))
                {
                    mouseLocalList = MouseLocalList.WN;
                    backControl.Cursor = Cursors.SizeNWSE;
                }
                else if (mRectangle[1].Contains(e.Location))
                {
                    mouseLocalList = MouseLocalList.N;
                    backControl.Cursor = Cursors.SizeNS;
                }
                else if (mRectangle[2].Contains(e.Location))
                {
                    mouseLocalList = MouseLocalList.EN;
                    backControl.Cursor = Cursors.SizeNESW;
                }
                else if (mRectangle[3].Contains(e.Location))
                {
                    mouseLocalList = MouseLocalList.E;
                    backControl.Cursor = Cursors.SizeWE;
                }
                else if (mRectangle[4].Contains(e.Location))
                {
                    mouseLocalList = MouseLocalList.ES;
                    backControl.Cursor = Cursors.SizeNWSE;
                }
                else if (mRectangle[5].Contains(e.Location))
                {
                    mouseLocalList = MouseLocalList.S;
                    backControl.Cursor = Cursors.SizeNS;
                }
                else if (mRectangle[6].Contains(e.Location))
                {
                    mouseLocalList = MouseLocalList.WS;
                    backControl.Cursor = Cursors.SizeNESW;
                }
                else if (mRectangle[7].Contains(e.Location))
                {
                    mouseLocalList = MouseLocalList.W;
                    backControl.Cursor = Cursors.SizeWE;
                }
                else if(mRectangle[8].Contains(e.Location))
                {
                    mouseLocalList = MouseLocalList.M;
                    backControl.Cursor = Cursors.SizeAll;
                }
            }
            if (isMouseDown)
            {
                if (startRect != Rectangle.Empty)//消除前一状态框
                {
                    ControlPaint.DrawReversibleFrame(
                                   startRect, Color.Black, FrameStyle.Dashed);
                }
                currentPoint = Cursor.Position;

                switch (mouseLocalList)
                {
                    case MouseLocalList.M://整体移动
                        startRect = new Rectangle(backControl.Parent.PointToScreen(backControl.Location).X + currentPoint.X - startPoint.X,
                                                  backControl.Parent.PointToScreen(backControl.Location).Y + currentPoint.Y - startPoint.Y,
                                                  backControl.Width, backControl.Height);
                        break;
                    case MouseLocalList.WN://西北
                        startRect = new Rectangle(backControl.Parent.PointToScreen(backControl.Location).X + currentPoint.X - startPoint.X,
                                                  backControl.Parent.PointToScreen(backControl.Location).Y + currentPoint.Y - startPoint.Y,
                                                  backControl.Width - currentPoint.X + startPoint.X,
                                                  backControl.Height - currentPoint.Y + startPoint.Y);
                        break;
                    case MouseLocalList.N:
                        startRect = new Rectangle(backControl.Parent.PointToScreen(backControl.Location).X,
                                                  backControl.Parent.PointToScreen(backControl.Location).Y + currentPoint.Y - startPoint.Y,
                                                  backControl.Width,
                                                  backControl.Height - currentPoint.Y + startPoint.Y);
                        break;
                    case MouseLocalList.EN:
                        startRect = new Rectangle(backControl.Parent.PointToScreen(backControl.Location).X,
                                                  backControl.Parent.PointToScreen(backControl.Location).Y + currentPoint.Y - startPoint.Y,
                                                  backControl.Width + currentPoint.X - startPoint.X,
                                                  backControl.Height - currentPoint.Y + startPoint.Y);
                        break;
                    case MouseLocalList.E:
                        startRect = new Rectangle(backControl.Parent.PointToScreen(backControl.Location).X,
                                                  backControl.Parent.PointToScreen(backControl.Location).Y,
                                                  backControl.Width + currentPoint.X - startPoint.X,
                                                  backControl.Height);
                        break;
                    case MouseLocalList.ES:
                        startRect = new Rectangle(backControl.Parent.PointToScreen(backControl.Location).X,
                                                  backControl.Parent.PointToScreen(backControl.Location).Y,
                                                  backControl.Width + currentPoint.X - startPoint.X,
                                                  backControl.Height + currentPoint.Y - startPoint.Y);
                        break;
                    case MouseLocalList.S:
                        startRect = new Rectangle(backControl.Parent.PointToScreen(backControl.Location).X,
                                                  backControl.Parent.PointToScreen(backControl.Location).Y,
                                                  backControl.Width,
                                                  backControl.Height + currentPoint.Y - startPoint.Y);
                        break;
                    case MouseLocalList.WS:
                        startRect = new Rectangle(backControl.Parent.PointToScreen(backControl.Location).X + currentPoint.X - startPoint.X,
                                                  backControl.Parent.PointToScreen(backControl.Location).Y,
                                                  backControl.Width - currentPoint.X + startPoint.X,
                                                  backControl.Height + currentPoint.Y - startPoint.Y);
                        break;
                    case MouseLocalList.W:
                        startRect = new Rectangle(backControl.Parent.PointToScreen(backControl.Location).X + currentPoint.X - startPoint.X,
                                                  backControl.Parent.PointToScreen(backControl.Location).Y,
                                                  backControl.Width - currentPoint.X + startPoint.X,
                                                  backControl.Height);
                        break;
                }

                ControlPaint.DrawReversibleFrame(
                                startRect, Color.Black, FrameStyle.Dashed);
            }
        }

        public void _control_MouseUp(object sender, MouseEventArgs e)
        {
            if (startRect != Rectangle.Empty)
            {
                ControlPaint.DrawReversibleFrame(
                                startRect, Color.Black, FrameStyle.Dashed);
                startRect = Rectangle.Empty;
            }
            if (isMouseDown)
            {
                isMouseDown = false;
                switch (mouseLocalList)
                {
                    case MouseLocalList.M:
                        backControl.Location = new Point(Math.Max(0,backControl.Left + currentPoint.X - startPoint.X),
                                                    Math.Max(0,backControl.Top + currentPoint.Y - startPoint.Y));
                        break;
                    case MouseLocalList.WN:
                        backControl.Location = new Point(Math.Max(0, backControl.Left + currentPoint.X - startPoint.X),
                                                    Math.Max(0, backControl.Top + currentPoint.Y - startPoint.Y));
                        backControl.Size = new Size(Math.Max(MinSize.Width, backControl.Width + startPoint.X - currentPoint.X),
                                                    Math.Max(MinSize.Height, backControl.Height + startPoint.Y - currentPoint.Y));
                        break;
                    case MouseLocalList.N:
                        backControl.Location = new Point(Math.Max(0, backControl.Left),
                                                    Math.Max(0, backControl.Top + currentPoint.Y - startPoint.Y));
                        backControl.Size = new Size(Math.Max(MinSize.Width, backControl.Width),
                                                    Math.Max(MinSize.Height, backControl.Height + startPoint.Y - currentPoint.Y));
                        break;
                    case MouseLocalList.EN:
                        backControl.Location = new Point(Math.Max(0, backControl.Left),
                                                    Math.Max(0, backControl.Top + currentPoint.Y - startPoint.Y));
                        backControl.Size = new Size(Math.Max(MinSize.Width, backControl.Width - startPoint.X + currentPoint.X),
                                                    Math.Max(MinSize.Height, backControl.Height + startPoint.Y - currentPoint.Y));
                        break;
                    case MouseLocalList.E:
                        backControl.Size = new Size(Math.Max(MinSize.Width, backControl.Width - startPoint.X + currentPoint.X),
                                                    Math.Max(MinSize.Height, backControl.Height));
                        break;
                    case MouseLocalList.ES:
                        backControl.Size = new Size(Math.Max(MinSize.Width, backControl.Width - startPoint.X + currentPoint.X),
                                                    Math.Max(MinSize.Height, backControl.Height - startPoint.Y + currentPoint.Y));
                        break;
                    case MouseLocalList.S:
                        backControl.Size = new Size(Math.Max(MinSize.Width, backControl.Width),
                                                    Math.Max(MinSize.Height, backControl.Height - startPoint.Y + currentPoint.Y));
                        break;
                    case MouseLocalList.WS:
                        backControl.Location = new Point(Math.Max(0, backControl.Left + currentPoint.X - startPoint.X),
                                                    Math.Max(0, backControl.Top));
                        backControl.Size = new Size(Math.Max(MinSize.Width, backControl.Width + startPoint.X - currentPoint.X),
                                                    Math.Max(MinSize.Height, backControl.Height - startPoint.Y + currentPoint.Y));
                        break;
                    case MouseLocalList.W:
                        backControl.Location = new Point(Math.Max(0, backControl.Left + currentPoint.X - startPoint.X),
                                                    Math.Max(0, backControl.Top));
                        backControl.Size = new Size(Math.Max(MinSize.Width, backControl.Width + startPoint.X - currentPoint.X),
                                                    Math.Max(MinSize.Height, backControl.Height));
                        break;
                }
            }
            backControl.Refresh();
        }
        public void _control_Paint(object sender, PaintEventArgs e)
        {
            if (isGetFocus)
            {
                if (!isTop)
                {
                    e.Graphics.DrawRectangles(new Pen(Color.Black, 1), mRectangle);
                }
                else
                {
                    for (int i = 0; i < mRectangle.Length - 1; i++)
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.Black), mRectangle[i]);
                    }
                    e.Graphics.DrawRectangles(new Pen(Color.Black, 1), mRectangle);
                }
            }
        }
        public void _control_MouseDown(object sender, MouseEventArgs e)
        {
            if (System.Windows.Forms.Control.ModifierKeys != Keys.Control)//单选一个控件
            {
                foreach (MoveControl mc in allControl)
                {
                    if (mc.Equals(this))
                    {
                        isGetFocus = true;
                    }
                    else
                    {
                        mc.IsMouseDown = false;
                        mc.IsGetFocus = false;
                    }
                    mc.isTop = false;
                    mc.BackControl.Refresh();
                }
                count = 1;
            }
            else
            {
                count = 0;
                foreach (MoveControl mc in allControl)//选一堆控件
                {
                    if (mc.Equals(this))
                    {
                        isGetFocus = true;
                        isTop = true;
                    }
                    else
                    {
                        mc.isTop = false;
                    }
                    if (mc.isGetFocus)
                    {
                        count++;
                    }
                    mc.BackControl.Refresh();
                }
            }
            startPoint = Cursor.Position;
            currentPoint = Cursor.Position;
            isMouseDown = true;
        }
        /// <summary>
        /// 去图片上绘制点
        /// </summary>
        public static void ClearPaint()
        {
            foreach (MoveControl cc in allControl)
            {
                cc.IsGetFocus = false;
                cc.IsMouseDown = false;
                cc.BackControl.Refresh();
            }
            count = 0;
        }
        /// <summary>
        /// 顶端对齐
        /// </summary>
        public static void TopAlign()
        {
            System.Windows.Forms.Control TopControl = null;
            foreach (MoveControl cc in allControl)
            {
                if (cc.IsTop)
                {
                    TopControl = cc.BackControl;
                }
            }
            if (count >= 2 && TopControl != null)//2个或2个以上的控件才有对齐功能
            {
                Point topControlPoint = TopControl.Parent.PointToScreen(TopControl.Location);
                foreach (MoveControl cc in allControl)
                {
                    if (cc.isGetFocus)
                    {
                        cc.BackControl.Top = cc.BackControl.Parent.PointToClient(topControlPoint).Y;
                    }
                }
            }
        }
        /// <summary>
        /// 左对齐
        /// </summary>
        public static void LeftAlign()
        {
            System.Windows.Forms.Control TopControl = null;
            foreach (MoveControl cc in allControl)
            {
                if (cc.IsTop)
                {
                    TopControl = cc.BackControl;
                }
            }
            if (count >= 2 && TopControl != null)//2个或2个以上的控件才有对齐功能
            {
                Point topControlPoint = TopControl.Parent.PointToScreen(TopControl.Location);
                foreach (MoveControl cc in allControl)
                {
                    if (cc.isGetFocus)
                    {
                        cc.BackControl.Left = cc.BackControl.Parent.PointToClient(topControlPoint).X;
                    }
                }
            }
        }
        /// <summary>
        /// 右对齐
        /// </summary>
        public static void RightAlign()
        {
            System.Windows.Forms.Control TopControl = null;
            foreach (MoveControl cc in allControl)
            {
                if (cc.IsTop)
                {
                    TopControl = cc.BackControl;
                }
            }
            if (count >= 2 && TopControl != null)//2个或2个以上的控件才有对齐功能
            {
                Point topControlPoint = TopControl.Parent.PointToScreen(new Point(TopControl.Location.X + TopControl.Width, 0));
                foreach (MoveControl cc in allControl)
                {
                    if (cc.isGetFocus)
                    {
                        cc.BackControl.Left = cc.BackControl.Parent.PointToClient(topControlPoint).X - cc.BackControl.Width;
                    }
                }
            }
        }
        /// <summary>
        /// 底部对齐
        /// </summary>
        public static void BottonAlign()
        {
            System.Windows.Forms.Control TopControl = null;
            foreach (MoveControl cc in allControl)
            {
                if (cc.IsTop)
                {
                    TopControl = cc.BackControl;
                }
            }
            if (count >= 2 && TopControl != null)//2个或2个以上的控件才有对齐功能
            {
                Point topControlPoint = TopControl.Parent.PointToScreen(new Point(0,TopControl.Location.Y + TopControl.Height));
                foreach (MoveControl cc in allControl)
                {
                    if (cc.isGetFocus)
                    {
                        cc.BackControl.Top = cc.BackControl.Parent.PointToClient(topControlPoint).Y - cc.BackControl.Height;
                    }
                }
            }
        }
        /// <summary>
        /// 垂直对齐
        /// </summary>
        public static void VerticalAlign()
        {
            System.Windows.Forms.Control TopControl = null;
            foreach (MoveControl cc in allControl)
            {
                if (cc.IsTop)
                {
                    TopControl = cc.BackControl;
                }
            }
            if (count >= 2 && TopControl != null)//2个或2个以上的控件才有对齐功能
            {
                Point topControlPoint = TopControl.Parent.PointToScreen(new Point(TopControl.Location.X + TopControl.Width / 2, 0));
                foreach (MoveControl cc in allControl)
                {
                    if (cc.isGetFocus)
                    {
                        cc.BackControl.Left = cc.BackControl.Parent.PointToClient(topControlPoint).X - cc.BackControl.Width / 2;
                    }
                }
            }
        }
        /// <summary>
        /// 中间对齐
        /// </summary>
        public static void CenterAlign()
        {
            System.Windows.Forms.Control TopControl = null;
            foreach (MoveControl cc in allControl)
            {
                if (cc.IsTop)
                {
                    TopControl = cc.BackControl;
                }
            }
            if (count >= 2 && TopControl != null)//2个或2个以上的控件才有对齐功能
            {
                Point topControlPoint = TopControl.Parent.PointToScreen(new Point(0, TopControl.Location.Y + TopControl.Height / 2));
                foreach (MoveControl cc in allControl)
                {
                    if (cc.isGetFocus)
                    {
                        cc.BackControl.Top = cc.BackControl.Parent.PointToClient(topControlPoint).Y - cc.BackControl.Height / 2;
                    }
                }
            }
        }
        /// <summary>
        /// 相同宽度
        /// </summary>
        public static void SameWidth()
        {
            System.Windows.Forms.Control TopControl = null;
            foreach (MoveControl cc in allControl)
            {
                if (cc.IsTop)
                {
                    TopControl = cc.BackControl;
                }
            }
            if (count >= 2 && TopControl != null)//2个或2个以上的控件才有对齐功能
            {
                foreach (MoveControl cc in allControl)
                {
                    if (cc.isGetFocus)
                    {
                        cc.BackControl.Width = TopControl.Width;
                    }
                }
            }
        }
        /// <summary>
        /// 相同高度
        /// </summary>
        public static void SameHeight()
        {
            System.Windows.Forms.Control TopControl = null;
            foreach (MoveControl cc in allControl)
            {
                if (cc.IsTop)
                {
                    TopControl = cc.BackControl;
                }
            }
            if (count >= 2 && TopControl != null)//2个或2个以上的控件才有对齐功能
            {
                foreach (MoveControl cc in allControl)
                {
                    if (cc.isGetFocus)
                    {
                        cc.BackControl.Height = TopControl.Height;
                    }
                }
            }
        }
        /// <summary>
        /// 相同大小
        /// </summary>
        public static void SameSize()
        {
            System.Windows.Forms.Control TopControl = null;
            foreach (MoveControl cc in allControl)
            {
                if (cc.IsTop)
                {
                    TopControl = cc.BackControl;
                }
            }
            if (count >= 2 && TopControl != null)//2个或2个以上的控件才有对齐功能
            {
                foreach (MoveControl cc in allControl)
                {
                    if (cc.isGetFocus)
                    {
                        cc.BackControl.Height = TopControl.Height;
                        cc.BackControl.Width = TopControl.Width;
                    }
                }
            }
        }
        public static void VerticalSpace()
        {
            System.Windows.Forms.Control TopControl = null;
            int minTop = 9999;
            int maxTop = 0;
            bool isInsert = false;
            List<MoveControl> tmpMoveControl = new List<MoveControl>();
            foreach (MoveControl cc in allControl)
            {
                if (cc.IsTop)
                {
                    TopControl = cc.BackControl;
                }
            }
            if (count >= 2 && TopControl != null)//2个或2个以上的控件才有对齐功能
            {
                foreach (MoveControl cc in allControl)
                {
                    if (cc.isGetFocus)
                    {
                        isInsert = false;
                        if (tmpMoveControl.Count > 0)
                        {
                            for (int i = 0; i < tmpMoveControl.Count; i++)
                            {
                                if (tmpMoveControl[i].BackControl.Parent.PointToScreen(tmpMoveControl[i].BackControl.Location).Y >
                                    cc.BackControl.Parent.PointToScreen(cc.BackControl.Location).Y)
                                {
                                    tmpMoveControl.Insert(i, cc);
                                    isInsert = true;
                                    break;
                                }
                            }
                        }
                        if (!isInsert)
                        {
                            tmpMoveControl.Add(cc);
                        }
                        if (cc.BackControl.Parent.PointToScreen(cc.BackControl.Location).Y < minTop)
                        {
                            minTop = cc.BackControl.Parent.PointToScreen(cc.BackControl.Location).Y;
                        }
                        if (cc.BackControl.Parent.PointToScreen(cc.BackControl.Location).Y > maxTop)
                        {
                            maxTop = cc.BackControl.Parent.PointToScreen(cc.BackControl.Location).Y;
                        }
                    }
                }
                float verticalSpace = (maxTop - minTop) / (float)(count - 1);
                foreach (MoveControl cc in allControl)
                {
                    if (cc.isGetFocus)
                    {
                        for (int i = 0; i < tmpMoveControl.Count; i++)
                        {
                            if (cc.Equals(tmpMoveControl[i]))
                            {
                                cc.BackControl.Top = cc.BackControl.Parent.PointToClient(new Point(cc.BackControl.Left, minTop + (int)(i * verticalSpace))).Y;
                                break;
                            }
                        }
                    }
                }
            }
        }
        public static void Horizontal()
        { }
    }
}
