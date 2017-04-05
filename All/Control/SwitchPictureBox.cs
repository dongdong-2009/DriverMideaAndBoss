using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
namespace All.Control
{
    [Serializable]
    [DefaultProperty("Images")]
    public class SwitchPictureBox : System.Windows.Forms.Control
    {
        int delayTime = 3000;
        /// <summary>
        /// 切换停留时间
        /// </summary>
        public int DelayTime
        {
            get { return delayTime; }
            set { delayTime = value; }
        }
        /// <summary>
        /// 图片切换效果
        /// </summary>
        public enum SwitchMethod
        {
            随机,
            淡入淡出
        }
        /// <summary>
        /// 图片切换效果
        /// </summary>
        public SwitchMethod SwitchValue
        { get; set; }
        [Serializable]
        public class ImageItem
        {
            private Bitmap tmpValue = null;
            public Bitmap Value
            {
                get { return tmpValue; }
                set { tmpValue = value; }
            }
        }
        private List<ImageItem> items = new List<ImageItem>();
        [TypeConverter(typeof(System.ComponentModel.CollectionConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<ImageItem> Images
        {
            get
            {
                return items;
            }
        }
        Bitmap backImage = null;
        public SwitchPictureBox()
        {
            this.BackColor = Color.LightPink;
            SwitchValue = SwitchMethod.随机;
        }
        protected override void OnAutoSizeChanged(EventArgs e)
        {
            base.OnAutoSizeChanged(e);
        }
    }
}
