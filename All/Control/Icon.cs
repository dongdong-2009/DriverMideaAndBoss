using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Windows.Forms;
namespace All.Control
{
    public partial class Icon : System.Windows.Forms.Control
    {
        Color clearColor = Color.Black;
        /// <summary>
        /// 清除的图标颜色
        /// </summary>
        [Description("清除的图标颜色")]
        [Category("Shuai")]
        public Color ClearColor
        {
            get { return clearColor; }
            set { clearColor = value; AllFlushColor.Clear(); this.Invalidate(); }
        }

        Color fillColor = Color.Green;
        /// <summary>
        /// 新填充颜色
        /// </summary>
        [Description("新填充颜色")]
        [Category("Shuai")]
        public Color FillColor
        {
            get { return fillColor; }
            set { fillColor = value;  this.Invalidate(); }
        }
        Bitmap picture = All.Properties.Resources.Air___01;
        /// <summary>
        /// 背景图
        /// </summary>
        [Description("背景图")]
        [Category("Shuai")]
        public Bitmap Picture
        {
            get { return picture; }
            set { picture = value; AllFlushColor.Clear(); this.Invalidate(); }
        }
        bool autoHeight = true;
        /// <summary>
        /// 自动调整高度
        /// </summary>
        [Description("自动调整高度")]
        [Category("Shuai")]
        public bool AutoHeight
        {
            get { return autoHeight; }
            set { autoHeight = value; AllFlushColor.Clear(); this.Invalidate(); }
        }
        bool board = true;
        /// <summary>
        /// 是否自动画边框
        /// </summary>
        [Description("是否自动画边框")]
        [Category("Shuai")]
        public bool Board
        {
            get { return board; }
            set { board = value; this.Invalidate(); }
        }
        bool saveNewColor = true;
        /// <summary>
        /// 为true时，将每一颜色所绘图片自动保存到内存，防止快速刷新改变颜色时占用大量CPU来计算点颜色
        /// </summary>
        [Description("为true时，将每一颜色所绘图片自动保存到内存，防止快速刷新改变颜色时占用大量CPU来计算点颜色")]
        [Category("Shuai")]
        public bool SaveNewColor
        {
            get { return saveNewColor; }
            set { saveNewColor = value; }
        }
        Bitmap backImage = null;

        /// <summary>
        /// 显示内容
        /// </summary>
        [Description("显示内容")]
        public enum ShowIconList
        {
            图像,
            文字
        }
        ShowIconList showIcon = ShowIconList.图像;

        /// <summary>
        /// 显示内容
        /// </summary>
        [Description("显示内容")]
        [Category("Shuai")]
        public ShowIconList ShowIcon
        {
            get { return showIcon; }
            set { showIcon = value; this.Invalidate(); }
        }

        string showNum = "";
        /// <summary>
        /// 显示数字，当显示数字值小于0时，默认显示图片，当显示数字值大于等于0时，默认显示数字
        /// </summary>
        [Description("显示数字，当显示数字值小于0时，默认显示图片，当显示数字值大于等于0时，默认显示数字")]
        [Category("Shuai")]
        public string ShowNum
        {
            get { return showNum; }
            set { showNum = value; this.Invalidate(); }
        }
        StringFormat sf = new StringFormat();
        public class OverFlushColor
        {
            public Color FillColor
            { get; set; }
            public bool Board
            { get; set; }
            public Image Value
            { get; set; }
            public ShowIconList ShowIcon
            { get; set; }
            public string ShowNum
            { get; set; }
            public OverFlushColor(Color color, bool board, Image value, ShowIconList showIcon,string showNum)
            {
                FillColor = color;
                Board = board;
                Value = value;
                ShowIcon = showIcon;
                ShowNum = showNum;
            }
        }
        List<OverFlushColor> AllFlushColor = new List<OverFlushColor>();
        public Icon()
        {
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();
        }
        ~Icon()
        {
            AllFlushColor.Clear();
            AllFlushColor = null;
        }
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                AllFlushColor.Clear();
                this.Invalidate();
                base.Font = value;
            }
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            if (autoHeight)
            {
                Height = Width;
            }
            backImage = new Bitmap(Width, Height);
            AllFlushColor.Clear();
            this.Invalidate();
            base.OnSizeChanged(e);
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs pe)
        {
            int index = AllFlushColor.FindIndex(
                draw =>
                {
                    bool result = (draw.FillColor == fillColor && draw.Board == this.board && draw.ShowIcon == this.showIcon);
                    switch (draw.ShowIcon)
                    {
                        case ShowIconList.文字:
                            result = result && (draw.ShowNum == this.ShowNum);
                            break;
                    }
                    return result;
                });
            if (index < 0)
            {
                backImage = new Bitmap(Width, Height);

                if (picture == null)
                {
                    picture = All.Properties.Resources.Air___01;
                }
                using (Graphics g = Graphics.FromImage(backImage))
                {
                    switch (showIcon)
                    {
                        case ShowIconList.图像:
                            for (int i = 0; i < picture.Width; i++)
                            {
                                for (int j = 0; j < picture.Height; j++)
                                {
                                    if (picture.GetPixel(i, j).ToArgb() != clearColor.ToArgb())
                                    {
                                        picture.SetPixel(i, j, fillColor);
                                    }
                                }
                            }
                            g.DrawImage(picture, new Rectangle(0, 0, backImage.Width, backImage.Height), new Rectangle(0, 0, picture.Width, picture.Height), GraphicsUnit.Pixel);
                            break;
                        case ShowIconList.文字:
                            g.DrawString(showNum, this.Font, new SolidBrush(fillColor), new Rectangle(0, 0, Width, Height), sf);
                            break;
                    }
                    if (board)
                    {
                        g.DrawRectangle(new Pen(fillColor, 4), 1, 1, Width - 2, Height - 2);
                    }
                    if (saveNewColor)
                    {
                        AllFlushColor.Add(new OverFlushColor(fillColor, board, backImage, showIcon,showNum));
                    }
                }
                pe.Graphics.DrawImageUnscaled(backImage, 0, 0, Width, Height);
            }
            else
            {
                pe.Graphics.DrawImageUnscaled(AllFlushColor[index].Value, 0, 0);
                //g.DrawImage(AllFlushColor[index].Value, new Rectangle(0, 0, Width, Height), new Rectangle(0, 0, AllFlushColor[index].Value.Width, AllFlushColor[index].Value.Height), GraphicsUnit.Pixel);
            }
            base.OnPaint(pe);
        }
    }
}
