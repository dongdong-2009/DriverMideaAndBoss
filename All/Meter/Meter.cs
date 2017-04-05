using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace All.Meter
{
    public abstract class Meter
    {
        //case All.Class.TypeUse.TypeList.Double:
        //  break;
        //case All.Class.TypeUse.TypeList.Byte:
        //  break;
        //case All.Class.TypeUse.TypeList.UShort:
        //  break;
        //case All.Class.TypeUse.TypeList.Int:
        //  break;
        //case All.Class.TypeUse.TypeList.String:
        //  break;
        //case All.Class.TypeUse.TypeList.Boolean:
        //  break;
        //case All.Class.TypeUse.TypeList.Float:
        //  break;

        //不要尝试在此类中写Lock参数（如Lcok住Write方法，Lock住Read方法）。
        //Only Lock独占文件的方法是不行的。很多事务标志，如自增，随机因子等握手参数必须要Lock住。
        /// <summary>
        /// 初始化参数
        /// </summary>
        public abstract Dictionary<string, string> InitParm
        { get; set; }
        public delegate void MeterConnChangeHandle(string text, bool conn);
        /// <summary>
        /// 仪表连接状态改变
        /// </summary>
        public event MeterConnChangeHandle MeterConnChange;
        /// <summary>
        /// 仪表名称
        /// </summary>
        public string Text
        { get; set; }

        /// <summary>
        /// 通讯父类
        /// </summary>
        public All.Communite.Communite Parent
        { get; set; }

        string error = "";
        /// <summary>
        /// 仪表故障名称
        /// </summary>
        public string Error
        {
            get { return error; }
            set
            {
                error = value;
                if (error != "")
                {
                    All.Class.Log.Add(error);
                }
            }
        }
        bool conn = true;
        /// <summary>
        /// 仪表连接状态
        /// </summary>
        public bool Conn
        {
            get { return conn; }
            set
            {
                if (conn != value)
                {
                    if (MeterConnChange != null)
                    {
                        MeterConnChange(Text, value);
                    }
                }
                conn = value;
            }
        }
        int timeOut = 1000;
        /// <summary>
        /// 仪表连接超时时间 
        /// </summary>
        public int TimeOut
        {
            get { return timeOut; }
            set { timeOut = value; }
        }
        int tmpErrorCount = 0;
        private int errorCount = 3;
        /// <summary>
        /// 允许出错次数
        /// </summary>
        public int ErrorCount
        {
            get { return errorCount; }
            set { errorCount = value; }
        }

        bool mostLog = true;
        /// <summary>
        /// 记录最详细的Log日志
        /// </summary>
        public bool MostLog
        {
            get { return mostLog; }
            set { mostLog = value; }
        }
        int clearMaxCount = 50;
        /// <summary>
        /// 当指定发送速度过快，而远程通讯失败时，发送的指令可能会累积，ClearMostCode为True时，会自动清除超过此值的累积发送数据。
        /// </summary>
        public int ClearMaxCount
        {
            get { return clearMaxCount; }
            set { clearMaxCount = value; }
        }
        bool clearMostCode = false;
        /// <summary>
        /// 当指定发送速度过快，而远程通讯失败时，发送的指令可能会累积，值为True时，会自动清除超过clearMaxCount的累积发送数据。
        /// </summary>
        public bool ClearMostCode
        {
            get { return clearMostCode; }
            set { clearMostCode = value; }
        }
        public Meter()
        {
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="InitBuff"></param>
        public abstract void Init(Dictionary<string, string> initParm);
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public abstract bool Read<T>(out List<T> value, int start, int end);
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public abstract bool WriteInternal<T>(List<T> value, int start, int end);
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public abstract bool Read<T>(out T value, int start);
        /// <summary>
        /// 读取非兼容格式数据
        /// </summary>
        /// <param name="init"></param>
        /// <returns></returns>
        public abstract bool Read<T>(out List<T> value, Dictionary<string, string> parm);
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public abstract bool WriteInternal<T>(T value, int start);
        /// <summary>
        /// 写入非兼容格式数据
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool WriteInternal<T>(List<T> value, Dictionary<string, string> parm);


        /// <summary>
        /// 直接将指定数据写入到缓冲区
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sendBuff"></param>
        /// <returns></returns>
        protected bool Write<T>(T sendBuff)
        {
            bool result = true;
            try
            {
                if (this.Parent == null)
                {
                    All.Class.Error.Add("当前设备没有指定的通讯类", Environment.StackTrace);
                    result = false;
                    return result;
                }
                if ((this.Parent is Communite.Udp || this.Parent is Communite.TcpClient)
                    && InitParm.ContainsKey("RemotHost") && InitParm.ContainsKey("RemotPort"))
                {
                    this.Parent.Send<T>(sendBuff, InitParm);
                }
                else
                {
                    this.Parent.Send<T>(sendBuff);
                }
            }
            catch (Exception e)
            {
                result = false;
                All.Class.Error.Add(e);
            }
            return result;
        }
        /// <summary>
        /// 直接从缓冲区读取指定长度数据
        /// </summary>
        /// <typeparam name="T">读取数据类型</typeparam>
        /// <param name="returnCount">读取数据长度，长度为0时，读取缓冲区所有数据</param>
        /// <param name="readBuff">读取后数据</param>
        /// <returns>读取状态</returns>
        protected bool Read<T>(int returnCount, out T readBuff)
        {
            bool result = true;
            readBuff = default(T);
            try
            {
                if (this.Parent == null)
                {
                    All.Class.Error.Add("当前设备没有指定的通讯类", Environment.StackTrace);
                    result = false;
                    return result;
                }
                if (!this.Parent.IsOpen)
                {
                    this.Parent.Open();
                }
                int startTime = Environment.TickCount;
                bool timeOut = false;
                bool getData = false;
                if (returnCount > 0)
                {
                    do
                    {
                        System.Threading.Thread.Sleep(50);
                        if ((Environment.TickCount - startTime) > TimeOut)
                        {
                            timeOut = true;
                        }
                        if (this.Parent.DataRecive >= returnCount)
                        {
                            getData = true;
                        }
                    }
                    while (!timeOut && !getData);
                }
                else
                {
                    if (this.Parent.DataRecive > 0)
                    {
                        returnCount = this.Parent.DataRecive;
                        do
                        {
                            System.Threading.Thread.Sleep(50);
                            if ((Environment.TickCount - startTime) > TimeOut)
                            {
                                timeOut = true;
                            }
                            if (this.Parent.DataRecive == returnCount)
                            {
                                getData = true;
                            }
                            else
                            {
                                returnCount = this.Parent.DataRecive;
                            }
                        }
                        while (!timeOut && !getData);
                    }
                    else
                    {
                        getData = true;
                    }
                }
                if (timeOut && !getData)//超时
                {
                    result = false;
                    if (mostLog)
                    {
                        All.Class.Log.Add(string.Format("{0}读取数据超时,要求长度:{1},实际长度:{2}", Text, returnCount, this.Parent.DataRecive));
                    }
                }
                else//读取数据OK
                {
                    if (returnCount > 0)//实际长度不为0，主要判断 读取条码枪时，长度为0，通讯也正常的情况
                    {
                        byte[] readTmpBuff;
                        this.Parent.Read<byte[]>(out readTmpBuff);
                        if (readTmpBuff.Length < returnCount)
                        {
                            if (mostLog)
                            {
                                All.Class.Log.Add(string.Format("{0}实际读取的参数和要求参数长度不一致,要求长度:{1},实际长度:{2}", Text, returnCount, readTmpBuff.Length));
                            }
                            result = false;
                        }
                        else
                        {
                            switch (Class.TypeUse.GetType<T>())
                            {
                                case All.Class.TypeUse.TypeList.String:
                                    readBuff = (T)(object)Encoding.UTF8.GetString(readTmpBuff);
                                    break;
                                case All.Class.TypeUse.TypeList.Bytes:
                                    readBuff = (T)(object)readTmpBuff;
                                    break;
                                default:
                                    All.Class.Error.Add(string.Format("读取的参数类型不正确,只能读取Byte[]或者String,当前读取类型为:{0}", typeof(T).ToString()));
                                    result = false;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        //条码等，没有数据时，直接返回，不进行连接的判断
                        switch (All.Class.TypeUse.GetType<T>())
                        {
                            case Class.TypeUse.TypeList.String:
                                readBuff = (T)(object)"";
                                break;
                            case Class.TypeUse.TypeList.Boolean:
                                readBuff = (T)(object)false;
                                break;
                            case Class.TypeUse.TypeList.Byte:
                            case Class.TypeUse.TypeList.Double:
                            case Class.TypeUse.TypeList.Float:
                            case Class.TypeUse.TypeList.Int:
                            case Class.TypeUse.TypeList.UShort:
                                readBuff = (T)(object)0;
                                break;
                        }
                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                All.Class.Error.Add(e);
            }
            if (result)
            {
                Conn = true;
                tmpErrorCount = 0;
            }
            else
            {
                tmpErrorCount++;
                if (errorCount < tmpErrorCount)
                {
                    Conn = false;
                    tmpErrorCount = errorCount;
                }
            }
            return result;
        }
        /// <summary>
        /// 写入数据,并延时等待数据返回
        /// </summary>
        /// <typeparam name="T">写入数据类型</typeparam>
        /// <typeparam name="U">读取数据类型</typeparam>
        /// <param name="sendBuff">写入数据</param>
        /// <param name="returnCount">等待返回数据长度</param>
        /// <param name="readBuff">读取数据</param>
        /// <returns></returns>
        protected bool Write<T, U>(T sendBuff, int returnCount, out U readBuff)
        {
            bool result = true;
            readBuff = default(U);
            try
            {
                if (this.Parent == null)
                {
                    All.Class.Error.Add("当前设备没有指定的通讯类", Environment.StackTrace);
                    result = false;
                    return result;
                }
                if (!this.Parent.IsOpen)
                {
                    this.Parent.Open();
                }
                if ((this.Parent is Communite.Udp || this.Parent is Communite.TcpClient)
                    && InitParm.ContainsKey("RemotHost") && InitParm.ContainsKey("RemotPort"))
                {
                    this.Parent.Send<T>(sendBuff, InitParm);
                }
                else
                {
                    this.Parent.Send<T>(sendBuff);
                }
                int startTime = Environment.TickCount;
                bool timeOut = false;
                bool getData = false;
                do
                {
                    if ((Environment.TickCount - startTime) > TimeOut)
                    {
                        timeOut = true;
                    }
                    if (this.Parent.DataRecive >= returnCount)
                    {
                        getData = true;
                    }
                    System.Threading.Thread.Sleep(50);
                }
                while (!timeOut && !getData);
                if (timeOut && !getData)
                {
                    result = false;
                    if (mostLog)
                    {
                        if (this.Parent.DataRecive > 0)
                        {
                            byte[] readTmpBuff;
                            this.Parent.Read<byte[]>(out readTmpBuff);
                            All.Class.Log.Add(All.Class.Num.Hex2Str(readTmpBuff));
                        }
                        All.Class.Log.Add(string.Format("{0}读取数据超时,要求长度:{1},实际长度:{2}", Text, returnCount, this.Parent.DataRecive));
                    }
                }
                else
                {
                    byte[] readTmpBuff;
                    this.Parent.Read<byte[]>(out readTmpBuff);
                    if (readTmpBuff.Length < returnCount)
                    {
                        if (mostLog)
                        {
                            All.Class.Log.Add(string.Format("{0}实际读取的参数和要求参数长度不一致,要求长度:{1},实际长度:{2}", Text, returnCount, readTmpBuff.Length));
                        }
                        result = false;
                    }
                    else
                    {
                        switch (All.Class.TypeUse.GetType<U>())
                        {
                            case All.Class.TypeUse.TypeList.String:
                                readBuff = (U)(object)Encoding.UTF8.GetString(readTmpBuff);
                                break;
                            case All.Class.TypeUse.TypeList.Bytes:
                                readBuff = (U)(object)readTmpBuff;
                                break;
                            default:
                                All.Class.Error.Add(string.Format("读取的参数类型不正确,只能读取Byte[]或者String,当前读取类型为:{0}", typeof(U).ToString()));
                                result = false;
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                All.Class.Error.Add(e);
            }
            if (result)
            {
                Conn = true;
                tmpErrorCount = 0;
            }
            else
            {
                tmpErrorCount++;
                if (errorCount < tmpErrorCount)
                {
                    Conn = false;
                    tmpErrorCount = errorCount;
                }
            }
            return result;
        }

        Queue<WriteInvoke> write = new Queue<WriteInvoke>();

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <param name="Start"></param>
        public void Write<T>(T Value, int Start)
        {
            WriteInvoke wi = new WriteInvoke(Value, Start, typeof(T));
            if (this.clearMostCode && write.Count >= clearMaxCount)
            {
                write.Clear();
            }
            write.Enqueue(wi);
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="Value">写入值</param>
        /// <param name="Start">写入起点</param>
        /// <param name="Len">写入长度</param>
        public void Write<T>(List<T> Value, int Start, int End)
        {
            List<object> tmpValue = new List<object>();
            for (int i = 0; i < Value.Count; i++)
            {
                tmpValue.Add((object)(Value[i]));
            }
            WriteInvoke wi = new WriteInvoke(tmpValue, Start, End, typeof(T));
            if (this.clearMostCode && write.Count >= clearMaxCount)
            {
                write.Clear();
            }
            write.Enqueue(wi);
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="Value"></param>
        public void Write<T>(List<T> Value, Dictionary<string, string> Parm)
        {
            List<object> tmpValue = new List<object>();
            for (int i = 0; i < Value.Count; i++)
            {
                tmpValue.Add((object)(Value[i]));
            }
            WriteInvoke wi = new WriteInvoke(tmpValue, Parm, typeof(T));
            if (this.clearMostCode && write.Count >= clearMaxCount)
            {
                write.Clear();
            }
            write.Enqueue(wi);
        }
        /// <summary>
        /// 将异步传输过来的数据写入操作,在多线程中一个一个的同步执行
        /// </summary>
        public void WriteOneByOne()
        {
            while (write.Count > 0)
            {
                if (write.Count > 40)
                {
                    All.Class.Error.Add(new string[] { "写入模块", "队列长度" }, new string[] { this.Text, write.Count.ToString() });
                    All.Class.Error.Add("当前队列过长，影响写入实时性，请检查程序");
                }
                WriteInvoke wi = write.Dequeue();
                if (wi == null)
                {
                    return;
                }
                if (wi.OtherValue != null)
                {
                    if (wi.OtherValue.T.ToString() == typeof(bool).ToString())
                    {
                        List<bool> tmpValue = new List<bool>();
                        for (int i = 0; i < wi.OtherValue.Value.Count; i++)
                        {
                            tmpValue.Add((bool)wi.OtherValue.Value[i]);
                        }
                        if (!WriteInternal<bool>(tmpValue, wi.OtherValue.Parm))
                        {
                            if (!WriteInternal<bool>(tmpValue, wi.OtherValue.Parm))
                            {
                                if (!WriteInternal<bool>(tmpValue, wi.OtherValue.Parm))
                                {
                                    All.Class.Log.Add<bool>(tmpValue, wi.OtherValue.Parm);
                                }
                            }
                        }
                    }
                    if (wi.OtherValue.T.ToString() == typeof(string).ToString())
                    {
                        List<string> tmpValue = new List<string>();
                        for (int i = 0; i < wi.OtherValue.Value.Count; i++)
                        {
                            tmpValue.Add((string)wi.OtherValue.Value[i]);
                        }
                        if (!WriteInternal<string>(tmpValue, wi.OtherValue.Parm))
                        {
                            if (!WriteInternal<string>(tmpValue, wi.OtherValue.Parm))
                            {
                                if (!WriteInternal<string>(tmpValue, wi.OtherValue.Parm))
                                {
                                    All.Class.Log.Add<string>(tmpValue, wi.OtherValue.Parm);
                                }
                            }
                        }
                    }
                    if (wi.OtherValue.T.ToString() == typeof(byte).ToString())
                    {
                        List<byte> tmpValue = new List<byte>();
                        for (int i = 0; i < wi.OtherValue.Value.Count; i++)
                        {
                            tmpValue.Add((byte)wi.OtherValue.Value[i]);
                        }
                        if (!WriteInternal<byte>(tmpValue, wi.OtherValue.Parm))
                        {
                            if (!WriteInternal<byte>(tmpValue, wi.OtherValue.Parm))
                            {
                                if (!WriteInternal<byte>(tmpValue, wi.OtherValue.Parm))
                                {
                                    All.Class.Log.Add<byte>(tmpValue, wi.OtherValue.Parm);
                                }
                            }
                        }
                    }
                    if (wi.OtherValue.T.ToString() == typeof(float).ToString())
                    {
                        List<float> tmpValue = new List<float>();
                        for (int i = 0; i < wi.OtherValue.Value.Count; i++)
                        {
                            tmpValue.Add((float)wi.OtherValue.Value[i]);
                        }
                        if (!WriteInternal<float>(tmpValue, wi.OtherValue.Parm))
                        {
                            if (!WriteInternal<float>(tmpValue, wi.OtherValue.Parm))
                            {
                                if (!WriteInternal<float>(tmpValue, wi.OtherValue.Parm))
                                {
                                    All.Class.Log.Add<float>(tmpValue, wi.OtherValue.Parm);
                                }
                            }
                        }
                    }
                    if (wi.OtherValue.T.ToString() == typeof(double).ToString())
                    {
                        List<double> tmpValue = new List<double>();
                        for (int i = 0; i < wi.OtherValue.Value.Count; i++)
                        {
                            tmpValue.Add((double)wi.OtherValue.Value[i]);
                        }
                        if (!WriteInternal<double>(tmpValue, wi.OtherValue.Parm))
                        {
                            if (!WriteInternal<double>(tmpValue, wi.OtherValue.Parm))
                            {
                                if (!WriteInternal<double>(tmpValue, wi.OtherValue.Parm))
                                {
                                    All.Class.Log.Add<double>(tmpValue, wi.OtherValue.Parm);
                                }
                            }
                        }
                    }
                    if (wi.OtherValue.T.ToString() == typeof(int).ToString())
                    {
                        List<int> tmpValue = new List<int>();
                        for (int i = 0; i < wi.OtherValue.Value.Count; i++)
                        {
                            tmpValue.Add((int)wi.OtherValue.Value[i]);
                        }
                        if (!WriteInternal<int>(tmpValue, wi.OtherValue.Parm))
                        {
                            if (!WriteInternal<int>(tmpValue, wi.OtherValue.Parm))
                            {
                                if (!WriteInternal<int>(tmpValue, wi.OtherValue.Parm))
                                {
                                    All.Class.Log.Add<int>(tmpValue, wi.OtherValue.Parm);
                                }
                            }
                        }
                    }
                    if (wi.OtherValue.T.ToString() == typeof(ushort).ToString())
                    {
                        List<ushort> tmpValue = new List<ushort>();
                        for (int i = 0; i < wi.OtherValue.Value.Count; i++)
                        {
                            tmpValue.Add((ushort)wi.OtherValue.Value[i]);
                        }
                        if (!WriteInternal<ushort>(tmpValue, wi.OtherValue.Parm))
                        {
                            if (!WriteInternal<ushort>(tmpValue, wi.OtherValue.Parm))
                            {
                                if (!WriteInternal<ushort>(tmpValue, wi.OtherValue.Parm))
                                {
                                    All.Class.Log.Add<ushort>(tmpValue, wi.OtherValue.Parm);
                                }
                            }
                        }
                    }
                }
                if (wi.ListValue != null)
                {
                    if (wi.ListValue.T.ToString() == typeof(bool).ToString())
                    {
                        List<bool> tmpValue = new List<bool>();
                        for (int i = 0; i < wi.ListValue.Value.Count; i++)
                        {
                            tmpValue.Add((bool)wi.ListValue.Value[i]);
                        }
                        if (!WriteInternal<bool>(tmpValue, wi.ListValue.Start, wi.ListValue.End))
                        {
                            if (!WriteInternal<bool>(tmpValue, wi.ListValue.Start, wi.ListValue.End))
                            {
                                if (!WriteInternal<bool>(tmpValue, wi.ListValue.Start, wi.ListValue.End))
                                {
                                    All.Class.Log.Add<bool>(tmpValue, wi.ListValue.Start, wi.ListValue.End);
                                }
                            }
                        }
                    }
                    if (wi.ListValue.T.ToString() == typeof(string).ToString())
                    {
                        List<string> tmpValue = new List<string>();
                        for (int i = 0; i < wi.ListValue.Value.Count; i++)
                        {
                            tmpValue.Add((string)wi.ListValue.Value[i]);
                        }
                        if (!WriteInternal<string>(tmpValue, wi.ListValue.Start, wi.ListValue.End))
                        {
                            if (!WriteInternal<string>(tmpValue, wi.ListValue.Start, wi.ListValue.End))
                            {
                                if (!WriteInternal<string>(tmpValue, wi.ListValue.Start, wi.ListValue.End))
                                {
                                    All.Class.Log.Add<string>(tmpValue, wi.ListValue.Start, wi.ListValue.End);
                                }
                            }
                        }
                    }
                    if (wi.ListValue.T.ToString() == typeof(byte).ToString())
                    {
                        List<byte> tmpValue = new List<byte>();
                        for (int i = 0; i < wi.ListValue.Value.Count; i++)
                        {
                            tmpValue.Add((byte)wi.ListValue.Value[i]);
                        }
                        if (!WriteInternal<byte>(tmpValue, wi.ListValue.Start, wi.ListValue.End))
                        {
                            if (!WriteInternal<byte>(tmpValue, wi.ListValue.Start, wi.ListValue.End))
                            {
                                if (!WriteInternal<byte>(tmpValue, wi.ListValue.Start, wi.ListValue.End))
                                {
                                    All.Class.Log.Add<byte>(tmpValue, wi.ListValue.Start, wi.ListValue.End);
                                }
                            }
                        }
                    }
                    if (wi.ListValue.T.ToString() == typeof(float).ToString())
                    {
                        List<float> tmpValue = new List<float>();
                        for (int i = 0; i < wi.ListValue.Value.Count; i++)
                        {
                            tmpValue.Add((float)wi.ListValue.Value[i]);
                        }
                        if (!WriteInternal<float>(tmpValue, wi.ListValue.Start, wi.ListValue.End))
                        {
                            if (!WriteInternal<float>(tmpValue, wi.ListValue.Start, wi.ListValue.End))
                            {
                                if (!WriteInternal<float>(tmpValue, wi.ListValue.Start, wi.ListValue.End))
                                {
                                    All.Class.Log.Add<float>(tmpValue, wi.ListValue.Start, wi.ListValue.End);
                                }
                            }
                        }
                    }
                    if (wi.ListValue.T.ToString() == typeof(double).ToString())
                    {
                        List<double> tmpValue = new List<double>();
                        for (int i = 0; i < wi.ListValue.Value.Count; i++)
                        {
                            tmpValue.Add((double)wi.ListValue.Value[i]);
                        }
                        if (!WriteInternal<double>(tmpValue, wi.ListValue.Start, wi.ListValue.End))
                        {
                            if (!WriteInternal<double>(tmpValue, wi.ListValue.Start, wi.ListValue.End))
                            {
                                if (!WriteInternal<double>(tmpValue, wi.ListValue.Start, wi.ListValue.End))
                                {
                                    All.Class.Log.Add<double>(tmpValue, wi.ListValue.Start, wi.ListValue.End);
                                }
                            }
                        }
                    }
                    if (wi.ListValue.T.ToString() == typeof(int).ToString())
                    {
                        List<int> tmpValue = new List<int>();
                        for (int i = 0; i < wi.ListValue.Value.Count; i++)
                        {
                            tmpValue.Add((int)wi.ListValue.Value[i]);
                        }
                        if (!WriteInternal<int>(tmpValue, wi.ListValue.Start, wi.ListValue.End))
                        {
                            if (!WriteInternal<int>(tmpValue, wi.ListValue.Start, wi.ListValue.End))
                            {
                                if (!WriteInternal<int>(tmpValue, wi.ListValue.Start, wi.ListValue.End))
                                {
                                    All.Class.Log.Add<int>(tmpValue, wi.ListValue.Start, wi.ListValue.End);
                                }
                            }
                        }
                    }
                    if (wi.ListValue.T.ToString() == typeof(ushort).ToString())
                    {
                        List<ushort> tmpValue = new List<ushort>();
                        for (int i = 0; i < wi.ListValue.Value.Count; i++)
                        {
                            tmpValue.Add((ushort)wi.ListValue.Value[i]);
                        }
                        if (!WriteInternal<ushort>(tmpValue, wi.ListValue.Start, wi.ListValue.End))
                        {
                            if (!WriteInternal<ushort>(tmpValue, wi.ListValue.Start, wi.ListValue.End))
                            {
                                if (!WriteInternal<ushort>(tmpValue, wi.ListValue.Start, wi.ListValue.End))
                                {
                                    All.Class.Log.Add<ushort>(tmpValue, wi.ListValue.Start, wi.ListValue.End);
                                }
                            }
                        }
                    }
                }
                if (wi.PointValue != null)
                {
                    if (wi.PointValue.T.ToString() == typeof(bool).ToString())
                    {
                        if (!WriteInternal<bool>((bool)wi.PointValue.Value, wi.PointValue.Start))
                        {
                            if (!WriteInternal<bool>((bool)wi.PointValue.Value, wi.PointValue.Start))
                            {
                                All.Class.Log.Add<bool>((bool)wi.PointValue.Value, wi.PointValue.Start);
                            }
                        }
                    }
                    if (wi.PointValue.T.ToString() == typeof(string).ToString())
                    {
                        if (!WriteInternal<string>((string)wi.PointValue.Value, wi.PointValue.Start))
                        {
                            if (!WriteInternal<string>((string)wi.PointValue.Value, wi.PointValue.Start))
                            {
                                if (!WriteInternal<string>((string)wi.PointValue.Value, wi.PointValue.Start))
                                {
                                    All.Class.Log.Add<string>((string)wi.PointValue.Value, wi.PointValue.Start);
                                }
                            }
                        }
                    }
                    if (wi.PointValue.T.ToString() == typeof(byte).ToString())
                    {
                        if (!WriteInternal<byte>((byte)wi.PointValue.Value, wi.PointValue.Start))
                        {
                            if (!WriteInternal<byte>((byte)wi.PointValue.Value, wi.PointValue.Start))
                            {
                                if (!WriteInternal<byte>((byte)wi.PointValue.Value, wi.PointValue.Start))
                                {
                                    All.Class.Log.Add<byte>((byte)wi.PointValue.Value, wi.PointValue.Start);
                                }
                            }
                        }
                    }
                    if (wi.PointValue.T.ToString() == typeof(float).ToString())
                    {
                        if (!WriteInternal<float>((float)wi.PointValue.Value, wi.PointValue.Start))
                        {
                            if (!WriteInternal<float>((float)wi.PointValue.Value, wi.PointValue.Start))
                            {
                                if (!WriteInternal<float>((float)wi.PointValue.Value, wi.PointValue.Start))
                                {
                                    All.Class.Log.Add<float>((float)wi.PointValue.Value, wi.PointValue.Start);
                                }
                            }
                        }
                    }
                    if (wi.PointValue.T.ToString() == typeof(double).ToString())
                    {
                        if (!WriteInternal<double>((double)wi.PointValue.Value, wi.PointValue.Start))
                        {
                            if (!WriteInternal<double>((double)wi.PointValue.Value, wi.PointValue.Start))
                            {
                                if (!WriteInternal<double>((double)wi.PointValue.Value, wi.PointValue.Start))
                                {
                                    All.Class.Log.Add<double>((double)wi.PointValue.Value, wi.PointValue.Start);
                                }
                            }
                        }
                    }
                    if (wi.PointValue.T.ToString() == typeof(int).ToString())
                    {
                        if (!WriteInternal<int>((int)wi.PointValue.Value, wi.PointValue.Start))
                        {
                            if (!WriteInternal<int>((int)wi.PointValue.Value, wi.PointValue.Start))
                            {
                                if (!WriteInternal<int>((int)wi.PointValue.Value, wi.PointValue.Start))
                                {
                                    All.Class.Log.Add<int>((int)wi.PointValue.Value, wi.PointValue.Start);
                                }
                            }
                        }
                    }
                    if (wi.PointValue.T.ToString() == typeof(ushort).ToString())
                    {
                        if (!WriteInternal<ushort>((ushort)wi.PointValue.Value, wi.PointValue.Start))
                        {
                            if (!WriteInternal<ushort>((ushort)wi.PointValue.Value, wi.PointValue.Start))
                            {
                                if (!WriteInternal<ushort>((ushort)wi.PointValue.Value, wi.PointValue.Start))
                                {
                                    All.Class.Log.Add<ushort>((ushort)wi.PointValue.Value, wi.PointValue.Start);
                                }
                            }
                        }
                    }
                }
                System.Threading.Thread.Sleep(10);
            }
        }
    }
}
