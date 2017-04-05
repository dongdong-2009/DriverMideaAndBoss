using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace All.Control.Metro
{
    [DefaultEvent("Click")]
    public partial class TitleButton : UserControl
    {
        [Description("按钮显示图案列表")]
        public enum ButtonImageList
        {
            PowerOff,
            Back,
            Info,
            Question,
            User,
            Color,
            Min,
            Refresh
        }
        ButtonImageList button = ButtonImageList.Back;
        [Description("按钮显示图案")]
        [Category("Shuai")]
        [DefaultValue(ButtonImageList.Back)]
        /// <summary>
        /// 按钮类型
        /// </summary>
        public ButtonImageList ButtonImage
        {
            get { return button; }
            set
            {
                button = value;
                this.BackgroundImage = GetImage(false);
            }
        }
        bool oldValue = false;
        public void BlinkTimes(int second)
        {
            this.BackgroundImage = GetImage(false);
        }
        public TitleButton()
        {
            InitializeComponent();
            this.BackgroundImage = GetImage(false);
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            this.Height = this.Width;
            base.OnSizeChanged(e);
        }
        Image GetImage(bool MouseIn)
        {
            Image result = null;
            if (MouseIn)
            {
                switch (button)
                {
                    case ButtonImageList.PowerOff:
                        result = Power_MI.Image;
                        break;
                    case ButtonImageList.Back:
                        result = Back_MI.Image;
                        break;
                    case ButtonImageList.Info:
                        result = Info_MI.Image;
                        break;
                    case ButtonImageList.Question:
                        result = Question_MI.Image;
                        break;
                    case ButtonImageList.User:
                        result = User_MI.Image;
                        break;
                    case ButtonImageList.Color:
                        result = Color_MI.Image;
                        break;
                    case ButtonImageList.Min:
                        result = picMin_Mi.Image;
                        break;
                    case ButtonImageList.Refresh:
                        result = Refresh_MI.Image;
                        break;
                }
            }
            else
            {
                switch (button)
                {
                    case ButtonImageList.PowerOff:
                        result = Power_ML.Image;
                        break;
                    case ButtonImageList.Back:
                        result = Back_ML.Image;
                        break;
                    case ButtonImageList.Info:
                        result = Info_ML.Image;
                        break;
                    case ButtonImageList.Question:
                        result = Question_ML.Image;
                        break;
                    case ButtonImageList.User:
                        result = User_ML.Image;
                        break;
                    case ButtonImageList.Color:
                        result = Color_ML.Image;
                        break;
                    case ButtonImageList.Min:
                        result = picMin_ML.Image;
                        break;
                    case ButtonImageList.Refresh:
                        result = Refresh_ML.Image;
                        break;
                }
            }
            return result;
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            oldValue = true;
            this.BackgroundImage = GetImage(oldValue);
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            oldValue = false;
            this.BackgroundImage = GetImage(oldValue);
            base.OnMouseLeave(e);
        }

        private void TitleButton_Load(object sender, EventArgs e)
        {

        }
    }
}
