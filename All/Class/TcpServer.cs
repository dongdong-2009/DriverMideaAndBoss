using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Threading;
namespace All.Class
{
    public class TcpServer
    {
        System.Net.Sockets.TcpListener tcp;
        /// <summary>
        /// 本地监听端口
        /// </summary>
        public int LocalPort
        { get; set; }
        bool isListen = false;
        /// <summary>
        /// 是否已成功打开监听
        /// </summary>
        public bool IsListen
        {
            get
            {
                return isListen;
            }
        }
        Thread thListen;
        /// <summary>
        /// 所有TCP
        /// </summary>
        List<TcpClient> allTcp = new List<TcpClient>();
        /// <summary>
        /// 数据到达
        /// </summary>
        /// <param name="sender">数据接收UDP</param>
        /// <param name="reciveArgs">接收数据</param>
        public delegate void GetStringArgsHandle(TcpClient sender, ReciveString reciveArgs);
        /// <summary>
        /// 数据到达
        /// </summary>
        public event GetStringArgsHandle GetStringArgs;
        /// <summary>
        /// 数据到达
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reciveArgs"></param>
        public delegate void GetBytesArgsHandle(TcpClient sender, ReciveBytes reciveArgs);
        /// <summary>
        /// 数据到达
        /// </summary>
        public event GetBytesArgsHandle GetBytesArgs;

        List<byte> ReadAllBuff = new List<byte>();

        object lockObject = new object();
        public TcpServer(int localPort)
        {
            this.LocalPort = localPort;
        }
        public bool Open()
        {
            bool result = false;
            try
            {
                tcp = new System.Net.Sockets.TcpListener(IPAddress.Any, LocalPort);
                tcp.Start();
                thListen = new Thread(() => Listen());
                thListen.IsBackground = true;
                thListen.Start();
                result = true;
                isListen = true;
            }
            catch (Exception e)
            {
                All.Class.Error.Add(e);
            }
            return result;
        }
        public void Close()
        {
            for (int i = 0; i < allTcp.Count; i++)
            {
                allTcp[i].Close();
                allTcp[i] = null;
            }
            if (tcp != null)
            {
                tcp.Stop();
            }
            tcp = null;
        }
        ~TcpServer()
        {
            if (isListen)
            {
                Close();
            }
        }
        private void Listen()
        {
            while (true)
            {
                System.Net.Sockets.TcpClient t = tcp.AcceptTcpClient();
                TcpClient ssTcp = new TcpClient(t);
                ssTcp.GetBytesArgs += ssTcp_GetBytesArgs;
                allTcp.Add(ssTcp);
            }
        }
        void ssTcp_GetBytesArgs(TcpClient sender, ReciveBytes reciveArgs)
        {
            bool readOver = false;
            if (GetStringArgs != null)
            {
                GetStringArgs(sender, new ReciveString(reciveArgs.RemoteHost, reciveArgs.RemotePort, Encoding.UTF8.GetString(reciveArgs.Value)));
                readOver = true;
            }
            if (GetBytesArgs != null)
            {
                GetBytesArgs(sender, new ReciveBytes(reciveArgs.RemoteHost, reciveArgs.RemotePort, reciveArgs.Value));
                readOver = true;
            }
            if (!readOver)
            {
                ReadAllBuff.AddRange(reciveArgs.Value);
                if (ReadAllBuff.Count > 65535)
                {
                    Error.Add("TCP缓冲区字节数组过长", Environment.StackTrace);
                    ReadAllBuff.Clear();
                }
            }
        }
        /// <summary>
        /// 丢弃缓冲区数据
        /// </summary>
        public void DiscardBuffer()
        {
            ReadAllBuff.Clear();
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void Read(byte[] buff, int offset, int count)
        {
            if ((offset + count) > ReadAllBuff.Count)
            {
                Error.Add("TCP读取数据长度错误", Environment.StackTrace);
                return;
            }
            Array.Copy(ReadAllBuff.ToArray(), offset, buff, 0, count);
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="buff"></param>
        public void Read(byte[] buff)
        {
            Read(buff, 0, ReadAllBuff.Count);
        }
        public bool Write(string value)
        {
            return Write(Encoding.UTF8.GetBytes(value));
        }
        public bool Write(byte[] value)
        {
            bool result = true;
            for (int i = allTcp.Count - 1; i >= 0; i--)//发送前检查连接状态
            {
                if (!allTcp[i].IsListen)
                {
                    allTcp[i].Close();
                    allTcp.Remove(allTcp[i]);
                }
            }
            for (int i = 0; i < allTcp.Count; i++)
            {
                if (allTcp[i].IsListen)
                {
                    result = result && allTcp[i].Write(value);
                }
            }
            return result;
        }
    }
}
