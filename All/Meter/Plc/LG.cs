using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace All.Meter
{
    public class LG:Meter
    {
        Dictionary<string, string> initParm;

        public override Dictionary<string, string> InitParm
        {
            get { return initParm; }
            set { initParm = value; }
        }
        object lockObject = new object();
        ModbusRtu modbus = new ModbusRtu();
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
            if (!initParm.ContainsKey("Address"))
            {
                All.Class.Error.Add("LGPLC初始化参数中没有地址", Environment.StackTrace);
            }
            modbus.Init(initParm);
            modbus.Parent = this.Parent;
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
            List<T> tmp = new List<T>();
            value = default(T);
            bool result = Read<T>(out tmp, start, start);
            if (result && tmp.Count > 0)
            {
                value = tmp[0];
            }
            return result;
        }
        /// <summary>
        /// 暂时只支持读取M点和D点数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        public override bool Read<T>(out List<T> value, Dictionary<string, string> parm)
        {
            lock (lockObject)
            {
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


                bool result = true;
                value = new List<T>();
                List<byte> readBuff = new List<byte>();
                int tmpStart = 0;
                int tmpEnd = 0;
                switch (Class.TypeUse.GetType<T>())
                {
                    case Class.TypeUse.TypeList.Byte:
                    case Class.TypeUse.TypeList.UShort:
                    case Class.TypeUse.TypeList.Int:
                    case Class.TypeUse.TypeList.String:
                        tmpStart = 0x8000 + start;
                        tmpEnd = 0x8000 + end;
                        break;
                    case Class.TypeUse.TypeList.Boolean:
                        tmpStart = 0x1000 + (start >> 4);
                        tmpEnd = 0x1000 + (end >> 4);
                        break;
                    case Class.TypeUse.TypeList.Float:
                    case Class.TypeUse.TypeList.Double:
                            All.Class.Error.Add(string.Format("{0}:当前PLC不支持此类型数据", this.Text),Environment.StackTrace);
                        return false;
                    default:
                            All.Class.Error.Add(string.Format("{0}:不可知数据类型", this.Text),Environment.StackTrace);
                        return false;
                }
                result = modbus.Read<byte>(out readBuff, tmpStart, tmpEnd);
                if (result)
                {
                    switch (Class.TypeUse.GetType<T>())
                    {
                        case Class.TypeUse.TypeList.Byte:
                            for (int i = 0; i < readBuff.Count; i++)
                            {
                                value.Add((T)(object)readBuff[i]);
                            }
                            break;
                        case All.Class.TypeUse.TypeList.UShort:
                            for (int i = 0; i < readBuff.Count; i = i + 2)
                            {
                                value.Add((T)(object)(ushort)(readBuff[i] * 0x100 + readBuff[i + 1]));
                            }
                            break;
                        case All.Class.TypeUse.TypeList.Int:
                            for (int i = 0; i < readBuff.Count; i = i + 2)
                            {
                                int tmpValue = readBuff[i] * 0x100 + readBuff[i + 1];
                                if (tmpValue >= 0x8000)
                                {
                                    tmpValue = -(tmpValue ^ 0xFFFF) - 1;
                                }
                                value.Add((T)(object)(int)(tmpValue));
                            }
                            break;
                        case All.Class.TypeUse.TypeList.String:
                            byte[] SwitchHighLow = All.Class.Num.SwitchHighAndLow(readBuff.ToArray());
                            value.Add((T)(object)(Encoding.ASCII.GetString(SwitchHighLow)));
                            break;
                        case All.Class.TypeUse.TypeList.Boolean:
                            bool[] tmpBool = All.Class.Num.Byte2Bool(All.Class.Num.SwitchHighAndLow(readBuff.ToArray()));
                            int curStart = (start % 16);
                            for (int i = curStart; i < end - start + 1 + curStart; i++)
                            {
                                value.Add((T)(object)tmpBool[i]);
                            }
                            break;
                    }
                }
                return result;
            }
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
            List<T> tmp = new List<T>();
            tmp.Add(value);
            return WriteInternal<T>(tmp, start, start);
        }
        public override bool WriteInternal<T>(List<T> value, Dictionary<string, string> parm)
        {
            lock (lockObject)
            {
                bool result = true;
                int start = 0;
                int end = 0;
                if (parm.ContainsKey("Start"))
                {
                    start = All.Class.Num.ToInt(parm["Start"]);
                }
                if (parm.ContainsKey("End"))
                {
                    end = All.Class.Num.ToInt(parm["End"]);
                }
                int tmpStart = 0;
                int tmpEnd = 0;
                List<ushort> sendBuff = new List<ushort>();
                switch (Class.TypeUse.GetType<T>())
                {
                    case Class.TypeUse.TypeList.Byte:
                        tmpStart = 0x8000 + start;
                        tmpEnd = 0x8000 + end;
                        if ((value.Count % 2) != 0)
                        {
                            All.Class.Error.Add(string.Format("{0}:LGPLC多点写入数据时,只能以寄存器为单位进行写入,所以数量必须成双", this.Text));
                            return false;
                        }
                        for (int i = 0; i < value.Count; i = i + 2)
                        {
                            sendBuff.Add((ushort)((((byte)(object)value[i]) << 8) + (byte)(object)value[i + 1]));
                        }
                        break;
                    case All.Class.TypeUse.TypeList.UShort:
                        tmpStart = 0x8000 + start;
                        tmpEnd = 0x8000 + end;
                        for (int i = 0; i < value.Count; i++)
                        {
                            sendBuff.Add((ushort)(object)value[i]);
                        }
                        break;
                    case All.Class.TypeUse.TypeList.Int:
                        tmpStart = 0x8000 + start;
                        tmpEnd = 0x8000 + end;
                        for (int i = 0; i < value.Count; i++)
                        {
                            int tmp = (int)(object)value[i];
                            if (tmp < 0)
                            {
                                tmp = ((-tmp - 1) ^ 0xFFFF);
                            }
                            sendBuff.Add((ushort)(tmp & 0xFFFF));
                        }
                        break;
                    case All.Class.TypeUse.TypeList.String:
                        tmpStart = 0x8000 + start;
                        tmpEnd = 0x8000 + end;
                        byte[] tmpBuff = Encoding.ASCII.GetBytes(((string)(object)value[0]));
                        if ((tmpBuff.Length % 2) != 0)
                        {
                            All.Class.Error.Add(string.Format("{0}:LGPLC多点写入数据时,只能以寄存器为单位进行写入,所以字符长度必须成双", this.Text));
                            return false;
                        }
                        for (int i = 0; i < tmpBuff.Length; i = i + 2)
                        {
                            sendBuff.Add((ushort)((tmpBuff[i + 1] << 8) + tmpBuff[i]));
                        }
                        break;
                    case All.Class.TypeUse.TypeList.Boolean://写多个线圈要先读
                        List<bool> tmpBool = new List<bool>();
                        tmpStart = (start >> 4) * 16;//组合成须要的M点,按
                        tmpEnd = (end >> 4) * 16 + 15;
                        if (((tmpEnd - tmpStart) % 32) == 15)//组合成双数寄存器
                        {
                            tmpEnd = tmpEnd + 16;
                        }
                        if (!Read<bool>(out tmpBool, tmpStart, tmpEnd))
                        {
                            All.Class.Error.Add(string.Format("{0}:LGPLC批量写入M点前的预读失败", this.Text));
                            return false;
                        }
                        int curStart = (start % 16);
                        for (int i = 0; i < value.Count; i++)
                        {
                            tmpBool[curStart + i] = (bool)(object)value[i];
                        }
                        tmpStart = 0x1000 + (start >> 4);
                        tmpEnd = 0x1000 + (end >> 4);
                        byte[] tmpbuff = All.Class.Num.Bool2Byte(tmpBool.ToArray());

                        for (int i = 0; i < tmpbuff.Length; i = i + 2)
                        {
                            sendBuff.Add((ushort)((tmpbuff[i + 1] << 8) + tmpbuff[i]));
                        }
                        break;
                    case All.Class.TypeUse.TypeList.Float:
                    case All.Class.TypeUse.TypeList.Double:
                        All.Class.Error.Add("当前PLC不支持此类型数据", Environment.StackTrace);
                        return false;
                    default:
                        All.Class.Error.Add("不可知数据类型", Environment.StackTrace);
                        return false;
                }
                Dictionary<string, string> newParm = new Dictionary<string, string>();
                newParm.Add("Code", "16");
                newParm.Add("Start", tmpStart.ToString());
                result &= modbus.WriteInternal<ushort>(sendBuff, newParm);
                return result;
            }
        }
    }
}
