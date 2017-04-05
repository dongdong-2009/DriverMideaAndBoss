using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace All.Class
{
    /// <summary>
    /// 自已的存文件方式
    /// </summary>
    public class SSFile
    {
        /// <summary>
        /// 将字典数据转化为文本数据
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        public static string Dictionary2Text(Dictionary<string, string> buff)
        {
            string result = "";
            Dictionary<string, string>.Enumerator tmpEnum = buff.GetEnumerator();
            while (tmpEnum.MoveNext())
            {
                result = string.Format("{0}{1}\t{2}\r\n", result, tmpEnum.Current.Key, tmpEnum.Current.Value);
            }
            return result;
        }
        /// <summary>
        /// 将文本数据转化为字典
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        public static Dictionary<string, string> Text2Dictionary(string buff)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string[] tmpChar = buff.Replace(string.Format("\r\n"), string.Format("{0}", Num.SplitChar)).Split(new char[]{Num.SplitChar},StringSplitOptions.RemoveEmptyEntries);
            string[] tmpStr;
            for (int i = 0; i < tmpChar.Length; i++)
            {
                if (tmpChar[i].Length > 0)
                {
                    tmpStr = tmpChar[i].Replace(string.Format("\t"), string.Format("{0}", Num.SplitStr)).Split(new char[]{Num.SplitStr}, StringSplitOptions.RemoveEmptyEntries);
                    if (tmpStr.Length == 2)
                    {
                        result.Add(All.Class.Num.GetVisableStr(tmpStr[0]), tmpStr[1]);
                    }
                }
            }
            return result;
        }
    }
}
