using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO.Ports;
using System.Threading;
namespace All.Machine.Media
{
    public class SN600
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
        public static string GetSnCode(AllMachine.Machines machine, AllMachine.Modes mode)
        {
            string result = "";
            switch (mode)
            {
                case AllMachine.Modes.待机:
                    result = "AA0100000500F00000001E007D7D0F010E255";
                    break;
                case AllMachine.Modes.制热:
                    result = "AA0100000502F00300001E017D7D0F01DC55";
                    break;
                case AllMachine.Modes.制冷:
                    result = "AA0100000501F002000011017D7D0F01EB55";
                    break;
                case AllMachine.Modes.送风:
                    break;
            }
            return result;
        }
    }
}
