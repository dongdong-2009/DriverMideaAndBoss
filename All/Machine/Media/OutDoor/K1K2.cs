using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO.Ports;
using System.Threading;
namespace All.Machine.Media
{
    public class K1K2
    {
        /// <summary>
        /// 使用串口
        /// </summary>
        public SerialPort Com
        { get; set; }
        /// <summary>
        /// 机型
        /// </summary>
        public AllMachine.Machines Machine
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
        /// 超时时间
        /// </summary>
        public int TimeOut
        { get; set; }
        /// <summary>
        /// 当前步骤
        /// </summary>
        public string Step
        { get; set; }
        /// <summary>
        /// 当前风速
        /// </summary>
        public string Speed
        { get; set; }
        /// <summary>
        /// 环境温度
        /// </summary>
        public float TempHuangJin
        { get; set; }
        /// <summary>
        /// 冷凝温度
        /// </summary>
        public float[] TempLengNing
        { get; set; }
        /// <summary>
        /// 排气温度
        /// </summary>
        public float[] TempPaiQi
        { get; set; }
        /// <summary>
        /// 电流
        /// </summary>
        public float[] CurA
        { get; set; }
        /// <summary>
        /// 电子
        /// </summary>
        public float[] DianZiPengZhangFa
        { get; set; }
        /// <summary>
        /// 频率
        /// </summary>
        public float PinLv
        { get; set; }
        /// <summary>
        /// 压机是否打开
        /// </summary>
        public bool[] YaJiStatue
        { get; set; }
        /// <summary>
        /// 电磁阀状态
        /// </summary>
        public bool[] SwitchStatue
        { get; set; }
        /// <summary>
        /// 读取故障
        /// </summary>
        public string ReadError
        { get; set; }
        /// <summary>
        /// OA读取电量
        /// </summary>
        public float OADianLiang
        { get; set; }
        /// <summary>
        /// 保护,以前的程序有显示,所以也加上
        /// </summary>
        public Dictionary<string, bool> Protect
        { get; set; }
        public enum Protects
        {
            压缩机过欠压保护,
            其他保护,
            压缩机顶部温度保护,
            排气高压保护,
            排气低压保护,
            压缩机电流保护,
            排气管温度保护
        }
        bool exit = false;
        Thread thRead;
        public K1K2(SerialPort com, AllMachine.Machines machine)
        {
            this.Com = com;
            this.Machine = machine;
            this.Conn = false;
            this.Error = "";
            this.TimeOut = 1000;

            TempLengNing = new float[2];
            TempPaiQi = new float[3];
            CurA = new float[3];
            DianZiPengZhangFa = new float[2];
            YaJiStatue = new bool[3];
            SwitchStatue = new bool[8];

            Protect = new Dictionary<string, bool>();
            Enum.GetNames(typeof(Protects)).ToList().ForEach(str =>
                {
                    Protect.Add(str, false);
                });
        }
        public K1K2(string com, AllMachine.Machines machine)
            : this(new SerialPort(com), machine)
        {
        }
        #region//默认的K1K2信号
        /// <summary>
        /// 获取默认的XY信号
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string GetSnCode(AllMachine.Modes mode)
        {
            byte[] buff = new byte[16];
            buff[0] = 0xAA;
            buff[2] = 0xFF;
            buff[3] = 0x00;
            buff[4] = 0x80;
            buff[5] = 0x00;
            buff[6] = 0x00;
            buff[7] = 0x00;
            buff[8] = 0x00;
            buff[9] = 0x00;
            buff[10] = 0x00;
            buff[11] = 0x00;
            buff[12] = 0x00;
            buff[13] = 0x00;
            buff[15] = 0x55;
            switch (mode)
            {
                case AllMachine.Modes.查询数据:
                    buff[1] = 0xC0;
                    buff[14] = 0xC1;
                    break;
                case AllMachine.Modes.查询电表:
                    buff[1] = 0xC1;
                    buff[14] = 0xC0;
                    break;
            }
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
                    case AllMachine.Machines.商用过冷机:
                        this.Com.BaudRate = 600;
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
                buff = All.Class.Num.Str2Hex(GetSnCode(AllMachine.Modes.查询数据));
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
                            #region//赋值
                            //模式
                            if ((buff[i + 8] & 1) == 1)
                            {
                                Step = "主变频模式";
                            }
                            if ((buff[i + 8] & 2) == 2)
                            {
                                Step = "混合模式";
                            }
                            //风速
                            if ((buff[i + 9] & 1) == 1)
                            {
                                Speed = "低风";
                            }
                            if ((buff[i + 9] & 2) == 2)
                            {
                                Speed = "中风";
                            }
                            if ((buff[i + 9] & 4) == 4)
                            {
                                Speed = "高风";
                            }
                            //温度
                            if (buff[i + 10] <= 0xF0)
                            {
                                TempHuangJin = buff[i + 10] / 2.0f - 20;
                            }
                            else
                            { TempHuangJin = -999; }
                            for (int j = 0; j < TempLengNing.Length; j++)
                            {
                                if (buff[i + 11 + j] <= 0xF0)
                                {
                                    TempLengNing[j] = buff[i + 11 + j] / 2.0f - 20;
                                }
                                else
                                { TempLengNing[j] = -999; }
                            }
                            //排气
                            for (int j = 0; j < TempPaiQi.Length; j++)
                            {
                                if (buff[i + 13 + j] <= 0xC8)
                                {
                                    TempPaiQi[j] = buff[i + 13 + j] - 20;
                                }
                                else
                                { TempPaiQi[j] = -999; }
                            }
                            //电流
                            for (int j = 0; j < CurA.Length; j++)
                            {
                                if (buff[i + 17 + j] <= 0xC8)
                                {
                                    CurA[j] = buff[i + 17 + j];
                                }
                                else
                                { CurA[j] = -999; }
                            }
                            //频率
                            if (buff[i + 20] <= 0xFA)
                            {
                                PinLv = buff[i + 20];
                            }
                            else
                            { PinLv = -999; }
                            //电子膨胀阀
                            for (int j = 0; j < DianZiPengZhangFa.Length; j++)
                            {
                                if (buff[i + 21 + j] < 0xFF)
                                {
                                    DianZiPengZhangFa[j] = buff[i + 21 + j] * 8;
                                }
                                else
                                { DianZiPengZhangFa[j] = -999; }
                            }
                            //开关
                            for (int j = 0; j < SwitchStatue.Length; j++)
                            {
                                if ((buff[i + 23] & (1 << j)) == (1 << j))
                                {
                                    SwitchStatue[j] = true;
                                }
                                else
                                {
                                    SwitchStatue[j] = false;
                                }
                            }
                            for (int j = 0; j < YaJiStatue.Length; j++)
                            {
                                if ((buff[i + 24] & (1 << j)) == (1 << j))
                                {
                                    YaJiStatue[j] = true;
                                }
                                else
                                {
                                    YaJiStatue[j] = false;
                                }
                            }
                            #region//故障
                            ReadError = "";
                            if ((buff[i + 25] & 128) == 128)
                            {
                                ReadError += "EF.其他故障                  \r\n";
                            }
                            if ((buff[i + 25] & 2) == 2)
                            {
                                ReadError += "E9.电压故障                  \r\n";
                            }
                            if ((buff[i + 26] & 1) == 1)
                            {
                                ReadError += "E0.室外机通讯故障            \r\n";
                            }
                            if ((buff[i + 26] & 2) == 2)
                            {
                                ReadError += "E1.相序错或缺相              \r\n";
                            }
                            if ((buff[i + 26] & 8) == 8)
                            {
                                ReadError += "E3.T3或T4或排气温度传感器故障\r\n";
                            }
                            if ((buff[i + 26] & 64) == 64)
                            {
                                ReadError += "E6.T6传感器故障              \r\n";
                            }
                            if ((buff[i + 27] & 2) == 2)
                            {
                                ReadError += "P9.压缩机过欠压保护          \r\n";
                                Protect[Protects.压缩机过欠压保护.ToString()] = true;
                            }
                            else
                            {
                                Protect[Protects.压缩机过欠压保护.ToString()] = false;
                            }
                            if ((buff[i + 27] & 128) == 128)
                            {
                                ReadError += "PF.其他保护                  \r\n";
                                Protect[Protects.其他保护.ToString()] = true;
                            }
                            else
                            {
                                Protect[Protects.其他保护.ToString()] = false;
                            }
                            if ((buff[i + 28] & 1) == 1)
                            {
                                ReadError += "P0.压缩机顶部温度保护        \r\n";
                                Protect[Protects.压缩机顶部温度保护.ToString()] = true;
                            }
                            else
                            {
                                Protect[Protects.压缩机顶部温度保护.ToString()] = false;
                            }
                            if ((buff[i + 28] & 2) == 2)
                            {
                                ReadError += "P1.排气高压保护              \r\n";
                                Protect[Protects.排气高压保护.ToString()] = true;
                            }
                            else
                            {
                                Protect[Protects.排气高压保护.ToString()] = false;
                            }
                            if ((buff[i + 28] & 4) == 4)
                            {
                                ReadError += "P2.排气低压保护              \r\n";
                                Protect[Protects.排气低压保护.ToString()] = true;
                            }
                            else
                            {

                                Protect[Protects.排气低压保护.ToString()] = false;
                            }
                            if ((buff[i + 28] & 8) == 8)
                            {
                                ReadError += "P3.压缩机电流保护            \r\n";
                                Protect[Protects.压缩机电流保护.ToString()] = true;
                            }
                            else
                            {
                                Protect[Protects.压缩机电流保护.ToString()] = false;
                            }
                            if ((buff[i + 28] & 16) == 16)
                            {
                                ReadError += "P4.排气管温度保护            \r\n";
                                Protect[Protects.排气管温度保护.ToString()] = true;
                            }
                            else
                            {
                                Protect[Protects.排气管温度保护.ToString()] = false;
                            }
                            if ((buff[i + 28] & 32) == 32)
                            {
                                ReadError += "P5.冷凝器高温保护            \r\n";
                            }
                            if ((buff[i + 28] & 64) == 64)
                            {
                                ReadError += "P6.变频模块保护              \r\n";
                            }
                            #endregion
                            #endregion
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
                check = false;

                buff = All.Class.Num.Str2Hex(GetSnCode(AllMachine.Modes.查询电表));
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
                        if (buff[i] == 0xAA && buff[i + 1] == 0xC1 && buff[i + 31] == 0x55)
                        {
                            check = true;
                            if (buff[i + 8] != 0xFF && buff[i + 9] != 0xFF && buff[i + 10] != 0xFF && buff[i + 11] != 0xFF)
                            {
                                OADianLiang = All.Class.Num.ToFloat(string.Format("{0:X}{1:X}{2:X}.{3:X}", buff[i + 11], buff[i + 10], buff[i + 9], buff[i + 8]));
                                break;
                            }
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
