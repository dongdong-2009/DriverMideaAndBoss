using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace All.Meter
{
    public class NetModbusRtu:Meter
    {
        Dictionary<string, string> initParm;
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
        byte address = 0;
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
            if (!InitParm.ContainsKey("Address"))
            {
                All.Class.Error.Add("标准Modbus参数中没有地址", Environment.StackTrace);
            }
            else
            {
                address = All.Class.Num.ToByte(InitParm["Address"]);
            }
        }
        public override bool Read<T>(out List<T> value, Dictionary<string, string> parm)
        {
            bool result = true;
            value = new List<T>();
            int start = 0;
            int end = 0;
            byte[] sendBuff = new byte[12];
            int readLen = 0;
            if (!parm.ContainsKey("Start"))
            {
                All.Class.Error.Add(string.Format("{0}:读取数据不包含起始点", this.Text), Environment.StackTrace);
                return false;
            }
            start = All.Class.Num.ToInt(parm["Start"]);
            if (!parm.ContainsKey("End"))
            {
                All.Class.Error.Add(string.Format("{0}:读取数据不包含结束点", this.Text), Environment.StackTrace);
                return false;
            }
            end = All.Class.Num.ToInt(parm["End"]);
            lock (lockObject)
            {
                switch (All.Class.TypeUse.GetType<T>())
                {
                    case Class.TypeUse.TypeList.String:
                        break;
                    case Class.TypeUse.TypeList.Float:
                        break;
                    case Class.TypeUse.TypeList.Boolean:
                        break;
                    case Class.TypeUse.TypeList.Byte:
                        break;
                    case Class.TypeUse.TypeList.UShort:
                        break;
                    case Class.TypeUse.TypeList.Int:
                        break;
                    default:
                        All.Class.Error.Add("ModbusRtu不支持当前的数据类型读取");
                        return false;
                }
                sendBuff[0] = 0x00;
                sendBuff[1] = 0x00;
                sendBuff[2] = 0x00;
                sendBuff[3] = 0x00;
                sendBuff[4] = 0x00;
                sendBuff[5] = 0x06;
                sendBuff[6] = address;
                sendBuff[8] = (byte)(((start) >> 8) & 0xFF);
                sendBuff[9] = (byte)(((start) >> 0) & 0xFF);
                switch (All.Class.TypeUse.GetType<T>())
                {
                    case Class.TypeUse.TypeList.Byte:
                    case Class.TypeUse.TypeList.String:
                    case Class.TypeUse.TypeList.UShort:
                    case Class.TypeUse.TypeList.Float:
                    case Class.TypeUse.TypeList.Int://此处坚决不改,如有不对的,改寄存器读取地址
                        sendBuff[7] = 0x03;
                        sendBuff[10] = (byte)(((end - start + 1) >> 8) & 0xFF);
                        sendBuff[11] = (byte)(((end - start + 1) >> 0) & 0xFF);
                        readLen = 9 + (end - start + 1) * 2;
                        break;
                    case Class.TypeUse.TypeList.Boolean:
                        sendBuff[7] = 0x01;
                        sendBuff[10] = (byte)((((int)(Math.Ceiling((end - start + 1) / 16.0f) * 16.0f)) >> 8) & 0xFF);
                        sendBuff[11] = (byte)((((int)(Math.Ceiling((end - start + 1) / 16.0f) * 16.0f)) >> 0) & 0xFF);
                        readLen = 9 + (int)(Math.Ceiling((end - start + 1) / 16.0f)) * 2;
                        break;

                }
                byte[] readBuff;
                if (Write<byte[], byte[]>(sendBuff, readLen, out readBuff))
                {
                    if (sendBuff[0] == readBuff[0] && sendBuff[1] == readBuff[1] && sendBuff[2] == readBuff[2] && sendBuff[3] == readBuff[3] &&
                        sendBuff[4] == readBuff[4] && (readBuff[5] + 6) == readBuff.Length && readBuff[6] == sendBuff[6] && readBuff[7] == sendBuff[7])
                    {
                        switch (Class.TypeUse.GetType<T>())
                        {
                            case Class.TypeUse.TypeList.Byte:
                                for (int i = 0, j = 9; i < (end - start + 1) * 2 && j < readBuff.Length; i++, j++)
                                {
                                    value.Add((T)(object)readBuff[j]);
                                }
                                break;
                            case Class.TypeUse.TypeList.Int:
                                for (int i = 0, j = 9; i < (end - start + 1) * 2 && j < readBuff.Length; i = i + 2, j = j + 2)
                                {
                                    int tmpValue = readBuff[i] * 0x100 + readBuff[i + 1];
                                    if (tmpValue >= 0x8000)
                                    {
                                        tmpValue = -(tmpValue ^ 0xFFFF) - 1;
                                    }
                                    value.Add((T)(object)tmpValue);
                                }
                                break;
                            case Class.TypeUse.TypeList.UShort:
                                for (int i = 0, j = 9; i < (end - start + 1) * 2 && j < readBuff.Length; i = i + 2, j = j + 2)
                                {
                                    value.Add((T)(object)(ushort)(readBuff[j] * 0x100 + readBuff[j + 1]));
                                }
                                break;
                            case Class.TypeUse.TypeList.Float:
                                for (int i = 0, j = 9; i < (end - start + 1) * 2 && j < readBuff.Length; i = i + 4, j = j + 4)
                                {
                                    value.Add((T)(object)All.Class.Num.ByteToFloat(readBuff, j, All.Class.Num.QueueList.四三二一));
                                }
                                break;
                            case Class.TypeUse.TypeList.String:
                                if (parm.ContainsKey("Part") && All.Class.Num.ToInt(parm["Part"]) > 0)
                                {
                                    int part = All.Class.Num.ToInt(parm["Part"]);
                                    for (int i = 0; i < (end - start + 1) * 2; i = i + part)
                                    {
                                        value.Add((T)(object)Encoding.GetEncoding("GB2312").GetString(readBuff, 9 + i, part));
                                    }
                                }
                                else
                                {
                                    value.Add((T)(object)Encoding.GetEncoding("GB2312").GetString(readBuff, 9, (end - start + 1) * 2));
                                }
                                break;
                            case Class.TypeUse.TypeList.Boolean:
                                bool[] tmpBool = All.Class.Num.Byte2Bool(All.Class.Num.GetByte(readBuff, 9, readLen - 9));
                                for (int i = start; i <= end; i++)
                                {
                                    value.Add((T)(object)tmpBool[i - start]);
                                }
                                break;
                            default:
                                All.Class.Error.Add("McgsModbusRtu不支持当前的数据类型读取");
                                return false;
                        }
                    }
                    else
                    {
                        All.Class.Error.Add(string.Format("{0}:读取数据校验错误", this.Text), Environment.StackTrace);
                        return false;
                    }
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }
        public override bool Read<T>(out List<T> value, int start, int end)
        {
            Dictionary<string, string> parm = new Dictionary<string, string>();
            parm.Add("Start", start.ToString());
            parm.Add("End", end.ToString());
            return Read(out value, parm);
        }
        public override bool Read<T>(out T value, int start)
        {
            List<T> buff = new List<T>();
            bool result = Read<T>(out buff, start, start);
            if (buff.Count > 0)
            {
                value = buff[0];
            }
            else
            {
                value = default(T);
            }
            return result;
        }
        public override bool WriteInternal<T>(List<T> value, Dictionary<string, string> parm)
        {
            bool result = true;
            if (value == null || value.Count <= 0)
            {
                return false;
            }
            int start = 0;
            //int end = 0;
            if (!parm.ContainsKey("Start"))
            {
                All.Class.Error.Add(string.Format("{0}:读取数据不包含起始点", this.Text), Environment.StackTrace);
                return false;
            }
            start = All.Class.Num.ToInt(parm["Start"]);
            //if (!parm.ContainsKey("End"))
            //{
            //    All.Class.Error.Add(string.Format("{0}:读取数据不包含结束点", this.Text), Environment.StackTrace);
            //    return false;
            //}
            //end = All.Class.Num.ToInt(parm["End"]);
            lock (lockObject)
            {
                byte[] sendBuff = new byte[0];
                byte[] strBuff = new byte[0];
                int readLen = 0;
                switch (Class.TypeUse.GetType<T>())
                {
                    case Class.TypeUse.TypeList.Byte:
                        sendBuff = new byte[13 + value.Count];
                        sendBuff[7] = 0x10;
                        sendBuff[11] = (byte)(((sendBuff.Length - 13) / 2) & 0xFF);
                        sendBuff[12] = (byte)(((sendBuff.Length - 13) / 1) & 0xFF);
                        break;
                    case Class.TypeUse.TypeList.UShort:
                    case Class.TypeUse.TypeList.Int:
                        sendBuff = new byte[13 + value.Count * 2];
                        sendBuff[7] = 0x10;
                        sendBuff[11] = (byte)(((sendBuff.Length - 13) / 2) & 0xFF);
                        sendBuff[12] = (byte)(((sendBuff.Length - 13) / 1) & 0xFF);
                        break;
                    case Class.TypeUse.TypeList.Float:
                        sendBuff = new byte[13 + value.Count * 4];
                        sendBuff[7] = 0x10;
                        sendBuff[11] = (byte)(((sendBuff.Length - 13) / 2) & 0xFF);
                        sendBuff[12] = (byte)(((sendBuff.Length - 13) / 1) & 0xFF);
                        break;
                    case Class.TypeUse.TypeList.String:
                        strBuff = Encoding.GetEncoding("GB2312").GetBytes((string)(object)value[0]);
                        if ((strBuff.Length % 2) == 1)
                        {
                            List<byte> tmpList = strBuff.ToList();
                            tmpList.Add(0);
                            strBuff = tmpList.ToArray();
                        }
                        sendBuff = new byte[13 + strBuff.Length];
                        sendBuff[7] = 0x10;
                        sendBuff[11] = (byte)(((sendBuff.Length - 13) / 2) & 0xFF);
                        sendBuff[12] = (byte)(((sendBuff.Length - 13) / 1) & 0xFF);
                        break;
                    case Class.TypeUse.TypeList.Boolean:
                        sendBuff = new byte[13 + (int)(Math.Ceiling(value.Count / 8.0f))];
                        sendBuff[7] = 0x0F;
                        sendBuff[11] = (byte)(value.Count);
                        sendBuff[12] = (byte)(sendBuff.Length - 13);
                        break;
                    default:
                        All.Class.Error.Add("McgsModbusRtu不支持当前的数据类型写入");
                        return false;
                }
                sendBuff[0] = 0x00;
                sendBuff[1] = 0x00;
                sendBuff[2] = 0x00;
                sendBuff[3] = 0x00;
                sendBuff[4] = 0x00;
                sendBuff[5] = (byte)(sendBuff.Length - 6);
                sendBuff[6] = address;
                sendBuff[8] = (byte)(((start) >> 8) & 0xFF);
                sendBuff[9] = (byte)(((start) >> 0) & 0xFF);
                sendBuff[10] = 0x00;
                switch (Class.TypeUse.GetType<T>())
                {
                    case Class.TypeUse.TypeList.Byte:
                        for (int i = 0, j = 13; i < value.Count && j < sendBuff.Length; i++, j++)
                        {
                            sendBuff[j] = (byte)(object)value[i];
                        }
                        readLen = 12;
                        break;
                    case Class.TypeUse.TypeList.Int:
                        for (int i = 0, j = 13; i < value.Count && j < sendBuff.Length; i++, j = j + 2)
                        {
                            int tmpInt = (int)(object)value[i];
                            if (tmpInt < 0)
                            {
                                tmpInt = ((-tmpInt - 1) ^ 0xFFFF);
                            }
                            sendBuff[j] = (byte)((tmpInt >> 8) & 0xFF);
                            sendBuff[j + 1] = (byte)((tmpInt >> 0) & 0xFF);
                        }
                        readLen = 12;
                        break;
                    case Class.TypeUse.TypeList.UShort:
                        for (int i = 0, j = 13; i < value.Count && j < sendBuff.Length; i++, j = j + 2)
                        {
                            ushort tmpUshort=(ushort)(object)(value[i]);
                            sendBuff[j] = (byte)((tmpUshort >> 8) & 0xFF);
                            sendBuff[j + 1] = (byte)((tmpUshort >> 0) & 0xFF);
                        }
                        readLen = 12;
                        break;
                    case Class.TypeUse.TypeList.String:
                        Array.Copy(strBuff, 0, sendBuff, 13, strBuff.Length);
                        readLen = 12;
                        break;
                    case Class.TypeUse.TypeList.Float:
                        for (int i = 0, j = 13; i < value.Count && j < sendBuff.Length; i++, j = j + 4)
                        {
                            Array.Copy(All.Class.Num.FlotToByte((float)(object)value[i], Class.Num.QueueList.四三二一), 0, sendBuff, j, 4);
                        }
                        readLen = 12;
                        break;
                    case Class.TypeUse.TypeList.Boolean:
                        int tmpLen = (int)(Math.Ceiling(value.Count / 8.0f));
                        List<bool> tmpBool = new List<bool>();
                        for (int i = 0; i < value.Count; i++)
                        {
                            tmpBool.Add((bool)(object)value[i]);
                        }
                        for (int i = value.Count; i < tmpLen * 8; i++)
                        {
                            tmpBool.Add(false);
                        }//将数据补齐为8的整数个，方便转为字节
                        Array.Copy(All.Class.Num.Bool2Byte(tmpBool.ToArray()), 0, sendBuff, 13, sendBuff.Length - 13);
                        readLen = 12;
                        break;
                }
                byte[] readBuff;
                if (Write<byte[], byte[]>(sendBuff, readLen, out readBuff))
                {
                    if (sendBuff[0] == readBuff[0] && sendBuff[1] == readBuff[1] && sendBuff[2] == readBuff[2] && sendBuff[3] == readBuff[3] &&
                        sendBuff[4] == readBuff[4] && (readBuff[5] + 6) == readBuff.Length && readBuff[6] == sendBuff[6] && readBuff[7] == sendBuff[7])
                    {
                        //写入OK
                    }
                    else
                    {
                        All.Class.Error.Add(string.Format("{0}:读取数据校验错误", this.Text), Environment.StackTrace);
                        return false;
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
            List<T> buff = new List<T>();
            buff.Add(value);
            return WriteInternal<T>(buff, start, start);
        }
    }
}
