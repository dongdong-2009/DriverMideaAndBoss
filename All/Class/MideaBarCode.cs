using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace All.Class
{
    public class MideaBarCode
    {
        /// <summary>
        /// 机型
        /// </summary>
        public enum MachineLists
        {
            外销,
            内销
        }
        /// <summary>
        /// 检查条码
        /// </summary>
        /// <param name="BarCode"></param>
        /// <returns></returns>
        private static bool CheckBar(string BarCode)
        {
            if (BarCode.Length != 22)
            {
                Log.Add(string.Format("当前转换的美的条码长度不正确，条码为:{0}", BarCode), Environment.StackTrace);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 从条码和订单号比对得出是否外销
        /// </summary>
        /// <param name="OrderName"></param>
        /// <param name="BarCode"></param>
        /// <returns></returns>
        public static MachineLists GetMachine(string OrderName, string BarCode)
        {
            MachineLists result = MachineLists.内销;
            if (!CheckBar(BarCode))
            {
                return result;
            }
            string tmp = BarCode.Substring(0, 8);
            if (OrderName.IndexOf(tmp) >= 0)
            {
                result = MachineLists.外销;
            }
            return result;
        }
        /// <summary>
        /// 直接判断是否外销
        /// </summary>
        /// <param name="BarCode"></param>
        /// <returns></returns>
        public static MachineLists GetMachine(string BarCode)
        {
            //内销:211270 0059G 6A23 090 0033
            //外销:01020304 0059G 6A23 90 033
            MachineLists result = MachineLists.内销;
            if (!CheckBar(BarCode))
            {
                return result;
            }
            string tmp = BarCode.Substring(12, 1);
            try
            {
                int month = Convert.ToInt16(tmp, 16);
                if (month <= 0 || month > 12)
                {
                    return MachineLists.外销;
                }
            }
            catch 
            { return MachineLists.外销; }

            tmp = BarCode.Substring(13, 2);
            try
            {
                int day = int.Parse(tmp);
                if (day <= 0 || day > 31)
                {
                    return MachineLists.外销;
                }
            }
            catch
            { return MachineLists.外销; }
            return result;
        }
        /// <summary>
        /// 外销条码转换
        /// </summary>
        /// <param name="WaiXiaoBarCode"></param>
        /// <param name="OrderName"></param>
        /// <returns></returns>
        public static string WaiXiaoBarChange(string WaiXiaoBarCode, string OrderName)
        {
            string result = "";
            if (!CheckBar(WaiXiaoBarCode) || OrderName.Length<8)
            {
                return "";
            }
            result = string.Format("240{0}{1}0{2}0{3}", OrderName.Substring(OrderName.Length - 8),
                WaiXiaoBarCode.Substring(13, 4), WaiXiaoBarCode.Substring(17, 2), WaiXiaoBarCode.Substring(19, 3));
            return result;
        }
        /// <summary>
        /// 从22位美的码中找到日期
        /// </summary>
        /// <param name="BarCode"></param>
        /// <returns></returns>
        public static DateTime GetTimeFromBar(string BarCode)
        {
            DateTime result = DateTime.Now;
            if (!CheckBar(BarCode))
            {
                return result;
            }
            int year = result.Year;
            string tmpYear = "0";
            string tmpMonth = "0";
            string tmpDay = "00";
            switch (GetMachine(BarCode))
            {
                case MachineLists.外销:
                    tmpYear = BarCode.Substring(13, 1).ToUpper();
                    tmpMonth = BarCode.Substring(14, 1).ToUpper();
                    tmpDay = BarCode.Substring(15, 2).ToUpper();
                    break;
                case MachineLists.内销:
                    tmpYear = BarCode.Substring(11, 1).ToUpper();
                    tmpMonth = BarCode.Substring(12, 1).ToUpper();
                    tmpDay = BarCode.Substring(13, 2).ToUpper();
                    break;
            }
            switch (tmpYear)
            {
                case "5"://2015
                case "6":
                case "7":
                case "8":
                case "9"://2019
                    year = Num.ToInt(tmpYear) + 2010;
                    break;
                case "0":
                case "1":
                case "2":
                case "3":
                case "4"://2024
                    year = Num.ToInt(tmpYear) + 2020;
                    break;
                case "A":
                    year = 2025;
                    break;
                case "B":
                    year = 2026;
                    break;
                case "C":
                    year = 2027;
                    break;
                case "D":
                    year = 2028;
                    break;
                case "F":
                    year = 2029;
                    break;
                case "G":
                    year = 2030;
                    break;
                case "H":
                    year = 2031;
                    break;
                case "J":
                    year = 2032;
                    break;
                case "K":
                    year = 2033;
                    break;
                case "L":
                    year = 2034;
                    break;
                case "M":
                    year = 2035;
                    break;
                case "N":
                    year = 2036;
                    break;
                case "P":
                    year = 2037;
                    break;
                case "Q":
                    year = 2038;
                    break;
                case "R":
                    year = 2039;
                    break;
                case "S":
                    year = 2040;
                    break;
                case "T":
                    year = 2041;
                    break;
                case "U":
                    year = 2042;
                    break;
                case "V":
                    year = 2043;
                    break;
                case "W":
                    year = 2044;
                    break;
                case "X":
                    year = 2045;
                    break;
                case "Y":
                    year = 2046;
                    break;
                case "Z"://2047
                    year = 2047;
                    break;
            }
            int month = result.Month;
            int day = result.Day;
            try
            {
                month = Convert.ToInt16(tmpMonth, 16);
                day = All.Class.Num.ToInt(tmpDay);
            }
            catch { }
            result = new DateTime(year, month, day);
            return result;
        }
        /// <summary>
        /// 从条码中读取流水号
        /// </summary>
        /// <param name="BarCode"></param>
        /// <returns></returns>
        public static int GetIndexFromBar(string BarCode)
        {
            string tmp = "";
            switch (GetMachine(BarCode))
            {
                case MachineLists.内销:
                    tmp = BarCode.Substring(18, 4);
                    break;
                case MachineLists.外销:
                    tmp = BarCode.Substring(19, 3);
                    break;
            }
            return All.Class.Num.ToInt(tmp);
        }
        /// <summary>
        /// 从条码中获取标准流水码前段
        /// </summary>
        /// <param name="BarCode"></param>
        /// <returns></returns>
        public static string GetPadFromBar(string BarCode)
        {
            string tmp = "";
            switch (GetMachine(BarCode))
            {
                case MachineLists.内销:
                    tmp = BarCode.Substring(0, 18).PadRight(22, '*');
                    break;
                case MachineLists.外销:
                    tmp = BarCode.Substring(0, 19).PadRight(22, '*');
                    break;
            }
            return tmp;
        }
        /// <summary>
        /// 从22位美的码中找到机型
        /// </summary>
        /// <param name="BarCode"></param>
        /// <returns></returns>
        public static string GetModeFromBar(string BarCode)
        {
            if (!CheckBar(BarCode))
            {
                return "";
            }
            string result = "";
            switch (GetMachine(BarCode))
            {
                case MachineLists.内销:
                    result = BarCode.Substring(6, 5);
                    break;
                case MachineLists.外销:
                    result = BarCode.Substring(8, 5);
                    break;
            }
            return result;
        }
        public static string GetMachineFromBar(string BarCode)
        {
            if (!CheckBar(BarCode))
            {
                return "";
            }
            switch (GetMachine(BarCode))
            {
                case MachineLists.内销:
                    switch (BarCode.Substring(15, 2))
                    {
                        case "00":
                            return "越南工厂";
                        case "01":
                            return "顺德家用";
                        case "02":
                            return "芜湖家用";
                        case "03":
                            return "武汉家用";
                        case "04":
                            return "顺德商用";
                        case "05":
                            return "重庆家用";
                        case "06":
                            return "重庆商用";
                        case "07":
                            return "邯郸家用";
                        case "08":
                            return "南沙空调";
                        case "09":
                            return "合肥商用";
                        case "10":
                            return "合肥冰箱";
                        case "11":
                            return "南沙冰箱";
                        case "12":
                            return "荆州冰箱";
                        case "13":
                            return "合肥洗衣机";
                        case "14":
                            return "无锡洗衣机";
                        case "15":
                            return "芜湖热水器";
                        case "16":
                            return "顺德饮水机";
                        case "17":
                            return "顺德洗涤器";
                        case "18":
                            return "芜湖洗涤器";
                        case "19":
                            return "顺德厨房电器";
                        case "20":
                            return "顺德厨房电器";
                        case "21":
                            return "芜湖厨房电器";
                        case "22":
                            return "苏州吸尘器";
                        case "23":
                            return "顺德热电";
                        case "24":
                            return "顺德水料";
                        case "25":
                            return "中山环电";
                        case "26":
                            return "中山环电";
                        case "27":
                            return "贵雅光源";
                        case "99":
                            return "OEM代工厂";
                    }
                    break;
                case MachineLists.外销:
                    break;
            }
            return "";
        }
    }
}
