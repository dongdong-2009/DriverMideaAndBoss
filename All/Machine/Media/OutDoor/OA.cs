using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
namespace All.Machine.Media
{
    public class OA
    {
        /// <summary>
        /// 电量等数据改变方式
        /// </summary>
        public enum DataMethods
        {
            固定,
            自增
        }
        /// <summary>
        /// 使用的串口
        /// </summary>
        public SerialPort Com
        { get; set; }
        /// <summary>
        /// 故障
        /// </summary>
        public string Error
        { get; set; }
        /// <summary>
        /// 连接状态
        /// </summary>
        public bool Conn
        { get; set; }
        /// <summary>
        /// 数据来源方式
        /// </summary>
        public DataMethods DataMethod
        { get; set; }
        /// <summary>
        /// 当DataMethod为固定时,读取电量将返回此结果,当DataMethod为自增时,读取电量将在此结果上自增
        /// </summary>
        public int Data
        { get; set; }
        /// <summary>
        /// 超时时间
        /// </summary>
        public int TimeOut
        { get; set; }
        Thread thRead;
        bool exit = false;
        int data = 0;
        public OA(SerialPort com)
        {
            this.Com = com;
            this.Conn = false;
            this.TimeOut = 35000;
            this.DataMethod = DataMethods.自增;
        }
        public OA(string com)
            : this(new SerialPort(com))
        { }
        public void Open()
        {
            try
            {
                this.Com.BaudRate = 1200;
                this.Com.Parity = Parity.Even;
                this.Com.DataBits = 8;
                this.Com.StopBits = StopBits.One;
                this.Com.Open();
                exit = false;
                data = this.Data;
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
            int len = 0;
            bool check = false;
            byte[] buffRead;
            byte[] buffSend;
            byte[] tmpBuff;
            int start = 0;
            int end = 0;
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
                    Thread.Sleep(40);
                    if (len > 0 && len == this.Com.BytesToRead)
                    {
                        buffRead = new byte[len];
                        this.Com.Read(buffRead, 0, len);
                        start = buffRead.ToList().FindIndex(tmp => tmp == 0x68);
                        end = buffRead.ToList().FindIndex(tmp => tmp == 0x16);
                        if (start >= 0 && end >= 0 
                            && buffRead[start] == 0x68 && buffRead[start + 7] == 0x68 && buffRead[end] == 0x16
                            && All.Class.Check.SumCheck(buffRead,start, end - start - 1) == buffRead[end - 1])
                        {
                            check = true;
                            switch (buffRead[start + 8])//控制码
                            {
                                case 0x0A:
                                    if ((end - start + 1) >= 18)
                                    {
                                        buffSend = new byte[12];
                                        buffSend[0] = 0x68;
                                        buffSend[1] = (byte)(buffRead[start + 10] - 0x33);
                                        buffSend[2] = (byte)(buffRead[start + 11] - 0x33);
                                        buffSend[3] = (byte)(buffRead[start + 12] - 0x33);
                                        buffSend[4] = (byte)(buffRead[start + 13] - 0x33);
                                        buffSend[5] = (byte)(buffRead[start + 14] - 0x33);
                                        buffSend[6] = (byte)(buffRead[start + 15] - 0x33);
                                        buffSend[7] = 0x68;
                                        buffSend[8] = 0x8A;
                                        buffSend[9] = 0x00;
                                        buffSend[10] = All.Class.Check.SumCheck(buffSend, 11);
                                        buffSend[11] = 0x16;
                                        this.Com.Write(buffSend, 0, buffSend.Length);
                                    }
                                    break;
                                case 0x01:
                                    if ((end - start + 1) >= 14)
                                    {
                                        switch (DataMethod)
                                        {
                                            case DataMethods.自增:
                                                data = data + 100;
                                                break;
                                        }
                                        tmpBuff = All.Class.Num.Str2Hex(string.Format("{0:D8}", data));
                                        buffSend = new byte[18];
                                        for (int i = 0; i <= 7; i++)
                                        {
                                            buffSend[i] = buffRead[start + i];//头码+地址
                                        }
                                        buffSend[8] = 0x81;//命令码
                                        buffSend[9] = 0x06;//数据长度
                                        buffSend[10] = buffRead[start + 10];
                                        buffSend[11] = buffRead[start + 11];
                                        buffSend[12] = (byte)((tmpBuff[0] + 0x33) & 0xFF);
                                        buffSend[13] = (byte)((tmpBuff[1] + 0x33) & 0xFF);
                                        buffSend[14] = (byte)((tmpBuff[2] + 0x33) & 0xFF);
                                        buffSend[15] = (byte)((tmpBuff[3] + 0x33) & 0xFF);
                                        buffSend[16] = All.Class.Check.SumCheck(buffSend, 16);
                                        buffSend[17] = 0x16;
                                        this.Com.Write(buffSend, 0, buffSend.Length);
                                    }
                                    break;
                            }
                        }
                    }
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}
