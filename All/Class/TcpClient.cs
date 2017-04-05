using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Net;
using System.Net.Sockets;
namespace All.Class
{
    public class TcpClient
    {
        public string RemotHost
        { get; set; }
        public int RemotPort
        { get; set; }
        /// <summary>
        /// 当前接收数据长度
        /// </summary>
        public int DataRecive
        {
            get
            {
                return ReadAllBuff.Count;
            }
        }
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
        Thread thListen;
        System.Net.Sockets.TcpClient tcp;
        object lockObject = new object();
        public TcpClient(string remotHost, int remotPort)
        {
            this.RemotHost = remotHost;
            this.RemotPort = remotPort;
        }
        public TcpClient(System.Net.Sockets.TcpClient tcpFromServer)
        {
            try
            {
                this.tcp = tcpFromServer;
                thListen = new Thread(() => Listen());
                thListen.IsBackground = true;
                thListen.Start();
                isListen = true;
            }
            catch (Exception e)
            {
                All.Class.Error.Add(e);
            }
        }
        public bool Open()
        {
            bool result = false;
            try
            {
                tcp = new System.Net.Sockets.TcpClient();
                
                tcp.Connect(this.RemotHost, this.RemotPort);
                if (tcp.Connected)
                {
                    thListen = new Thread(() => Listen());
                    thListen.IsBackground = true;
                    thListen.Start();
                    result = true;
                    isListen = true;
                }
            }
            catch (Exception e)
            {
                All.Class.Error.Add(new string[] { "远程地址", "远程端口" }, new string[] { this.RemotHost, this.RemotPort.ToString() });
                All.Class.Error.Add(e);
            }
            return result;
        }
        public bool Close()
        {
            bool result = false;
            try
            {
                if (thListen != null)
                {
                    thListen.Abort();
                }
                thListen = null;
                if (tcp != null)
                {
                    tcp.Close();
                }
                tcp = null;
                result = true;
                isListen = false;
            }
            catch (Exception e)
            {
                All.Class.Error.Add(e);
            }
            return result;
        }
        ~TcpClient()
        {
            if (isListen)
            {
                Close();
            }
        }
        public bool Write(string value)
        {
            return Write(Encoding.UTF8.GetBytes(value));
        }
        public bool Write(byte[] value)
        {
            if (tcp == null)
            {
                return false;
            }
            try
            {
                lock (lockObject)
                {
                    tcp.Client.Send(value);
                }
            }
            catch (Exception e)
            {
                Error.Add(e);
                isListen = false;
                return false;
            }
            return true;
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
        private void Listen()
        {
            bool readOver = false;
            byte[] buff = new byte[1024];
            byte[] readBuff;
            IPEndPoint ie = new IPEndPoint(IPAddress.Any,0);
            EndPoint send = (EndPoint)ie;
            int len = 0;
            //while (true)
            //{
                try
                {
                    while (true)
                    {
                        readOver = false;

                        len = tcp.Client.ReceiveFrom(buff, ref send);

                        if (GetStringArgs != null)
                        {
                            GetStringArgs(this, new ReciveString(this.RemotHost, this.RemotPort, Encoding.UTF8.GetString(All.Class.Num.GetByte(buff, 0, len))));
                            readOver = true;
                        }
                        if (GetBytesArgs != null)
                        {
                            GetBytesArgs(this, new ReciveBytes(this.RemotHost, this.RemotPort, All.Class.Num.GetByte(buff, 0, len)));
                            readOver = true;
                        }
                        if (!readOver)
                        {
                            readBuff = new byte[len];
                            Array.Copy(buff, 0, readBuff, 0, len);
                            ReadAllBuff.AddRange(readBuff.ToList());
                            if (ReadAllBuff.Count > 65535)
                            {
                                Error.Add("TCP缓冲区字节数组过长", Environment.StackTrace);
                                ReadAllBuff.Clear();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    if (tcp != null && tcp.Connected)
                    {
                        tcp.Close();
                    }
                    isListen = false;
                    tcp = null;
                    All.Class.Error.Add(new string[] { "远程地址", "远程端口" }, new string[] { this.RemotHost, this.RemotPort.ToString() });
                    All.Class.Error.Add(e);
                }
            //    Thread.Sleep(100);
            //}
        }
    }
}
