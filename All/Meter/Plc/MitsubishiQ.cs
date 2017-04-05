using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace All.Meter
{
    public class MitsubishiQ:Meter
    {
        Dictionary<string, string> initParm;

        public override Dictionary<string, string> InitParm
        {
            get { return initParm; }
            set { initParm = value; }
        }
        object lockObject = new object();
        int NetCode = 0x00;
        int PlcCode = 0xFF;
        int IoCode = 0x3FF;
        int StationCode = 0x00;
        int CpuTime = 0x10;
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
        }
        public override bool Read<T>(out List<T> value, Dictionary<string, string> parm)
        {
            value = new List<T>();
            bool result = true;
            lock (lockObject)
            {
                try
                {
                    int start = 0;
                    int end = 0;
                    string style = "";
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
                    if (parm.ContainsKey("Style"))
                    {
                        style = parm["Style"];
                    }
                    int readLen = 0;//写入数据中要读取的长度
                    string readValue = "";//返回字符串
                    string sendValue = string.Format("5000{0:X2}{1:X2}{2:X4}{3:X2}{4:X4}",
                        NetCode, PlcCode, IoCode, StationCode, 0x18);//相同的头文件
                    string startValue = "";//不同类型的数据，Start位可能为10进制数据，可能为16进制数据
                    sendValue = string.Format("{0}{1:X4}{2:X4}{3:X4}", sendValue, CpuTime, 0x0401, 0x0000);//0x0401为命令码
                    switch (Class.TypeUse.GetType<T>())
                    {
                        case All.Class.TypeUse.TypeList.UShort:
                            sendValue = string.Format("{0}R*", sendValue);
                            readLen = end - start + 1;
                            startValue = string.Format("{0:D6}", start);
                            break;
                        case All.Class.TypeUse.TypeList.Boolean:
                            switch (style)
                            {
                                case "X":
                                    sendValue = string.Format("{0}X*", sendValue);
                                    startValue = string.Format("{0:X6}", start);
                                    break;
                                case "L":
                                    sendValue = string.Format("{0}L*", sendValue);
                                    startValue = string.Format("{0:D6}", start);
                                    break;
                                default://默认M点
                                    sendValue = string.Format("{0}M*", sendValue);
                                    startValue = string.Format("{0:D6}", start);
                                    break;
                            }
                            readLen = (int)Math.Ceiling((end - start + 1) / 16f);
                            break;
                        default:
                            All.Class.Error.Add("MitsubishiQ读取不可知数据类型", Environment.StackTrace);
                            return false;
                    }
                    sendValue = string.Format("{0}{1}{2:X4}", sendValue, startValue, readLen);
                    if (Write<string, string>(sendValue, readLen * 4 + 22, out readValue))
                    {

                        if (readValue.IndexOf(string.Format("D000{0:X2}{1:X2}{2:X4}{3:X2}{4:X4}0000", NetCode, PlcCode, IoCode, StationCode, readLen * 4 + 4)) == 0)
                        {
                            ushort[] buff = new ushort[readLen];
                            for (int i = 0; i < readLen; i++)
                            {
                                buff[i] = Convert.ToUInt16(readValue.Substring(22 + i * 4, 4), 16);
                            }
                            switch (Class.TypeUse.GetType<T>())
                            {
                                case All.Class.TypeUse.TypeList.Boolean:
                                    bool[] tmpBool = All.Class.Num.Ushort2Bool(buff);
                                    for (int i = 0; i < (end - start + 1); i++)
                                    {
                                        value.Add((T)(object)tmpBool[i]);
                                    }
                                    break;
                                case All.Class.TypeUse.TypeList.UShort:
                                    for (int i = 0; i < readLen; i++)
                                    {
                                        value.Add((T)(object)buff[i]);
                                    }
                                    break;
                                default:
                                    All.Class.Error.Add("MitsubishiQ读取不可知数据类型", Environment.StackTrace);
                                    break;
                            }
                        }
                        else
                        {
                            result = false;
                            Error = "校验错误";
                            All.Class.Log.Add(string.Format("MitsubishiQ读取数据校验错误\r\n写入数据  ->  {0}\r\n返回数据  ->  {1}", sendValue, readValue), Environment.StackTrace);
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
            }
            return result;

        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <typeparam name="T">当T为bool时，M点为0开始，X点在原来的基础上加100000，L点在原来的基础上加200000</typeparam>
        /// <param name="value"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
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
        public override bool WriteInternal<T>(List<T> value, int start, int end)
        {
            throw new NotImplementedException();
        }
        public override bool WriteInternal<T>(T value, int start)
        {
            throw new NotImplementedException();
        }
        public override bool WriteInternal<T>(List<T> value, Dictionary<string, string> parm)
        {
            throw new NotImplementedException();
        }
    }
}
