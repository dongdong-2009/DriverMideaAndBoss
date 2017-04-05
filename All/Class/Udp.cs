using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace All.Class
{
    public class Udp
    {
        UdpClient udp;
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
        /// 本地监听端口
        /// </summary>
        public int LocalPort
        { get; set; }
        /// <summary>
        /// 远程端口
        /// </summary>
        public int RemotPort
        { get; set; }
        /// <summary>
        /// 远程IP地址
        /// </summary>
        public string RemotHost
        { get; set; }
        /// <summary>
        /// 数据到达
        /// </summary>
        /// <param name="sender">数据接收UDP</param>
        /// <param name="reciveArgs">接收数据</param>
        public delegate void GetStringArgsHandle(Udp sender, ReciveString reciveArgs);
        /// <summary>
        /// 数据到达
        /// </summary>
        public event GetStringArgsHandle GetStringArgs;
        /// <summary>
        /// 数据到达
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reciveArgs"></param>
        public delegate void GetBytesArgsHandle(Udp sender, ReciveBytes reciveArgs);
       /// <summary>
       /// 数据到达
       /// </summary>
        public event GetBytesArgsHandle GetBytesArgs;

        List<byte> ReadAllBuff = new List<byte>();
        Thread thListen;
        /// <summary>
        /// 初始化UDP
        /// </summary>
        /// <param name="localPort">本地监听端口</param>
        public Udp(int localPort)
        {
            this.LocalPort = localPort;
            this.RemotHost = "127.0.0.1";
            this.RemotPort = 20000;
        }
        /// <summary>
        /// 初始化UDP
        /// </summary>
        /// <param name="localPort">本地监听端口</param>
        /// <param name="remotHost">远程IP</param>
        /// <param name="remotPort">远程端口</param>
        public Udp(int localPort, string remotHost, int remotPort)
            : this(localPort)
        {
            this.RemotPort = remotPort;
            this.RemotHost = remotHost;
        }
        /// <summary>
        /// 关闭UDP
        /// </summary>
        public void Close()
        {
            if (thListen != null)
            {
                thListen.Abort();
            }
            thListen = null;
            if (udp != null)
            {
                udp.Close();
            }
            udp = null;
            isListen = false;
        }
        ~Udp()
        {
            if (isListen)
            {
                Close();
            }
        }
        /// <summary>
        /// 打开UDP端口
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            bool result = true;
            try
            {
                udp = new UdpClient(LocalPort);
                udp.Client.IOControl((IOControlCode) 0x9800000C, new byte[] { Convert.ToByte(false) }, new byte[4]);
                thListen = new Thread(new ThreadStart(Listen));
                thListen.IsBackground = true;
                thListen.Start();
                isListen = true;
            }
            catch (Exception e)
            {
                All.Class.Error.Add(new string[] { "远程地址", "远程端口" }, new string[] { this.RemotHost, this.RemotPort.ToString() });
                Error.Add(e);
                result = false;
            }
            return result;
        }
        private void Listen()
        {
            bool readOver = false;
            while (true)
            {
                try
                {
                    IPEndPoint tmpRemot = new IPEndPoint(IPAddress.Parse(RemotHost), RemotPort);
                    while (true)
                    {
                        byte[] buff = udp.Receive(ref tmpRemot);
                        readOver = false;
                        if (GetBytesArgs != null)
                        {
                            GetBytesArgs(this, new ReciveBytes(tmpRemot.Address.ToString(), tmpRemot.Port, buff));
                            readOver = true;
                        }
                        if (GetStringArgs != null)
                        {
                            GetStringArgs(this, new ReciveString(tmpRemot.Address.ToString(), tmpRemot.Port, Encoding.UTF8.GetString(buff)));
                            readOver = true;
                        }
                        if (!readOver)
                        {
                            ReadAllBuff.AddRange(buff.ToList());
                            if (ReadAllBuff.Count > 65535)
                            {
                                Error.Add("UDP缓冲区字节数组过长", Environment.StackTrace);
                                ReadAllBuff.Clear();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    All.Class.Error.Add(new string[] { "远程地址", "远程端口" }, new string[] { this.RemotHost, this.RemotPort.ToString() });
                    Error.Add(e);
                 }
                Thread.Sleep(100);
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Write(string value)
        {
            return Write(Encoding.UTF8.GetBytes(value));
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        public bool Write(byte[] buff)
        {
            return Write(buff, this.RemotHost, this.RemotPort);
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="remotHost"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Write(string value, string remotHost, int port)
        {
            return Write(Encoding.UTF8.GetBytes(value), remotHost, port);
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="remotHost"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Write(byte[] buff, string remotHost, int port)
        {
            if (udp == null)
            {
                return false;
            }
            try
            {
                udp.Send(buff, buff.Length, new IPEndPoint(IPAddress.Parse(remotHost), port));
            }
            catch (Exception e)
            {
                Error.Add(e);
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
        public void Read(byte[] buff,int offset,int count)
        {
            if ((offset + count) > ReadAllBuff.Count)
            {
                Error.Add("UDP读取数据长度错误", Environment.StackTrace);
                return;
            }
            Array.Copy(ReadAllBuff.ToArray(), offset, buff, 0, count);
            ReadAllBuff.RemoveRange(0, count);
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="buff"></param>
        public void Read(byte[] buff)
        {
            Read(buff, 0, ReadAllBuff.Count);
        }
    }
    /// <summary>
    /// 接收字符数据
    /// </summary>
    public class ReciveString
    {
        /// <summary>
        /// 远程IP地址
        /// </summary>
        public string RemoteHost
        { get; set; }
        /// <summary>
        /// 远程端口
        /// </summary>
        public int RemotePort
        { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public string Value
        { get; set; }
        public ReciveString(string remoteHost, int remotePort, string value)
        {
            this.RemoteHost = remoteHost;
            this.RemotePort = remotePort;
            this.Value = value;
        }
    }
    /// <summary>
    /// 接收字节数据
    /// </summary>
    public class ReciveBytes
    {
        /// <summary>
        /// 远程IP地址
        /// </summary>
        public string RemoteHost
        { get; set; }
        /// <summary>
        /// 远程端口
        /// </summary>
        public int RemotePort
        { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Value
        { get; set; }
        public ReciveBytes(string remoteHost, int remotePort, byte[] value)
        {
            this.RemoteHost = remoteHost;
            this.RemotePort = remotePort;
            this.Value = (byte[])value.Clone();
        }
    }
}
