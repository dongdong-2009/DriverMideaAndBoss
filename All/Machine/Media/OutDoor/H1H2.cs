using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.IO.Ports;
namespace All.Machine.Media
{
    public class H1H2
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

        //string SnCode = "";//发送指令
        bool exit = false;
        Thread thRead;
        public H1H2(SerialPort com, AllMachine.Machines machine)
        {
            this.Com = com;
            this.Machine = machine;
            this.Conn = false;
            this.Error = "";
            this.TimeOut = 5000;
            //SnCode = XY.GetSnCode(AllMachine.Modes.待机);
        }
        public H1H2(string com, AllMachine.Machines machine)
            : this(new SerialPort(com), machine)
        {
        }
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
                        this.Com.BaudRate = 600;
                        break;
                    case AllMachine.Machines.商用过冷机:
                        this.Com.BaudRate = 1200;
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
            int timeStart = Environment.TickCount;
            int len = 0;
            while (!exit)
            {
                len = this.Com.BytesToRead;
                Thread.Sleep(20);
                if (len >= 16 && len == this.Com.BytesToRead)
                {
                    buff = new byte[len];
                    this.Com.Read(buff, 0, len);
                    for (int i = 0; i < buff.Length - 15; i++)
                    {
                        if (buff[i] == 0xAA && buff[i + 15] == 0x55)
                        {
                            this.Conn = true;
                            timeStart = Environment.TickCount;
                        }
                    }
                }
                if ((Environment.TickCount - timeStart) > this.TimeOut)
                {
                    this.Conn = false;
                }
            }
        }
    }
}
