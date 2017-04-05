using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace All.Class
{
    public static class Error
    {
        static object lockObject = new object();
        static string errorPath = "";
        /// <summary>
        /// 故障路径
        /// </summary>
        public static string ErrorPath
        {
            get
            {
                if (errorPath == "")
                {
                    errorPath = string.Format("{0}\\Error\\", FileIO.GetNowPath());
                    if (!System.IO.Directory.Exists(ErrorPath))
                    {
                        System.IO.Directory.CreateDirectory(errorPath);
                    }
                }
                return errorPath;
            }
        }
        static string errorFile = "";
        /// <summary>
        /// 故障文件
        /// </summary>
        public static string ErrorFile
        {
            get
            {
                if (errorFile == "")
                {
                    lock (lockObject)
                    {
                        errorFile = string.Format("{0}\\{1:yyyy-MM-dd}.Txt", ErrorPath, DateTime.Now);
                        if (!System.IO.File.Exists(errorFile))
                        {
                            System.IO.StreamWriter sw = System.IO.File.CreateText(errorFile);
                            sw.Close();
                            sw.Dispose();
                        }
                    }
                }
                return errorFile;
            }
        }
        /// <summary>
        /// 用于记录全局错误，补捉的未知错误
        /// </summary>
        static string unKonwErrorFile = "";
        public static string UnKnowErrorFile
        {
            get
            {
                if (unKonwErrorFile == "")
                {
                    lock (lockObject)
                    {
                        unKonwErrorFile = string.Format("{0}\\UnNoErrorFile.Txt", ErrorPath, DateTime.Now);
                        if (!System.IO.File.Exists(unKonwErrorFile))
                        {
                            System.IO.StreamWriter sw = System.IO.File.CreateText(unKonwErrorFile);
                            sw.Close();
                            sw.Dispose();
                        }
                    }
                }
                return unKonwErrorFile;
            }
        }
        /// <summary>
        /// 将消息写入Log
        /// </summary>
        /// <param name="e"></param>
        /// <param name="StackTrace"></param>
        public static void Add(string e, string StackTrace)
        {
            lock (lockObject)
            {
                string[] tmpBuff = StackTrace.Split('\n');
                tmpBuff = tmpBuff[2].Split(new string[] { "位置" }, System.StringSplitOptions.RemoveEmptyEntries);
                string value = "";
                if (tmpBuff.Length > 0 && tmpBuff[0].IndexOf("在") >= 0)
                {
                    value = string.Format("{0}出错模块  ->  {1}\r\n", value, tmpBuff[0].Substring(tmpBuff[0].IndexOf("在")));
                }
                if (tmpBuff.Length > 1 && tmpBuff[1].IndexOf("行号") >= 0)
                {
                    value = string.Format("{0}出错位置  ->  {1}\r\n", value, tmpBuff[1].Substring(tmpBuff[1].IndexOf("行号")));
                }
                value = string.Format("{0}出错原因  ->  {1}\r\n", value, e);
                FileIO.Write(ErrorFile, string.Format("出错时间  ->  {0:yyyy-MM-dd HH:mm:ss}\r\n{1}\r\n", DateTime.Now, value));
            }
        }
        /// <summary>
        /// 将消息写入Log
        /// </summary>
        /// <param name="title">附加标题</param>
        /// <param name="value">附加信息</param>
        public static void Add(string[] title, string[] value)
        {
            lock (lockObject)
            {
                for (int i = 0; i < title.Length && i < value.Length; i++)
                {
                    FileIO.Write(ErrorFile, string.Format("{0}  ->  {1}\r\n", title[i], value[i]));
                }
            }
        }
        /// <summary>
        /// 将消息写入Log
        /// </summary>
        /// <param name="e"></param>
        public static void Add(Exception e)
        {
            lock (lockObject)
            {
                string value = "";
                if (e.Source != null)
                {
                    string[] tmpBuff = e.StackTrace.Split('\n');
                    tmpBuff = tmpBuff[tmpBuff.Length - 1].Split(new string[] { "位置" }, System.StringSplitOptions.RemoveEmptyEntries);


                    if (tmpBuff.Length > 0 && tmpBuff[0].IndexOf("在") >= 0)
                    {
                        value = string.Format("{0}出错模块  ->  {1}\r\n", value, tmpBuff[0].Substring(tmpBuff[0].IndexOf("在")));
                    }
                    if (tmpBuff.Length > 1 && tmpBuff[1].IndexOf("行号") >= 0)
                    {
                        value = string.Format("{0}出错位置  ->  {1}\r\n", value, tmpBuff[1].Substring(tmpBuff[1].IndexOf("行号")));
                    }
                    value = string.Format("{0}出错原因  ->  {1}\r\n", value, e.Message);
                    FileIO.Write(ErrorFile, string.Format("出错时间  ->  {0:yyyy-MM-dd HH:mm:ss}\r\n{1}\r\n", DateTime.Now, value));
                }
                else
                {
                    value = string.Format("{0}出错原因  ->  {1}\r\n", value, e.ToString());
                    FileIO.Write(ErrorFile, string.Format("出错时间  ->  {0:yyyy-MM-dd HH:mm:ss}\r\n{1}\r\n", DateTime.Now, value));

                }
            }
        }
        /// <summary>
        /// 将消息写入Log
        /// </summary>
        /// <param name="e"></param>
        public static void Add(string e)
        {
            lock (lockObject)
            {
                FileIO.Write(ErrorFile, string.Format("出错时间  ->  {0:yyyy-MM-dd HH:mm:ss}\r\n出错内容  ->  {1}\r\n\r\n", DateTime.Now, e));
            }
        }
        /// <summary>
        /// 添加未知错误
        /// </summary>
        /// <param name="e"></param>
        public static void AddUnKonwError(Exception e)
        {
            FileIO.Write(UnKnowErrorFile, string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now));
            FileIO.Write(UnKnowErrorFile, e.StackTrace);
            FileIO.Write(UnKnowErrorFile, e.Message);
            FileIO.Write(UnKnowErrorFile, e.TargetSite.Name);
            FileIO.Write(UnKnowErrorFile, e.ToString());
            FileIO.Write(UnKnowErrorFile, "\r\n");
        }
        /// <summary>
        /// 删除指定日期之前的文档
        /// </summary>
        /// <param name="time">指定日期</param>
        public static void DelMoreError(DateTime time)
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(ErrorPath);
            List<string> fileName = new List<string>();
            foreach (System.IO.FileInfo fi in di.GetFiles())
            {
                if (fi.LastAccessTime < time)
                {
                    fileName.Add(fi.FullName);
                }
            }
            fileName.ForEach(
                file =>
                {
                    try
                    {
                        System.IO.File.Delete(file);
                    }
                    catch
                    { }
                });
        }
    }
}
