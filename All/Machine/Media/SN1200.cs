using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.IO.Ports;
namespace All.Machine.Media
{
    public class SN1200
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
                    result = "AA0000000300FD55";
                    break;
                case AllMachine.Modes.制热:
                    result = "AA0B02000300F055";
                    break;
                case AllMachine.Modes.制冷:
                    result = "AA0A01000300F255";
                    break;
                case AllMachine.Modes.送风:
                    break;
            }
            return result;
        }
    }
}
