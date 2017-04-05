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
    public partial class MediaPlayer : UserControl
    {

        public MediaPlayer()
        {
            InitializeComponent();
            mediaPlayer1.MediaEnd += MediaPlayer_MediaEnd;
            mediaPlayer1.MediaFaild += MediaPlayer_MediaFaild;
            mediaPlayer1.MediaDoubleClick += MediaPlayer_DoubleClick;
            mediaPlayer1.MediaMouseClick += MediaPlayer_MouseClick;
        }

        void MediaPlayer_MediaFaild(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {
            All.Class.Error.Add(e.ErrorException);
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
            get { return (int)mediaPlayer1.Volumn; }
            set { mediaPlayer1.Volumn = value; }
        }
        /// <summary>
        /// 总长度
        /// </summary>
        [Description("总长度")]
        [Category("Shuai")]
        public int NaturalDuration
        {
            get { return mediaPlayer1.NaturalDuration; }
        }
        /// <summary>
        /// 当前播放位置
        /// </summary>
        [Description("当前播放位置")]
        [Category("Shuai")]
        public int Position
        {
            get { return mediaPlayer1.Position; }
            set { mediaPlayer1.Position = value; }
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

            mediaPlayer1.SetFile(file);
        }
        /// <summary>
        /// 播放
        /// </summary>
        public void Play()
        {
            mediaPlayer1.Play();
            //timer1.Enabled = true;
        }
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            mediaPlayer1.Stop();
            //timer1.Enabled = false;
        }
        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            mediaPlayer1.Pause();
            //timer1.Enabled = false;
        }

        private void MediaPlayer_MediaEnd(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MediaEnd != null)
            {
                MediaEnd();
            }
        }
        void MediaPlayer_MouseClick(object sender)
        {
            if (MediaMouseClick != null)
            {
                MediaMouseClick();
            }
        }
        private void MediaPlayer_DoubleClick(object sender)
        {
            if (MediaMouseDoubleClick != null)
            {
                MediaMouseDoubleClick();
            }
        }

    }
}
