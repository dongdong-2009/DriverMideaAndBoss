using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace All.Class
{
    public static class Log
    {
        static object lockObject = new object();
        static string logPath = "";
        /// <summary>
        /// Log路径
        /// </summary>
        public static string LogPath
        {
            get
            {
                if (logPath == "")
                {
                    logPath = string.Format("{0}\\Log\\", FileIO.GetNowPath());
                    if (!System.IO.Directory.Exists(LogPath))
                    {
                        System.IO.Directory.CreateDirectory(logPath);
                    }
                }
                return logPath;
            }
        }
        static string logFile = "";
        /// <summary>
        /// Log文件
        /// </summary>
        public static string LogFile
        {
            get
            {
                if (logFile == "")
                {
                    lock (lockObject)
                    {
                        logFile = string.Format("{0}\\{1:yyyy-MM-dd}.Txt", LogPath, DateTime.Now);
                        if (!System.IO.File.Exists(logFile))
                        {
                            System.IO.StreamWriter sw = System.IO.File.CreateText(logFile);
                            sw.Close();
                            sw.Dispose();
                        }
                    }
                }
                return logFile;
            }
        }
        /// <summary>
        /// 将消息写入Log
        /// </summary>
        /// <param name="e"></param>
        public static void Add(string e)
        {
            FileIO.Write(LogFile, string.Format("Log时间  ->  {0:yyyy-MM-dd HH:mm:ss}\r\nLog内容  ->  {1}\r\n\r\n", DateTime.Now, e));
        }
        /// <summary>
        /// 将消息写入Log
        /// </summary>
        /// <param name="e"></param>
        /// <param name="StackTrace"></param>
        public static void Add(string e, string StackTrace)
        {
            string[] tmpBuff = StackTrace.Split('\n');
            tmpBuff = tmpBuff[2].Split(new string[] { "位置" }, System.StringSplitOptions.RemoveEmptyEntries);
            string value = "";
            if (tmpBuff.Length > 0 && tmpBuff[0].IndexOf("在") >= 0)
            {
                value = string.Format("{0}Log模块  ->  {1}\r\n", value, tmpBuff[0].Substring(tmpBuff[0].IndexOf("在")));
            }
            if (tmpBuff.Length > 1 && tmpBuff[1].IndexOf("行号") >= 0)
            {
                value = string.Format("{0}Log位置  ->  {1}\r\n", value, tmpBuff[1].Substring(tmpBuff[1].IndexOf("行号")));
            }
            value = string.Format("{0}Log原因  ->  {1}\r\n", value, e);
            FileIO.Write(LogFile, string.Format("Log时间  ->  {0:yyyy-MM-dd HH:mm:ss}\r\n{1}\r\n", DateTime.Now, value));
        }
        /// <summary>
        /// 将读，写数据失败的故障记录下来
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parm"></param>
        public static void Add<T>(List<T> buff, Dictionary<string, string> parm)
        {
            string value = "";
            value = string.Format("{0}数据长度  ->  {1}\r\n", value, buff.Count);
            value = string.Format("{0}数据内容  ->", value);
            for (int i = 0; i < buff.Count; i++)
            {
                value = string.Format("{0}  {1}", value, buff[i].ToString());
            }
            value = string.Format("{0}\r\n", value);
            value = string.Format("{0}数据条件  ->", value);
            parm.Keys.ToList().ForEach(
            key =>
            {
                value = string.Format("{0}  {1}={2}", value, key, parm[key]);
            });
            value = string.Format("{0}\r\n", value);
            value = string.Format("数据时间  ->  {0:yyyy-MM-dd HH:mm:ss}\r\n{1}\r\n", DateTime.Now, value);
            FileIO.Write(LogFile,value);
        }
        /// <summary>
        /// 记录写入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buff"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static void Add<T>(List<T> buff, int start, int end)
        {
            string value = "";
            value = string.Format("{0}数据长度  ->  {1}\r\n", value, buff.Count);
            value = string.Format("{0}数据内容  ->", value);
            for (int i = 0; i < buff.Count; i++)
            {
                value = string.Format("{0}  {1}", value, buff[i].ToString());
            }
            value = string.Format("{0}\r\n", value);
            value = string.Format("{0}写入起点  ->  {1}\r\n", value, start);
            value = string.Format("{0}写入终点  ->  {1}\r\n", value, end);
            value = string.Format("数据时间  ->  {0:yyyy-MM-dd HH:mm:ss}\r\n{1}\r\n", DateTime.Now, value);
            FileIO.Write(LogFile, value);
        }
        /// <summary>
        /// 记录写入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buff"></param>
        /// <param name="start"></param>
        public static void Add<T>(T buff, int start)
        {
            List<T> buffs = new List<T>();
            buffs.Add(buff);
            Add<T>(buffs, start, start);
        }
        /// <summary>
        /// 删除指定日期之前的文档
        /// </summary>
        /// <param name="time">指定日期</param>
        public static void DelMoreLog(DateTime time)
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(LogPath);
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
