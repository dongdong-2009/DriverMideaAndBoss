using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace All.Meter
{
    public class SimensSmart200Net : Meter
    {
        Dictionary<string, string> initParm;
        /// <summary>
        /// PLC机架号
        /// </summary>
        public int JiJiaHao
        { get; set; }
        /// <summary>
        /// PLC槽号
        /// </summary>
        public int CaoHao
        { get; set; }

        /// <summary>
        /// 事务处理标志
        /// </summary>
        int ShiWuChuLi = 2;

        object lockObject = new object();
        public override Dictionary<string, string> InitParm
        {
            get { return initParm; }
            set { initParm = value; }
        }
        public override void Init(Dictionary<string, string> initParm)
        {
            this.InitParm = initParm;
            JiJiaHao = 0;
            CaoHao = 2;
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
            if (initParm.ContainsKey("JiJiaHao"))
            {
                this.JiJiaHao = All.Class.Num.ToInt(initParm["JiJiaHao"]);
            }
            if (initParm.ContainsKey("CaoHao"))
            {
                this.CaoHao = All.Class.Num.ToInt(initParm["CaoHao"]);
            }
            Shake();
        }
        /// <summary>
        /// 西门子特有的握手
        /// </summary>
        private void Shake()
        {
            //暂时没搞清机架号和槽号有什么用，查不出个问题
            lock (lockObject)
            {
                byte[] sendBuff = new byte[22];
                byte[] readBuff = new byte[22];
                sendBuff[0] = 0x03;//版本号
                sendBuff[1] = 0x00;//保留
                sendBuff[2] = 0x00;//总长度高字节
                sendBuff[3] = 0x16;//总长度低字节
                sendBuff[4] = 0x11;//后面字节长度
                sendBuff[5] = 0xE0;//连接请求
                sendBuff[6] = 0x00;//00
                sendBuff[7] = 0x00;//00
                sendBuff[8] = 0x00;//连接次数高字节
                sendBuff[9] = 0x01;//连接次数低字节
                sendBuff[10] = 0x00;//00
                sendBuff[11] = 0xC1;//参数1，PLC端 机架号，槽号等
                sendBuff[12] = 0x02;//数据长度
                sendBuff[13] = 0x10;
                sendBuff[14] = 0x00;
                sendBuff[15] = 0xC2;//参数2，远程端数据
                sendBuff[16] = 0x02;//数据长度
                sendBuff[17] = 0x03;
                sendBuff[18] = 0x01;
                sendBuff[19] = 0xC0;//参数0，固定
                sendBuff[20] = 0x01;//数据长度
                sendBuff[21] = 0x0A;
                if (Write<byte[], byte[]>(sendBuff, 22, out readBuff))
                {
                    if (sendBuff[19] != readBuff[11] || sendBuff[20] != readBuff[12] || sendBuff[21] != readBuff[13] ||
                       sendBuff[11] != readBuff[14] || sendBuff[12] != readBuff[15] || sendBuff[13] != readBuff[16] || sendBuff[14] != readBuff[17] ||
                       sendBuff[15] != readBuff[18] || sendBuff[16] != readBuff[19] || sendBuff[17] != readBuff[20] || sendBuff[18] != readBuff[21])
                    {
                        All.Class.Error.Add(string.Format("{0}:握手过程校验失败", this.Text), Environment.StackTrace);
                        return;
                    }
                }
                else
                {
                    return;
                }
                sendBuff = new byte[25];
                sendBuff[0] = 0x03;
                sendBuff[1] = 0x00;
                sendBuff[2] = 0x00;
                sendBuff[3] = 0x19;
                sendBuff[4] = 0x02;
                sendBuff[5] = 0xF0;
                sendBuff[6] = 0x80;
                sendBuff[7] = 0x32;
                sendBuff[8] = 0x01;
                sendBuff[9] = 0x00;
                sendBuff[10] = 0x00;
                sendBuff[11] = 0xCC;
                sendBuff[12] = 0xC1;
                sendBuff[13] = 0x00;
                sendBuff[14] = 0x08;
                sendBuff[15] = 0x00;
                sendBuff[16] = 0x00;
                sendBuff[17] = 0xF0;
                sendBuff[18] = 0x00;
                sendBuff[19] = 0x00;
                sendBuff[20] = 0x01;
                sendBuff[21] = 0x00;
                sendBuff[22] = 0x01;
                sendBuff[23] = 0x03;
                sendBuff[24] = 0xC0;
                if (Write<byte[], byte[]>(sendBuff, 27, out readBuff))
                {
                    if (sendBuff[0] != readBuff[0] || sendBuff[1] != readBuff[1] || sendBuff[2] != readBuff[2] ||
                       sendBuff[4] != readBuff[4] || sendBuff[5] != readBuff[5] || sendBuff[6] != readBuff[6] ||
                       sendBuff[7] != readBuff[7] || readBuff[8] != 0x03 || sendBuff[9] != readBuff[9] || sendBuff[10] != readBuff[10])
                    {
                        All.Class.Error.Add(string.Format("{0}:握手过程校验失败", this.Text), Environment.StackTrace);
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }
        public override bool Read<T>(out List<T> value, Dictionary<string, string> parm)
        {
            lock (lockObject)
            {
                value = new List<T>();
                bool result = true;
                byte[] sendBuff = new byte[0];
                byte[] readBuff;
                int tmpStart = 0;
                int tmpEnd = 0;
                int tmpLen = 0;
                int readLen = 0;
                float start = 0;
                float end = 0;
                float Block = 1;
                if (!Conn)
                {
                    Shake();
                }
                try
                {
                    if (!parm.ContainsKey("Start"))
                    {
                        All.Class.Error.Add(string.Format("{0}:读取数据不包含起始点", this.Text), Environment.StackTrace);
                        return false;
                    }
                    start = All.Class.Num.ToFloat(parm["Start"]);
                    if (!parm.ContainsKey("End"))
                    {
                        All.Class.Error.Add(string.Format("{0}:读取数据不包含结束点", this.Text), Environment.StackTrace);
                        return false;
                    }
                    end = All.Class.Num.ToFloat(parm["End"]);
                    if (parm.ContainsKey("Block"))
                    {
                        Block = All.Class.Num.ToFloat(parm["Block"]);
                    }
                    ShiWuChuLi++;
                    sendBuff = new byte[31];
                    sendBuff[0] = 0x03;
                    sendBuff[1] = 0x00;
                    sendBuff[2] = 0x00;
                    sendBuff[3] = 0x1F;
                    sendBuff[4] = 0x02;
                    sendBuff[5] = 0xF0;
                    sendBuff[6] = 0x80;
                    sendBuff[7] = 0x32;
                    sendBuff[8] = 0x01;
                    sendBuff[9] = 0x00;
                    sendBuff[10] = 0x00;
                    sendBuff[11] = (byte)((ShiWuChuLi >> 8) & 0xFF);
                    sendBuff[12] = (byte)((ShiWuChuLi >> 0) & 0xFF);
                    sendBuff[13] = 0x00;
                    sendBuff[14] = 0x0E;
                    sendBuff[15] = 0x00;
                    sendBuff[16] = 0x00;
                    sendBuff[17] = 0x04;
                    sendBuff[18] = 0x01;
                    sendBuff[19] = 0x12;
                    sendBuff[20] = 0x0A;
                    sendBuff[21] = 0x10;
                    sendBuff[22] = 0x02;
                    switch (Class.TypeUse.GetType<T>())
                    {
                        case All.Class.TypeUse.TypeList.Boolean://位
                            tmpStart = (int)Math.Floor(start);
                            tmpEnd = (int)Math.Floor(end);
                            tmpLen = tmpEnd - tmpStart + 1;
                            sendBuff[23] = (byte)((tmpLen >> 8) & 0xFF);
                            sendBuff[24] = (byte)((tmpLen >> 0) & 0xFF);
                            readLen = 25 + tmpLen;
                            break;
                        case All.Class.TypeUse.TypeList.Byte://单字
                        case All.Class.TypeUse.TypeList.String:
                            sendBuff[23] = (byte)(((((int)end - (int)start + 1)) >> 8) & 0xFF);
                            sendBuff[24] = (byte)(((((int)end - (int)start + 1)) >> 0) & 0xFF);
                            readLen = 25 + (int)(end - start + 1);
                            break;
                        case All.Class.TypeUse.TypeList.UShort://双字
                        case All.Class.TypeUse.TypeList.Int:
                            if (((int)(end - start) & 1) == 1)
                            {
                                end = end + 1;//读双字，end必须和start同为奇数或同为偶数
                            }
                            sendBuff[23] = (byte)(((((int)end - (int)start + 2)) >> 8) & 0xFF);
                            sendBuff[24] = (byte)(((((int)end - (int)start + 2)) >> 0) & 0xFF);
                            readLen = 25 + (int)(end - start + 2);
                            break;
                        default:
                            All.Class.Error.Add("Siemens1200Net读取不可知数据类型", Environment.StackTrace);
                            return false;
                    }
                    sendBuff[25] = (byte)((((int)Block) >> 8) & 0xFF);
                    sendBuff[26] = (byte)((((int)Block) >> 0) & 0xFF);
                    sendBuff[27] = 0x84;
                    sendBuff[28] = (byte)((((int)Math.Floor(start) * 8) >> 0x10) & 0xFF);
                    sendBuff[29] = (byte)((((int)Math.Floor(start) * 8) >> 0x08) & 0xFF);
                    sendBuff[30] = (byte)((((int)Math.Floor(start) * 8) >> 0x00) & 0xFF);
                    if (Write<byte[], byte[]>(sendBuff, readLen, out readBuff))
                    {
                        if (readBuff[0] == 0x03 && readLen >= 22 && readBuff[12] == sendBuff[12] && readBuff[21] == 0xFF && ((readBuff[23] * 0x100 + readBuff[24]) / 8 + 25) == readLen)
                        {
                            switch (Class.TypeUse.GetType<T>())
                            {
                                case All.Class.TypeUse.TypeList.Byte:
                                    for (int i = 25,j=0; j < (readBuff[23] * 0x100 + readBuff[24]) / 8 && i < readBuff.Length; i++,j++)
                                    {
                                        value.Add((T)(object)readBuff[i]);
                                    }
                                    break;
                                case All.Class.TypeUse.TypeList.UShort:
                                    for (int i = 25, j = 0; j < (readBuff[23] * 0x100 + readBuff[24]) / 8 && i < readBuff.Length; i = i + 2, j = j + 2)
                                    {
                                        value.Add((T)(object)(ushort)(readBuff[i] * 0x100 + readBuff[i + 1]));
                                    }
                                    break;
                                case All.Class.TypeUse.TypeList.Int:
                                    for (int i = 25, j = 0; j < (readBuff[23] * 0x100 + readBuff[24]) / 8 && i < readBuff.Length; i = i + 2, j = j + 2)
                                    {
                                        int tmpValue = readBuff[i] * 0x100 + readBuff[i + 1];
                                        if (tmpValue >= 32768)
                                        {
                                            tmpValue = -(tmpValue ^ 0xFFFF) - 1;
                                        }
                                        value.Add((T)(object)(int)tmpValue);
                                    }
                                    break;
                                case All.Class.TypeUse.TypeList.String:
                                    value.Add((T)(object)Encoding.ASCII.GetString(readBuff, 25, (readBuff[23] * 0x100 + readBuff[24]) / 8));
                                    break;
                                case All.Class.TypeUse.TypeList.Boolean:
                                    bool[] tmpBool = All.Class.Num.Byte2Bool(readBuff, 25, tmpLen);
                                    int startBit = Convert.ToInt16((start * 10 - tmpStart * 10));
                                    int endBit = Convert.ToInt16((end * 10 - tmpEnd * 10));
                                    for (int i = startBit; i < tmpBool.Length && i < (tmpLen - 1) * 8 + endBit + 1; i++)
                                    {
                                        value.Add((T)(object)tmpBool[i]);
                                    }
                                    break;
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
                catch (Exception e)
                {
                    All.Class.Error.Add(e);
                    result = false;
                }
                return result;
            }
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
            lock (lockObject)
            {
                bool result = true;
                int tmpStart = 0;
                int tmpEnd = 0;
                int tmpLen = 0;
                float start = 0;
                float Block = 1;
                byte[] buff = new byte[1];
                byte[] sendBuff = new byte[1];
                byte[] readBuff;
                if (!Conn)
                {
                    Shake();
                }
                try
                {
                    if (!parm.ContainsKey("Start"))
                    {
                        All.Class.Error.Add(string.Format("{0}:写入数据不包含起始点", this.Text), Environment.StackTrace);
                        return false;
                    }
                    start = All.Class.Num.ToFloat(parm["Start"]);
                    if (parm.ContainsKey("Block"))
                    {
                        Block = All.Class.Num.ToFloat(parm["Block"]);
                    }
                    if (value.Count <= 0)
                    {
                        All.Class.Error.Add(string.Format("{0}:写入值为空，不能写入", this.Text), Environment.StackTrace);
                        return false;
                    }
                    ShiWuChuLi++;
                    switch (Class.TypeUse.GetType<T>())
                    {
                        case All.Class.TypeUse.TypeList.String:
                            buff = Encoding.ASCII.GetBytes((string)(object)value[0]);
                            sendBuff = new byte[0x23 + buff.Length];
                            sendBuff[22] = 0x02;
                            sendBuff[27] = 0x84;
                            sendBuff[32] = 0x04;
                            sendBuff[33] = (byte)((((int)buff.Length * 8) >> 0x08) & 0xFF);
                            sendBuff[34] = (byte)((((int)buff.Length * 8) >> 0x00) & 0xFF);
                            break;
                        case All.Class.TypeUse.TypeList.UShort:
                            buff = new byte[value.Count * 2];
                            for (int i = 0, j = 0; i < value.Count & j < buff.Length; i++, j = j + 2)
                            {
                                buff[j + 0] = (byte)(((ushort)(object)value[i] >> 8) & 0xFF);
                                buff[j + 1] = (byte)(((ushort)(object)value[i] >> 0) & 0xFF);
                            }
                            sendBuff = new byte[0x23 + buff.Length];
                            sendBuff[22] = 0x02;
                            sendBuff[27] = 0x84;
                            sendBuff[32] = 0x04;
                            sendBuff[33] = (byte)((((int)buff.Length * 8) >> 0x08) & 0xFF);
                            sendBuff[34] = (byte)((((int)buff.Length * 8) >> 0x00) & 0xFF);
                            break;
                        case All.Class.TypeUse.TypeList.Int:
                            buff = new byte[value.Count * 2];
                            int tmpValue = 0;
                            for (int i = 0, j = 0; i < value.Count & j < buff.Length; i++, j = j + 2)
                            {
                                tmpValue = (int)(object)value[i];
                                if (tmpValue < 0)
                                {
                                    tmpValue = -(tmpValue ^ 0xFFFF) - 1;
                                }
                                buff[j + 0] = (byte)((tmpValue >> 8) & 0xFF);
                                buff[j + 1] = (byte)((tmpValue >> 0) & 0xFF);
                            }
                            sendBuff = new byte[0x23 + buff.Length];
                            sendBuff[22] = 0x02;
                            sendBuff[27] = 0x84;
                            sendBuff[32] = 0x04;
                            sendBuff[33] = (byte)((((int)buff.Length * 8) >> 0x08) & 0xFF);
                            sendBuff[34] = (byte)((((int)buff.Length * 8) >> 0x00) & 0xFF);
                            break;
                        case All.Class.TypeUse.TypeList.Byte:
                            buff = new byte[value.Count];
                            for (int i = 0; i < buff.Length; i++)
                            {
                                buff[i] = (byte)(object)value[i];
                            }
                            sendBuff = new byte[0x23 + buff.Length];
                            sendBuff[22] = 0x02;
                            sendBuff[27] = 0x84;
                            sendBuff[32] = 0x04;
                            sendBuff[33] = (byte)((((int)buff.Length * 8) >> 0x08) & 0xFF);
                            sendBuff[34] = (byte)((((int)buff.Length * 8) >> 0x00) & 0xFF);
                            break;
                        case All.Class.TypeUse.TypeList.Boolean:
                            tmpStart = (int)Math.Floor(start);
                            tmpLen = Convert.ToInt16((start * 10 - tmpStart * 10));
                            tmpEnd = tmpLen + value.Count;
                            List<bool> tmpBool = new List<bool>();
                            Dictionary<string, string> tmpBuff = new Dictionary<string, string>();
                            tmpBuff.Add("Start", tmpStart.ToString());
                            tmpBuff.Add("End", string.Format("{0:f1}", tmpStart + (int)Math.Ceiling(tmpEnd / 8.0f) - 0.3f));
                            if (!Read<bool>(out tmpBool, tmpBuff))
                            {
                                return false;
                            }
                            bool[] tmpWriteBool = tmpBool.ToArray();
                            for (int i = tmpLen, j = 0; i < tmpLen + value.Count && j < value.Count; i++, j++)
                            {
                                tmpWriteBool[i] = (bool)(object)value[j];
                            }
                            buff = All.Class.Num.Bool2Byte(tmpWriteBool);
                            sendBuff = new byte[0x23 + buff.Length];
                            sendBuff[22] = 0x02;
                            sendBuff[27] = 0x84;
                            sendBuff[32] = 0x04;
                            sendBuff[33] = (byte)((((int)buff.Length * 8) >> 0x08) & 0xFF);
                            sendBuff[34] = (byte)((((int)buff.Length * 8) >> 0x00) & 0xFF);
                            break;
                        default:
                            All.Class.Error.Add("Siemens1200Net写入不可知数据类型", Environment.StackTrace);
                            return false;
                    }
                    sendBuff[0] = 0x03;
                    sendBuff[1] = 0x00;
                    sendBuff[2] = 0x00;
                    sendBuff[3] = (byte)sendBuff.Length;
                    sendBuff[4] = 0x02;
                    sendBuff[5] = 0xF0;
                    sendBuff[6] = 0x80;
                    sendBuff[7] = 0x32;
                    sendBuff[8] = 0x01;
                    sendBuff[9] = 0x00;
                    sendBuff[10] = 0x00;
                    sendBuff[11] = (byte)((ShiWuChuLi >> 8) & 0xFF);
                    sendBuff[12] = (byte)((ShiWuChuLi >> 0) & 0xFF);
                    sendBuff[13] = 0x00;
                    sendBuff[14] = 0x0E;
                    sendBuff[15] = (byte)(((buff.Length + 4) >> 8) & 0xFF);
                    sendBuff[16] = (byte)(((buff.Length + 4) >> 0) & 0xFF);
                    sendBuff[17] = 0x05;
                    sendBuff[18] = 0x01;
                    sendBuff[19] = 0x12;
                    sendBuff[20] = 0x0A;
                    sendBuff[21] = 0x10;

                    sendBuff[23] = (byte)((buff.Length >> 8) & 0xFF);
                    sendBuff[24] = (byte)((buff.Length >> 0) & 0xFF);
                    sendBuff[25] = (byte)((((int)Block) >> 8) & 0xFF);
                    sendBuff[26] = (byte)((((int)Block) >> 0) & 0xFF);

                    sendBuff[28] = (byte)((((int)Math.Floor((float)tmpStart) * 8) >> 0x10) & 0xFF);
                    sendBuff[29] = (byte)((((int)Math.Floor((float)tmpStart) * 8) >> 0x08) & 0xFF);
                    sendBuff[30] = (byte)((((int)Math.Floor((float)tmpStart) * 8) >> 0x00) & 0xFF);
                    sendBuff[31] = 0x00;


                    for (int i = 0, j = 35; i < buff.Length && j < sendBuff.Length; i++, j++)
                    {
                        sendBuff[j] = buff[i];
                    }
                    if (Write<byte[], byte[]>(sendBuff, 22, out readBuff))
                    {
                        //写DB块返回值0300001602F0803203000000040002000100000501FF
                        //写M点返回值 0300001602F0803203000000040002000100000501FF

                        if (readBuff[0] != 0x03 || readBuff[21] != 0xFF || readBuff[12] != sendBuff[12])
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
                catch (Exception e)
                {
                    result = false;
                    All.Class.Error.Add(e);
                }
            return result;
            }
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
