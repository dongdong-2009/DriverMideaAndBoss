using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace All.Meter
{
    public class SSWrite:Meter
    {
        Dictionary<string, string> initParm = new Dictionary<string, string>();
        public override Dictionary<string, string> InitParm
        {
            get
            {
                return initParm;
            }
            set
            {
                initParm = value;
            }
        }
        object lockObject = new object();
        public override void Init(Dictionary<string, string> initParm)
        {
            this.InitParm = initParm;
            if (initParm.ContainsKey("Text"))
            {
                this.Text = initParm["Text"];
            }
            if (initParm.ContainsKey("TimeOut"))
            {
                this.TimeOut = All.Class.Num.ToInt(initParm["TimeOut"]);
            }
            if (initParm.ContainsKey("ErrorCount"))
            {
                this.ErrorCount = All.Class.Num.ToInt(initParm["ErrorCount"]);
            }
            if (this.Parent.Sons.Count > 1)
            {
                All.Class.Error.Add("信息交互通讯类,此设备必须独占一个通讯方式,父通讯类下只能存在一个子设备", Environment.StackTrace);
                return;
            }
        }
        public override bool Read<T>(out List<T> value, Dictionary<string, string> parm)
        {
            throw new NotImplementedException();
        }
        public override bool Read<T>(out List<T> value, int start, int end)
        {
            throw new NotImplementedException();
        }
        public override bool Read<T>(out T value, int start)
        {
            throw new NotImplementedException();
        }
        public override bool WriteInternal<T>(List<T> value, Dictionary<string, string> parm)
        {
            bool result = false;

            lock (lockObject)
            {
                int end = 0;
                int start = 0;
                List<byte> titleBuff = new List<byte>();
                List<byte> dataBuff = new List<byte>();
                List<byte> endBuff = new List<byte>();
                if (!parm.ContainsKey("Start"))
                {
                    Class.Error.Add("当前读取的数据中不包含起始地址", Environment.StackTrace);
                    return false;
                }
                else
                {
                    start = Class.Num.ToInt(parm["Start"]);
                }
                if (!parm.ContainsKey("End"))
                {
                    Class.Error.Add("当前读取的数据中不包含数据长度", Environment.StackTrace);
                    return false;
                }
                else
                {
                    end = Class.Num.ToInt(parm["End"]);
                }
                if (end < start)
                {
                    end = end + start;
                    start = end - start;
                    end = end - start;
                }
                int LiuShuiHaoStart = (int)All.Class.Num.GetRandom(0, 0xFFFF);

                titleBuff.Add(Convert.ToByte('S'));
                titleBuff.Add(Convert.ToByte('S'));
                titleBuff.Add((byte)((LiuShuiHaoStart >> 8) & 0xFF));
                titleBuff.Add((byte)((LiuShuiHaoStart >> 0) & 0xFF));
                titleBuff.Add((byte)All.Class.TypeUse.GetType<T>());
                titleBuff.Add((byte)((start >> 8) & 0xFF));
                titleBuff.Add((byte)((start >> 0) & 0xFF));
                titleBuff.Add((byte)((end >> 8) & 0xFF));
                titleBuff.Add((byte)((end >> 0) & 0xFF));

                dataBuff = GetSendValue<T>(value);

                endBuff.Add(Convert.ToByte('X'));
                endBuff.Add(Convert.ToByte('X'));

                byte[] sendBuff = new byte[titleBuff.Count + dataBuff.Count + endBuff.Count];
                Array.Copy(titleBuff.ToArray(), 0, sendBuff, 0, titleBuff.Count);
                Array.Copy(dataBuff.ToArray(), 0, sendBuff, titleBuff.Count, dataBuff.Count);
                Array.Copy(endBuff.ToArray(), 0, sendBuff, titleBuff.Count + dataBuff.Count, endBuff.Count);

                byte[] readBuff;
                if (Write<byte[], byte[]>(sendBuff, 16, out readBuff))
                {
                    if (readBuff[0] != sendBuff[0] || readBuff[1] != sendBuff[1] || readBuff[2] != sendBuff[2]
                     || readBuff[3] != sendBuff[3] || readBuff[4] != sendBuff[4] || readBuff[5] != sendBuff[5]
                     || readBuff[6] != sendBuff[6] || readBuff[7] != sendBuff[7] || readBuff[8] != sendBuff[8]
                     || readBuff[14] != endBuff[0] || readBuff[15] != endBuff[1])
                    {
                        All.Class.Error.Add(new string[] { "写入数据", "返回数据" }, new string[] { All.Class.Num.Hex2Str(sendBuff), All.Class.Num.Hex2Str(readBuff) });
                        All.Class.Error.Add(string.Format("{0}:写入数据校验错误", this.Text), Environment.StackTrace);
                    }
                    else
                    {
                        if (readBuff[9] == Convert.ToByte('A') && readBuff[10] == Convert.ToByte('l')
                            && readBuff[11] == Convert.ToByte('l') && readBuff[12] == Convert.ToByte('O') && readBuff[13] == Convert.ToByte('K'))
                        {
                            result = true;
                        }
                        if (readBuff[9] == Convert.ToByte('E') && readBuff[10] == Convert.ToByte('r')
                            && readBuff[11] == Convert.ToByte('r') && readBuff[12] == Convert.ToByte('o') && readBuff[13] == Convert.ToByte('r'))
                        {
                            All.Class.Error.Add("写入远程数据时，远程端返回错误", Environment.StackTrace);
                        }
                    }
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }
        public override bool WriteInternal<T>(List<T> value, int start, int end)
        {
            Dictionary<string, string> parm = new Dictionary<string, string>();
            parm.Add("Start", start.ToString());
            parm.Add("End", end.ToString());
            return WriteInternal<T>(value, parm);
        }
        public override bool WriteInternal<T>(T value, int start)
        {
            List<T> tmpValue = new List<T>();
            tmpValue.Add(value);
            return WriteInternal<T>(tmpValue, start, start);
        }
        /// <summary>
        /// 将要发送的数据转化发送指令
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private List<byte> GetSendValue<T>(List<T> value)
        {
            List<byte> result = new List<byte>();
            byte[] tmpBuff;
            if (value == null)
            {
                return result;
            }
            switch (All.Class.TypeUse.GetType<T>())
            {
                case Class.TypeUse.TypeList.Byte:
                    for (int i = 0; i < value.Count; i++)
                    {
                        result.Add((byte)(object)value[i]);
                    }
                    break;
                case Class.TypeUse.TypeList.String:
                    for (int i = 0; i < value.Count; i++)
                    {
                        tmpBuff = Encoding.UTF8.GetBytes((string)(object)value[i]);
                        result.Add((byte)((tmpBuff.Length >> 8) & 0xFF));
                        result.Add((byte)((tmpBuff.Length >> 0) & 0xFF));
                        for (int j = 0; j < tmpBuff.Length; j++)
                        {
                            result.Add(tmpBuff[j]);
                        }
                    }
                    break;
                case Class.TypeUse.TypeList.Double:
                    for (int i = 0; i < value.Count; i++)
                    {
                        tmpBuff = BitConverter.GetBytes((double)(object)value[i]);
                        for (int j = 0; j < tmpBuff.Length; j++)
                        {
                            result.Add(tmpBuff[j]);
                        }
                    }
                    break;
                case Class.TypeUse.TypeList.UShort:
                    for (int i = 0; i < value.Count; i++)
                    {
                        tmpBuff = BitConverter.GetBytes((ushort)(object)value[i]);
                        for (int j = 0; j < tmpBuff.Length; j++)
                        {
                            result.Add(tmpBuff[j]);
                        }
                    }
                    break;
                case Class.TypeUse.TypeList.Int:
                    for (int i = 0; i < value.Count; i++)
                    {
                        tmpBuff = BitConverter.GetBytes((int)(object)value[i]);
                        for (int j = 0; j < tmpBuff.Length; j++)
                        {
                            result.Add(tmpBuff[j]);
                        }
                    }
                    break;
                case Class.TypeUse.TypeList.Float:
                    for (int i = 0; i < value.Count; i++)
                    {
                        tmpBuff = BitConverter.GetBytes((float)(object)value[i]);
                        for (int j = 0; j < tmpBuff.Length; j++)
                        {
                            result.Add(tmpBuff[j]);
                        }
                    }
                    break;
                case Class.TypeUse.TypeList.Boolean:
                    for (int i = 0; i < value.Count; i++)
                    {
                        result.Add(((bool)(object)value[i]) ? (byte)1 : (byte)0);
                    }
                    break;
            }
            return result;
        }
    }
}
