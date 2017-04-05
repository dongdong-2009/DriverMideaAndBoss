using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace All.Meter
{
    /// <summary>
    /// 用于上下位机通讯
    /// </summary>
    public class SSCommunicate:Meter
    {
        byte[] readByte = new byte[0];
        string[] readString = new string[0];
        double[] readDouble = new double[0];
        ushort[] readUshort = new ushort[0];
        int[] readInt = new int[0];
        float[] readFloat = new float[0];
        bool[] readBool = new bool[0];


        Dictionary<string, string> initParm = new Dictionary<string, string>();
        object lockObject = new object();
        public override Dictionary<string, string> InitParm
        {
            get { return initParm; }
            set { initParm = value; }
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
            if (initParm.ContainsKey("byte"))
            {
                readByte = new byte[All.Class.Num.ToInt(initParm["byte"])];
                readByte.Initialize();
            }
            if (initParm.ContainsKey("string"))
            {
                readString = new string[All.Class.Num.ToInt(initParm["string"])];
                for (int i = 0; i < readString.Length; i++)
                {
                    readString[i] = "";
                }
            }
            if (initParm.ContainsKey("double"))
            {
                readDouble = new double[All.Class.Num.ToInt(initParm["double"])];
                readDouble.Initialize();
            }
            if (initParm.ContainsKey("int"))
            {
                readInt = new int[All.Class.Num.ToInt(initParm["int"])];
                readInt.Initialize();
            }
            if (initParm.ContainsKey("ushort"))
            {
                readUshort = new ushort[All.Class.Num.ToInt(initParm["ushort"])];
                readUshort.Initialize();
            }
            if (initParm.ContainsKey("float"))
            {
                readFloat = new float[All.Class.Num.ToInt(initParm["float"])];
                readFloat.Initialize();
            }
            if (initParm.ContainsKey("bool"))
            {
                readBool = new bool[All.Class.Num.ToInt(initParm["bool"])];
                readBool.Initialize();
            }
            if (this.Parent.Sons.Count > 1)
            {
                All.Class.Error.Add("信息交互通讯类,此设备必须独占一个通讯方式,父通讯类下只能存在一个子设备", Environment.StackTrace);
                return;
            }
            if (readBool.Length > 0 || readByte.Length > 0 || readDouble.Length > 0 ||
                readFloat.Length > 0 || readInt.Length > 0 || readString.Length > 0 || 
                readUshort.Length > 0)
            {
                new Thread(() => Flush())
                {
                    IsBackground = true
                }.Start();
            }
        }
        /// <summary>
        /// 接收远程数据并返回结果
        /// </summary>
        private void Flush()
        {
            byte[] readBuff;
            while (true)
            {
                lock (lockObject)
                {
                    if (this.Read<byte[]>(0, out readBuff))
                    {
                        if (readBuff != null && readBuff.Length >= 12)
                        {
                            this.Write<byte[]>(GetReadValue(readBuff));
                        }
                    }
                }
                Thread.Sleep(30);
            }
        }
        public override bool Read<T>(out List<T> value, Dictionary<string, string> parm)
        {
            int start = 0, end = 0;
            value = new List<T>();
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
            if (start > end)
            {
                start = start + end;
                end = start - end;
                start = start - end;
            }
            switch (All.Class.TypeUse.GetType<T>())
            {
                case Class.TypeUse.TypeList.Byte:
                    if (end >= readByte.Length)
                    {
                        All.Class.Error.Add(new string[] { "读取结束", "本地长度" }, new string[] { end.ToString(), readByte.Length.ToString() });
                        All.Class.Error.Add("远程发送的数据区间大于初始化区间", Environment.StackTrace);
                        return false;
                    }
                    for (int i = start; i <= end; i++)
                    {
                        value.Add((T)(object)readByte[i]);
                    }
                    break;
                case Class.TypeUse.TypeList.String:
                    if (end >= readString.Length)
                    {
                        All.Class.Error.Add(new string[] { "读取结束", "本地长度" }, new string[] { end.ToString(), readString.Length.ToString() });
                        All.Class.Error.Add("远程发送的数据区间大于初始化区间", Environment.StackTrace);
                        return false;
                    }
                    for (int i = start; i <= end; i++)
                    {
                        value.Add((T)(object)readString[i]);
                    }
                    break;
                case Class.TypeUse.TypeList.Double:
                    if (end >= readDouble.Length)
                    {
                        All.Class.Error.Add(new string[] { "读取结束", "本地长度" }, new string[] { end.ToString(), readDouble.Length.ToString() });
                        All.Class.Error.Add("远程发送的数据区间大于初始化区间", Environment.StackTrace);
                        return false;
                    }
                    for (int i = start; i <= end; i++)
                    {
                        value.Add((T)(object)readDouble[i]);
                    }
                    break;
                case Class.TypeUse.TypeList.UShort:
                    if (end >= readUshort.Length)
                    {
                        All.Class.Error.Add(new string[] { "读取结束", "本地长度" }, new string[] { end.ToString(), readUshort.Length.ToString() });
                        All.Class.Error.Add("远程发送的数据区间大于初始化区间", Environment.StackTrace);
                        return false;
                    }
                    for (int i = start; i <= end; i++)
                    {
                        value.Add((T)(object)readUshort[i]);
                    }
                    break;
                case Class.TypeUse.TypeList.Int:
                    if (end >= readInt.Length)
                    {
                        All.Class.Error.Add(new string[] { "读取结束", "本地长度" }, new string[] { end.ToString(), readInt.Length.ToString() });
                        All.Class.Error.Add("远程发送的数据区间大于初始化区间", Environment.StackTrace);
                        return false;
                    }
                    for (int i = start; i <= end; i++)
                    {
                        value.Add((T)(object)readInt[i]);
                    }
                    break;
                case Class.TypeUse.TypeList.Float:
                    if (end >= readFloat.Length)
                    {
                        All.Class.Error.Add(new string[] { "读取结束", "本地长度" }, new string[] { end.ToString(), readFloat.Length.ToString() });
                        All.Class.Error.Add("远程发送的数据区间大于初始化区间", Environment.StackTrace);
                        return false;
                    }
                    for (int i = start; i <= end; i++)
                    {
                        value.Add((T)(object)readFloat[i]);
                    }
                    break;
                case Class.TypeUse.TypeList.Boolean:
                    if (end >= readBool.Length)
                    {
                        All.Class.Error.Add(new string[] { "读取结束", "本地长度" }, new string[] { end.ToString(), readBool.Length.ToString() });
                        All.Class.Error.Add("远程发送的数据区间大于初始化区间", Environment.StackTrace);
                        return false;
                    }
                    for (int i = start; i <= end; i++)
                    {
                        value.Add((T)(object)readBool[i]);
                    }
                    break;
            }
            return true;
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
            List<T> tmp;
            value = default(T);
            bool result = Read<T>(out tmp, start, start);
            if (tmp != null && tmp.Count > 0)
            {
                value = tmp[0];
            }
            return result;
        }
        public override bool WriteInternal<T>(List<T> value, Dictionary<string, string> parm)
        {
            bool result = false;

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
            int LiuShuiHaoStart=(int)All.Class.Num.GetRandom(0,0xFFFF);

            
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
            lock (lockObject)
            {
                if (Write<byte[], byte[]>(sendBuff, 16, out readBuff))
                {
                    if (readBuff[0] != sendBuff[0] || readBuff[1] != sendBuff[1] || readBuff[2] != sendBuff[2]
                     || readBuff[3] != sendBuff[3] || readBuff[4] != sendBuff[4] || readBuff[5] != sendBuff[5]
                     || readBuff[6] != sendBuff[6] || readBuff[7] != sendBuff[7] || readBuff[8] != sendBuff[8]
                     || readBuff[14] != endBuff[0] || readBuff[15] != endBuff[1])
                    {
                        All.Class.Error.Add(new string[] { "写入数据", "读取数据" }, new string[] { All.Class.Num.Hex2Str(sendBuff), All.Class.Num.Hex2Str(readBuff) });
                        All.Class.Error.Add(string.Format("{0}:读取数据校验错误", this.Text), Environment.StackTrace);
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
        /// <summary>
        /// 将远程发送过来的数据解析并返回解析结果
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private byte[] GetReadValue(byte[] value)
        {
            byte[] result = new byte[16];
            int index = 0;
            int len = 0;
            do
            {
                len = 0;
                //头尾校验
                if (value[index] != Convert.ToByte('S') || value[value.Length - 1] != Convert.ToByte('X')
                    || value[index + 1] != Convert.ToByte('S') || value[value.Length - 2] != Convert.ToByte('X'))
                {
                    result[0] = Convert.ToByte('S');
                    result[1] = Convert.ToByte('S');
                    for (int i = 2; i < 9; i++)
                    {
                        result[i] = value[index + i];
                    }
                    result[9] = Convert.ToByte('E');
                    result[10] = Convert.ToByte('r');
                    result[11] = Convert.ToByte('r');
                    result[12] = Convert.ToByte('o');
                    result[13] = Convert.ToByte('r');
                    result[14] = Convert.ToByte('X');
                    result[15] = Convert.ToByte('X');
                    All.Class.Error.Add(new string[] { "接收数据,错误模块" }, new string[] { All.Class.Num.Hex2Str(value), this.Text });
                    All.Class.Error.Add("接收数据校验错误", Environment.StackTrace);
                    return result;
                }
                //类型校验
                if (value[index + 4] >= Enum.GetNames(typeof(All.Class.TypeUse.TypeList)).Length)
                {
                    result[0] = Convert.ToByte('S');
                    result[1] = Convert.ToByte('S');
                    for (int i = 2; i < 9; i++)
                    {
                        result[i] = value[index + i];
                    }
                    result[9] = Convert.ToByte('E');
                    result[10] = Convert.ToByte('r');
                    result[11] = Convert.ToByte('r');
                    result[12] = Convert.ToByte('o');
                    result[13] = Convert.ToByte('r');
                    result[14] = Convert.ToByte('X');
                    result[15] = Convert.ToByte('X');
                    All.Class.Error.Add(new string[] { "接收数据,错误模块" }, new string[] { All.Class.Num.Hex2Str(value), this.Text });
                    All.Class.Error.Add("接收数据类型错误错误", Environment.StackTrace);
                    return result;
                }
                int start = value[index + 5] * 0x100 + value[index + 6];
                int end = value[index + 7] * 0x100 + value[index + 8];
                if (start > end)
                {
                    start = start + end;
                    end = start - end;
                    start = start - end;
                }
                switch (All.Class.TypeUse.GetType(value[index + 4]))
                {
                    case Class.TypeUse.TypeList.Byte:
                        if (end >= readByte.Length)
                        {
                            All.Class.Error.Add(new string[] { "读取数据", "本地长度" }, new string[] { Class.Num.Hex2Str(value), readByte.Length.ToString() });
                            All.Class.Error.Add("远程发送的数据区间大于初始化区间,本地读取设置过小,或远程发送区间过大", Environment.StackTrace);
                            return result;
                        }
                        for (int i = start, j = 9; i <= end && i < readByte.Length && j < value.Length - 2; i++, j++)
                        {
                            readByte[i] = value[index + j];
                            len++;
                        }
                        break;
                    case Class.TypeUse.TypeList.String:
                        if (end >= readString.Length)
                        {
                            All.Class.Error.Add(new string[] { "读取数据", "本地长度" }, new string[] { Class.Num.Hex2Str(value), readString.Length.ToString() });
                            All.Class.Error.Add("远程发送的数据区间大于初始化区间,本地读取设置过小,或远程发送区间过大", Environment.StackTrace);
                            return result;
                        }
                        for (int i = start, j = 9; i <= end && i < readString.Length && j < value.Length - 2; i++)
                        {
                            if (value.Length <= (j + 2 + value[index + j] * 0x100 + value[index + j + 1]))
                            {
                                break;
                            }
                            readString[i] = Encoding.UTF8.GetString(value, index + j + 2, value[index + j] * 0x100 + value[index + j + 1]);
                            j = j + 2 + value[index + j] * 0x100 + value[index + j + 1];
                            len += 2 + value[index + j] * 0x100 + value[index + j + 1];
                        }
                        break;
                    case Class.TypeUse.TypeList.Double:
                        if (end >= readDouble.Length)
                        {
                            All.Class.Error.Add(new string[] { "读取数据", "本地长度" }, new string[] { Class.Num.Hex2Str(value), readDouble.Length.ToString() });
                            All.Class.Error.Add("远程发送的数据区间大于初始化区间,本地读取设置过小,或远程发送区间过大", Environment.StackTrace);
                            return result;
                        }
                        for (int i = start, j = 9; i <= end && i < readDouble.Length && j <= value.Length - 2 - 8; i++, j = j + 8)
                        {
                            readDouble[i] = BitConverter.ToDouble(value, index + j);
                            len += 8;
                        }
                        break;
                    case Class.TypeUse.TypeList.UShort:
                        if (end >= readUshort.Length)
                        {
                            All.Class.Error.Add(new string[] { "读取数据", "本地长度" }, new string[] { Class.Num.Hex2Str(value), readUshort.Length.ToString() });
                            All.Class.Error.Add("远程发送的数据区间大于初始化区间,本地读取设置过小,或远程发送区间过大", Environment.StackTrace);
                            return result;
                        }
                        for (int i = start, j = 9; i <= end && i < readUshort.Length && j <= value.Length - 2 - 2; i++, j = j + 2)
                        {
                            readUshort[i] = BitConverter.ToUInt16(value, index + j);
                            len += 2;
                        }
                        break;
                    case Class.TypeUse.TypeList.Int:
                        if (end >= readInt.Length)
                        {
                            All.Class.Error.Add(new string[] { "读取数据", "本地长度" }, new string[] { Class.Num.Hex2Str(value), readInt.Length.ToString() });
                            All.Class.Error.Add("远程发送的数据区间大于初始化区间,本地读取设置过小,或远程发送区间过大", Environment.StackTrace);
                            return result;
                        }
                        for (int i = start, j = 9; i <= end && i < readInt.Length && j <= value.Length - 2 - 4; i++, j = j + 4)
                        {
                            readInt[i] = BitConverter.ToInt32(value, index + j);
                            len += 4;
                        }
                        break;
                    case Class.TypeUse.TypeList.Float:
                        if (end >= readFloat.Length)
                        {
                            All.Class.Error.Add(new string[] { "读取数据", "本地长度" }, new string[] { Class.Num.Hex2Str(value), readFloat.Length.ToString() });
                            All.Class.Error.Add("远程发送的数据区间大于初始化区间,本地读取设置过小,或远程发送区间过大", Environment.StackTrace);
                            return result;
                        }
                        for (int i = start, j = 9; i <= end && i < readFloat.Length && j <= value.Length - 2 - 4; i++, j = j + 4)
                        {
                            readFloat[i] = BitConverter.ToSingle(value, index + j);
                            len += 4;
                        }
                        break;
                    case Class.TypeUse.TypeList.Boolean:
                        if (end >= readBool.Length)
                        {
                            All.Class.Error.Add(new string[] { "读取数据", "本地长度" }, new string[] { Class.Num.Hex2Str(value), readBool.Length.ToString() });
                            All.Class.Error.Add("远程发送的数据区间大于初始化区间,本地读取设置过小,或远程发送区间过大", Environment.StackTrace);
                            return result;
                        }
                        for (int i = start, j = 9; i <= end && i < readBool.Length && j <= value.Length - 2 - 1; i++, j++)
                        {
                            readBool[i] = (value[index + j] == 1);
                            len++;
                        }
                        break;

                }
                result[0] = Convert.ToByte('S');
                result[1] = Convert.ToByte('S');
                for (int i = 2; i < 9; i++)
                {
                    result[i] = value[i];
                }
                result[9] = Convert.ToByte('A');
                result[10] = Convert.ToByte('l');
                result[11] = Convert.ToByte('l');
                result[12] = Convert.ToByte('O');
                result[13] = Convert.ToByte('K');
                result[14] = Convert.ToByte('X');
                result[15] = Convert.ToByte('X');
                index = index + len + 11;
            } while (index < value.Length);
            return result;
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
                case  Class.TypeUse.TypeList.Float:
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
    }
}
