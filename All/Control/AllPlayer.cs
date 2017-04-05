using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace All.Control
{
    public partial class AllPlayer : UserControl
    {
        /// <summary>
        /// 播放文件类型
        /// </summary>
        public enum FileList
        {
            /// <summary>
            /// 图片
            /// </summary>
            Image,
            /// <summary>
            /// 视频
            /// </summary>
            Video
        }

        int picDelayTime = 3;
        /// <summary>
        /// 图片停留时间
        /// </summary>
        [Description("图片停留时间")]
        [Category("Shuai")]
        public int PicDelayTime
        {
            get { return picDelayTime; }
            set { picDelayTime = value; }
        }

        List<PlayList> playList = new List<PlayList>();


        public AllPlayer()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 播放
        /// </summary>
        public void Play()
        {
 
        }
        public void Stop()
        { }

        /// <summary>
        /// 添加播放文件
        /// </summary>
        /// <param name="playFile"></param>
        public void AddFile(PlayList playFile)
        {
            if (playFile.FileName == "")
            {
                All.Class.Error.Add("添加的播放文件名称为空，不能正常添加", Environment.StackTrace);
                return;
            }
            if (playFile.FileName.IndexOf("\\") == 0)
            {
                playFile.FileName = string.Format("{0}{1}", All.Class.FileIO.GetNowPath(), playFile.FileName);
            }
            this.playList.Add(playFile);
        }
        /// <summary>
        /// 文件播放类
        /// </summary>
        public class PlayList
        {
            /// <summary>
            /// 文件名称
            /// </summary>
            public string FileName
            { get; set; }
            /// <summary>
            /// 文件类型
            ///     Video,仅支持：Mp4,Wmv,Avi
            ///     Image,仅支持：Bmp,Png,Jpg,Jpge
            /// </summary>
            public FileList FileList
            { get; set; }
            /// <summary>
            /// 文件播放
            /// </summary>
            /// <param name="FileName">文件名称</param>
            /// <param name="FileList">文件播放类型</param>
            public PlayList(string FileName, FileList FileList)
            {
                this.FileName = FileName;
                this.FileList = FileList;
            }
        }
    }
}
