using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace All.Communite
{
    public class TcpClient:Communite
    {
        public All.Class.TcpClient Tcp
        {
            get;
            set;
        }
        public override string Text
        {
            get;
            set;
        }
        public override bool IsOpen
        {
            get {
                return Tcp.IsListen;
            }
        }
        public override int DataRecive
        {
            get
            {
                if (Tcp == null)
                {
                    return 0;
                }
                return Tcp.DataRecive;
            }
        }
        public override event Communite.GetErrorHandle GetError;
        public override void Init(Dictionary<string, string> InitBuff)
        {
            if (InitBuff.ContainsKey("Text"))
            {
                this.Text = InitBuff["Text"];
            }
            if (!InitBuff.ContainsKey("RemotHost"))
            {
                All.Class.Error.Add(string.Format("初始化TCP失败,初始化字符串不包括远程地址,设备名称:{0}", this.Text), Environment.StackTrace);
                return;
            }
            if (!InitBuff.ContainsKey("RemotPort"))
            {
                All.Class.Error.Add(string.Format("初始化TCP失败,初始化字符串不包括远程端口,设备名称:{0}", this.Text), Environment.StackTrace);
                return;
            }
            Tcp = new Class.TcpClient(InitBuff["RemotHost"], Class.Num.ToInt(InitBuff["RemotPort"]));
            
            if (InitBuff.ContainsKey("FlushTick"))
            {
                this.FlushTick = Class.Num.ToInt(InitBuff["FlushTick"]);
            }
        }
        public override void Read<T>(out T Value)
        {
            Value = default(T);
            if (Tcp == null || !Tcp.IsListen)
            {
                Class.Error.Add(string.Format("{0}:Tcp为Null或tcp没有打开监听", this.Text), Environment.StackTrace);
                return;
            }

            int readLen = DataRecive;
            byte[] buff = new byte[readLen];
            Tcp.Read(buff, 0, readLen);

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
                throw new Exception(string.Format("{0}:数据类型错误,此处只能读取string类型或者byte[]类型", this.Text));
            }
        }
        public override void Open()
        {
            try
            {
                if (!Tcp.Open())
                {
                    Class.Log.Add(string.Format("{0}:TCP连接监听失败,远程地址{1}", this.Text, Tcp.RemotHost));
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
            if (Tcp != null)
            {
                Tcp.Close();
            }
        }
        public override void Send<T>(T value)
        {
            Send<T>(value, new Dictionary<string, string>());
        }
        public override void Send<T>(T value, Dictionary<string, string> SendBuff)
        {
            if (Tcp == null || !Tcp.IsListen)
            {
                Class.Error.Add(string.Format("{0}:tcp为Null或Udp没有打开监听", this.Text), Environment.StackTrace);
                return;
            }
            if (typeof(T) == typeof(byte[]))
            {
                byte[] buff = (byte[])(object)value;
                Tcp.DiscardBuffer();
                Tcp.Write(buff);
            }
            else if (typeof(T) == typeof(string))
            {
                string buff = (string)(object)value;
                Tcp.DiscardBuffer();
                Tcp.Write(buff);
            }
            else
            {
                throw new Exception(string.Format("{0}:数据类型错误,此处只能写入string类型或者byte[]类型", this.Text));
            }
        }
    }
}
