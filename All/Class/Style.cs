using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace All.Class
{
    public static class Style
    {
        /// <summary>
        /// 主题
        /// </summary>
        public enum Themes
        {
            /// <summary>
            /// 暗色
            /// </summary>
            Default,
            /// <summary>
            /// 亮色
            /// </summary>
            Light,
            /// <summary>
            /// 暗色
            /// </summary>
            Dark
        }
        /// <summary>
        /// 基础颜色
        /// </summary>
        public static System.Drawing.Color Color
        {
            get { return LocalValue.Color; }
            set { LocalValue.Color = value; }
        }
        /// <summary>
        /// 背景主题
        /// </summary>
        public static Themes Theme
        {
            get { return LocalValue.Theme; }
            set { LocalValue.Theme = value; }

        }
        static  List<ChangeSytle> AllControl = new List<ChangeSytle>();
        /// <summary>
        /// 改变颜色
        /// </summary>
        /// <param name="color"></param>
        public static void ChangeColor(System.Drawing.Color color)
        {
            Color = color;
            foreach (ChangeSytle cs in AllControl)
            {
                cs.ChangeColor(color);
            }
        }
        /// <summary>
        /// 改变主题
        /// </summary>
        /// <param name="theme"></param>
        public static void ChangeTheme(Themes theme)
        {
            Theme = theme;
            foreach (ChangeSytle cs in AllControl)
            {
                cs.ChangeTheme(theme);
            }
        }
        /// <summary>
        /// 添加控件
        /// </summary>
        /// <param name="cs"></param>
        public static void Add(ChangeSytle cs)
        {
            AllControl.Add(cs);
        }
        /// <summary>
        /// 移除控件
        /// </summary>
        /// <param name="cs"></param>
        public static void Remove(ChangeSytle cs)
        {
            AllControl.Remove(cs);
        }
    }
    /// <summary>
    /// 改变风格
    /// </summary>
    public  interface ChangeSytle
    {
        /// <summary>
        /// 改变颜色
        /// </summary>
        /// <param name="color"></param>
        void ChangeColor(System.Drawing.Color color);
        /// <summary>
        /// 改变主题
        /// </summary>
        /// <param name="theme"></param>
        void ChangeTheme(Style.Themes theme);
    }
}
