using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO.Ports;
using System.Threading;
namespace All.Machine.Media
{
    public class PQ
    {
        public SerialPort Com
        { get; set; }
        public AllMachine.Machines Machine
        { get; set; }
        public string Error
        { get; set; }
        public bool Conn
        { get; set; }
        int doorCount = 1;

        public int DoorCount
        {
            get { return doorCount; }
            set {
                if (doorCount > 0)
                {
                    doorCount = value;
                }
            }
        }
        public int TimeOut
        { get; set; }
        
        string SnCode = "";//发送指令
        bool exit = false;
        Thread thRead;
        public PQ(SerialPort com, AllMachine.Machines machine)
        {
            this.Com = com;
            this.Machine = machine;
            this.DoorCount = 1;
            this.Conn = false;
            TimeOut = 5000;
        }
        public PQ(string com, AllMachine.Machines machine)
            : this(new SerialPort(com), machine)
        {
        }
        public void Open()
        {
            try
            {
                switch (this.Machine)
                {
                    case AllMachine.Machines.商用V4:
                    case AllMachine.Machines.商用V6:
                    case AllMachine.Machines.商用定变组合:
                    case AllMachine.Machines.商用两管制:
                        this.Com.BaudRate = 4800;
                        break;
                    case AllMachine.Machines.商用三管制:
                    case AllMachine.Machines.商用过冷机:
                        this.Com.BaudRate = 9600;
                        break;
                    default:
                        All.Class.Error.Add(string.Format("当前输入的机器:{0},没有PQ通讯功能", this.Machine), Environment.StackTrace);
                        return;
                }
                this.Com.Parity = Parity.None;
                this.Com.DataBits = 8;
                this.Com.StopBits = StopBits.One;
                this.Com.RtsEnable = true;
                this.Com.DtrEnable = true;
                this.Com.Open();
                exit = false;
                thRead = new Thread(() => Flush());
                thRead.IsBackground = true;
                thRead.Start();
            }
            catch (Exception e)
            {
                Error = e.Message;
                All.Class.Error.Add(e);
            }
        }
        /// <summary>
        /// 实际使用中，自动修正发送指令
        /// </summary>
        /// <param name="FE"></param>
        /// <param name="InDoorIndex"></param>
        /// <param name="OutDoorIndex"></param>
        /// <returns></returns>
        private byte[] ChangeSnCode(bool FE,byte InDoorIndex, byte OutDoorIndex)
        {
            byte[] buff = All.Class.Num.Str2Hex(this.SnCode);
            byte[] result = new byte[0];
            int index = 0;
            switch (Machine)
            {
                case AllMachine.Machines.商用V4:
                    if (OutDoorIndex == 3)
                    {
                        buff[6] = 0xFA;
                    }
                    else
                    {
                        buff[6] = 0x00;
                    }
                    buff[1] = InDoorIndex;

                    AllMachine.GetCheck(Machine, buff, out buff[8]);
                    result = new byte[10];
                    if (FE)
                    {
                        result = new byte[12];
                        result[index++] = 0xFE;
                    }
                    for (int i = 0; i < buff.Length && i < result.Length; i++)
                    {
                        result[index++] = buff[i];
                    }
                    if (FE)
                    {
                        result[index++] = 0xFE;
                    }
                    break;
                case AllMachine.Machines.商用V6:
                case AllMachine.Machines.商用定变组合:
                    result = new byte[10];
                    if (FE)
                    {
                        result = new byte[12];
                        result[index++] = 0xFE;
                    }
                    for (int i = 0; i < buff.Length && i < result.Length; i++)
                    {
                        result[index++] = buff[i];
                    }
                    if (FE)
                    {
                        result[index++] = 0xFE;
                    }
                    break;
                case AllMachine.Machines.商用两管制:
                case AllMachine.Machines.商用三管制:
                    buff[4] = InDoorIndex;
                    AllMachine.GetCheck(Machine, buff, out buff[30]);
                    result = new byte[32];
                    if (FE)
                    {
                        result = new byte[34];
                        result[index++] = 0xFE;
                    }
                    for (int i = 0; i < buff.Length && i < result.Length; i++)
                    {
                        result[index++] = buff[i];
                    }
                    if (FE)
                    {
                        result[index++] = 0xFE;
                    }
                    break;
            }
            return result;
        }
        /// <summary>
        /// 获取默认的PQ指令
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string GetSnCode(AllMachine.Machines machine, AllMachine.Modes mode)
        {
            byte[] buff = new byte[10];

            switch (machine)
            {
                case AllMachine.Machines.商用V4:
                    buff = new byte[10];
                    buff[0] = 0xAA;
                    buff[1] = 0x01;//内机序号
                    buff[2] = 0x00;//模式
                    buff[3] = 0x3C;//能级需求
                    buff[4] = 0x25;
                    buff[5] = 0x01;
                    buff[6] = 0x00;
                    buff[7] = 0x00;
                    buff[8] = 0x00;//校验
                    buff[9] = 0x55;
                    switch (mode)
                    {
                        case AllMachine.Modes.待机:
                            buff[2] = 0x00;
                            buff[4] = 0x19;
                            break;
                        case AllMachine.Modes.制冷:
                            buff[2] = 0x02;
                            buff[4] = 0x11;
                            break;
                        case AllMachine.Modes.制热:
                            buff[2] = 0x03;
                            buff[4] = 0x1E;
                            break;
                    }
                    AllMachine.GetCheck(AllMachine.Machines.商用V4, buff, out buff[8]);
                    break;
                case AllMachine.Machines.商用V6:
                case AllMachine.Machines.商用定变组合:
                    buff = new byte[10];
                    buff[0] = 0xAA;
                    buff[1] = 0x01;//内机序号
                    buff[2] = 0x00;//模式
                    buff[3] = 0x00;//能级需求
                    buff[4] = 0x00;
                    buff[5] = 0x00;
                    buff[6] = 0x00;
                    buff[7] = 0x00;
                    buff[8] = 0xFF;//校验
                    buff[9] = 0x55;
                    break;
                case AllMachine.Machines.商用两管制:
                case AllMachine.Machines.商用三管制:
                    buff = new byte[32];
                    buff[0] = 0xAA;
                    buff[1] = 0xC0;
                    buff[2] = 0x80;
                    buff[3] = 0x00;
                    buff[4] = 0x00;
                    buff[5] = 0x00;
                    buff[6] = 0xE0;
                    buff[7] = 0x14;
                    buff[8] = 0x88;
                    buff[9] = 0x01;
                    buff[10] = 0x15;
                    buff[11] = 0x5A;
                    buff[12] = 0x5A;
                    buff[13] = 0x48;
                    buff[14] = 0xFF;
                    buff[15] = 0xFF;
                    buff[16] = 0xFF;
                    buff[17] = 0x00;
                    buff[18] = 0x00;
                    buff[19] = 0x01;
                    buff[20] = 0x04;
                    buff[21] = 0x04;
                    buff[22] = 0x00;
                    buff[23] = 0x00;
                    buff[24] = 0x00;
                    buff[25] = 0x00;
                    buff[26] = 0x00;
                    buff[27] = 0x0A;
                    buff[28] = 0x1E;
                    buff[29] = 0xF2;
                    AllMachine.GetCheck(machine, buff, out buff[30]);
                    buff[31] = 0x55;
                    break;
            }
            return All.Class.Num.Hex2Str(buff);
        }
        public void Close()
        {
            exit = true;
            if (thRead != null)
            {
                thRead.Join(500);
                thRead.Abort();
                thRead = null;
            }
            if (this.Com != null)
            {
                if (this.Com.IsOpen)
                {
                    this.Com.Close();
                }
            }
        }
        public void Send(string SnCode)
        {
            this.SnCode = SnCode;
        }
        public void Send(string SnCode,byte NengJi)
        {
            byte[] buff = All.Class.Num.Str2Hex(SnCode);
            switch (buff.Length)
            {
                case 10:
                    buff[3] = NengJi;
                    break;
                case 12:
                    buff[4] = NengJi;
                    break;
                default:
                    break;
            }
            this.SnCode = All.Class.Num.Hex2Str(buff);
        }
        private void Flush()
        {
            int len = 0;
            bool check = false;
            byte[] buff;
            int startTime = Environment.TickCount;
            while (!exit)
            {
                if (check)
                {
                    startTime = Environment.TickCount;
                    this.Conn = true;
                    check = false;
                }
                else
                {
                    if ((Environment.TickCount - startTime) > TimeOut)
                    {
                        this.Conn = false;
                    }
                }

                try
                {
                    len = this.Com.BytesToRead;
                    Thread.Sleep(20);
                    if (len > 0 && len == this.Com.BytesToRead)
                    {
                        switch (Machine)
                        {
                            case AllMachine.Machines.商用V4:
                                switch (len)
                                {
                                    case 10:
                                        buff = new byte[len];
                                        this.Com.Read(buff, 0, len);
                                        if (buff[0] == 0xAA && buff[9] == 0x55)
                                        {
                                            check = true;
                                            for (int i = 0; i < DoorCount; i++)
                                            {
                                                this.Com.Write(ChangeSnCode(false, (byte)(i + 1), buff[7]), 0, 10);
                                            }
                                        }
                                        break;
                                    case 11:
                                    case 12:
                                        buff = new byte[len];
                                        this.Com.Read(buff, 0, len);
                                        if ((buff[0] == 0xFE && buff[1] == 0xAA && buff[10] == 0x55)
                                            || (buff[0] == 0xAA && buff[9] == 0x55 && buff[10] == 0xFE))
                                        {
                                            check = true;
                                            for (int i = 0; i < DoorCount; i++)
                                            {
                                                this.Com.Write(ChangeSnCode(true, (byte)(i + 1), buff[8]), 0, 12);
                                                All.Class.Log.Add(All.Class.Num.Hex2Str(ChangeSnCode(true, (byte)(i + 1), buff[8])));
                                            }
                                        }
                                        break;
                                    default:
                                        buff = new byte[len];
                                        try
                                        {
                                            this.Com.Read(buff, 0, len);
                                        }
                                        catch (Exception e)
                                        {
                                            All.Class.Error.Add(e);
                                            this.Com.DiscardInBuffer();
                                            this.Com.DiscardOutBuffer();
                                        }
                                        All.Class.Log.Add(All.Class.Num.Hex2Str(buff), Environment.StackTrace);
                                        break;
                                }
                                break;
                            case AllMachine.Machines.商用V6:
                            case AllMachine.Machines.商用定变组合:
                                switch (len)
                                {
                                    case 10:
                                        buff = new byte[len];
                                        this.Com.Read(buff, 0, len);
                                        if (buff[0] == 0xAA && buff[1] == 0x00 && buff[9] == 0x55)
                                        {
                                            check = true;
                                            this.Com.Write(ChangeSnCode(false, 0, 0), 0, 10);
                                        }
                                        break;
                                    case 11:
                                    case 12:
                                        buff = new byte[len];
                                        this.Com.Read(buff, 0, len);
                                        if ((buff[0] == 0xFE && buff[1] == 0xAA && buff[2] == 0x00 && buff[10] == 0x55)
                                            || (buff[0] == 0xAA && buff[1] == 0x00 && buff[9] == 0x55 && buff[10] == 0xFE))
                                        {
                                            check = true;
                                            this.Com.Write(ChangeSnCode(true, 0, 0), 0, 12);
                                        }
                                        break;
                                    default:
                                        buff = new byte[len];
                                        try
                                        {
                                            this.Com.Read(buff, 0, len);
                                        }
                                        catch (Exception e)
                                        {
                                            All.Class.Error.Add(e);
                                            this.Com.DiscardInBuffer();
                                            this.Com.DiscardOutBuffer();
                                        }
                                        All.Class.Log.Add(All.Class.Num.Hex2Str(buff), Environment.StackTrace);
                                        break;
                                }
                                break;
                            case AllMachine.Machines.商用两管制:
                            case AllMachine.Machines.商用三管制:
                                switch (len)
                                {
                                    case 24:
                                        buff = new byte[len];
                                        this.Com.Read(buff, 0, len);
                                        if (buff[0] == 0xAA && buff[1] == 0xC0 && buff[23] == 0x55)
                                        {
                                            check = true;
                                            if (buff[2] == 0x00 || buff[2] == 0x05 || buff[2] == 0x09 || buff[2] == 0x0E)
                                            {
                                                this.Com.Write(ChangeSnCode(false, buff[2], 0), 0, 32);
                                            }
                                        }
                                        break;
                                    case 25:
                                    case 26:
                                        buff = new byte[len];
                                        this.Com.Read(buff, 0, len);
                                        if ((buff[0] == 0xAA && buff[1] == 0xC0 && buff[23] == 0x55 && buff[24] == 0xFE)
                                            || (buff[0] == 0xFE && buff[1] == 0xAA && buff[2] == 0xC0 && buff[24] == 0x55))
                                        {
                                            check = true;
                                            if ((buff[0] == 0xAA && (buff[2] == 0x00 || buff[2] == 0x05 || buff[2] == 0x09 || buff[2] == 0x0E)) ||
                                                (buff[1] == 0xAA && (buff[3] == 0x00 || buff[3] == 0x05 || buff[3] == 0x09 || buff[3] == 0x0E)))
                                            {
                                                this.Com.Write(
                                                    ChangeSnCode(true, (buff[0] == 0xFE ? buff[3] : buff[2]), 0),//取标准指令
                                                    0, 34);
                                            }
                                        }
                                        break;
                                    default:
                                        buff = new byte[len];
                                        try
                                        {
                                            this.Com.Read(buff, 0, len);
                                        }
                                        catch (Exception e)
                                        {
                                            All.Class.Error.Add(e);
                                            this.Com.DiscardInBuffer();
                                            this.Com.DiscardOutBuffer();
                                        }
                                        All.Class.Log.Add(All.Class.Num.Hex2Str(buff), Environment.StackTrace);
                                        break;
                                }
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    All.Class.Error.Add(e);
                    Thread.Sleep(100);
                }
            }
        }

    }
}
