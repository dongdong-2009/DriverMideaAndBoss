using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
namespace All.Class
{
    public static class FileIO
    {
        static object lockObject = new object();
        /// <summary>
        /// 获取当前文件位置
        /// </summary>
        /// <returns></returns>
        public static string GetNowPath()
        {
            FileInfo fi = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().GetModule("All.dll").FullyQualifiedName);
            return fi.DirectoryName;
        }
        /// <summary>
        /// 检测文件夹的路径是否存在，不存在则新建
        /// </summary>
        /// <param name="directory"></param>
        public static void CheckDirectory(string directory)
        {
            string[] dir = directory.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            if (dir.Length == 1)
            {
                return;
            }
            else
            {
                string tmpDir = "";
                for (int i = 0; i < dir.Length - 1; i++)
                {
                    tmpDir = string.Format("{0}{1}\\", tmpDir, dir[i]);
                }
                CheckDirectory(tmpDir);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
        }
        /// <summary>
        /// 检测文件的路径是否存在，不存在则新建
        /// </summary>
        /// <param name="file"></param>
        public static void CheckFileDirectory(string file)
        {
            CheckDirectory(file.Substring(0, file.LastIndexOf('\\')));
        }
        /// <summary>
        /// 读取指定文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ReadFile(string fileName)
        {
            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.OpenOrCreate);
            byte[] buff = new byte[fs.Length];
            fs.Read(buff, 0, buff.Length);
            fs.Flush();
            fs.Close();
            fs.Dispose();
            fs = null;
            if (buff != null && buff.Length > 3 
                && buff[0] == 0xEF
                && buff[1] == 0xBB
                && buff[2] == 0xBF)
            {
                return new UTF8Encoding(false).GetString(buff, 3, buff.Length - 3);
            }
            return Encoding.UTF8.GetString(buff);
        }
        /// <summary>
        /// 读取指定文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ReadFile(string fileName,Encoding encod)
        {
            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.OpenOrCreate);
            byte[] buff = new byte[fs.Length];
            fs.Read(buff, 0, buff.Length);
            fs.Flush();
            fs.Close();
            fs.Dispose();
            fs = null;
            return encod.GetString(buff);
        }
        /// <summary>
        /// 将数据写入指定文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="value"></param>
        public static void Write(string fileName, string value, FileMode fm)
        {
            lock (lockObject)
            {
                using (FileStream fs = new FileStream(fileName,fm))
                {
                    byte[] buff = Encoding.UTF8.GetBytes(value);
                    fs.Write(buff, 0, buff.Length);
                    fs.Close();
                }
            }
        }
        /// <summary>
        /// 将数据写入指定文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="value"></param>
        public static void Write(string fileName, string value)
        {
            Write(fileName, value, FileMode.Append);
        }
        /// <summary>
        /// 将指定文字写入到文件,并换行
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="value"></param>
        public static void WriteLine(string fileName, string value)
        {
            Write(fileName, string.Format("{0}\r\n", value));
        }
        /// <summary>
        /// 将指定文字写入到文件,并换行,且添加日期
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="value"></param>
        public static void WriteDate(string fileName, string value)
        {
            Write(fileName, string.Format("{0:yyyy-MM-dd HH:mm:ss}->{1}\r\n", DateTime.Now, value));
        }
        /// <summary>
        /// 获取指定文件的版本
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFileCode(string fileName,string NullValue)
        {
            string result = NullValue;
            if (System.IO.File.Exists(fileName))
            {
                lock (lockObject)
                {
                    if (fileName.ToUpper().IndexOf(".exe".ToUpper()) > 0 || fileName.ToUpper().IndexOf(".dll".ToUpper()) > 0)
                    {
                        System.Diagnostics.FileVersionInfo fi = System.Diagnostics.FileVersionInfo.GetVersionInfo(fileName);
                        result = fi.FileVersion;
                    }
                    else
                    {
                        using (System.Security.Cryptography.HashAlgorithm ah = System.Security.Cryptography.HashAlgorithm.Create())
                        {
                            using (FileStream fs = new FileStream(fileName, FileMode.Open))
                            {
                                result = All.Class.Num.Hex2Str(ah.ComputeHash(fs));
                            }
                        }
                    }
                }
            }
            return result;
        }
        public static string[] GetLastFile(string directory, int count)
        {
            if (!Directory.Exists(directory))
            {
                return null;
            }
            List<string> result = new List<string>();

            return result.ToArray();
        }
    }
}
