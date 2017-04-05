using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace All.Machine.Media
{
    public class AllMachine
    {
        /// <summary>
        /// 美的机器
        /// </summary>
        public enum Machines
        {
            定频机,
            家用旧变频,
            家用新变频,
            家用出口变频,
            家用UL,
            商用V4,
            商用V6,
            商用定变组合,
            商用两管制,
            商用三管制,
            商用过冷机,
            合用定频1200,
            合用定频1023
        }
        /// <summary>
        /// 工作模式
        /// </summary>
        public enum Modes
        {
            待机,
            制热,
            制冷,
            送风,
            查询数据,
            查询电表
        }
        public static void GetCheck(Machines machine, byte[] buff, out byte low)
        {
            byte high = 0;
            GetCheck(machine, buff, out low, out high);
        }
        public static void GetCheck(Machines machine, byte[] buff, out byte low, out byte high)
        {
            low = 0;
            high = 0;
            int sum = 0;
            int start = 0;
            switch (machine)
            {
                case Machines.家用旧变频:
                case Machines.家用出口变频:
                    if (buff[0] == 0xAA)
                    {
                        start = 1;
                    }
                    for (int i = start; i <= 14 + start && i<buff.Length; i++)
                    {
                        sum = sum + buff[i];
                    }
                    low = (byte)((((sum & 0xFF) ^ 0xFF) + 1) & 0xFF);
                    break;
                case Machines.合用定频1200:
                    if (buff[0] == 0xAA)
                    {
                        start = 1;
                    }
                    for (int i = start; i <= start + 4 && i<buff.Length; i++)
                    {
                        sum = sum + buff[i];
                    }
                    low = (byte)((((sum & 0xFF) ^ 0xFF) + 1) & 0xFF);
                    break;
                case Machines.商用V4:
                    if (buff[0] == 0xAA)
                    {
                        start = 1;
                    }
                    for (int i = start; i <= 6 + start && i < buff.Length; i++)
                    {
                        sum = sum + buff[i];
                    }
                    low = (byte)((((sum & 0xFF) ^ 0xFF) + 1) & 0xFF);
                    break;
                case Machines.商用两管制:
                case Machines.商用三管制:
                    if (buff[0] == 0xAA)
                    {
                        start = 1;
                    }
                    for (int i = start; i <= 28 + start && i < buff.Length; i++)
                    {
                        sum = sum + buff[i];
                    }
                    low = (byte)((((sum & 0xFF) ^ 0xFF) + 1) & 0xFF);
                    break;
            }
        }
        /// <summary>
        /// 获取默认信号
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string GetDefaultSn(Machines machine,Modes mode)
        {
            string result = "";
            switch (machine)
            {
                case Machines.家用旧变频:
                    break;
                case Machines.家用新变频:
                    break;
                case Machines.家用出口变频:
                    break;
                case Machines.家用UL:
                    break;
                case Machines.商用V4:
                    break;
                case Machines.商用V6:
                    break;
                case Machines.商用定变组合:
                    break;
                case Machines.商用两管制:
                    break;
                case Machines.商用三管制:
                    break;
                case Machines.商用过冷机:
                    break;
                case Machines.合用定频1200:
                    break;
                case Machines.合用定频1023:
                    break;
                case Machines.定频机:
                default:
                    break;
            }
            return result;
        }
    }
}
