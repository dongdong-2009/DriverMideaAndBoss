using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace All.Communite
{
    public class Udp : Communite
    {
        All.Class.Udp udpClient;

        public All.Class.Udp UdpClient
        {
            get { return udpClient; }
            set { udpClient = value; }
        }
        public override bool IsOpen
        {
            get 
            {
                return udpClient.IsListen;
            }
        }
        public override string Text
        {
            get;
            set;
        }
        public override int DataRecive
        {
            get 
            {
                if (udpClient == null)
                {
                    return 0;
                }
                return udpClient.DataRecive;
            }
        }
        public override event Communite.GetErrorHandle GetError;
        public override void Init(Dictionary<string, string> InitBuff)
        {
            if (InitBuff.ContainsKey("Text"))
            {
                this.Text = InitBuff["Text"];
            }
            if (!InitBuff.ContainsKey("LocalPort"))
            {
                All.Class.Error.Add(string.Format("初始化UDP失败,初始化字符串不包括本地监听端口,设备名称:{0}", this.Text), Environment.StackTrace);
                return;
            }
            int localPort = Class.Num.ToInt(InitBuff["LocalPort"]);
            udpClient = new Class.Udp(localPort);
            if (InitBuff.ContainsKey("RemotHost"))
            {
                udpClient.RemotHost = InitBuff["RemotHost"];
            }
            if (InitBuff.ContainsKey("RemotPort"))
            {
                udpClient.RemotPort = Class.Num.ToInt(InitBuff["RemotPort"]);
            }
            if (InitBuff.ContainsKey("FlushTick"))
            {
                this.FlushTick = Class.Num.ToInt(InitBuff["FlushTick"]);
            }
        }
        public override void Open()
        {
            try
            {
                if (!udpClient.Open())
                {
                    Class.Log.Add(string.Format("{0}:UDP连接监听失败,监听端口{1}",this.Text, udpClient.LocalPort));
                }
            }
            catch (Exception e)
            {
                Class.Error.Add(e);
                if (GetError != null)
                {
                    GetError(e);
                }
            }
        }
        public override void Close()
        {
            if (udpClient != null)
            {
                udpClient.Close();
            }
        }
        public override void Read<T>(out T Value)
        {
            Value = default(T);
            if (udpClient == null || !udpClient.IsListen)
            {
                Class.Error.Add(string.Format("{0}:Udp为Null或Udp没有打开监听",this.Text), Environment.StackTrace);
                return;
            }

            int readLen = DataRecive;
            byte[] buff = new byte[readLen];
            udpClient.Read(buff, 0, readLen);

            if (typeof(T) == typeof(byte[]))
            {
                Value = (T)(object)buff;
            }
            else if (typeof(T) == typeof(string))
            {
                Value = (T)(object)Encoding.UTF8.GetString(buff, 0, readLen);
            }
            else
            {
                throw new Exception(string.Format("{0}:数据类型错误,此处只能读取string类型或者byte[]类型",this.Text));
            }
        }
        public override void Send<T>(T value)
        {
            Send<T>(value, new Dictionary<string,string>());
        }
        public override void Send<T>(T value, Dictionary<string, string> SendBuff)
        {
            if (udpClient == null || !udpClient.IsListen)
            {
                Class.Error.Add(string.Format("{0}:Udp为Null或Udp没有打开监听",this.Text), Environment.StackTrace);
                return;
            }
            string remotHost = udpClient.RemotHost;
            int remotPort = udpClient.RemotPort;
            if (SendBuff.ContainsKey("RemotHost"))
            {
                remotHost = SendBuff["RemotHost"];
            }
            if (SendBuff.ContainsKey("RemotPort"))
            {
                remotPort = Class.Num.ToInt(SendBuff["RemotPort"]);
            }
            if (typeof(T) == typeof(byte[]))
            {
                byte[] buff = (byte[])(object)value;
                udpClient.DiscardBuffer();
                udpClient.Write(buff, remotHost, remotPort);
            }
            else if (typeof(T) == typeof(string))
            {
                string buff = (string)(object)value;
                udpClient.DiscardBuffer();
                udpClient.Write(buff, remotHost, remotPort);
            }
            else
            {
                throw new Exception(string.Format("{0}:数据类型错误,此处只能写入string类型或者byte[]类型",this.Text));
            }
        }
    }
}
