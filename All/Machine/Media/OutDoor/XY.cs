using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.IO.Ports;
namespace All.Machine.Media
{
    public class XY
    {
        public SerialPort Com
        { get; set; }
        public AllMachine.Machines Machine
        { get; set; }
        public string Error
        { get; set; }
        public bool Conn
        { get; set; }
        public int TimeOut
        { get; set; }

        bool exit = false;
        Thread thRead;
        public XY(SerialPort com, AllMachine.Machines machine)
        {
            this.Com = com;
            this.Machine = machine;
            this.Conn = false;
            this.Error = "";
            this.TimeOut = 1000;

        }
        public XY(string com, AllMachine.Machines machine)
            : this(new SerialPort(com), machine)
        {
        }
        #region//默认的XY信号
        /// <summary>
        /// 获取默认的XY信号
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string GetSnCode(AllMachine.Modes mode)
        {
            byte[] buff = new byte[18];
            buff[0] = 0xFE;
            buff[1] = 0xAA;
            buff[2] = 0xC0;
            buff[3] = 0x00;
            buff[4] = 0x00;
            buff[5] = 0x80;
            buff[6] = 0x00;
            buff[7] = 0x00;
            buff[8] = 0x00;
            buff[9] = 0x00;
            buff[10] = 0x00;
            buff[11] = 0x00;
            buff[12] = 0x00;
            buff[13] = 0x00;
            buff[14] = 0x3F;
            buff[15] = 0x81;
            buff[16] = 0x55;
            buff[17] = 0xFE;
            return All.Class.Num.Hex2Str(buff);
        }
        #endregion
        /// <summary>
        /// 开始通讯
        /// </summary>
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
                    case AllMachine.Machines.商用三管制:
                        this.Com.BaudRate = 4800;
                        break;
                    case AllMachine.Machines.商用过冷机:
                        this.Com.BaudRate = 9600;
                        break;
                    default:
                        All.Class.Error.Add(string.Format("当前输入的机器:{0},没有XY通讯功能", this.Machine), Environment.StackTrace);
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
        /// 关闭通讯
        /// </summary>
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
        private void Flush()
        {
            byte[] buff;
            int errorCount = 0;
            bool check = false;
            bool isTimeOut = false;
            bool isGetData = false;
            int timeStart = Environment.TickCount;
            while (!exit)
            {
                check = false;
                buff = All.Class.Num.Str2Hex(GetSnCode(AllMachine.Modes.待机));
                this.Com.DiscardInBuffer();
                this.Com.DiscardOutBuffer();
                this.Com.Write(buff, 0, buff.Length);
                isTimeOut = false;
                isGetData = false;
                timeStart = Environment.TickCount;
                do
                {
                    Thread.Sleep(50);
                    if (this.Com.BytesToRead >= 32)
                    {
                        isGetData = true;
                    }
                    if ((Environment.TickCount - timeStart) > this.TimeOut)
                    {
                        isTimeOut = true;
                    }
                } while (!isTimeOut && !isGetData);
                if (isGetData)
                {
                    buff = new byte[this.Com.BytesToRead];
                    this.Com.Read(buff, 0, buff.Length);
                    for (int i = 0; i < buff.Length - 31; i++)
                    {
                        if (buff[i] == 0xAA && buff[i + 1] == 0xC0 && buff[i + 31] == 0x55)
                        {
                            check = true;
                            break;
                        }
                    }
                }
                if (check)
                {
                    this.Conn = true;
                    errorCount = 0;
                }
                else
                {
                    errorCount++;
                    if (errorCount >= 3)
                    {
                        errorCount = 3;
                        this.Conn = false;
                    }
                }
                
                Thread.Sleep(1000);
            }
        }
    }
}
