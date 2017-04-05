using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace All.Meter
{
    public class RFID:Meter
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
        /// <summary>
        /// 模块通讯地址
        /// </summary>
        public byte Address
        {
            get { return address; }
            set { address = value; }
        }
        public override void Init(Dictionary<string, string> initParm)
        {
            this.initParm = initParm;
            if (InitParm.ContainsKey("Text"))
            {
                this.Text = InitParm["Text"];
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
                All.Class.Error.Add("RFID初始化参数中没有地址", Environment.StackTrace);
            }
            else
            {
                address = All.Class.Num.ToByte(InitParm["Address"]);
            }
            MostLog = false;
        }
        public override bool Read<T>(out List<T> value, Dictionary<string, string> parm)
        {
            //53 57 00 07 01 12 03 00 0D 00 2C
            lock (lockObject)
            {
                bool result = true;
                value = new List<T>();
                try
                {
                    //读取标签号
                    byte[] sendBuff = new byte[8];
                    sendBuff[0] = 0x53;
                    sendBuff[1] = 0x57;
                    sendBuff[2] = 0x00;
                    sendBuff[3] = 0x04;
                    sendBuff[4] = address;
                    sendBuff[5] = 0x11;
                    sendBuff[6] = 0x00;
                    byte tmpSumCheck = 0;
                    All.Class.Check.SumCheck(sendBuff, 7, out tmpSumCheck);
                    sendBuff[7] = (byte)(0xFF & ((tmpSumCheck ^ 0xFF) + 1));
                    byte[] readBuff;
                    if (Write<byte[], byte[]>(sendBuff, 17, out readBuff))
                    {
                        if (readBuff[0] != 0x43 || readBuff[1] != 0x54 || readBuff[6] != 0x00 || (readBuff[3] != readBuff.Length - 4))
                        { 
                            All.Class.Log.Add(string.Format("RFID返回数据指令不正确\r\n写入数据  ->  {0}\r\n返回数据  ->  {1}", All.Class.Num.Hex2Str(sendBuff), All.Class.Num.Hex2Str(readBuff)), Environment.StackTrace);
                            return false;
                        }
                        All.Class.Check.SumCheck(readBuff, readBuff.Length - 1, out tmpSumCheck);
                        if (readBuff[readBuff.Length - 1] != (byte)((tmpSumCheck ^ 0xFF) + 1))
                        {
                            All.Class.Log.Add(string.Format("RFID返回数据校验不正确\r\n写入数据  ->  {0}\r\n返回数据  ->  {1}", All.Class.Num.Hex2Str(sendBuff), All.Class.Num.Hex2Str(readBuff)), Environment.StackTrace);
                            return false;
                        }
                        switch (Class.TypeUse.GetType<T>())
                        {
                            case All.Class.TypeUse.TypeList.Byte:
                                for (int i = 7; i < readBuff.Length - 1; i++)
                                {
                                    value.Add((T)(object)(readBuff[i]));
                                }
                                break;
                            case All.Class.TypeUse.TypeList.String:
                                value.Add((T)(object)All.Class.Num.Hex2Str(readBuff, 10, readBuff[9]));
                                break;
                            default:
                                All.Class.Error.Add("RFID读取数据类型不正确", Environment.StackTrace);
                                return false;
                        }
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                    /*//这个是读取寄存器数据的。
                    byte[] sendBuff = new byte[11];
                    sendBuff[0] = 0x53;//固定码
                    sendBuff[1] = 0x57;//固定码
                    sendBuff[2] = 0x00;//固定码
                    sendBuff[3] = 0x07;//此处为长度控制,即此字节后面的总长度
                    sendBuff[4] = address;//地址
                    sendBuff[5] = 0x12;//命令
                    sendBuff[6] = 0x03;//区域
                    sendBuff[7] = 0x00;//起始地址
                    sendBuff[8] = 0x0D;//读取长度
                    if (parm.ContainsKey("Area"))
                    {
                        switch (parm["Area"].ToUpper())
                        {
                            case "0":
                            case "RESERVED":
                                sendBuff[6] = 0;
                                break;
                            case "1":
                            case "EPC":
                                sendBuff[6] = 1;
                                break;
                            case "2":
                            case "TIP":
                                sendBuff[6] = 2;
                                break;
                            case "3":
                            case "USER":
                                sendBuff[6] = 3;
                                break;
                            default:
                                All.Class.Log.Add(string.Format("RFID读取区域指令不正确,没有当前区域{0},程序修改使用用户区:3", parm["Area"]));
                                break;
                        }
                    }
                    if (parm.ContainsKey("Start"))
                    {
                        sendBuff[7] = All.Class.Num.ToByte(parm["Start"]);
                    }
                    if (!parm.ContainsKey("Len"))
                    {
                        All.Class.Error.Add("RFID读取指令不包含读取长度");
                        return false;
                    }
                    sendBuff[8] = All.Class.Num.ToByte(parm["Len"]);
                    sendBuff[9] = 0;
                    byte tmpSumCheck = 0;
                    All.Class.Check.SumCheck(sendBuff, 10, out tmpSumCheck);
                    sendBuff[10] = (byte)(0xFF & ((tmpSumCheck ^ 0xFF) + 1));
                    byte[] ReadBuff;
                    if (Write<byte[], byte[]>(sendBuff, sendBuff[8] * 2 + 8, out ReadBuff))
                    {
                        if (ReadBuff[0] != 0x43 || ReadBuff[1] != 0x54 || ReadBuff[2] != 0x00 || (ReadBuff[3] != ReadBuff.Length - 4))
                        {
                            All.Class.Log.Add(string.Format("RFID返回数据指令不正确\r\n写入数据  ->  {0}\r\n返回数据  ->  {1}", All.Class.Num.Hex2Str(sendBuff), All.Class.Num.Hex2Str(ReadBuff)), Environment.StackTrace);
                            return false;
                        }
                        All.Class.Check.SumCheck(ReadBuff, ReadBuff.Length - 1, out tmpSumCheck);
                        if (ReadBuff[ReadBuff.Length - 1] != (byte)((tmpSumCheck ^ 0xFF) + 1))
                        {
                            All.Class.Log.Add(string.Format("RFID返回数据校验不正确\r\n写入数据  ->  {0}\r\n返回数据  ->  {1}", All.Class.Num.Hex2Str(sendBuff), All.Class.Num.Hex2Str(ReadBuff)), Environment.StackTrace);
                            return false;
                        }
                        switch (typeof(T).ToString())
                        {
                            case All.Class.TypeUse.TypeList.Byte:
                                for (int i = 7; i < ReadBuff.Length - 1; i++)
                                {
                                    value.Add((T)(object)(ReadBuff[i]));
                                }
                                break;
                            case All.Class.TypeUse.TypeList.String:
                                value.Add((T)(object)(Encoding.ASCII.GetString(ReadBuff, 7, sendBuff[8] * 2)));
                                break;
                            default:
                                All.Class.Error.Add("RFID读取数据类型不正确", Environment.StackTrace);
                                return false;
                        }
                    }
                    else
                    {
                        result = false;
                        if (this.Text.IndexOf("上线") >= 0)
                        {
                            value.Add((T)(object)"112233".PadRight(30, ' '));
                            result = true;
                        }
                    }*/
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
            parm.Add("Area", "User");
            parm.Add("Start", start.ToString());
            parm.Add("End", end.ToString());
            
            return Read<T>(out value, parm);
        }
        public override bool Read<T>(out T value, int start)
        {
            value = default(T);
            List<T> tmp = new List<T>();
            bool result= Read<T>(out tmp, start, start);
            if (result && tmp.Count > 0)
            {
                value = tmp[0];
            }
            return result;
        }
        public override bool WriteInternal<T>(List<T> value, Dictionary<string, string> parm)
        {
            lock (lockObject)
            {
                bool result = true;
                List<byte> tmpBuff = new List<byte>();
                switch (Class.TypeUse.GetType<T>())
                {
                    case All.Class.TypeUse.TypeList.Byte:
                        for (int i = 0; i < value.Count; i++)
                        {
                            tmpBuff.Add((byte)(object)value[i]);
                        }
                        break;
                    case All.Class.TypeUse.TypeList.String:
                        tmpBuff = Encoding.ASCII.GetBytes((string)(object)value[0]).ToList();
                        break;
                    default:
                        All.Class.Error.Add("RFID写入的数据类型不正确", Environment.StackTrace);
                        return false;
                }
                int len = tmpBuff.Count + 11;
                if (parm.ContainsKey("Len"))
                {
                    len = Math.Min(All.Class.Num.ToInt(parm["Len"]) * 2 + 11, len);
                }
                if (((len - 11) % 2) != 0)
                {
                    All.Class.Error.Add("RFID写入数据长度不正确,写入方法以字为基础,字节数应该是双数", Environment.StackTrace);
                    return false;
                }
                byte[] sendBuff = new byte[len];
                sendBuff[0] = 0x53;
                sendBuff[1] = 0x57;
                sendBuff[2] = 0x00;
                sendBuff[3] = (byte)(len - 4);
                sendBuff[4] = address;
                sendBuff[5] = 0x13;
                if (parm.ContainsKey("Area"))
                {
                    switch (parm["Area"].ToUpper())
                    {
                        case "0":
                        case "RESERVED":
                            sendBuff[6] = 0;
                            break;
                        case "1":
                        case "EPC":
                            sendBuff[6] = 1;
                            break;
                        case "2":
                        case "TIP":
                            sendBuff[6] = 2;
                            break;
                        case "3":
                        case "USER":
                            sendBuff[6] = 3;
                            break;
                        default:
                            All.Class.Log.Add(string.Format("RFID读取区域指令不正确,没有当前区域{0},程序修改使用用户区:3", parm["Area"]));
                            break;
                    }
                }
                if (parm.ContainsKey("Start"))
                {
                    sendBuff[7] = All.Class.Num.ToByte(parm["Start"]);
                }
                sendBuff[8] = (byte)((sendBuff.Length - 11) / 2);
                for (int i = 9; i < sendBuff.Length - 2; i++)
                {
                    sendBuff[i] = tmpBuff[i - 9];
                }
                sendBuff[sendBuff.Length - 2] = 0x00;
                byte tmpSumCheck;
                All.Class.Check.SumCheck(sendBuff, sendBuff.Length - 1, out tmpSumCheck);
                sendBuff[sendBuff.Length - 1] = (byte)(0xFF & ((tmpSumCheck ^ 0xFF) + 1));
                byte[] ReadBuff;
                if (Write<byte[], byte[]>(sendBuff, 8, out ReadBuff))
                {
                    if (ReadBuff[0] != 0x43 || ReadBuff[1] != 0x54 || ReadBuff[2] != 0x00 || (ReadBuff[3] != ReadBuff.Length - 4))
                    {
                        All.Class.Log.Add(string.Format("RFID返回数据指令不正确\r\n写入数据  ->  {0}\r\n返回数据  ->  {1}", All.Class.Num.Hex2Str(sendBuff), All.Class.Num.Hex2Str(ReadBuff)), Environment.StackTrace);
                        return false;
                    }
                    All.Class.Check.SumCheck(ReadBuff, ReadBuff.Length - 1, out tmpSumCheck);
                    if (ReadBuff[ReadBuff.Length - 1] != (byte)((tmpSumCheck ^ 0xFF) + 1))
                    {
                        All.Class.Log.Add(string.Format("RFID返回数据校验不正确\r\n写入数据  ->  {0}\r\n返回数据  ->  {1}", All.Class.Num.Hex2Str(sendBuff), All.Class.Num.Hex2Str(ReadBuff)), Environment.StackTrace);
                        return false;
                    }
                    if (ReadBuff[6] != 0x00)
                    {
                        Parent.Close();
                        System.Threading.Thread.Sleep(50);
                        Parent.Open();
                        result = false;
                        All.Class.Error.Add(string.Format("RFID数据写入不正确\r\n写入数据  ->  {0}\r\n返回数据  ->  {1}", All.Class.Num.Hex2Str(sendBuff), All.Class.Num.Hex2Str(ReadBuff)), Environment.StackTrace);
                    }
                }
                else
                {
                    result = false;
                }
                return result;
            }
        }
        public override bool WriteInternal<T>(List<T> value, int start, int end)
        {
            int len = end - start + 1;
            Dictionary<string, string> parm = new Dictionary<string, string>();
            parm.Add("Area", "User");
            parm.Add("Start", start.ToString());
            parm.Add("Len", len.ToString());
            return WriteInternal<T>(value, parm);
        }
        public override bool WriteInternal<T>(T value, int start)
        {
            List<T> tmp = new List<T>();
            tmp.Add(value);
            return WriteInternal<T>(tmp, start, start);
        }
    }
}
