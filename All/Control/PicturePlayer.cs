using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
namespace All.Control
{
    public partial class PicturePlayer : System.Windows.Forms.Control
    {

        public const string FileFilter = "图片文件|*.jpg;*.bmp;*.png;*.jpeg|jpg文件|*.jpg|bmp文件|*.bmp|png文件|*.png|jpeg文件|*.jpeg";

        public delegate void PlayOverHandle();
        /// <summary>
        /// 一轮播放完成
        /// </summary>
        public event PlayOverHandle PlayOver;
        /// <summary>
        /// 刷新时间
        /// </summary>
        const int TickTime = 10;
        /// <summary>
        /// 变化时间
        /// </summary>
        private int changeTime = 1000;

        public int ChangeTime
        {
            get { return changeTime; }
            set { changeTime = value; }
        }
        /// <summary>
        /// 开始计时
        /// </summary>
        int startDelayTime = 0;
        private int delayTime = 5000;
        /// <summary>
        /// 停留时间
        /// </summary>
        public int DelayTime
        {
            get { return delayTime; }
            set { delayTime = value; }
        }
        Player.FlushMethod.FlushList flushList = Player.FlushMethod.FlushList.横向百业窗;
        /// <summary>
        /// 刷新方法
        /// </summary>
        public Player.FlushMethod.FlushList FlushList
        {
            get { return flushList; }
            set { flushList = value; }
        }
        List<Image> filePath = new List<Image>();

        public List<Image> FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }
        /// <summary>
        /// 当前是否正在播放
        /// </summary>
        bool run = false;
        System.Windows.Forms.Timer timFlush = new System.Windows.Forms.Timer();

        Image Image1 = null;//当前播放图片
        Image Image2 = null;
        /// <summary>
        /// 单张图片是否播放完成
        /// </summary>
        bool over = false;
        /// <summary>
        /// 当前播放的图片序号
        /// </summary>
        int picPlayIndex = -1;//当前播放序号
        /// <summary>
        /// 背景图
        /// </summary>
        Bitmap backImage = null;

        //刷新方法
        Player.FlushMethod flushMethod;

        public PicturePlayer()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();
            timFlush.Interval = TickTime;
            timFlush.Tick += timFlush_Tick;
        }
        ~PicturePlayer()
        {
            timFlush.Enabled = false;
            timFlush.Stop();
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            if (Width > 0 && Height > 0)
            {
                backImage = new Bitmap(Width, Height);
                this.Invalidate();
            }
            base.OnSizeChanged(e);
        }
        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            if (backImage == null)
            {
                backImage = new Bitmap(Width, Height);
                using (Graphics g = Graphics.FromImage(backImage))
                {
                    g.Clear(BackColor);
                }
            }
            e.Graphics.DrawImageUnscaled(backImage, 0, 0);
            //base.OnPaint(e);
        }
        public void Start()
        {
            if (!run && SetPic())
            {
                runOver = false;
                over = false;
                timFlush.Enabled = true;
                run = true;
            }
        }
        bool runOver = false;
        private void timFlush_Tick(object sender, EventArgs e)
        {
            if (run)
            {
                startDelayTime = 0;
                if (over)
                {
                    run = false;
                    over = false;
                }
                else
                {
                    flushMethod.Flush();
                    //this.Invalidate();
                }
            }
            else
            {
                if (startDelayTime == 0)
                {
                    startDelayTime = Environment.TickCount;
                }
                else
                {
                    if ((Environment.TickCount - startDelayTime) > delayTime)
                    {
                        Start();
                    }
                }
            }
            if (runOver)
            {
                timFlush.Stop();
                run = false;
                over = true;
                runOver = false;
                return;
            }
        }
        public void Stop()
        {
            runOver = true;
        }
        private void RunOver()
        {
            over = true;
            timFlush_Tick(timFlush, new EventArgs());
            flushMethod.Dispose();
        }
        /// <summary>
        /// 设置图片
        /// </summary>
        /// <returns></returns>
        private bool SetPic()
        {
            bool result = false;
            if (backImage == null)
            {
                backImage = new Bitmap(Width, Height);
            }
            if (filePath == null || filePath.Count <= 0)//无图片，则显示背景色
            {
                using (Graphics g = Graphics.FromImage(backImage))
                {
                    g.Clear(BackColor);
                }
                this.Invalidate();
                if (PlayOver != null)
                {
                    PlayOver();
                }
                return false;
            }
            else if (filePath.Count == 1)//单张图片，直接显示图片
            {
                using (Graphics g = Graphics.FromImage(backImage))
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.DrawImage(filePath[0], new Rectangle(0, 0, Width, Height),
                    new Rectangle(0, 0, filePath[0].Width, filePath[0].Height), GraphicsUnit.Pixel);
                    if (PlayOver != null)
                    {
                        PlayOver();
                        return false;
                    }
                }
            }
            else if (filePath.Count >= 2)//多张图片，开始轮播
            {
                picPlayIndex++;
                if (picPlayIndex == filePath.Count)
                {
                    if (PlayOver != null)
                    {
                        PlayOver();
                        return false;
                    }
                }
                picPlayIndex = (picPlayIndex % filePath.Count);
                Image1 = filePath[picPlayIndex];

                int tmpPicPlayIndex = picPlayIndex;
                tmpPicPlayIndex++;
                tmpPicPlayIndex = (tmpPicPlayIndex % filePath.Count);
                Image2 = filePath[tmpPicPlayIndex];

                if (flushMethod != null)
                {
                    flushMethod.FlushOver -= flushMethod_FlushOver;
                    flushMethod = null;
                }
                All.Class.Reflex<Player.FlushMethod> r =
                    new Class.Reflex<Player.FlushMethod>("All", string.Format("All.Control.Player.{0}", flushList));
                flushMethod = (Player.FlushMethod)r.Get();
                flushMethod.Init(this.Handle,Image1, Image2, backImage, changeTime, TickTime);
                flushMethod.FlushOver += flushMethod_FlushOver;
                result = true;
            }
            this.Invalidate();
            return result;
        }
        void flushMethod_FlushOver()
        {
            RunOver();
        }
    }
}
namespace All.Control.Player
{
    public abstract class FlushMethod
    {
        protected Bitmap ImgPic1;
        protected Bitmap ImgPic2;

        protected IntPtr HwndPic1;
        protected IntPtr HwndPic2;
        protected IntPtr HwndPic;

        protected Graphics GrapPic1;
        protected Graphics GrapPic2;
        protected Graphics GrapPic;

        protected IntPtr tempPic1;
        protected IntPtr tempPic2;

        protected IntPtr OldHwndPic1;
        protected IntPtr OldHwndPic2;

        public IntPtr Hwnd
        { get; set; }
        public Image Pic1
        { get; set; }
        public Image Pic2
        { get; set; }
        public Image Back
        { get; set; }
        public int DelayTime
        { get; set; }
        public int TickTime
        { get; set; }
        public enum FlushList
        {
            横向百业窗,
            竖向百业窗,
            从中间十字形展开,
            从中间方形展开,
            从上向下平移,
            从左向右平移,
            从左向右挤压,
            随机//【随机】,会自动随机切换前面的效果
        }
        /// <summary>
        /// 初始化属性赋值
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="pic1"></param>
        /// <param name="pic2"></param>
        /// <param name="back"></param>
        /// <param name="delayTime"></param>
        /// <param name="tickTime"></param>
        public void Init(IntPtr hwnd, Image pic1, Image pic2, Image back, int delayTime, int tickTime)
        {
            this.Hwnd = hwnd;
            this.Pic1 = pic1;
            this.Pic2 = pic2;
            this.Back = back;
            this.DelayTime = delayTime;
            this.TickTime = tickTime;
            Init();
        }
        /// <summary>
        /// 初始化整个画板
        /// </summary>
        protected void InitHdc()
        {
            GrapPic = Graphics.FromHwnd(Hwnd);
            HwndPic = GrapPic.GetHdc();

            //把原图在内存中进行缩放绘画，使不同尺寸的图变成一样大小
            ImgPic1 = new Bitmap(Back.Width, Back.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(ImgPic1))
            {
                g.DrawImage(Pic1, new Rectangle(0, 0, Back.Width, Back.Height), new Rectangle(0, 0, Pic1.Width, Pic1.Height), GraphicsUnit.Pixel);
            }

            ImgPic2 = new Bitmap(Back.Width, Back.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(ImgPic2))
            {
                g.DrawImage(Pic2, new Rectangle(0, 0, Back.Width, Back.Height), new Rectangle(0, 0, Pic2.Width, Pic2.Height), GraphicsUnit.Pixel);
            }
            //初始化各参数
            GrapPic1 = Graphics.FromImage(ImgPic1);
            HwndPic1 = GrapPic1.GetHdc();
            tempPic1 = ImgPic1.GetHbitmap();
            OldHwndPic1 = All.Class.Api.SelectObject(HwndPic1, tempPic1);

            GrapPic2 = Graphics.FromImage(ImgPic2);
            HwndPic2 = GrapPic2.GetHdc();
            tempPic2 = ImgPic2.GetHbitmap();
            OldHwndPic2 = All.Class.Api.SelectObject(HwndPic2, tempPic2);
        }
        /// <summary>
        /// 初始化各刷新的不同需求
        /// </summary>
        protected abstract void Init();
        /// <summary>
        /// 释放画板
        /// </summary>
        public void Dispose()
        {
            if (OldHwndPic2 != IntPtr.Zero)//如果替换出来的句柄不为空，则把原句柄放回去。
                All.Class.Api.SelectObject(HwndPic1, OldHwndPic2);
            if (OldHwndPic1 != IntPtr.Zero)
                All.Class.Api.SelectObject(HwndPic2, OldHwndPic1);

            if (tempPic2 != IntPtr.Zero)
                All.Class.Api.DeleteObject(tempPic2);
            if (tempPic1 != IntPtr.Zero)
                All.Class.Api.DeleteObject(tempPic1);

            if (GrapPic1 != null)
            {
                if (HwndPic1 != IntPtr.Zero)
                {
                    GrapPic1.ReleaseHdc(HwndPic1);
                }
                GrapPic1.Dispose();
            }
            if (GrapPic2 != null)
            {
                if (HwndPic2 != IntPtr.Zero)
                {
                    GrapPic2.ReleaseHdc(HwndPic2);
                }
                GrapPic2.Dispose();
            }
            if (GrapPic != null)
            {
                if (HwndPic != IntPtr.Zero)
                {
                    GrapPic.ReleaseHdc(HwndPic);
                }
                GrapPic.Dispose();
            }
            if (ImgPic1 != null)
            {
                ImgPic1.Dispose();
            }
            if (ImgPic2 != null)
            {
                ImgPic2.Dispose();
            }
        }
        public abstract void Flush();
        public delegate void FlushOverHandle();
        public event FlushOverHandle FlushOver;
        protected void FlushOverValue()
        {
            if (FlushOver != null)
            {
                FlushOver();
            }
        }
    }
    public class 从中间方形展开 : FlushMethod
    {
        int flushIndex = 0;
        float tmpHeight = 0;
        float tmpWidth = 0;
        protected override void Init()
        {
            InitHdc();
            tmpHeight = (float)Back.Height / 2 / ((float)DelayTime / (float)TickTime);
            tmpWidth = (float)Back.Width / 2 / ((float)DelayTime / (float)TickTime);
        }
        public override void Flush()
        {
            flushIndex++;
            if ((flushIndex * tmpHeight) < (Back.Height / 2)
                && (flushIndex * tmpWidth) < (Back.Width / 2))
            {
                All.Class.Api.BitBlt(HwndPic, 0, 0, Back.Width, (int)(Back.Height / 2.0f - tmpHeight * flushIndex),
                    HwndPic1, 0, 0, All.Class.Api.ROP_SrcCopy);

                All.Class.Api.BitBlt(HwndPic, 0, (int)(Back.Height / 2.0f - tmpHeight * flushIndex), (int)(Back.Width / 2.0f - tmpWidth * flushIndex), (int)(flushIndex * tmpHeight * 2),
                    HwndPic1, 0, (int)(Back.Height / 2.0f - tmpHeight * flushIndex), All.Class.Api.ROP_SrcCopy);

                All.Class.Api.BitBlt(HwndPic, (int)(Back.Width / 2.0f - flushIndex * tmpWidth), (int)(Back.Height / 2.0f - flushIndex * tmpHeight), (int)(flushIndex * tmpWidth * 2), (int)(flushIndex * tmpHeight * 2),
                    HwndPic2, (int)(Back.Width / 2.0f - flushIndex * tmpWidth), (int)(Back.Height / 2.0f - flushIndex * tmpHeight), All.Class.Api.ROP_SrcCopy);

                All.Class.Api.BitBlt(HwndPic, (int)(Back.Width / 2.0f + flushIndex * tmpWidth), (int)(Back.Height / 2.0f - flushIndex * tmpHeight), (int)(Back.Width / 2.0f - tmpWidth * flushIndex), (int)(flushIndex * tmpHeight * 2),
                    HwndPic1, (int)(Back.Width / 2.0f + flushIndex * tmpWidth), (int)(Back.Height / 2.0f - tmpHeight * flushIndex), All.Class.Api.ROP_SrcCopy);

                All.Class.Api.BitBlt(HwndPic, 0, (int)(Back.Height / 2.0f + tmpHeight * flushIndex), Back.Width, (int)(Back.Height / 2.0f - tmpHeight * flushIndex),
                    HwndPic1, 0, (int)(Back.Height / 2.0f + tmpHeight * flushIndex), All.Class.Api.ROP_SrcCopy);
            }
            else
            {
                All.Class.Api.BitBlt(HwndPic, 0, 0, Back.Width, Back.Height, HwndPic2, 0, 0, All.Class.Api.ROP_SrcCopy);
                FlushOverValue();
            }
        }
    }
    public class 从中间十字形展开 : FlushMethod
    {
        int flushIndex = 0;
        float tmpHeight = 0;
        float tmpWidth = 0;
        protected override void Init()
        {
            InitHdc();
            tmpHeight = (float)Back.Height / 2 / ((float)DelayTime / (float)TickTime);
            tmpWidth = (float)Back.Width / 2 / ((float)DelayTime / (float)TickTime);
        }
        public override void Flush()
        {
            flushIndex++;
            if ((flushIndex * tmpHeight ) < (Back.Height/2)
                && (flushIndex * tmpWidth) < (Back.Width / 2))
            {
                All.Class.Api.BitBlt(HwndPic, 0, 0, (int)(Back.Width / 2.0f - flushIndex * tmpWidth), (int)(Back.Height / 2.0f - flushIndex * tmpHeight),
                    HwndPic1, (int)(flushIndex * tmpWidth), (int)(flushIndex * tmpHeight), All.Class.Api.ROP_SrcCopy);
                All.Class.Api.BitBlt(HwndPic, (int)(Back.Width / 2.0f + flushIndex * tmpWidth), 0, (int)(Back.Width / 2.0f - flushIndex * tmpWidth), (int)(Back.Height / 2.0f - flushIndex * tmpHeight),
                    HwndPic1, Back.Width / 2, (int)( flushIndex * tmpHeight), All.Class.Api.ROP_SrcCopy);
                All.Class.Api.BitBlt(HwndPic, 0, (int)(Back.Height / 2.0f + flushIndex * tmpHeight), (int)(Back.Width / 2.0f - flushIndex * tmpWidth), (int)(Back.Height / 2.0f - flushIndex * tmpHeight),
                    HwndPic1, (int)(flushIndex * tmpWidth), (int)(Back.Height / 2.0f ), All.Class.Api.ROP_SrcCopy);
                All.Class.Api.BitBlt(HwndPic, (int)(Back.Width / 2.0f + flushIndex * tmpWidth), (int)(Back.Height / 2.0f + flushIndex * tmpHeight), (int)(Back.Width / 2.0f - flushIndex * tmpWidth), (int)(Back.Height / 2.0f - flushIndex * tmpHeight),
                    HwndPic1, Back.Width / 2, Back.Height / 2, All.Class.Api.ROP_SrcCopy);
                All.Class.Api.BitBlt(HwndPic, (int)(Back.Width / 2 - flushIndex * tmpWidth), 0, (int)(flushIndex * tmpWidth * 2), Back.Height,
                    HwndPic2, (int)(Back.Width / 2 - flushIndex * tmpWidth), 0, All.Class.Api.ROP_SrcCopy);
                All.Class.Api.BitBlt(HwndPic, 0, (int)(Back.Height / 2 - flushIndex * tmpHeight), Back.Width, (int)(flushIndex * tmpHeight * 2),
                    HwndPic2, 0, (int)(Back.Height / 2 - flushIndex * tmpHeight), All.Class.Api.ROP_SrcCopy);
            }
            else
            {
                All.Class.Api.BitBlt(HwndPic, 0, 0, Back.Width, Back.Height, HwndPic2, 0, 0, All.Class.Api.ROP_SrcCopy);
                FlushOverValue();
            }
        }
    }
    public class 横向百业窗 : FlushMethod
    {
        const int SplitCount = 15;
        int flushIndex = 0;
        float tmpHeight = 0;
        protected override void Init()
        {
            InitHdc();
            tmpHeight = (float)Back.Height / ((float)DelayTime / (float)TickTime) / SplitCount;
        }
        public override void Flush()
        {
            flushIndex++;
            if ((flushIndex * tmpHeight * SplitCount) < Back.Height)
            {
                //All.Class.Api.BitBlt(HwndPic, 0, 0, Back.Width, Back.Height,
                //    HwndPic2, 0, 0, All.Class.Api.ROP_SrcCopy);//好像先贴整个底图会有点闪，不明白为什么 
                for (int i = 0; i < SplitCount; i++)
                {
                    All.Class.Api.BitBlt(HwndPic, 0, Back.Height * i / SplitCount, Back.Width, (int)(flushIndex * tmpHeight),
                        HwndPic2, 0, Back.Height * i / SplitCount, All.Class.Api.ROP_SrcCopy);
                    All.Class.Api.BitBlt(HwndPic, 0, Back.Height * i / SplitCount + (int)(flushIndex * tmpHeight), Back.Width, Back.Height / SplitCount - (int)(flushIndex * tmpHeight),
                        HwndPic1, 0, Back.Height * i / SplitCount + (int)(flushIndex * tmpHeight), All.Class.Api.ROP_SrcCopy);
                }
            }
            else
            {
                All.Class.Api.BitBlt(HwndPic, 0, 0, Back.Width, Back.Height, HwndPic2, 0, 0, All.Class.Api.ROP_SrcCopy);
                FlushOverValue();
            }
        }
    }
    public class 竖向百业窗 : FlushMethod
    {
        const int SplitCount = 15;
        int flushIndex = 0;
        float tmpWidth = 0;
        protected override void Init()
        {
            InitHdc();
            tmpWidth = (float)Back.Width / ((float)DelayTime / (float)TickTime) / SplitCount;
        }
        public override void Flush()
        {
            flushIndex++;
            int left = 0;
            if ((flushIndex * tmpWidth * SplitCount) < Back.Width)
            {
                for (int i = 0; i < SplitCount; i++)
                {
                    left = Back.Width * i / SplitCount;
                    All.Class.Api.BitBlt(HwndPic, left, 0, (int)(flushIndex * tmpWidth), Back.Height,
                        HwndPic2, left, 0, All.Class.Api.ROP_SrcCopy);
                    All.Class.Api.BitBlt(HwndPic, left + (int)(flushIndex * tmpWidth), 0, Back.Width / SplitCount - (int)(flushIndex * tmpWidth), Back.Height,
                        HwndPic1, left + (int)(flushIndex * tmpWidth), 0, All.Class.Api.ROP_SrcCopy);
                }
            }
            else
            {
                All.Class.Api.BitBlt(HwndPic, 0, 0, Back.Width, Back.Height, HwndPic2, 0, 0, All.Class.Api.ROP_SrcCopy);
                FlushOverValue();
            }
        }
    }
    public class 从上向下平移 : FlushMethod
    {
        int flushIndex = 0;
        float tmpHeight = 0;

        protected override void Init()
        {
            InitHdc();
            tmpHeight = (float)Back.Height / ((float)DelayTime / (float)TickTime);
        }
        public override void Flush()
        {
            flushIndex++;
            if ((flushIndex * tmpHeight) < Back.Height)
            {
                All.Class.Api.BitBlt(HwndPic, 0, 0, Back.Width, (int)(tmpHeight * flushIndex), HwndPic2, 0, Back.Height - (int)(tmpHeight * flushIndex), All.Class.Api.ROP_SrcCopy);
                All.Class.Api.BitBlt(HwndPic, 0, (int)(tmpHeight * flushIndex), Back.Width, Back.Height - (int)(tmpHeight * flushIndex), HwndPic1, 0, 0, All.Class.Api.ROP_SrcCopy);
            }
            else
            {
                All.Class.Api.BitBlt(HwndPic, 0, 0, Back.Width, Back.Height, HwndPic2, 0, 0, All.Class.Api.ROP_SrcCopy);
                FlushOverValue();
            }
        }
    }
    public class 从左向右挤压:FlushMethod
    {
        int flushIndex = 0;
        float tmpWidth = 0;
        protected override void Init()
        {
            InitHdc();
            tmpWidth = (float)Back.Width / ((float)DelayTime / (float)TickTime);
        }
        public override void Flush()
        {
            flushIndex++;
            if ((flushIndex * tmpWidth) < Back.Width)
            {
                All.Class.Api.StretchBlt(HwndPic, 0, 0, (int)(tmpWidth * flushIndex), Back.Height, HwndPic2, 0, 0, ImgPic2.Width, ImgPic2.Height, All.Class.Api.ROP_SrcCopy);
                All.Class.Api.StretchBlt(HwndPic, (int)(tmpWidth * flushIndex), 0, Back.Width - (int)(tmpWidth * flushIndex), Back.Height, HwndPic1, 0, 0, ImgPic1.Width, ImgPic1.Height, All.Class.Api.ROP_SrcCopy);
            }
            else
            {
                All.Class.Api.BitBlt(HwndPic, 0, 0, Back.Width, Back.Height, HwndPic2, 0, 0, All.Class.Api.ROP_SrcCopy);
                FlushOverValue();
            }
        }
    }
    public class 从左向右平移 : FlushMethod
    {
        int flushIndex = 0;
        float tmpWidth = 0;
        protected override void Init()
        {
            InitHdc();
            tmpWidth = (float)Back.Width / ((float)DelayTime / (float)TickTime);
        }
        public override void Flush()
        {
            flushIndex++;
            if ((flushIndex * tmpWidth) < Back.Width)
            {
                All.Class.Api.BitBlt(HwndPic, 0, 0, (int)(tmpWidth * flushIndex), Back.Height, HwndPic2, Back.Width - (int)(tmpWidth * flushIndex), 0, All.Class.Api.ROP_SrcCopy);
                All.Class.Api.BitBlt(HwndPic, (int)(tmpWidth * flushIndex), 0, Back.Width - (int)(tmpWidth * flushIndex), Back.Height, HwndPic1, 0, 0, All.Class.Api.ROP_SrcCopy);
            }
            else
            {
                All.Class.Api.BitBlt(HwndPic, 0, 0, Back.Width, Back.Height, HwndPic2, 0, 0, All.Class.Api.ROP_SrcCopy);
                FlushOverValue();
            }
        }
    }
    public class 随机 : FlushMethod
    {
        Array flushlist = Enum.GetValues(typeof(FlushList));

        FlushMethod f;

        public override void Flush()
        {
            f.Flush();
        }
        protected override void Init()
        {
            int index = (int)Math.Ceiling(All.Class.Num.GetRandom(0.001f, flushlist.Length - 1)) - 1;

            All.Class.Reflex<Player.FlushMethod> r =
                new Class.Reflex<Player.FlushMethod>("All", string.Format("All.Control.Player.{0}", (FlushList)Enum.Parse(typeof(FlushList),flushlist.GetValue(index).ToString())));
            f = (Player.FlushMethod)r.Get();
            f.Init(Hwnd, Pic1, Pic2, Back, DelayTime, TickTime);
            f.FlushOver += f_FlushOver;
        }
        void f_FlushOver()
        {
            FlushOverValue();
            f.Dispose();
        }
    }
}
