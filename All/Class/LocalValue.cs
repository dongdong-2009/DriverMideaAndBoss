using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace All.Class
{
    public static class LocalValue
    {
        static System.Drawing.Color color = System.Drawing.Color.Empty;
        /// <summary>
        /// 颜色
        /// </summary>
        public static System.Drawing.Color Color
        {
            get
            {
                if (color == System.Drawing.Color.Empty)
                {
                    string R = Region.ReadValue("StyleColorR");
                    string G = Region.ReadValue("StyleColorG");
                    string B = Region.ReadValue("StyleColorB");
                    if (R != "" && G != "" && B != "")
                    {
                        color = System.Drawing.Color.FromArgb(
                            Class.Num.ToInt(R), Class.Num.ToInt(G), Class.Num.ToInt(B));
                    }
                }
                return color;
            }
            set
            {
                color = value;
                Region.WriteValue("StyleColorR", color.R.ToString());
                Region.WriteValue("StyleColorG", color.G.ToString());
                Region.WriteValue("StyleColorB", color.B.ToString());
            }
        }
        static Style.Themes theme = Style.Themes.Default;
        /// <summary>
        /// 主题
        /// </summary>
        public static Style.Themes Theme
        {
            get
            {
                if (theme == Style.Themes.Default)
                {
                    Enum.TryParse<Style.Themes>(Region.ReadValue("Theme"), out theme);
                    if (theme == Style.Themes.Default)
                    {
                        theme = Style.Themes.Dark;
                        Region.WriteValue("Theme", theme.ToString());
                    }
                }
                return theme;
            }
            set
            {
                theme = value;
                Region.WriteValue("Theme", theme.ToString());
            }
        }
    }
    
}
