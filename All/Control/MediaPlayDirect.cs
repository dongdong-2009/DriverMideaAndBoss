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
    public partial class MediaPlayDirect : UserControl
    {
        public MediaPlayDirect()
        {

            InitializeComponent();
            rspMediaPlayer1.Finished += MediaPlayDirectShow_Finished;
            rspMediaPlayer1.DoubleClick_Ex += MediaPlayDirectShow_DoubleClick_Ex;
            rspMediaPlayer1.Click_Ex += MediaPlayDirectShow_Click_Ex;
        }
        /// <summary>
        /// 声音大小 
        /// </summary>
        [Description("声音大小")]
        [Category("Shuai")]
        public int Volumn
        {
            get { return (int)rspMediaPlayer1.GetVolume(); }
            set { rspMediaPlayer1.SetVolume(value); }
        }
        /// <summary>
        /// 总长度
        /// </summary>
        [Description("总长度")]
        [Category("Shuai")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public int NaturalDuration
        {
            get { return (int)TimeSpan.Parse(rspMediaPlayer1.GetLenghtString()).TotalSeconds; }
        }
        /// <summary>
        /// 当前播放位置
        /// </summary>
        [Description("当前播放位置")]
        [Category("Shuai")]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Position
        {
            get { return (int)TimeSpan.Parse(rspMediaPlayer1.GetCurrentPositionString()).TotalSeconds; }
            //set { rspMediaPlayer1.Seek(value); }
        }

        void MediaPlayDirectShow_Click_Ex(EventArgs e)
        {
            if (MediaMouseClick != null)
            {
                MediaMouseClick();
            }
        }

        void MediaPlayDirectShow_DoubleClick_Ex(EventArgs e)
        {
            if (MediaMouseDoubleClick != null)
            {
                MediaMouseDoubleClick();
            }
        }

        void MediaPlayDirectShow_Finished(RSPMediaPlayer.RSPMediaPlayer.FinishedEventArgs e)
        {
            //rspMediaPlayer1.Cancel();
            if (MediaEnd != null)
            {
                MediaEnd();
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

        string fileName = "";
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
            fileName = file;
            rspMediaPlayer1.OpenClip(file, RSPMediaPlayer.PlayerState.Paused);
        }
        /// <summary>
        /// 播放
        /// </summary>
        public void Play()
        {
            if (fileName != "")
            {
                rspMediaPlayer1.OpenClip(fileName, RSPMediaPlayer.PlayerState.Playing);
            }
            //timer1.Enabled = true;
        }
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            rspMediaPlayer1.Cancel();
            //timer1.Enabled = false;
        }
        /// <summary>
        /// 暂停
        /// </summary>
        public  void Pause()
        {
            rspMediaPlayer1.Pause();
            //timer1.Enabled = false;
        }

    }
}
