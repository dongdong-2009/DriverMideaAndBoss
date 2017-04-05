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
    public partial class PlayWindow : Form
    {
        int delayTime = 3;
        /// <summary>
        /// 停留时间,停留时间为0时，不显示
        /// </summary>
        [Description("停留延时")]
        [Category("Shuai")]
        public int DelayTime
        {
            get { return delayTime; }
            set { delayTime = value; }
        }
        public delegate void ExitHandle();
        public event ExitHandle Exit;

        /// <summary>
        /// 当前是否正在自动播放中
        /// </summary>
        [Description("当前是否正播放中")]
        [Category("Shuai")]
        public bool Playing
        { get; set; }
        private Screen showScreen = Screen.PrimaryScreen;

        /// <summary>
        /// 显示的屏幕
        /// </summary>
        [Description("显示的屏幕")]
        [Category("Shuai")]
        public Screen ShowScreen
        {
            get { return showScreen; }
            set { showScreen = value; }
        }
        /// <summary>
        /// 播放下界面
        /// </summary>
        bool playNext = false;
        public PlayWindow()
        {
            Playing = false;
            InitializeComponent();
        }
        /// <summary>
        /// 播放下一界面
        /// </summary>
        public void PlayNext()
        {
            playNext = true;
        }
        private void PlayWindow_Load(object sender, EventArgs e)
        {
            float width = this.Width;
            float height = this.Height;
            float x = showScreen.Bounds.Width / width;
            float y = showScreen.Bounds.Height / height;

            SetWidthAndHeight(x, y, this.Controls);
            this.Location = showScreen.Bounds.Location;
            this.Size = showScreen.Bounds.Size;

            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PlayWindow_KeyDown);
        }
        private void SetWidthAndHeight(float x, float y, System.Windows.Forms.Control.ControlCollection c)
        {
            if (c == null)
            {
                return;
            }
            foreach (System.Windows.Forms.Control cc in c)
            {
                SetWidthAndHeight(x, y, cc.Controls);
                cc.Font = new System.Drawing.Font(cc.Font.FontFamily, cc.Font.Size * x);
                cc.Left = (int)(cc.Left * x);
                cc.Top = (int)(cc.Top * y);
                cc.Width = (int)(cc.Width * x);
                cc.Height = (int)(cc.Height * y);
                if (cc is All.Control.Metro.ItemBox)
                {
                    All.Control.Metro.ItemBox tmp = cc as All.Control.Metro.ItemBox;
                    tmp.TitleFont = new Font(tmp.TitleFont.FontFamily, tmp.TitleFont.Size * x);
                    tmp.ValueFont = new Font(tmp.ValueFont.FontFamily, tmp.ValueFont.Size * x);
                    tmp.LineBold = (int)(tmp.LineBold * x);
                }
            }
        }
        private void PlayWindow_KeyPress(object sender, KeyPressEventArgs e)
        {
        }
        private void PlayWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                if (Exit != null)
                {
                    Exit();
                }
                if (!Playing)//非正在播放过程则关闭窗体
                {
                    this.Close();
                }
            }
        }
        protected override void WndProc(ref Message m)
        {
            if (Playing)
            {
                if (m.Msg == All.Class.Api.WM_MouseActivate)
                {
                    m.Result = new IntPtr(All.Class.Api.MA_NoActivate);
                    return;
                }
                else if (m.Msg == All.Class.Api.WM_NcActivate)
                {
                    if (((int)m.WParam & 0xFFFF) != All.Class.Api.WA_InActive)
                    {
                        if (m.LParam != IntPtr.Zero)
                        {
                            All.Class.Api.SetActiveWindow(m.LParam);
                        }
                        else
                        {
                            All.Class.Api.SetActiveWindow(IntPtr.Zero);
                        }
                    }
                }
            }
            base.WndProc(ref m);
        }
        protected override bool ShowWithoutActivation
        {
            get
            {
                return false;
            }
        }
        private void PlayWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
        /// <summary>
        /// 隐藏窗体
        /// </summary>
        public virtual void HideWindow()
        { }
        /// <summary>
        /// 显示窗体
        /// </summary>
        public virtual void ShowWindow()
        { }
        /// <summary>
        /// 自动播放控制
        /// </summary>
        public class AutoPlayOneByOne
        {
            System.Windows.Forms.Timer timPlay = new Timer();
            List<PlayWindow> playList = new List<PlayWindow>();
            int index = 0;
            int startTime = 0;
            /// <summary>
            /// 添加自动轮播窗体
            /// </summary>
            /// <param name="playWindow"></param>
            public void Add(PlayWindow playWindow)
            {
                if (!playList.Contains(playWindow) && playWindow.DelayTime > 0)
                {
                    playList.Add(playWindow);
                    playWindow.Playing = true;
                    playWindow.Exit += new PlayWindow.ExitHandle(Play_Exit);
                }
            }
            /// <summary>
            /// 删除轮播窗体
            /// </summary>
            /// <param name="playWindow"></param>
            public void Del(PlayWindow playWindow)
            {
                if (playList.Contains(playWindow))
                {
                    playWindow.Exit -= Play_Exit;
                    playList.Remove(playWindow);
                }
            }
            /// <summary>
            /// 自动开始轮播
            /// </summary>
            public void Start()
            {
                if (playList.Count > 0)
                {
                    index = playList.Count - 1;
                    playList[index].PlayNext();
                    timPlay.Interval = 1000;
                    timPlay_Tick(null, new EventArgs());
                    timPlay.Tick += new EventHandler(timPlay_Tick);
                    timPlay.Enabled = true;
                }
            }
            /// <summary>
            /// 停止轮播并退出
            /// </summary>
            public void Stop()
            {
                timPlay.Tick -= timPlay_Tick;
                timPlay.Stop();
                timPlay.Enabled = false;
                foreach (PlayWindow pw in playList)
                {
                    pw.HideWindow();
                    pw.Close();
                    pw.Dispose();
                }
                startTime = 0;
            }
            /// <summary>
            /// 轮播过程，时间到，先显示下一个窗体，再关闭上一个窗体，防闪烁
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            void timPlay_Tick(object sender, EventArgs e)
            {
                if (index < 0 || index >= playList.Count)
                {
                    Stop();
                    return;
                }
                if (((Environment.TickCount - startTime) >= playList[index].DelayTime * 1000)
                    || playList[index].playNext)
                {
                    playList[index].playNext = false;
                    index++;
                    index = index % playList.Count;
                    Show(index);

                    Close(index);
                    startTime = Environment.TickCount;
                }

            }
            void Close(int index)
            {
                try
                {
                    if (((index + playList.Count - 1) % playList.Count) != index)
                    {
                        playList[(index + playList.Count - 1) % playList.Count].Visible = false;
                        playList[(index + playList.Count - 1) % playList.Count].HideWindow();
                    }
                }
                catch { }
            }
            void Show(int index)
            {
                try
                {
                    if (!playList[index].Visible)
                    {
                        playList[index].Visible = true;
                        playList[index].ShowWindow();
                    }
                    else
                    {
                        playList[index].Show();
                        playList[index].ShowWindow();
                    }
                }
                catch { }
            }
            /// <summary>
            /// 在窗体上按Esc键退出
            /// </summary>
            void Play_Exit()
            {
                Stop();
            }
        }
    }
}
