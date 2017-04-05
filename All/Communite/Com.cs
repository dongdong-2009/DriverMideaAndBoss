using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO.Ports;
namespace All.Communite
{
    public class Com : Communite
    {
        SerialPort com;
        public override string Text
        {
            get;
            set;
        }
        public override int DataRecive
        {
            get
            {
                if (com == null || !com.IsOpen)
                {
                    return 0;
                }
                return com.BytesToRead;
            }
        }
        public override bool IsOpen
        {
            get
            {
                return com.IsOpen;
            }
        }
        public override event Communite.GetErrorHandle GetError;
        ~Com()
        {
            if (com != null)
            {
                Close();
                com.Dispose();
            }
        }

        public override void Init(Dictionary<string, string> InitBuff)
        {
            com = new SerialPort();
            if (InitBuff.ContainsKey("Text"))
            {
                this.Text = InitBuff["Text"];
            }
            if (!InitBuff.ContainsKey("PortName"))
            {
                All.Class.Error.Add(string.Format("初始化串口通讯类失败,初始化字符串不包含串口号,设备名称:{0}", this.Text), Environment.StackTrace);
                return;
            }
            com.PortName = InitBuff["PortName"];


            if (InitBuff.ContainsKey("BaudRate"))
            {
                com.BaudRate = Class.Num.ToInt(InitBuff["BaudRate"]);
            }
            if (InitBuff.ContainsKey("Parity"))
            {
                switch (Class.Num.ToString(InitBuff["Parity"]).ToUpper())
                {
                    case "0":
                    case "NONE":
                        com.Parity = Parity.None;
                        break;
                    case "1":
                    case "ODD":
                        com.Parity = Parity.Odd;
                        break;
                    case "2":
                    case "EVEN":
                        com.Parity = Parity.Even;
                        break;
                }
            }
            if (InitBuff.ContainsKey("DataBits"))
            {
                com.DataBits = Class.Num.ToInt(InitBuff["DataBits"]);
            }
            if (InitBuff.ContainsKey("StopBits"))
            {
                switch (Class.Num.ToString(InitBuff["StopBits"]).ToUpper())
                {
                    case "0":
                    case "NONE":
                        com.StopBits = StopBits.None;
                        break;
                    case "1":
                    case "ONE":
                        com.StopBits = StopBits.One;
                        break;
                    case "1.5":
                        com.StopBits = StopBits.OnePointFive;
                        break;
                    case "2":
                    case "TWO":
                        com.StopBits = StopBits.Two;
                        break;
                }
            }
            if(InitBuff.ContainsKey("RtsEnable"))
            {
                switch (Class.Num.ToString(InitBuff["RtsEnable"]).ToUpper())
                {
                    case "0":
                    case "FALSE":
                        com.RtsEnable = false;
                        break;
                    case "1":
                    case "TRUE":
                        com.RtsEnable = true;
                        break;
                }
            }
            if (InitBuff.ContainsKey("DtrEnable"))
            {
                switch (Class.Num.ToString(InitBuff["DtrEnable"]).ToUpper())
                {
                    case "0":
                    case "FALSE":
                        com.DtrEnable = false;
                        break;
                    case "1":
                    case "TRUE":
                        com.DtrEnable = true;
                        break;
                }
            }
            if (InitBuff.ContainsKey("FlushTick"))
            {
                this.FlushTick = Class.Num.ToInt(InitBuff["FlushTick"]);
            }
        }
        public override void Close()
        {
            try
            {
                if (com.IsOpen)
                {
                    com.Close();
                }
            }
            catch (Exception e)
            {
                Class.Error.Add(e);
            }
        }
        public override void Open()
        {
            try
            {
                if (!com.IsOpen)
                {
                    com.Open();
                }
            }
            catch(Exception e)
            {
                Class.Error.Add(e);
                if (GetError != null)
                {
                    GetError(e);
                }
            }
        }
        public override void Read<T>(out T Value)
        {
            Value = default(T);
            if (com == null || !com.IsOpen)
            {
                Class.Error.Add(string.Format("{0}:Com为Null或Com没有打开监听", this.Text), Environment.StackTrace);
                return;
            }

            int readLen = DataRecive;
            byte[] buff = new byte[readLen];
            com.Read(buff, 0, readLen);

            if (typeof(T) == typeof(byte[]))
            {
                Value = (T)(object)buff;
            }
            else if (typeof(T) == typeof(string))
            {
                Value = (T)(object)Encoding.ASCII.GetString(buff, 0, readLen);
            }
            else
            {
                throw new Exception(string.Format("{0}:数据类型错误,此处只能读取string类型或者byte[]类型", this.Text));
            }
        }
        public override void Send<T>(T value)
        {
            if (com == null || !com.IsOpen)
            {
                Class.Error.Add(string.Format("{0}:Com为Null或Com没有打开监听",this.Text),Environment.StackTrace);
                return;
            }

            if (typeof(T) == typeof(byte[]))
            {
                byte[] buff = (byte[])(object)value;
                com.DiscardInBuffer();
                com.Write(buff, 0, buff.Length);
            }
            else if (typeof(T) == typeof(string))
            {
                string buff = (string)(object)value;
                com.DiscardInBuffer();
                com.Write(buff);
            }
            else
            {
                throw new Exception(string.Format("{0}:数据类型错误,此处只能写入string类型或者byte[]类型",this.Text));
            }
        }
        public override void Send<T>(T value, Dictionary<string, string> SendBuff)
        {
            if (SendBuff == null)
            {
                All.Class.Error.Add(string.Format("{0}的初始化数据为空", this.Text), Environment.StackTrace);
            }
            if (SendBuff.ContainsKey("BaudRate"))
            {
                com.BaudRate = Class.Num.ToInt(SendBuff["BaudRate"]);
            }
            if (SendBuff.ContainsKey("Parity"))
            {
                switch (Class.Num.ToString(SendBuff["Parity"]).ToUpper())
                {
                    case "0":
                    case "NONE":
                        com.Parity = Parity.None;
                        break;
                    case "1":
                    case "ODD":
                        com.Parity = Parity.Odd;
                        break;
                    case "2":
                    case "EVEN":
                        com.Parity = Parity.Even;
                        break;
                }
            }
            if (SendBuff.ContainsKey("DataBits"))
            {
                com.DataBits = Class.Num.ToInt(SendBuff["DataBits"]);
            }
            if (SendBuff.ContainsKey("StopBits"))
            {
                switch (Class.Num.ToString(SendBuff["StopBits"]).ToUpper())
                {
                    case "0":
                    case "NONE":
                        com.StopBits = StopBits.None;
                        break;
                    case "1":
                    case "ONE":
                        com.StopBits = StopBits.One;
                        break;
                    case "1.5":
                        com.StopBits = StopBits.OnePointFive;
                        break;
                    case "2":
                    case "TWO":
                        com.StopBits = StopBits.Two;
                        break;
                }
            }
            if (SendBuff.ContainsKey("RtsEnable"))
            {
                switch (Class.Num.ToString(SendBuff["RtsEnable"]).ToUpper())
                {
                    case "0":
                    case "FALSE":
                        com.RtsEnable = false;
                        break;
                    case "1":
                    case "TRUE":
                        com.RtsEnable = true;
                        break;
                }
            }
            if (SendBuff.ContainsKey("DtrEnable"))
            {
                switch (Class.Num.ToString(SendBuff["DtrEnable"]).ToUpper())
                {
                    case "0":
                    case "FALSE":
                        com.DtrEnable = false;
                        break;
                    case "1":
                    case "TRUE":
                        com.DtrEnable = true;
                        break;
                }
            }
            Send<T>(value);
        }
    }
}
