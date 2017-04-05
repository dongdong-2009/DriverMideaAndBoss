using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace All.Control
{
    public partial class MediaPlayerLocal : UserControl
    {
        public const string FileFilter = "视频文件|*.mp4;*.avi;*.rmvb;*.wmv|mp4文件|*.mp4|avi文件|*.avi|rmvb文件|*.rmvb|wmv文件|*.wmv";
        public MediaPlayerLocal()
        {
            InitializeComponent();
            axWindowsMediaPlayer1.DoubleClickEvent += axWindowsMediaPlayer1_DoubleClickEvent;
            axWindowsMediaPlayer1.ClickEvent += axWindowsMediaPlayer1_ClickEvent;
            axWindowsMediaPlayer1.PlayStateChange += axWindowsMediaPlayer1_PlayStateChange;
            axWindowsMediaPlayer1.StatusChange += axWindowsMediaPlayer1_StatusChange;
            axWindowsMediaPlayer1.stretchToFit = true;
            axWindowsMediaPlayer1.settings.autoStart = true;
        }

        void axWindowsMediaPlayer1_StatusChange(object sender, EventArgs e)
        {
            //Console.WriteLine(axWindowsMediaPlayer1.status);
        }
        /// <summary>
        /// 检查当前文件是否可以播放
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool CheckPlayFile(string filter)
        {
            return FileFilter.ToUpper().IndexOf(filter.ToUpper()) >= 0;
        }
        void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            //1     停止
            //2     暂停
            //3     播放
            //6     缓冲
            //8     播放完成
            //9     连接中
            //10    准备就绪
            switch (e.newState)
            {
                case 8:
                    if (MediaEnd != null)
                    {
                        MediaEnd();
                    }
                    break;
                case 3:
                    break;
                case 10:
                    break;
            }
        }

        void axWindowsMediaPlayer1_ClickEvent(object sender, AxWMPLib._WMPOCXEvents_ClickEvent e)
        {
            if (MediaMouseClick != null)
            {
                MediaMouseClick();
            }
        }

        void axWindowsMediaPlayer1_DoubleClickEvent(object sender, AxWMPLib._WMPOCXEvents_DoubleClickEvent e)
        {
            if (MediaMouseDoubleClick != null)
            {
                MediaMouseDoubleClick();
            }
        }
        /// <summary>
        /// 文件播放完成
        /// </summary>
        public delegate void MediaEndHandle();
        /// <summary>
        /// 文件播放完成
        /// </summary>
        [Category("ShuaiShuai")]
        [Description("文件播放完成")]
        public event MediaEndHandle MediaEnd;

        /// <summary>
        /// 双击
        /// </summary>
        public delegate void MouseDoubleClickHandle();
        /// <summary>
        /// 双击事件
        /// </summary>
        [Category("ShuaiShuai")]
        [Description("双击事件")]
        public event MouseDoubleClickHandle MediaMouseDoubleClick;

        /// <summary>
        /// 单击事件
        /// </summary>
        public delegate void MouseClickHandle();
        /// <summary>
        /// 单击事件
        /// </summary>
        [Category("ShuaiShuai")]
        [Description("单击事件")]
        public event MouseClickHandle MediaMouseClick;

        /// <summary>
        /// 声音大小 
        /// </summary>
        [Description("声音大小")]
        [Category("Shuai")]
        public int Volumn
        {
            get { return axWindowsMediaPlayer1.settings.volume; }
            set { axWindowsMediaPlayer1.settings.volume = value; }
        }
        /// <summary>
        /// 总长度
        /// </summary>
        [Description("总长度")]
        [Category("Shuai")]
        public int NaturalDuration
        {
            get { return (int)axWindowsMediaPlayer1.currentMedia.duration; }
        }
        /// <summary>
        /// 当前播放位置
        /// </summary>
        [Description("当前播放位置")]
        [Category("Shuai")]
        public int Position
        {
            get { return (int)axWindowsMediaPlayer1.Ctlcontrols.currentPosition; }
            set { axWindowsMediaPlayer1.Ctlcontrols.currentPosition = value; }
        }
        /// <summary>
        /// 设置播放文件 
        /// </summary>
        /// <param name="file"></param>
        public void SetFile(string file)
        {
            if (!System.IO.File.Exists(file))
            {
                All.Class.Error.Add("指定播放的音频视频文件不存在", Environment.StackTrace);
                return;
            }
            axWindowsMediaPlayer1.URL = file;
        }
        /// <summary>
        /// 播放
        /// </summary>
        public void Play()
        {
            //axWindowsMediaPlayer1.fullScreen = true;
            //axWindowsMediaPlayer1.stretchToFit = true;
            //axWindowsMediaPlayer1.w
            //bool timeOut = false;
            //int start = Environment.TickCount;
            //do
            //{
            //    Application.DoEvents();
            //    if ((Environment.TickCount - start) > 1000)
            //    {
            //        timeOut = true;
            //    }
            //    System.Threading.Thread.Sleep(100);
            //} while (!timeOut &&
            //    axWindowsMediaPlayer1.playState!=WMPLib.WMPPlayState.wmppsPaused &&
            //    axWindowsMediaPlayer1.playState!=WMPLib.WMPPlayState.wmppsReady);
            //if (!timeOut)
            //{
                axWindowsMediaPlayer1.Ctlcontrols.play();
            //}
        }
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            //timer1.Enabled = false;
        }
        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            axWindowsMediaPlayer1.Ctlcontrols.pause();
            //timer1.Enabled = false;
        }

        private void MediaPlayer_MediaEnd(object sender, System.Windows.RoutedEventArgs e)
        {
        }
        void MediaPlayer_MouseClick(object sender)
        {
        }
        private void MediaPlayer_DoubleClick(object sender)
        {
        }
    }
}
