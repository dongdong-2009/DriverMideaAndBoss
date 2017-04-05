using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
namespace All.Class
{
    public static class DownLoadFile
    {
        const string AnonymousUser = "anonymous";

        const string AnonymousPassword = "someone@example.com";

        public static void WebDownLoad(string webUrl, string lcoalFile)
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile(webUrl, lcoalFile);
            }
        }
        /// <summary>
        /// 下载指定文件
        /// </summary>
        /// <param name="ftpUrl"></param>
        /// <param name="localFile"></param>
        public static void FtpDownLoad(string ftpUrl, string localFile)
        {
            FtpDownLoad(ftpUrl, "", "", localFile);
        }
        /// <summary>
        /// 获取指定文件的长度
        /// </summary>
        /// <param name="ftpUrl"></param>
        /// <param name="user"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public static long FtpFileLength(string ftpUrl, string user, string passWord)
        {
            return GetFileValue<long>(ftpUrl, user, passWord);
        }
        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="ftpUrl"></param>
        /// <param name="user"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public static string FtpFileList(string ftpUrl, string user, string passWord)
        {
            return GetFileValue<string>(ftpUrl, user, passWord);
        }
        /// <summary>
        /// 获取指定文件最后修改日期
        /// </summary>
        /// <param name="ftpUrl"></param>
        /// <param name="user"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public static DateTime FtpFileTime(string ftpUrl, string user, string passWord)
        {
            return GetFileValue<DateTime>(ftpUrl, user, passWord);
        }
        private static T GetFileValue<T>(string ftpUrl,string user,string passWord)
        {
            T result = default(T);
            Uri requestUri = new Uri(ftpUrl);
            if (requestUri.Scheme != Uri.UriSchemeFtp)
            {
                All.Class.Log.Add("指定的FTP地址不正确");
                return result;
            } 
            string fileName = Path.GetFileName(requestUri.AbsolutePath);
            switch (typeof(T).ToString())
            {
                case "System.Int64":
                case "System.DateTime":
                    if (String.IsNullOrEmpty(fileName))
                    {
                        All.Class.Log.Add("空文件");
                        return result;
                    }
                    break;
            }
            FtpWebRequest request = null;
            FtpWebResponse response = null;
            try
            {
                request = (FtpWebRequest)WebRequest.Create(requestUri);
                switch (typeof(T).ToString())
                {
                    case "System.Int64":
                        request.Method = WebRequestMethods.Ftp.GetFileSize;
                        break;
                    case "System.DateTime":
                        request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                        break;
                    case "System.String":
                        request.Method = WebRequestMethods.Ftp.ListDirectory;
                        break;
                }
                if (!String.IsNullOrEmpty(user))
                {
                    request.Credentials = new NetworkCredential(user, passWord);
                }
                else
                {
                    request.Credentials = new NetworkCredential(AnonymousUser,
                      !String.IsNullOrEmpty(passWord) ?
                      passWord : AnonymousPassword);
                }
                response = (FtpWebResponse)request.GetResponse();
                switch (typeof(T).ToString())
                {
                    case "System.Int64":
                        result = (T)(object)response.ContentLength;
                        break;
                    case "System.DateTime":
                        result = (T)(object)response.LastModified;
                        break;
                    case "System.String":
                        System.IO.StreamReader sr = new StreamReader(response.GetResponseStream());
                        result=(T)(Object)sr.ReadToEnd();
                        sr.Dispose();
                        break;
                }

            }
            catch(Exception e)
            {
                All.Class.Error.Add(e);
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                response = null;
                request = null;                
            }
            return result;
        }
        /// <summary>
        /// 从远程FTP路径下载文件到指定路径
        /// </summary>
        /// <param name="FtpUrl"></param>
        /// <param name="user"></param>
        /// <param name="passWord"></param>
        /// <param name="localFile"></param>
        public static void FtpDownLoad(string FtpUrl,string user, string passWord,string localFile)
        {

            FileIO.CheckFileDirectory(localFile);

            Uri requestUri = new Uri(FtpUrl);
            if (requestUri.Scheme != Uri.UriSchemeFtp)
            {
                All.Class.Log.Add("指定的FTP地址不正确");
                return;
            }
            string fileName = Path.GetFileName(requestUri.AbsolutePath);
            if (String.IsNullOrEmpty(fileName))
            {
                All.Class.Log.Add("空文件");
                return;
            } 
            Stream bitStream = null;
            FileStream fileStream = null;
            StreamReader reader = null;
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(requestUri);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                if (!String.IsNullOrEmpty(user))
                {
                    request.Credentials = new NetworkCredential(user, passWord);
                }
                else
                {
                    request.Credentials = new NetworkCredential(AnonymousUser,
                      !String.IsNullOrEmpty(passWord) ?
                      passWord : AnonymousPassword);
                }
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                bitStream = response.GetResponseStream();
                fileStream = File.Create(localFile);
                byte[] buffer = new byte[1024];
                Int32 bytesRead = 0;
                while ((bytesRead = bitStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fileStream.Write(buffer, 0, bytesRead);
                }
                bitStream.Close();
                fileStream.Close();
            }
            catch (Exception e)
            {
                All.Class.Error.Add(e);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
                if (bitStream != null)
                {
                    bitStream.Dispose();
                }
                if (fileStream != null)
                {
                    fileStream.Dispose();
                }
            } 
        }
        //}

    }
}
