using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;
namespace All.Class
{
    public static class Check
    {
        /// <summary>
        /// 正则表达式
        /// </summary>
        public enum RegularList
        {
            整数,
            正整数,
            负整数,
            非正整数,
            非负整数,
            浮点数,
            正浮点数,
            负浮点数,
            非正浮点数,
            非负浮点数,
            英文字母,
            小写英文字母,
            大写英文字母,
            数字和英文字母,
            IP地址,
            邮箱,
            输入中的浮点数,
            十六进制字符
        }
        /// <summary>
        /// 计算和校验
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="len"></param>
        /// <param name="value"></param>
        public static void SumCheck(byte[] buff, int len, out byte value)
        {
            byte hi = 0;
            SumCheck(buff, 0, len, out value, out hi);
        }
        public static byte SumCheck(byte[] buff, int start, int len)
        {
            byte value = 0,hi=0;
            SumCheck(buff, start, len, out value, out hi);
            return value;
        }
        public static byte SumCheck(byte[] buff, int len)
        {
            return SumCheck(buff, 0, len);
        }
        /// <summary>
        /// 计算CRC校验
        /// </summary>
        /// <param name="buff">指定要计算数据</param>
        /// <param name="len">计算数据长度</param>
        /// <param name="crcLo">返回CRC低字节</param>
        /// <param name="crcHi">返回CRC高字节</param>
        public static void Crc16(byte[] buff, int len, out byte crcLo, out byte crcHi)
        {
            crcLo = 0;
            crcHi = 0;
            if (len <= 0)
            {
                return;
            }
            int i, j;
            ushort maa = 0xFFFF;
            ushort mbb = 0;
            for (i = 0; i < len; i++)
            {
                crcHi = (byte)((maa >> 8) & 0xFF);
                crcLo = (byte)((maa) & 0xFF);
                maa = (ushort)((crcHi << 8) & 0xFF00);
                maa = (ushort)(maa + ((crcLo ^ buff[i]) & 0xFF));
                for (j = 0; j < 8; j++)
                {
                    mbb = 0;
                    mbb = (ushort)(maa & 0x1);
                    maa = (ushort)((maa >> 1) & 0x7FFF);
                    if (mbb != 0)
                    {
                        maa = (ushort)((maa ^ 0xA001) & 0xFFFF);
                    }
                }
            }
            crcLo = (byte)(maa & 0xFF);
            crcHi = (byte)((maa >> 8) & 0xFF);
        }
        /// <summary>
        /// 求校验和
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <param name="SumLow"></param>
        /// <param name="SumHigh"></param>
        public static void SumCheck(byte[] buff, int start, int len, out byte SumLow, out byte SumHigh)
        {
            SumLow = 0;
            SumHigh = 0;
            if ((start + len) > buff.Length)
            {
                throw new Exception("求校验和数组下标越界");
            }
            UInt32 tmpValue = 0;
            for (int i = start; i < start + len; i++)
            {
                tmpValue += buff[i];
            }
            SumLow = (byte)(tmpValue & 0xFF);
            SumHigh = (byte)((tmpValue >> 8) & 0xFF);
        }
        /// <summary>
        /// 判断当前str是否符合规则,判断用户输入是否准确
        /// </summary>
        /// <param name="str">string,判断的字符串</param>
        /// <param name="regualrList">RegularList,已知规则</param>
        /// <returns>bool,判断结果</returns>
        public static bool isFix(string str, RegularList regualrList)
        {
            Regex rg = new Regex(@"^-?\d+$");
            switch (regualrList)
            {
                case RegularList.十六进制字符:
                    rg = new Regex(@"^[A-Fa-f0-9]+$");
                    break;
                case RegularList.整数:
                    rg = new Regex(@"^-?\d+$");
                    break;
                case RegularList.正整数:
                    rg = new Regex(@"^[0-9]*[1-9][0-9]*$");
                    break;
                case RegularList.负整数:
                    rg = new Regex(@"^-[0-9]*[1-9][0-9]*$");
                    break;
                case RegularList.非正整数:
                    rg = new Regex(@"^((-\d+)|(0+))$");
                    break;
                case RegularList.非负整数:
                    rg = new Regex(@"^\d+$");
                    break;
                case RegularList.输入中的浮点数:
                    rg = new Regex(@"^(-?\d+)((\.\d+)|(\.))?$");
                    break;
                case RegularList.浮点数:
                    rg = new Regex(@"^(-?\d+)(\.\d+)?$");
                    break;
                case RegularList.正浮点数:
                    rg = new Regex(@"^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$");
                    break;
                case RegularList.负浮点数:
                    rg = new Regex(@"^(-(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*)))$");
                    break;
                case RegularList.非正浮点数:
                    rg = new Regex(@"^((-\d+(\.\d+)?)|(0+(\.0+)?))$");
                    break;
                case RegularList.非负浮点数:
                    rg = new Regex(@"^\d+(\.\d+)?$");
                    break;
                case RegularList.英文字母:
                    rg = new Regex(@"^[A-Za-z]+$");
                    break;
                case RegularList.大写英文字母:
                    rg = new Regex(@"^[A-Z]+$");
                    break;
                case RegularList.小写英文字母:
                    rg = new Regex(@"^[a-z]+$");
                    break;
                case RegularList.数字和英文字母:
                    rg = new Regex(@"^[A-Za-z0-9]+$");
                    break;
                case RegularList.IP地址:
                    rg = new Regex(@"^((?:(?:25[0-5]|2[0-4]\d|[01]?\d?\d)\.){3}(?:25[0-5]|2[0-4]\d|[01]?\d?\d))$");
                    break;
                case RegularList.邮箱:
                    rg = new Regex(@"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$");
                    break;
            }
            return rg.IsMatch(str);
        }
    }
}
