using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace All.Meter
{
    [Obsolete("不可用，上下位机通讯请使用SSCommunicate")]
    public class SSServer:Meter
    {
        Dictionary<string, string> initParm;
        object lockObject = new object();
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
            if (this.Parent is All.Communite.Com
                || this.Parent is All.Communite.Http
                || this.Parent is All.Communite.TcpClient)
            {
                All.Class.Error.Add("通讯类只能使用UDP的连接方式,其他方式暂时不行", Environment.StackTrace);
            }
        }
        public override bool Read<T>(out List<T> value, Dictionary<string, string> parm)
        {
            value = new List<T>();
            bool result = true;
            int end = 0;
            int start = 0;
            lock (lockObject)
            {
                string sendTime = "";
                string sendData = "";
                sendTime = string.Format("{0}{1}Time{2}{3:yyyy-MM-dd HH:mm:ss}", sendTime, Class.Num.SplitChar, Class.Num.SplitStr, DateTime.Now);
                sendData = string.Format("{0}{1}Type{2}{3}", sendData, Class.Num.SplitChar, Class.Num.SplitStr, typeof(T).ToString().PadRight(14,' '));
                int curLen = 0;
                if (!parm.ContainsKey("Start"))
                {
                    Class.Error.Add("当前读取的数据中不包含起始地址", Environment.StackTrace);
                    return false;
                }
                else
                {
                    start = Class.Num.ToInt(parm["Start"]);
                }
                sendData = string.Format("{0}{1}Start{2}{3}", sendData, Class.Num.SplitChar, Class.Num.SplitStr, start);
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

                sendData = string.Format("{0}{1}End{2}{3}", sendData, Class.Num.SplitChar, Class.Num.SplitStr, end);
                byte[] readBuff = new byte[0];
                switch (Class.TypeUse.GetType<T>())
                {
                    case All.Class.TypeUse.TypeList.String:
                        if (!Write<string, byte[]>(string.Format("Read{0}{1}", sendTime, sendData.Replace("Type", "Len ")), 40, out readBuff))
                        {
                            return false;
                        }
                        else
                        {
                            if (readBuff[0] != 0x53 || readBuff[1] != 0x65 || readBuff[2] != 0x6E || readBuff[3] != 0x64//标头
                                || readBuff[10] != sendData[10] || readBuff[11] != sendData[11] || readBuff[12] != sendData[12] || readBuff[13] != sendData[13]
                                || readBuff[14] != sendData[14] || readBuff[15] != sendData[15] || readBuff[16] != sendData[16] || readBuff[17] != sendData[17]
                                || readBuff[18] != sendData[18] || readBuff[19] != sendData[19] || readBuff[20] != sendData[20] || readBuff[21] != sendData[21]
                                || readBuff[22] != sendData[22] || readBuff[23] != sendData[23] || readBuff[24] != sendData[24] || readBuff[25] != sendData[25]
                                || readBuff[26] != sendData[26] || readBuff[27] != sendData[27] || readBuff[28] != sendData[28])//时间
                            {
                                curLen = Class.Num.ToInt(Encoding.ASCII.GetString(readBuff, 35, 5));
                                sendTime = string.Format("{0}{1}Time{2}{3:yyyy-MM-dd HH:mm:ss}", sendTime, Class.Num.SplitChar, Class.Num.SplitStr, DateTime.Now);
                            }
                            else
                            {
                                Class.Error.Add(string.Format("数据校验失败，发送数据:{0},返回数据{1}",
                                    string.Format("Read{0}{1}", sendTime, sendData.Replace("Type", "Len ")),
                                    Class.Num.Hex2Str(readBuff)), Environment.StackTrace);
                                return false;
                            }
                        }
                        break;
                    case All.Class.TypeUse.TypeList.Byte:
                    case All.Class.TypeUse.TypeList.Boolean:
                        curLen = end - start + 1;
                        break;
                    case All.Class.TypeUse.TypeList.UShort:
                        curLen = (end - start + 1) * 2;
                        break;
                    case All.Class.TypeUse.TypeList.Int:
                    case All.Class.TypeUse.TypeList.Float:
                        curLen = (end - start + 1) * 4;
                        break;
                    case All.Class.TypeUse.TypeList.Double:
                        curLen = (end - start + 1) * 8;
                        break;
                }
                if (!Write<string, byte[]>(string.Format("Read{0}{1}", sendTime, sendData), curLen + 35, out readBuff))
                {
                    return false;
                }
                else
                {
                    if (readBuff[0] != 0x53 || readBuff[1] != 0x65 || readBuff[2] != 0x6E || readBuff[3] != 0x64
                        || readBuff[10] != sendData[10] || readBuff[11] != sendData[11] || readBuff[12] != sendData[12] || readBuff[13] != sendData[13]
                        || readBuff[14] != sendData[14] || readBuff[15] != sendData[15] || readBuff[16] != sendData[16] || readBuff[17] != sendData[17]
                        || readBuff[18] != sendData[18] || readBuff[19] != sendData[19] || readBuff[20] != sendData[20] || readBuff[21] != sendData[21]
                        || readBuff[22] != sendData[22] || readBuff[23] != sendData[23] || readBuff[24] != sendData[24] || readBuff[25] != sendData[25]
                        || readBuff[26] != sendData[26] || readBuff[27] != sendData[27] || readBuff[28] != sendData[28])
                    {
                        switch (Class.TypeUse.GetType<T>())
                        {
                            case All.Class.TypeUse.TypeList.Boolean:
                                for (int i = 35; i < readBuff.Length; i++)
                                {
                                    if (readBuff[i] == 0)
                                    {
                                        value.Add((T)(object)false);
                                    }
                                    else
                                    {
                                        value.Add((T)(object)true);
                                    }
                                }
                                break;
                            case All.Class.TypeUse.TypeList.Byte:
                                for (int i = 35; i < readBuff.Length; i++)
                                {
                                    value.Add((T)(object)readBuff[i]);
                                }
                                break;
                            case All.Class.TypeUse.TypeList.UShort:
                                for (int i = 35; i < readBuff.Length; i = i + 2)
                                {
                                    value.Add((T)(object)(ushort)(readBuff[i] * 0x100 + readBuff[i + 1]));
                                }
                                break;
                            case All.Class.TypeUse.TypeList.Int:
                                for (int i = 35; i < readBuff.Length; i = i + 4)
                                {
                                    value.Add((T)(object)BitConverter.ToInt32(readBuff, i));
                                }
                                break;
                            case All.Class.TypeUse.TypeList.Float:
                                for (int i = 35; i < readBuff.Length; i = i + 4)
                                {
                                    value.Add((T)(object)BitConverter.ToSingle(readBuff, i));
                                }
                                break;
                            case All.Class.TypeUse.TypeList.Double:
                                for (int i = 35; i < readBuff.Length; i = i + 8)
                                {
                                    value.Add((T)(object)BitConverter.ToDouble(readBuff, i));
                                }
                                break;
                            case All.Class.TypeUse.TypeList.String:
                                int len = 0;
                                int index = 35;
                                for (int i = 0; i < end - start + 1; i++)
                                {
                                    len = (readBuff[index] << 8) + readBuff[index + 1];
                                    value.Add((T)(object)Encoding.UTF8.GetString(readBuff, index + 2, len));
                                    index = index + 2 + len;
                                }
                                break;
                        }
                    }
                    else
                    {
                        Class.Error.Add(string.Format("数据校验失败，发送数据:{0},返回数据{1}",
                            string.Format("Read{0}{1}", sendTime, sendData),
                            Class.Num.Hex2Str(readBuff)), Environment.StackTrace);
                        return false;
                    }
                }
            }
            return result;
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
            throw new NotImplementedException();
        }
        public override bool WriteInternal<T>(List<T> value, Dictionary<string, string> parm)
        {
            throw new NotImplementedException();
        }
        public override bool WriteInternal<T>(List<T> value, int start, int end)
        {
            throw new NotImplementedException();
        }
        public override bool WriteInternal<T>(T value, int start)
        {
            throw new NotImplementedException();
        }

    }
}
