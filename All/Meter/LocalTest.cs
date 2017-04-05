using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace All.Meter
{
    public class LocalTest:Meter
    {
        Dictionary<string, string> initParm;

        public override Dictionary<string, string> InitParm
        {
            get { return initParm; }
            set { initParm = value; }
        }
        object lockObject = new object();
        public override void Init(Dictionary<string, string> initParm)
        {
            if (initParm.ContainsKey("Text"))
            {
                this.Text = initParm["Text"];
            }
            if (initParm.ContainsKey("TimeOut"))
            {
                this.TimeOut = All.Class.Num.ToInt(initParm["TimeOut"]);
            }
            InitParm = initParm;
        }
        public override bool Read<T>(out List<T> value, int start, int end)
        {
            Dictionary<string, string> parm = new Dictionary<string, string>();
            parm.Add("Start", start.ToString());
            parm.Add("End", end.ToString());

            return Read<T>(out value, parm);
        }
        public override bool Read<T>(out T value, int start)
        {
            List<T> tmpbuff = new List<T>();
            bool result = Read<T>(out tmpbuff, start, start);
            value = tmpbuff[0];
            return result;
        }
        public override bool Read<T>(out List<T> value, Dictionary<string, string> parm)
        {
            lock (lockObject)
            {
                bool result = true;
                value = new List<T>();
                int start = 0;
                int end = 0;
                if (parm.ContainsKey("Start"))
                {
                    All.Class.Error.Add(string.Format("{0}:读取数据不包含起始点", this.Text), Environment.StackTrace);
                }
                start = All.Class.Num.ToInt(parm["Start"]);
                if (parm.ContainsKey("End"))
                {
                    All.Class.Error.Add(string.Format("{0}:读取数据不包含结束点", this.Text), Environment.StackTrace);
                }
                end = All.Class.Num.ToInt(parm["End"]);
                try
                {
                    string sendBuff = "";
                    if (typeof(T) == typeof(string) || typeof(T) == typeof(double) || typeof(T) == typeof(float))
                    {
                        for (int i = start; i <= end; i++)
                        {
                            sendBuff = string.Format("{0}{1:F3}{2}", sendBuff, i + All.Class.Num.GetRandom(0, 1), All.Class.Num.SplitStr);
                        }
                    }
                    if (typeof(T) == typeof(int) || typeof(T) == typeof(ushort) || typeof(T) == typeof(byte))
                    {
                        for (int i = start; i <= end; i++)
                        {
                            sendBuff = string.Format("{0}{1}{2}", sendBuff, i * 2 + (int)All.Class.Num.GetRandom(0, 1), All.Class.Num.SplitStr);
                        }
                    }
                    if (typeof(T) == typeof(bool))
                    {
                        for (int i = start; i <= end; i++)
                        {
                            sendBuff = string.Format("{0}{1}{2}", sendBuff, All.Class.Num.GetRandom(0, 1) > 0.5, All.Class.Num.SplitStr);
                        }
                    }
                    sendBuff = sendBuff.Substring(0, sendBuff.Length - 1);
                    if (this.Parent is Communite.Udp && InitParm.ContainsKey("RemotHost") && InitParm.ContainsKey("RemotPort"))
                    {
                        this.Parent.Send<string>(sendBuff, InitParm);
                    }
                    else
                    {
                        this.Parent.Send<string>(sendBuff);
                    }
                    System.Threading.Thread.Sleep(100);
                    if (this.Parent.DataRecive > 0)
                    {
                        string tmp = "";
                        this.Parent.Read<string>(out tmp);
                        if (tmp != sendBuff)
                        {
                            All.Class.Error.Add("读取到数据和发送的数据不相同，查问题吧", Environment.StackTrace);
                        }
                        else
                        {
                            string[] buff = tmp.Split(All.Class.Num.SplitStr);
                            if (typeof(T) == typeof(string))
                            {
                                for (int i = 0; i < buff.Length; i++)
                                {
                                    value.Add((T)(object)buff[i]);
                                }
                            }
                            if (typeof(T) == typeof(ushort))
                            {
                                for (int i = 0; i < buff.Length; i++)
                                {
                                    value.Add((T)(object)All.Class.Num.ToUshort(buff[i]));
                                }
                            }
                            if (typeof(T) == typeof(int))
                            {
                                for (int i = 0; i < buff.Length; i++)
                                {
                                    value.Add((T)(object)All.Class.Num.ToInt(buff[i]));
                                }
                            }
                            if (typeof(T) == typeof(float))
                            {
                                for (int i = 0; i < buff.Length; i++)
                                {
                                    value.Add((T)(object)All.Class.Num.ToFloat(buff[i]));
                                }
                            }
                            if (typeof(T) == typeof(double))
                            {
                                for (int i = 0; i < buff.Length; i++)
                                {
                                    value.Add((T)(object)All.Class.Num.ToBool(buff[i]));
                                }
                            }
                            if (typeof(T) == typeof(byte))
                            {
                                for (int i = 0; i < buff.Length; i++)
                                {
                                    value.Add((T)(object)All.Class.Num.ToByte(buff[i]));
                                }
                            }
                            if (typeof(T) == typeof(bool))
                            {
                                for (int i = 0; i < buff.Length; i++)
                                {
                                    value.Add((T)(object)All.Class.Num.ToBool(buff[i]));
                                }
                            }
                            this.Conn = true;
                        }
                    }
                    else
                    {
                        result = false;
                        Error = string.Format("{0}:获取数据超时", Text);
                    }
                }
                catch (Exception e)
                {
                    result = false;
                    this.Conn = false;
                    All.Class.Error.Add(e);
                }
                return result;
            }
        }
        public override bool WriteInternal<T>(List<T> value, int start, int end)
        {
            lock (lockObject)
            {
                bool result = true;
                string sendBuff = "";
                try
                {
                    for (int i = 0; i < value.Count; i++)
                    {
                        sendBuff = string.Format("{0}{1}{2}", sendBuff, value[i], All.Class.Num.SplitStr);
                    }
                    sendBuff = sendBuff.Substring(0, sendBuff.Length - 1);
                    if (this.Parent is Communite.Udp && InitParm.ContainsKey("RemotHost") && InitParm.ContainsKey("RemotPort"))
                    {
                        this.Parent.Send<string>(sendBuff, InitParm);
                    }
                    else
                    {
                        this.Parent.Send<string>(sendBuff);
                    }
                    System.Threading.Thread.Sleep(100);
                    if (this.Parent.DataRecive <= 0)
                    {
                        result = false;
                        Conn = false;
                    }
                    else
                    {
                        Conn = true;
                    }
                }
                catch (Exception e)
                {
                    result = false;
                    All.Class.Error.Add(e);
                }
                return result;
            }
        }
        public override bool WriteInternal<T>(T value, int start)
        {
            List<T> tmp = new List<T>();
            tmp.Add(value);
            return WriteInternal<T>(tmp, start, start);
        }
        public override bool WriteInternal<T>(List<T> value, Dictionary<string, string> parm)
        {
            throw new NotImplementedException();
        }
    }
}
