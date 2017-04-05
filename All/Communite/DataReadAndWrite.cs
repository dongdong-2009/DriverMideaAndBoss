using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
namespace All.Communite
{
    public class DataReadAndWrite
    {
        #region//所有通讯
        /// <summary>
        /// 所有通讯类集合
        /// </summary>
        public List<All.Communite.Communite> AllCommunite
        { get; set; }
        /// <summary>
        /// 通讯故障
        /// </summary>
        /// <param name="e">故障内容</param>
        public delegate void GetErrorHandle(Exception e);
        /// <summary>
        /// 通讯故障
        /// </summary>
        public event GetErrorHandle GetError;
        #endregion
        #region//所有数据
        /// <summary>
        /// 通讯参数
        /// </summary>
        private class CommunParm<T>
        {
            /// <summary>
            /// 下标号
            /// </summary>
            public List<int> Index
            { get; set; }
            /// <summary>
            /// 对齐长度
            /// </summary>
            public int Pad
            { get; set; }
            /// <summary>
            /// 是否将实时数据进行数据库保存
            /// </summary>
            bool SaveToData = false;

            /// <summary>
            /// 读取或写入数据数组
            /// </summary>
            AllData.Data<T> ReadAndWriteBuff = new AllData.Data<T>();
            /// <summary>
            /// 当前操作设备
            /// </summary>
            Meter.Meter curMeter;
            /// <summary>
            /// 参数
            /// </summary>
            Dictionary<string, string> parm;
            /// <summary>
            /// 保存数据临时数据连接
            /// </summary>
            All.Class.DataReadAndWrite sql;
            /// <summary>
            /// 初始化通讯参数
            /// </summary>
            /// <param name="Buff">读取后数据存放位置</param>
            /// <param name="curMeter">操作设备</param>
            /// <param name="Parm">数据参数</param>
            /// <param name="SQL">保存临时数据</param>
            public CommunParm(AllData.Data<T> Buff, Meter.Meter curMeter, Dictionary<string, string> Parm, All.Class.DataReadAndWrite SQL)
            {
                sql = SQL;
                SaveToData = true;
                ReadAndWriteBuff = Buff;
                this.curMeter = curMeter;
                parm = Parm;
                Index = new List<int>();
                Pad = 0;
                XmlToCommunite(parm);
            }
            /// <summary>
            /// 初始化通讯参数
            /// </summary>
            /// <param name="Buff">读取后数据存放位置</param>
            /// <param name="curMeter">操作设备</param>
            /// <param name="Parm">数据参数</param>
            public CommunParm(AllData.Data<T> Buff, Meter.Meter curMeter, Dictionary<string, string> Parm)
            {
                SaveToData = false;
                ReadAndWriteBuff = Buff;
                this.curMeter = curMeter;
                parm = Parm;
                Index = new List<int>();
                Pad = 0;
                XmlToCommunite(parm);
            }
            /// <summary>
            /// 设置是否保存数据
            /// </summary>
            /// <param name="SQL"></param>
            public void SetSaveSQL(All.Class.DataReadAndWrite SQL)
            {
                if (SQL == null)
                {
                    SaveToData = false;
                    sql = null;
                }
                else
                {
                    sql = SQL;
                    SaveToData = true;
                }
            }
            #region//将通讯的XML数据转化程序识别数据
            private void XmlToCommunite(Dictionary<string, string> buff)
            {
                if (buff.ContainsKey("Index"))
                {
                    string[] tmpIndex = All.Class.Num.ToString(buff["Index"]).Split(',');
                    foreach (string s in tmpIndex)
                    {
                        this.Index.Add(All.Class.Num.ToInt(s));
                    }
                }
                if (buff.ContainsKey("Part"))
                {
                    this.Pad = All.Class.Num.ToInt(buff["Part"]);
                }
            }
            public void Write()
            {
                curMeter.Write<T>(ReadAndWriteBuff.Value, parm);
            }
            /// <summary>
            /// 按解析后的读取格式,去读取一次数据
            /// </summary>
            public void Read()
            {
                List<T> tmpBuff = new List<T>();
                string ReadTableName = "";
                if(curMeter.Read<T>(out tmpBuff,this.parm))
                {
                    //此处可以加一个通用的数据处理流程,从文档加载数据换算方法.
                    if (typeof(T) == typeof(string) && Pad > 0 && tmpBuff.Count == 1)
                    {
                        string tmpValue = (string)(object)tmpBuff[0];
                        for (int i = 0, j = 0; i < tmpValue.Length && j < this.Index.Count; i = i + Pad, j++)
                        {
                            if (this.Index[j] >= ReadAndWriteBuff.Value.Count || (i + Pad) > tmpValue.Length)
                            {
                                Class.Error.Add(string.Format("数据下标大于数组长度,数据类型：{0}", typeof(T)), Environment.StackTrace);
                            }
                            else
                            {
                                ReadAndWriteBuff.Value[this.Index[j]] = (T)(object)tmpValue.Substring(i, Pad);
                                if (ReadAndWriteBuff.RaiseChangeEveryTime[this.Index[j]] || (ReadAndWriteBuff.InOldValue[this.Index[j]].ToString() != ReadAndWriteBuff.Value[this.Index[j]].ToString()))
                                {
                                    ReadAndWriteBuff.Change(this.Index[j]);
                                    ReadAndWriteBuff.OldValue[this.Index[j]] = ReadAndWriteBuff.InOldValue[this.Index[j]];
                                    ReadAndWriteBuff.InOldValue[this.Index[j]] = ReadAndWriteBuff.Value[this.Index[j]];
                                }
                            }
                        }
                        //}
                        //else//不切割数据，读到多少算多少
                        //{
                        //    for (int i = 0; i < tmpBuff.Count && i < this.Index.Count; i++)
                        //    {
                        //        ReadAndWriteBuff.Value[this.Index[i]] = (T)(object)tmpBuff[i];
                        //        if (ReadAndWriteBuff.RaiseChangeEveryTime[this.Index[i]] || (ReadAndWriteBuff.InOldValue[this.Index[i]].ToString() != ReadAndWriteBuff.Value[this.Index[i]].ToString()))
                        //        {
                        //            ReadAndWriteBuff.Change(this.Index[i]);
                        //            ReadAndWriteBuff.OldValue[this.Index[i]] = ReadAndWriteBuff.InOldValue[this.Index[i]];
                        //            ReadAndWriteBuff.InOldValue[this.Index[i]] = ReadAndWriteBuff.Value[this.Index[i]];
                        //        }

                        //    }
                        //}
                        // }
                    }
                    else
                    {
                        for (int i = 0; i < this.Index.Count; i++)
                        {
                            if (this.Index[i] >= ReadAndWriteBuff.Value.Count || i >= tmpBuff.Count)
                            {
                                Class.Error.Add(string.Format("数据下标大于数组长度,数据类型：{0}", typeof(T)), Environment.StackTrace);
                            }
                            else
                            {
                                ReadAndWriteBuff.Value[this.Index[i]] = (T)(object)tmpBuff[i];
                                if (ReadAndWriteBuff.RaiseChangeEveryTime[this.Index[i]] || (ReadAndWriteBuff.InOldValue[this.Index[i]].ToString() != ReadAndWriteBuff.Value[this.Index[i]].ToString()))
                                {
                                    ReadAndWriteBuff.Change(this.Index[i]);
                                    ReadAndWriteBuff.OldValue[this.Index[i]] = ReadAndWriteBuff.InOldValue[this.Index[i]];
                                    ReadAndWriteBuff.InOldValue[this.Index[i]] = ReadAndWriteBuff.Value[this.Index[i]];
                                }
                            }
                        }
                    }
                    //保存数据
                    if (SaveToData && tmpBuff != null)
                    {
                        for (int i = 0; i < this.Index.Count && i < tmpBuff.Count; i++)
                        {
                            //没有存数据表名称，则先获取表名称
                            if (ReadTableName == "")
                            {
                                if (typeof(T) == typeof(string))
                                {
                                    ReadTableName = "ReadString";
                                }
                                if (typeof(T) == typeof(int))
                                {
                                    ReadTableName = "ReadInt";
                                }
                                if (typeof(T) == typeof(float))
                                {
                                    ReadTableName = "ReadFloat";
                                }
                                if (typeof(T) == typeof(ushort))
                                {
                                    ReadTableName = "ReadUshort";
                                }
                                if (typeof(T) == typeof(double))
                                {
                                    ReadTableName = "ReadDouble";
                                }
                                if (typeof(T) == typeof(bool))
                                {
                                    ReadTableName = "ReadBool";
                                }
                                if (typeof(T) == typeof(byte))
                                {
                                    ReadTableName = "ReadByte";
                                }
                            }

                            if (typeof(T) == typeof(string) )
                            {
                                sql.Write(string.Format("update {0} set  Data='{1}' where Id={2}", ReadTableName, All.Class.Num.ToString(tmpBuff[i]).Trim().Replace("\0",""), this.Index[i]));
                            }
                            if (typeof(T) == typeof(double) || typeof(T) == typeof(float)
                                || typeof(T) == typeof(int) || typeof(T) == typeof(ushort)
                                || typeof(T) == typeof(byte)
                                || typeof(T) == typeof(bool))
                            {
                                sql.Write(string.Format("update {0} set  Data={1} where Id={2}", ReadTableName, tmpBuff[i], this.Index[i]));
                            }
                        }
                    }
                }
            }
            #endregion
        }
        /// <summary>
        /// 所有数据
        /// </summary>
        public class AllData
        {
            /// <summary>
            /// 单组数据
            /// </summary>
            /// <typeparam name="T"></typeparam>
            public class Data<T>
            {
                /// <summary>
                /// 1.实际数据 2.在计算数据个数时,当下标号用
                /// </summary>
                public List<T> Value
                { get; set; }
                /// <summary>
                /// 内部原始值
                /// </summary>
                internal List<T> InOldValue
                { get; set; }
                /// <summary>
                /// 数据名称
                /// </summary>
                public List<string> Info
                { get; set; }
                /// <summary>
                /// 原始值
                /// </summary>
                public List<T> OldValue
                { get; set; }
                /// <summary>
                /// 是否每次读取值后都触发。
                /// </summary>
                public List<bool> RaiseChangeEveryTime
                { get; set; }
                public delegate void ChangeValueHandle(T Value,T OldValue, string Info,int index);
                public event ChangeValueHandle ChangeValue;
                public Data()
                {
                    Value = new List<T>();
                    Info = new List<string>();
                    OldValue = new List<T>();
                    InOldValue = new List<T>();
                    RaiseChangeEveryTime = new List<bool>();
                }
                /// <summary>
                /// 值改变
                /// </summary>
                /// <param name="index"></param>
                public void Change(int index)
                {
                    if (ChangeValue != null)
                    {
                        ChangeValue(Value[index], InOldValue[index], Info[index], index);
                    }
                }
                /// <summary>
                /// 清空当前数据
                /// </summary>
                public void Clear()
                {
                    if (Value != null)
                    {
                        Value.Clear();
                    }
                    if (Info != null)
                    {
                        Info.Clear();
                    }
                    if (OldValue != null)
                    {
                        OldValue.Clear();
                    }
                    if (InOldValue != null)
                    {
                        InOldValue.Clear();
                    }
                    if (RaiseChangeEveryTime != null)
                    {
                        RaiseChangeEveryTime.Clear();
                    }
                }
                /// <summary>
                /// 添加一组数据,并赋默认值
                /// </summary>
                /// <param name="info"></param>
                public void Add(string info)
                {
                    switch (All.Class.TypeUse.GetType<T>())
                    {
                        case Class.TypeUse.TypeList.String:
                            Value.Add((T)(object)"");
                            OldValue.Add((T)(object)"");
                            InOldValue.Add((T)(object)"");
                            RaiseChangeEveryTime.Add(false);
                            break;
                        default:
                            Value.Add(default(T));
                            OldValue.Add(default(T));
                            InOldValue.Add(default(T));
                            RaiseChangeEveryTime.Add(false);
                            break;
                    }
                    Info.Add(info);
                }
            }
            public Data<byte> ByteValue
            { get; set; }
            public Data<string> StringValue
            { get; set; }
            public Data<int> IntValue
            { get; set; }
            public Data<ushort> UshortValue
            { get; set; }
            public Data<bool> BoolValue
            { get; set; }
            public Data<float> FloatValue
            { get; set; }
            public Data<double> DoubleValue
            { get; set; }
            public AllData()
            {
                ByteValue = new Data<byte>();
                StringValue = new Data<string>();
                IntValue = new Data<int>();
                UshortValue = new Data<ushort>();
                BoolValue = new Data<bool>();
                FloatValue = new Data<float>();
                DoubleValue = new Data<double>();
            }
        }
        #endregion
        #region//XML文档
        /// <summary>
        /// 设备信息
        /// </summary>
        public class MeterFile
        {
            /// <summary>
            /// 附加信息
            /// </summary>
            public Dictionary<string, string> Attribute
            { get; set; }
            /// <summary>
            /// 读取数据集合
            /// </summary>
            public List<Dictionary<string, string>> ReadAttribute
            { get; set; }
            /// <summary>
            /// 写入数据集合
            /// </summary>
            public List<Dictionary<string, string>> WriteAttribute
            { get; set; }
            public MeterFile()
            {
                Attribute = new Dictionary<string, string>();
                ReadAttribute = new List<Dictionary<string, string>>();
                WriteAttribute = new List<Dictionary<string, string>>();
            }
        }
        /// <summary>
        /// 通讯信息
        /// </summary>
        public class CommuniteFile
        {
            /// <summary>
            /// 附加信息
            /// </summary>
            public Dictionary<string, string> Attribute
            { get; set; }
            /// <summary>
            /// 所有设备集合
            /// </summary>
            public List<MeterFile> AllMeter
            { get; set; }
            public CommuniteFile()
            {
                Attribute = new Dictionary<string, string>();
                AllMeter = new List<MeterFile>();
            }
        }
        #endregion
        #region//初始化
        /// <summary>
        /// 所有通讯信息
        /// </summary>
        public List<CommuniteFile> AllCommuniteFile
        { get; set; }
        /// <summary>
        /// 所有读取数据
        /// </summary>
        public AllData AllReadValue
        { get; set; }
        /// <summary>
        /// 所有写入数据
        /// </summary>
        public AllData AllWriteValue
        { get; set; }
        /// <summary>
        /// 是否将实时数据进行数据库保存
        /// </summary>
        public bool SaveToAccess
        { get; set; }
        /// <summary>
        /// 是否将实时数据进行文本保存
        /// </summary>
        public bool SaveToFile
        { get; set; }
        /// <summary>
        /// 是否移除Text比Index数量少的错误
        /// </summary>
        public bool RemoveTextError
        { get; set; }
        /// <summary>
        /// 本地存放数据
        /// </summary>
        Class.DataReadAndWrite SQL;

        public delegate void ReadCommuniteOnceHandle(Communite communite, int communiteIndex);
        public delegate void ReadMeterOnceHandle(Communite communite, Meter.Meter meter, int communiteIndex);
        /// <summary>
        /// 整个通讯读取一次完毕
        /// </summary>
        public event ReadCommuniteOnceHandle ReadCommuniteOnce;
        /// <summary>
        /// 单个设备读取一次完毕
        /// </summary>
        public event ReadMeterOnceHandle ReadMeterOnce;

        Thread[] thReadAndWrite;
        public DataReadAndWrite()
        {
            AllCommuniteFile = new List<CommuniteFile>();
            AllReadValue = new AllData();
            AllWriteValue = new AllData();
            AllCommunite = new List<Communite>();
            SaveToAccess = true;
            SaveToFile = true;
            RemoveTextError = false;
        }
        ~DataReadAndWrite()
        {
            if (thReadAndWrite != null)
            {
                for (int i = 0; i < thReadAndWrite.Length; i++)
                {
                    thReadAndWrite[i].Abort();
                }
            }
            for (int i = 0; i < AllCommunite.Count; i++)
            {
                AllCommunite[i].Close();
            }
            if (SQL != null)
            {
                if (SQL.Conn.State == ConnectionState.Open)
                {
                    SQL.Conn.Close();
                    SQL.Conn.Dispose();
                }
            }
            AllCommunite.Clear();
            AllCommuniteFile.Clear();
        }
        /// <summary>
        /// 加载所有数据,从XML文档数据 反射到整个通讯类
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            int timeStart = Environment.TickCount;
            if (!LoadXmlFile())
            {
                return false;
            }
            All.Class.Log.Add(string.Format("LoadXmlFile时间      ->    {0}ms", Environment.TickCount - timeStart));
            timeStart = Environment.TickCount;
            if (!LoadValueCount())
            {
                return false;
            }
            All.Class.Log.Add(string.Format("LoadValueCount时间   ->    {0}ms", Environment.TickCount - timeStart));
            timeStart = Environment.TickCount;
            if (!LoadData())
            {
                return false;
            }
            All.Class.Log.Add(string.Format("LoadData时间         ->    {0}ms", Environment.TickCount - timeStart));
            timeStart = Environment.TickCount;
            if (!LoadCommunicate())
            {
                return false;
            }
            All.Class.Log.Add(string.Format("LoadCommunicate时间  ->    {0}ms", Environment.TickCount - timeStart));
            timeStart = Environment.TickCount;
            if (!LoadMeter())
            {
                return false;
            }
            All.Class.Log.Add(string.Format("LoadMeter时间        ->    {0}ms", Environment.TickCount - timeStart));
            timeStart = Environment.TickCount;
            return true ;
        }
        #endregion
        #region//多线程读写
        /// <summary>
        /// 开启多线程进行数据的读写
        /// </summary>
        public void Run()
        {
            if (AllCommunite.Count <= 0)
            {
                return;
            }
            thReadAndWrite = new Thread[AllCommunite.Count];
            for (int i = 0; i < thReadAndWrite.Length; i++)
            {
                int index = i;//不能删,直接用i会导致多线程数据传输错误
                thReadAndWrite[index] = new Thread(() => Loop(AllCommunite[index], AllCommuniteFile[index], index));
                thReadAndWrite[index].IsBackground = true;
                thReadAndWrite[index].Start();
            }
        }
        /// <summary>
        /// 多线程循环读写所有类型的数据
        /// </summary>
        /// <param name="communite">通讯类，以通讯类为单位进行多线程读写</param>
        /// <param name="communiteFile">参数类，当前通讯类的通讯参数</param>
        private void Loop(Communite communite, CommuniteFile communiteFile,int communiteIndex)
        {
            for (int i = 0; i < communite.Sons.Count; i++)
            {
                if (communiteFile.Attribute.ContainsKey("Use") &&
                    !All.Class.Num.ToBool(communiteFile.Attribute["Use"]))//不使用的，则直接返回，不参加读数
                {
                    return;
                }
                //if (communiteFile.Attribute.ContainsKey("Run") &&
                //    !All.Class.Num.ToBool(communiteFile.Attribute["Run"]))//只有没有初始化的才重新初始化
                //{
                    for (int j = 0; j < communiteFile.AllMeter.Count; j++)
                    {
                        communite.Sons[i].Init(communiteFile.AllMeter[j].Attribute);
                    }
                //}
            }
            int StartRead = Environment.TickCount;
            while (true)
            {
                for (int i = 0; i < communite.Sons.Count; i++)
                {
                    communite.Sons[i].WriteOneByOne();
                }
                if ((Environment.TickCount - StartRead) > communite.FlushTick)
                {
                    for (int i = 0; i < communite.Sons.Count; i++)
                    {
                        for (int j = 0; j < communiteFile.AllMeter[i].WriteAttribute.Count; j++)
                        {
                            communite.Sons[i].WriteOneByOne();
                            if (communiteFile.AllMeter[i].WriteAttribute[j].ContainsKey("Data"))
                            {
                                switch (communiteFile.AllMeter[i].WriteAttribute[j]["Data"].ToUpper())
                                {
                                    case "STRING":
                                        CommunParm<string> tmpString = new CommunParm<string>(AllWriteValue.StringValue, communite.Sons[i], communiteFile.AllMeter[i].WriteAttribute[j]);
                                        if (SaveToAccess)
                                        {
                                            tmpString.SetSaveSQL(SQL);
                                        }
                                        tmpString.Write();
                                        break;
                                    case "BOOL":
                                        CommunParm<bool> tmpBool = new CommunParm<bool>(AllWriteValue.BoolValue, communite.Sons[i], communiteFile.AllMeter[i].WriteAttribute[j]);
                                        if (SaveToAccess)
                                        {
                                            tmpBool.SetSaveSQL(SQL);
                                        }
                                        tmpBool.Write();
                                        break;
                                    case "BYTE":
                                        CommunParm<byte> tmpByte = new CommunParm<byte>(AllWriteValue.ByteValue, communite.Sons[i], communiteFile.AllMeter[i].WriteAttribute[j]);
                                        if (SaveToAccess)
                                        {
                                            tmpByte.SetSaveSQL(SQL);
                                        }
                                        tmpByte.Write();
                                        break;
                                    case "USHORT":
                                        CommunParm<ushort> tmpUshort = new CommunParm<ushort>(AllWriteValue.UshortValue, communite.Sons[i], communiteFile.AllMeter[i].WriteAttribute[j]);
                                        if (SaveToAccess)
                                        {
                                            tmpUshort.SetSaveSQL(SQL);
                                        }
                                        tmpUshort.Write();
                                        break;
                                    case "INT":
                                        CommunParm<int> tmpInt = new CommunParm<int>(AllWriteValue.IntValue, communite.Sons[i], communiteFile.AllMeter[i].WriteAttribute[j]);
                                        if (SaveToAccess)
                                        {
                                            tmpInt.SetSaveSQL(SQL);
                                        }
                                        tmpInt.Write();
                                        break;
                                    case "FLOAT":
                                        CommunParm<float> tmpFloat = new CommunParm<float>(AllWriteValue.FloatValue, communite.Sons[i], communiteFile.AllMeter[i].WriteAttribute[j]);
                                        if (SaveToAccess)
                                        {
                                            tmpFloat.SetSaveSQL(SQL);
                                        }
                                        tmpFloat.Write();
                                        break;
                                    case "DOUBLE":
                                        CommunParm<double> tmpDouble = new CommunParm<double>(AllWriteValue.DoubleValue, communite.Sons[i], communiteFile.AllMeter[i].WriteAttribute[j]);
                                        if (SaveToAccess)
                                        {
                                            tmpDouble.SetSaveSQL(SQL);
                                        }
                                        tmpDouble.Write();
                                        break;
                                }
                            }

                        }
                        for (int j = 0; j < communiteFile.AllMeter[i].ReadAttribute.Count; j++)
                        {
                            communite.Sons[i].WriteOneByOne();
                            if (communiteFile.AllMeter[i].ReadAttribute[j].ContainsKey("Data"))
                            {
                                switch (communiteFile.AllMeter[i].ReadAttribute[j]["Data"].ToUpper())
                                {
                                    case "STRING":
                                        CommunParm<string> tmpString = new CommunParm<string>(AllReadValue.StringValue, communite.Sons[i], communiteFile.AllMeter[i].ReadAttribute[j]);
                                        if (SaveToAccess)
                                        {
                                            tmpString.SetSaveSQL(SQL);
                                        }
                                        tmpString.Read();
                                        break;
                                    case "BOOL":
                                        CommunParm<bool> tmpBool = new CommunParm<bool>(AllReadValue.BoolValue, communite.Sons[i], communiteFile.AllMeter[i].ReadAttribute[j]);
                                        if (SaveToAccess)
                                        {
                                            tmpBool.SetSaveSQL(SQL);
                                        }
                                        tmpBool.Read();
                                        break;
                                    case "BYTE":
                                        CommunParm<byte> tmpByte = new CommunParm<byte>(AllReadValue.ByteValue, communite.Sons[i], communiteFile.AllMeter[i].ReadAttribute[j]);
                                        if (SaveToAccess)
                                        {
                                            tmpByte.SetSaveSQL(SQL);
                                        }
                                        tmpByte.Read();
                                        break;
                                    case "USHORT":
                                        CommunParm<ushort> tmpUshort = new CommunParm<ushort>(AllReadValue.UshortValue, communite.Sons[i], communiteFile.AllMeter[i].ReadAttribute[j]);
                                        if (SaveToAccess)
                                        {
                                            tmpUshort.SetSaveSQL(SQL);
                                        }
                                        tmpUshort.Read();
                                        break;
                                    case "INT":
                                        CommunParm<int> tmpInt = new CommunParm<int>(AllReadValue.IntValue, communite.Sons[i], communiteFile.AllMeter[i].ReadAttribute[j]);
                                        if (SaveToAccess)
                                        {
                                            tmpInt.SetSaveSQL(SQL);
                                        }
                                        tmpInt.Read();
                                        break;
                                    case "FLOAT":
                                        CommunParm<float> tmpFloat = new CommunParm<float>(AllReadValue.FloatValue, communite.Sons[i], communiteFile.AllMeter[i].ReadAttribute[j]);
                                        if (SaveToAccess)
                                        {
                                            tmpFloat.SetSaveSQL(SQL);
                                        }
                                        tmpFloat.Read();
                                        break;
                                    case "DOUBLE":
                                        CommunParm<double> tmpDouble = new CommunParm<double>(AllReadValue.DoubleValue, communite.Sons[i], communiteFile.AllMeter[i].ReadAttribute[j]);
                                        if (SaveToAccess)
                                        {
                                            tmpDouble.SetSaveSQL(SQL);
                                        }
                                        tmpDouble.Read();
                                        break;
                                }
                            }
                        }
                        if (ReadMeterOnce != null)//设备读取一次完成
                        {
                            ReadMeterOnce(communite, communite.Sons[i], communiteIndex);
                        }
                    }
                    StartRead = Environment.TickCount;
                    if (ReadCommuniteOnce != null)//通讯读取一次完成
                    {
                        ReadCommuniteOnce(communite, communiteIndex);
                    }
                }
                Thread.Sleep(50);
            }
        }
        #endregion
        #region//加载xml
        /// <summary>
        /// 加载通讯XML文档
        /// </summary>
        /// <returns></returns>
        private bool LoadXmlFile()
        {
            bool result = true;
            XmlNode tmpXml = Class.XmlHelp.GetXmlNode(string.Format("{0}\\Data\\MeterConnect.Mdb", Class.FileIO.GetNowPath()));
            if (tmpXml == null)
            {
                return false;
            }
            CommuniteFile tmpConnect;
            MeterFile tmpMeter;
            foreach (XmlNode tmpConnectNode in tmpXml.ChildNodes)//取所有Connect
            {
                if (tmpConnectNode.NodeType != XmlNodeType.Element)
                {
                    continue;
                }
                tmpConnect = new CommuniteFile();
                tmpConnect.Attribute = Class.XmlHelp.GetAttribute(tmpConnectNode);
                foreach (XmlNode tmpMeterNode in tmpConnectNode.ChildNodes)//取所有设备
                {
                    if (tmpMeterNode.NodeType != XmlNodeType.Element)
                    {
                        continue;
                    }
                    tmpMeter = new MeterFile();
                    tmpMeter.Attribute = Class.XmlHelp.GetAttribute(tmpMeterNode);
                    foreach (XmlNode tmpReadAndWriteNode in tmpMeterNode.ChildNodes)
                    {
                        if (tmpReadAndWriteNode.NodeType != XmlNodeType.Element)
                        {
                            continue;
                        }
                        switch (tmpReadAndWriteNode.Name)
                        {
                            case "Read":
                                foreach (XmlNode tmpValue in tmpReadAndWriteNode.ChildNodes)
                                {
                                    if (tmpValue.NodeType != XmlNodeType.Element)//排除其他辅助标签
                                    {
                                        continue;
                                    }
                                    tmpMeter.ReadAttribute.Add(Class.XmlHelp.GetInner(tmpValue));
                                }
                                break;
                            case "Write":
                                foreach (XmlNode tmpValue in tmpReadAndWriteNode.ChildNodes)
                                {
                                    if (tmpValue.NodeType != XmlNodeType.Element)
                                    {
                                        continue;
                                    }
                                    tmpMeter.WriteAttribute.Add(Class.XmlHelp.GetInner(tmpValue));
                                }
                                break;
                        }
                    }
                    tmpConnect.AllMeter.Add(tmpMeter);
                }
                AllCommuniteFile.Add(tmpConnect);
            }
            return result;
        }
        #endregion
        #region//加载数据
        /// <summary>
        /// 从加载的XML文档中分析出读,写数据的个数,并检查是否重复
        /// </summary>
        /// <returns></returns>
        private  bool LoadValueCount()
        {
            bool result = true;
            if (AllCommuniteFile.Count == 0)
            {
                All.Class.Error.Add("当前没有加载设置文档,不能加载历史数据",Environment.StackTrace);
                return false;
            }
            //数据计数
            AllData.Data<int> rInt = new AllData.Data<int>();
            AllData.Data<int> rString = new AllData.Data<int>();
            AllData.Data<int> rFloat = new AllData.Data<int>();
            AllData.Data<int> rDouble = new AllData.Data<int>();
            AllData.Data<int> rBool = new AllData.Data<int>();
            AllData.Data<int> rByte = new AllData.Data<int>();
            AllData.Data<int> rUshort = new AllData.Data<int>();

            AllData.Data<int> wInt = new AllData.Data<int>();
            AllData.Data<int> wString = new AllData.Data<int>();
            AllData.Data<int> wFloat = new AllData.Data<int>();
            AllData.Data<int> wDouble = new AllData.Data<int>();
            AllData.Data<int> wBool = new AllData.Data<int>();
            AllData.Data<int> wByte = new AllData.Data<int>();
            AllData.Data<int> wUshort = new AllData.Data<int>();

            
            int tmpIndex = 0;//数据下标
            string tmpInfo = "";//数据名称
            string tmpIndexStr = "";
            string tmpInfoStr = "";
            string[] tmpIndexStrBuff;
            string[] tmpInfoStrBuff;
            for (int i = 0; i < AllCommuniteFile.Count; i++)
            {
                for (int j = 0; j < AllCommuniteFile[i].AllMeter.Count; j++)
                {
                    for (int k = 0; k < AllCommuniteFile[i].AllMeter[j].WriteAttribute.Count; k++)
                    {
                        tmpIndexStr = AllCommuniteFile[i].AllMeter[j].WriteAttribute[k]["Index"];
                        tmpIndexStrBuff = tmpIndexStr.Split(',');
                        tmpInfoStr = AllCommuniteFile[i].AllMeter[j].WriteAttribute[k]["Text"];
                        tmpInfoStrBuff = tmpInfoStr.Split(',');
                        if (tmpIndexStrBuff.Length < 1)
                        {
                            continue;
                        }
                        for (int l = 0; l < tmpIndexStrBuff.Length; l++)
                        {
                            tmpIndex = Class.Num.ToInt(tmpIndexStrBuff[l]);
                            if (l < tmpInfoStrBuff.Length)
                            {
                                tmpInfo = tmpInfoStrBuff[l];
                            }
                            else
                            {
                                if (RemoveTextError)
                                {
                                    if (AllCommuniteFile[i].AllMeter[j].Attribute.ContainsKey("Text"))
                                    {
                                        All.Class.Error.Add(new string[] { "出错硬件" }, new string[] { AllCommuniteFile[i].AllMeter[j].Attribute["Text"] });
                                    }
                                    All.Class.Error.Add("Xml文件中Index的','数量和Text中的','数量不一致", Environment.StackTrace);//此处报错，是因为XML里面读取或写入的Text数量和Index里面的数量不匹配，关系不大，可以继续用
                                }
                            }
                            switch (AllCommuniteFile[i].AllMeter[j].WriteAttribute[k]["Data"].ToUpper())
                            {
                                case "STRING":
                                    if (wString.Value.Contains(tmpIndex))
                                    {
                                        Class.Error.Add(string.Format("写入数据序号重复,类型{0},序号{1},名称{2}",
                                            AllCommuniteFile[i].AllMeter[j].WriteAttribute[k]["Data"], tmpIndex,tmpInfo),Environment.StackTrace);
                                        continue;
                                    }
                                    if (wString.Value.Count < tmpIndex)//对于下标排序中间的空位,下标补-1
                                    {
                                        for (int m = wString.Value.Count; m < tmpIndex; m++)
                                        {
                                            wString.Value.Add(-1);
                                            wString.Info.Add("空位");
                                        }
                                        wString.Value.Add(tmpIndex);
                                        wString.Info.Add(tmpInfo);
                                    }
                                    else if (wString.Value.Count == tmpIndex)//对于下标按顺序来的话,直接在后面添加数据
                                    {
                                        wString.Value.Add(tmpIndex);
                                        wString.Info.Add(tmpInfo);
                                    }
                                    else//对于下标Count>tmpIndex,直接把中间的值改变就可以了
                                    {
                                        wString.Value[tmpIndex] = tmpIndex;
                                        wString.Info[tmpIndex] = tmpInfo;
                                    }
                                    break;
                                case "INT":
                                    if (wInt.Value.Contains(tmpIndex))
                                    {
                                        Class.Error.Add(string.Format("写入数据序号重复,类型{0},序号{1},名称{2}",
                                            AllCommuniteFile[i].AllMeter[j].WriteAttribute[k]["Data"], tmpIndex, tmpInfo), Environment.StackTrace);
                                        continue;
                                    }
                                    if (wInt.Value.Count < tmpIndex)//对于下标排序中间的空位,下标补-1
                                    {
                                        for (int m = wInt.Value.Count; m < tmpIndex; m++)
                                        {
                                            wInt.Value.Add(-1);
                                            wInt.Info.Add("空位");
                                        }
                                        wInt.Value.Add(tmpIndex);
                                        wInt.Info.Add(tmpInfo);
                                    }
                                    else if (wInt.Value.Count == tmpIndex)//对于下标按顺序来的话,直接在后面添加数据
                                    {
                                        wInt.Value.Add(tmpIndex);
                                        wInt.Info.Add(tmpInfo);
                                    }
                                    else//对于下标Count>tmpIndex,直接把中间的值改变就可以了
                                    {
                                        wInt.Value[tmpIndex] = tmpIndex;
                                        wInt.Info[tmpIndex] = tmpInfo;
                                    }
                                    break;
                                case "USHORT":
                                    if (wUshort.Value.Contains(tmpIndex))
                                    {
                                        Class.Error.Add(string.Format("写入数据序号重复,类型{0},序号{1},名称{2}",
                                            AllCommuniteFile[i].AllMeter[j].WriteAttribute[k]["Data"], tmpIndex, tmpInfo), Environment.StackTrace);
                                        continue;
                                    }
                                    if (wUshort.Value.Count < tmpIndex)//对于下标排序中间的空位,下标补-1
                                    {
                                        for (int m = wUshort.Value.Count; m < tmpIndex; m++)
                                        {
                                            wUshort.Value.Add(-1);
                                            wUshort.Info.Add("空位");
                                        }
                                        wUshort.Value.Add(tmpIndex);
                                        wUshort.Info.Add(tmpInfo);
                                    }
                                    else if (wUshort.Value.Count == tmpIndex)//对于下标按顺序来的话,直接在后面添加数据
                                    {
                                        wUshort.Value.Add(tmpIndex);
                                        wUshort.Info.Add(tmpInfo);
                                    }
                                    else//对于下标Count>tmpIndex,直接把中间的值改变就可以了
                                    {
                                        wUshort.Value[tmpIndex] = tmpIndex;
                                        wUshort.Info[tmpIndex] = tmpInfo;
                                    }
                                    break;
                                case "BYTE":
                                    if (wByte.Value.Contains(tmpIndex))
                                    {
                                        Class.Error.Add(string.Format("写入数据序号重复,类型{0},序号{1},名称{2}",
                                            AllCommuniteFile[i].AllMeter[j].WriteAttribute[k]["Data"], tmpIndex, tmpInfo), Environment.StackTrace);
                                        continue;
                                    }
                                    if (wByte.Value.Count < tmpIndex)//对于下标排序中间的空位,下标补-1
                                    {
                                        for (int m = wByte.Value.Count; m < tmpIndex; m++)
                                        {
                                            wByte.Value.Add(-1);
                                            wByte.Info.Add("空位");
                                        }
                                        wByte.Value.Add(tmpIndex);
                                        wByte.Info.Add(tmpInfo);
                                    }
                                    else if (wByte.Value.Count == tmpIndex)//对于下标按顺序来的话,直接在后面添加数据
                                    {
                                        wByte.Value.Add(tmpIndex);
                                        wByte.Info.Add(tmpInfo);
                                    }
                                    else//对于下标Count>tmpIndex,直接把中间的值改变就可以了
                                    {
                                        wByte.Value[tmpIndex] = tmpIndex;
                                        wByte.Info[tmpIndex] = tmpInfo;
                                    }
                                    break;
                                case "BOOL":
                                    if (wBool.Value.Contains(tmpIndex))
                                    {
                                        Class.Error.Add(string.Format("写入数据序号重复,类型{0},序号{1},名称{2}",
                                            AllCommuniteFile[i].AllMeter[j].WriteAttribute[k]["Data"], tmpIndex, tmpInfo), Environment.StackTrace);
                                        continue;
                                    }
                                    if (wBool.Value.Count < tmpIndex)//对于下标排序中间的空位,下标补-1
                                    {
                                        for (int m = wBool.Value.Count; m < tmpIndex; m++)
                                        {
                                            wBool.Value.Add(-1);
                                            wBool.Info.Add("空位");
                                        }
                                        wBool.Value.Add(tmpIndex);
                                        wBool.Info.Add(tmpInfo);
                                    }
                                    else if (wBool.Value.Count == tmpIndex)//对于下标按顺序来的话,直接在后面添加数据
                                    {
                                        wBool.Value.Add(tmpIndex);
                                        wBool.Info.Add(tmpInfo);
                                    }
                                    else//对于下标Count>tmpIndex,直接把中间的值改变就可以了
                                    {
                                        wBool.Value[tmpIndex] = tmpIndex;
                                        wBool.Info[tmpIndex] = tmpInfo;
                                    }
                                    break;
                                case "FLOAT":
                                    if (wFloat.Value.Contains(tmpIndex))
                                    {
                                        Class.Error.Add(string.Format("写入数据序号重复,类型{0},序号{1},名称{2}",
                                            AllCommuniteFile[i].AllMeter[j].WriteAttribute[k]["Data"], tmpIndex, tmpInfo), Environment.StackTrace);
                                        continue;
                                    }
                                    if (wFloat.Value.Count < tmpIndex)//对于下标排序中间的空位,下标补-1
                                    {
                                        for (int m = wFloat.Value.Count; m < tmpIndex; m++)
                                        {
                                            wFloat.Value.Add(-1);
                                            wFloat.Info.Add("空位");
                                        }
                                        wFloat.Value.Add(tmpIndex);
                                        wFloat.Info.Add(tmpInfo);
                                    }
                                    else if (wFloat.Value.Count == tmpIndex)//对于下标按顺序来的话,直接在后面添加数据
                                    {
                                        wFloat.Value.Add(tmpIndex);
                                        wFloat.Info.Add(tmpInfo);
                                    }
                                    else//对于下标Count>tmpIndex,直接把中间的值改变就可以了
                                    {
                                        wFloat.Value[tmpIndex] = tmpIndex;
                                        wFloat.Info[tmpIndex] = tmpInfo;
                                    }
                                    break;
                                case "DOUBLE":
                                    if (wDouble.Value.Contains(tmpIndex))
                                    {
                                        Class.Error.Add(string.Format("写入数据序号重复,类型{0},序号{1},名称{2}",
                                            AllCommuniteFile[i].AllMeter[j].WriteAttribute[k]["Data"], tmpIndex, tmpInfo), Environment.StackTrace);
                                        continue;
                                    }
                                    if (wDouble.Value.Count < tmpIndex)//对于下标排序中间的空位,下标补-1
                                    {
                                        for (int m = wDouble.Value.Count; m < tmpIndex; m++)
                                        {
                                            wDouble.Value.Add(-1);
                                            wDouble.Info.Add("空位");
                                        }
                                        wDouble.Value.Add(tmpIndex);
                                        wDouble.Info.Add(tmpInfo);
                                    }
                                    else if (wDouble.Value.Count == tmpIndex)//对于下标按顺序来的话,直接在后面添加数据
                                    {
                                        wDouble.Value.Add(tmpIndex);
                                        wDouble.Info.Add(tmpInfo);
                                    }
                                    else//对于下标Count>tmpIndex,直接把中间的值改变就可以了
                                    {
                                        wDouble.Value[tmpIndex] = tmpIndex;
                                        wDouble.Info[tmpIndex] = tmpInfo;
                                    }
                                    break;
                            }
                        }
                    }
                    for (int k = 0; k < AllCommuniteFile[i].AllMeter[j].ReadAttribute.Count; k++)
                    {
                        tmpIndexStr = AllCommuniteFile[i].AllMeter[j].ReadAttribute[k]["Index"];
                        tmpIndexStrBuff = tmpIndexStr.Split(',');
                        tmpInfoStr = AllCommuniteFile[i].AllMeter[j].ReadAttribute[k]["Text"];
                        tmpInfoStrBuff = tmpInfoStr.Split(',');
                        if (tmpIndexStrBuff.Length < 1)
                        {
                            continue;
                        }
                        for (int l = 0; l < tmpIndexStrBuff.Length; l++)
                        {
                            tmpIndex = Class.Num.ToInt(tmpIndexStrBuff[l]);
                            if (l < tmpInfoStrBuff.Length)
                            {
                                tmpInfo = tmpInfoStrBuff[l];
                            }
                            else
                            {
                                if (RemoveTextError)
                                {
                                    if (AllCommuniteFile[i].AllMeter[j].Attribute.ContainsKey("Text"))
                                    {
                                        All.Class.Error.Add(new string[] { "出错硬件" }, new string[] { AllCommuniteFile[i].AllMeter[j].Attribute["Text"] });
                                    }
                                    All.Class.Error.Add("Xml文件中Index的','数量和Text中的','数量不一致", Environment.StackTrace);//此处报错，是因为XML里面读取或写入的Text数量和Index里面的数量不匹配，关系不大，可以继续用
                                }
                            }
                            
                            switch (AllCommuniteFile[i].AllMeter[j].ReadAttribute[k]["Data"].ToUpper())
                            {
                                case "STRING":
                                    if (rString.Value.Contains(tmpIndex))
                                    {
                                        Class.Error.Add(string.Format("读取数据序号重复,类型{0},序号{1},名称{2}",
                                            AllCommuniteFile[i].AllMeter[j].ReadAttribute[k]["Data"], tmpIndex, tmpInfo), Environment.StackTrace);
                                        continue;
                                    }
                                    if (rString.Value.Count < tmpIndex)//对于下标排序中间的空位,下标补-1
                                    {
                                        for (int m = rString.Value.Count; m < tmpIndex; m++)
                                        {
                                            rString.Value.Add(-1);
                                            rString.Info.Add("空位");
                                        }
                                        rString.Value.Add(tmpIndex);
                                        rString.Info.Add(tmpInfo);
                                    }
                                    else if (rString.Value.Count == tmpIndex)//对于下标按顺序来的话,直接在后面添加数据
                                    {
                                        rString.Value.Add(tmpIndex);
                                        rString.Info.Add(tmpInfo);
                                    }
                                    else//对于下标Count>tmpIndex,直接把中间的值改变就可以了
                                    {
                                        rString.Value[tmpIndex] = tmpIndex;
                                        rString.Info[tmpIndex] = tmpInfo;
                                    }
                                    break;
                                case "INT":
                                    if (rInt.Value.Contains(tmpIndex))
                                    {
                                        Class.Error.Add(string.Format("读取数据序号重复,类型{0},序号{1}",
                                            AllCommuniteFile[i].AllMeter[j].ReadAttribute[k]["Data"], tmpIndex, tmpInfo), Environment.StackTrace);
                                        continue;
                                    }
                                    if (rInt.Value.Count < tmpIndex)//对于下标排序中间的空位,下标补-1
                                    {
                                        for (int m = rInt.Value.Count; m < tmpIndex; m++)
                                        {
                                            rInt.Value.Add(-1);
                                            rInt.Info.Add("空位");
                                        }
                                        rInt.Value.Add(tmpIndex);
                                        rInt.Info.Add(tmpInfo);
                                    }
                                    else if (rInt.Value.Count == tmpIndex)//对于下标按顺序来的话,直接在后面添加数据
                                    {
                                        rInt.Value.Add(tmpIndex);
                                        rInt.Info.Add(tmpInfo);
                                    }
                                    else//对于下标Count>tmpIndex,直接把中间的值改变就可以了
                                    {
                                        rInt.Value[tmpIndex] = tmpIndex;
                                        rInt.Info[tmpIndex] = tmpInfo;
                                    }
                                    break;
                                case "USHORT":
                                    if (rUshort.Value.Contains(tmpIndex))
                                    {
                                        Class.Error.Add(string.Format("读取数据序号重复,类型{0},序号{1},名称{2}",
                                            AllCommuniteFile[i].AllMeter[j].ReadAttribute[k]["Data"], tmpIndex, tmpInfo), Environment.StackTrace);
                                        continue;
                                    }
                                    if (rUshort.Value.Count < tmpIndex)//对于下标排序中间的空位,下标补-1
                                    {
                                        for (int m = rUshort.Value.Count; m < tmpIndex; m++)
                                        {
                                            rUshort.Value.Add(-1);
                                            rUshort.Info.Add("空位");
                                        }
                                        rUshort.Value.Add(tmpIndex);
                                        rUshort.Info.Add(tmpInfo);
                                    }
                                    else if (rUshort.Value.Count == tmpIndex)//对于下标按顺序来的话,直接在后面添加数据
                                    {
                                        rUshort.Value.Add(tmpIndex);
                                        rUshort.Info.Add(tmpInfo);
                                    }
                                    else//对于下标Count>tmpIndex,直接把中间的值改变就可以了
                                    {
                                        rUshort.Value[tmpIndex] = tmpIndex;
                                        rUshort.Info[tmpIndex] = tmpInfo;
                                    }
                                    break;
                                case "BYTE":
                                    if (rByte.Value.Contains(tmpIndex))
                                    {
                                        Class.Error.Add(string.Format("读取数据序号重复,类型{0},序号{1},名称{2}",
                                            AllCommuniteFile[i].AllMeter[j].ReadAttribute[k]["Data"], tmpIndex, tmpInfo), Environment.StackTrace);
                                        continue;
                                    }
                                    if (rByte.Value.Count < tmpIndex)//对于下标排序中间的空位,下标补-1
                                    {
                                        for (int m = rByte.Value.Count; m < tmpIndex; m++)
                                        {
                                            rByte.Value.Add(-1);
                                            rByte.Info.Add("空位");
                                        }
                                        rByte.Value.Add(tmpIndex);
                                        rByte.Info.Add(tmpInfo);
                                    }
                                    else if (rByte.Value.Count == tmpIndex)//对于下标按顺序来的话,直接在后面添加数据
                                    {
                                        rByte.Value.Add(tmpIndex);
                                        rByte.Info.Add(tmpInfo);
                                    }
                                    else//对于下标Count>tmpIndex,直接把中间的值改变就可以了
                                    {
                                        rByte.Value[tmpIndex] = tmpIndex;
                                        rByte.Info[tmpIndex] = tmpInfo;
                                    }
                                    break;
                                case "BOOL":
                                    if (rBool.Value.Contains(tmpIndex))
                                    {
                                        Class.Error.Add(string.Format("读取数据序号重复,类型{0},序号{1},名称{2}",
                                            AllCommuniteFile[i].AllMeter[j].ReadAttribute[k]["Data"], tmpIndex, tmpInfo), Environment.StackTrace);
                                        continue;
                                    }
                                    if (rBool.Value.Count < tmpIndex)//对于下标排序中间的空位,下标补-1
                                    {
                                        for (int m = rBool.Value.Count; m < tmpIndex; m++)
                                        {
                                            rBool.Value.Add(-1);
                                            rBool.Info.Add("空位");
                                        }
                                        rBool.Value.Add(tmpIndex);
                                        rBool.Info.Add(tmpInfo);
                                    }
                                    else if (rBool.Value.Count == tmpIndex)//对于下标按顺序来的话,直接在后面添加数据
                                    {
                                        rBool.Value.Add(tmpIndex);
                                        rBool.Info.Add(tmpInfo);
                                    }
                                    else//对于下标Count>tmpIndex,直接把中间的值改变就可以了
                                    {
                                        rBool.Value[tmpIndex] = tmpIndex;
                                        rBool.Info[tmpIndex] = tmpInfo;
                                    }
                                    break;
                                case "FLOAT":
                                    if (rFloat.Value.Contains(tmpIndex))
                                    {
                                        Class.Error.Add(string.Format("读取数据序号重复,类型{0},序号{1},名称{2}",
                                            AllCommuniteFile[i].AllMeter[j].ReadAttribute[k]["Data"], tmpIndex, tmpInfo), Environment.StackTrace);
                                        continue;
                                    }
                                    if (rFloat.Value.Count < tmpIndex)//对于下标排序中间的空位,下标补-1
                                    {
                                        for (int m = rFloat.Value.Count; m < tmpIndex; m++)
                                        {
                                            rFloat.Value.Add(-1);
                                            rFloat.Info.Add("空位");
                                        }
                                        rFloat.Value.Add(tmpIndex);
                                        rFloat.Info.Add(tmpInfo);
                                    }
                                    else if (rFloat.Value.Count == tmpIndex)//对于下标按顺序来的话,直接在后面添加数据
                                    {
                                        rFloat.Value.Add(tmpIndex);
                                        rFloat.Info.Add(tmpInfo);
                                    }
                                    else//对于下标Count>tmpIndex,直接把中间的值改变就可以了
                                    {
                                        rFloat.Value[tmpIndex] = tmpIndex;
                                        rFloat.Info[tmpIndex] = tmpInfo;
                                    }
                                    break;
                                case "DOUBLE":
                                    if (rDouble.Value.Contains(tmpIndex))
                                    {
                                        Class.Error.Add(string.Format("读取数据序号重复,类型{0},序号{1},名称{2}",
                                            AllCommuniteFile[i].AllMeter[j].ReadAttribute[k]["Data"], tmpIndex, tmpInfo), Environment.StackTrace);
                                        continue;
                                    }
                                    if (rDouble.Value.Count < tmpIndex)//对于下标排序中间的空位,下标补-1
                                    {
                                        for (int m = rDouble.Value.Count; m < tmpIndex; m++)
                                        {
                                            rDouble.Value.Add(-1);
                                            rDouble.Info.Add("空位");
                                        }
                                        rDouble.Value.Add(tmpIndex);
                                        rDouble.Info.Add(tmpInfo);
                                    }
                                    else if (rDouble.Value.Count == tmpIndex)//对于下标按顺序来的话,直接在后面添加数据
                                    {
                                        rDouble.Value.Add(tmpIndex);
                                        rDouble.Info.Add(tmpInfo);
                                    }
                                    else//对于下标Count>tmpIndex,直接把中间的值改变就可以了
                                    {
                                        rDouble.Value[tmpIndex] = tmpIndex;
                                        rDouble.Info[tmpIndex] = tmpInfo;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            //加载读取数据个数,并写入默认值
            AllReadValue.BoolValue.Clear();
            for (int i = 0; i < rBool.Value.Count; i++)
            {
                AllReadValue.BoolValue.Add(rBool.Info[i]);
            }
            AllReadValue.FloatValue.Clear();
            for (int i = 0; i < rFloat.Value.Count; i++)
            {
                AllReadValue.FloatValue.Add(rFloat.Info[i]);
            }
            AllReadValue.DoubleValue.Clear();
            for (int i = 0; i < rDouble.Value.Count; i++)
            {
                AllReadValue.DoubleValue.Add(rDouble.Info[i]);
            }
            AllReadValue.ByteValue.Clear();
            for (int i = 0; i < rByte.Value.Count; i++)
            {
                AllReadValue.ByteValue.Add(rByte.Info[i]);
            }
            AllReadValue.IntValue.Clear();
            for (int i = 0; i < rInt.Value.Count; i++)
            {
                AllReadValue.IntValue.Add(rInt.Info[i]);
            }
            AllReadValue.UshortValue.Clear();
            for (int i = 0; i < rUshort.Value.Count; i++)
            {
                AllReadValue.UshortValue.Add(rUshort.Info[i]);
            }
            AllReadValue.StringValue.Clear();
            for (int i = 0; i < rString.Value.Count; i++)
            {
                AllReadValue.StringValue.Add(rString.Info[i]);
            }

            //加载写入数据 个数并写入默认值 
            AllWriteValue.BoolValue.Clear();
            for (int i = 0; i < wBool.Value.Count; i++)
            {
                AllWriteValue.BoolValue.Add(wBool.Info[i]);
            }
            AllWriteValue.FloatValue.Clear();
            for (int i = 0; i < wFloat.Value.Count; i++)
            {
                AllWriteValue.FloatValue.Add(wFloat.Info[i]);
            }
            AllWriteValue.DoubleValue.Clear();
            for (int i = 0; i < wDouble.Value.Count; i++)
            {
                AllWriteValue.DoubleValue.Add(wDouble.Info[i]);
            }
            AllWriteValue.ByteValue.Clear();
            for (int i = 0; i < wByte.Value.Count; i++)
            {
                AllWriteValue.ByteValue.Add(wByte.Info[i]);
            }
            AllWriteValue.IntValue.Clear();
            for (int i = 0; i < wInt.Value.Count; i++)
            {
                AllWriteValue.IntValue.Add(wInt.Info[i]);
            }
            AllWriteValue.UshortValue.Clear();
            for (int i = 0; i < wUshort.Value.Count; i++)
            {
                AllWriteValue.UshortValue.Add(wUshort.Info[i]);
            }
            AllWriteValue.StringValue.Clear();
            for (int i = 0; i < wString.Value.Count; i++)
            {
                AllWriteValue.StringValue.Add(wString.Info[i]);
            }
            return result;
        }
        /// <summary>
        /// 从数据库加载上次存放数据
        /// </summary>
        /// <returns></returns>
        private bool LoadData()
        {
            bool result = true;
            if (!SaveToAccess)
            {
                return result;
            }
            //数据库比对
            XmlNode tmpNode = All.Class.XmlHelp.GetXmlNode(string.Format("{0}\\Data\\DataConnect.Mdb", All.Class.FileIO.GetNowPath()));
            foreach (XmlNode tmpConn in tmpNode.ChildNodes)
            {
                if (tmpConn.NodeType != XmlNodeType.Element)
                {
                    continue;
                }
                Dictionary<string, string> connAttribute = All.Class.XmlHelp.GetAttribute(tmpConn);
                if (connAttribute.ContainsKey("Name") && connAttribute["Name"] == "ReadAndWriteCommunicateData")
                {
                    try
                    {
                        Dictionary<string, string> connStr = Class.XmlHelp.GetInner(tmpConn);

                        Class.Reflex<Class.DataReadAndWrite> r = new Class.Reflex<Class.DataReadAndWrite>("All", connAttribute["Class"]);
                        SQL = (Class.DataReadAndWrite)r.Get();
                        if (!SQL.Login(connStr["Address"], connStr["DataBase"], connStr["UserName"], connStr["Password"]))
                        {
                            result = false;
                        }
                        else
                        {
                            DataTable dt = new DataTable();
                            string[] ReadTableName = new string[] { "ReadBool", "ReadByte", "ReadDouble", "ReadFloat", "ReadInt", "ReadString", "ReadUshort" };
                            string[] WriteTableName = new string[] { "WriteBool", "WriteByte", "WriteDouble", "WriteFloat", "WriteInt", "WriteString", "WriteUshort" };
                            for (int i = 0; i < WriteTableName.Length; i++)
                            {
                                dt = SQL.Read(string.Format("Select Id,Data From {0} order by Id", WriteTableName[i]));
                                switch (WriteTableName[i])
                                {
                                    case "WriteBool":
                                        if (dt.Rows.Count != AllWriteValue.BoolValue.Value.Count)
                                        {
                                            SQL.Write(string.Format("delete from {0}", WriteTableName[i]));
                                            for (int j = 0; j < AllWriteValue.BoolValue.Value.Count; j++)
                                            {
                                                SQL.Write(string.Format("insert into {0} Values ({1},{2},'{3}')", WriteTableName[i], j, AllWriteValue.BoolValue.Value[j], AllWriteValue.BoolValue.Info[j]));
                                            }
                                        }
                                        else
                                        {
                                            for (int j = 0; j < AllWriteValue.BoolValue.Value.Count; j++)
                                            {
                                                AllWriteValue.BoolValue.Value[j] = Class.Num.ToBool(dt.Rows[j]["Data"]);
                                                SQL.Write(string.Format("update {0} set Info='{1}' where id={2}", WriteTableName[i], AllWriteValue.BoolValue.Info[j], j));
                                            }
                                        }
                                        break;
                                    case "WriteByte":
                                        if (dt.Rows.Count != AllWriteValue.ByteValue.Value.Count)
                                        {
                                            SQL.Write(string.Format("delete from {0}", WriteTableName[i]));
                                            for (int j = 0; j < AllWriteValue.ByteValue.Value.Count; j++)
                                            {
                                                SQL.Write(string.Format("insert into {0} Values ({1},{2},'{3}')", WriteTableName[i], j, AllWriteValue.ByteValue.Value[j], AllWriteValue.ByteValue.Info[j]));
                                            }
                                        }
                                        else
                                        {
                                            for (int j = 0; j < AllWriteValue.ByteValue.Value.Count; j++)
                                            {
                                                AllWriteValue.ByteValue.Value[j] = Class.Num.ToByte(dt.Rows[j]["Data"]);
                                                SQL.Write(string.Format("update {0} set Info='{1}' where id={2}", WriteTableName[i], AllWriteValue.ByteValue.Info[j], j));
                                            }
                                        }
                                        break;
                                    case "WriteUshort":
                                        if (dt.Rows.Count != AllWriteValue.UshortValue.Value.Count)
                                        {
                                            SQL.Write(string.Format("delete from {0}", WriteTableName[i]));
                                            for (int j = 0; j < AllWriteValue.UshortValue.Value.Count; j++)
                                            {
                                                SQL.Write(string.Format("insert into {0} Values ({1},{2},'{3}')", WriteTableName[i], j, AllWriteValue.UshortValue.Value[j], AllWriteValue.UshortValue.Info[j]));
                                            }
                                        }
                                        else
                                        {
                                            for (int j = 0; j < AllWriteValue.UshortValue.Value.Count; j++)
                                            {
                                                AllWriteValue.UshortValue.Value[j] = Class.Num.ToUshort(dt.Rows[j]["Data"]);
                                                SQL.Write(string.Format("update {0} set Info='{1}' where id={2}", WriteTableName[i], AllWriteValue.UshortValue.Info[j], j));
                                            }
                                        }
                                        break;
                                    case "WriteDouble":
                                        if (dt.Rows.Count != AllWriteValue.DoubleValue.Value.Count)
                                        {
                                            SQL.Write(string.Format("delete from {0}", WriteTableName[i]));
                                            for (int j = 0; j < AllWriteValue.DoubleValue.Value.Count; j++)
                                            {
                                                SQL.Write(string.Format("insert into {0} Values ({1},{2},'{3}')", WriteTableName[i], j, AllWriteValue.DoubleValue.Value[j], AllWriteValue.DoubleValue.Info[j]));
                                            }
                                        }
                                        else
                                        {
                                            for (int j = 0; j < AllWriteValue.DoubleValue.Value.Count; j++)
                                            {
                                                AllWriteValue.DoubleValue.Value[j] = Class.Num.ToDouble(dt.Rows[j]["Data"]);
                                                SQL.Write(string.Format("update {0} set Info='{1}' where id={2}", WriteTableName[i], AllWriteValue.DoubleValue.Info[j], j));
                                            }
                                        }
                                        break;
                                    case "WriteFloat":
                                        if (dt.Rows.Count != AllWriteValue.FloatValue.Value.Count)
                                        {
                                            SQL.Write(string.Format("delete from {0}", WriteTableName[i]));
                                            for (int j = 0; j < AllWriteValue.FloatValue.Value.Count; j++)
                                            {
                                                SQL.Write(string.Format("insert into {0} Values ({1},{2},'{3}')", WriteTableName[i], j, AllWriteValue.FloatValue.Value[j], AllWriteValue.FloatValue.Info[j]));
                                            }
                                        }
                                        else
                                        {
                                            for (int j = 0; j < AllWriteValue.FloatValue.Value.Count; j++)
                                            {
                                                AllWriteValue.FloatValue.Value[j] = Class.Num.ToFloat(dt.Rows[j]["Data"]);
                                                SQL.Write(string.Format("update {0} set Info='{1}' where id={2}", WriteTableName[i], AllWriteValue.FloatValue.Info[j], j));
                                            }
                                        }
                                        break;
                                    case "WriteInt":
                                        if (dt.Rows.Count != AllWriteValue.IntValue.Value.Count)
                                        {
                                            SQL.Write(string.Format("delete from {0}", WriteTableName[i]));
                                            for (int j = 0; j < AllWriteValue.IntValue.Value.Count; j++)
                                            {
                                                SQL.Write(string.Format("insert into {0} Values ({1},{2},'{3}')", WriteTableName[i], j, AllWriteValue.IntValue.Value[j], AllWriteValue.IntValue.Info[j]));
                                            }
                                        }
                                        else
                                        {
                                            for (int j = 0; j < AllWriteValue.IntValue.Value.Count; j++)
                                            {
                                                AllWriteValue.IntValue.Value[j] = Class.Num.ToInt(dt.Rows[j]["Data"]);
                                                SQL.Write(string.Format("update {0} set Info='{1}' where id={2}", WriteTableName[i], AllWriteValue.IntValue.Info[j], j));
                                            }
                                        }
                                        break;
                                    case "WriteString":
                                        if (dt.Rows.Count != AllWriteValue.StringValue.Value.Count)
                                        {
                                            SQL.Write(string.Format("delete from {0}", WriteTableName[i]));
                                            for (int j = 0; j < AllWriteValue.StringValue.Value.Count; j++)
                                            {
                                                SQL.Write(string.Format("insert into {0} Values ({1},'{2}','{3}')", WriteTableName[i], j, AllWriteValue.StringValue.Value[j], AllWriteValue.StringValue.Info[j]));
                                            }
                                        }
                                        else
                                        {
                                            for (int j = 0; j < AllWriteValue.StringValue.Value.Count; j++)
                                            {
                                                AllWriteValue.StringValue.Value[j] = Class.Num.ToString(dt.Rows[j]["Data"]);
                                                SQL.Write(string.Format("update {0} set Info='{1}' where id={2}", WriteTableName[i], AllWriteValue.StringValue.Info[j], j));
                                            }
                                        }
                                        break;
                                }
                            }
                            for (int i = 0; i < ReadTableName.Length; i++)
                            {
                                dt = SQL.Read(string.Format("select Id,Data From {0} order by Id", ReadTableName[i]));
                                switch (ReadTableName[i])
                                {
                                    case "ReadBool":
                                        if (dt.Rows.Count != AllReadValue.BoolValue.Value.Count)
                                        {
                                            SQL.Write(string.Format("Delete  from {0}", ReadTableName[i]));
                                            for (int j = 0; j < AllReadValue.BoolValue.Value.Count; j++)
                                            {
                                                SQL.Write(string.Format("insert into {0} values ({1},{2},'{3}')", ReadTableName[i], j, AllReadValue.BoolValue.Value[j], AllReadValue.BoolValue.Info[j]));
                                            }
                                        }
                                        else
                                        {
                                            for (int j = 0; j < AllReadValue.BoolValue.Value.Count; j++)
                                            {
                                                AllReadValue.BoolValue.Value[j] = Class.Num.ToBool(dt.Rows[j]["Data"]);
                                                SQL.Write(string.Format("update {0} Set Info='{1}' where ID={2}", ReadTableName[i], AllReadValue.BoolValue.Info[j], j));
                                            }
                                        }
                                        break;
                                    case "ReadByte":
                                        if (dt.Rows.Count != AllReadValue.ByteValue.Value.Count)
                                        {
                                            SQL.Write(string.Format("Delete from {0}", ReadTableName[i]));
                                            for (int j = 0; j < AllReadValue.ByteValue.Value.Count; j++)
                                            {
                                                SQL.Write(string.Format("insert into {0} values ({1},{2},'{3}')", ReadTableName[i], j, AllReadValue.ByteValue.Value[j], AllReadValue.ByteValue.Info[j]));
                                            }
                                        }
                                        else
                                        {
                                            for (int j = 0; j < AllReadValue.ByteValue.Value.Count; j++)
                                            {
                                                AllReadValue.ByteValue.Value[j] = Class.Num.ToByte(dt.Rows[j]["Data"]);
                                                SQL.Write(string.Format("update {0} Set Info='{1}' where ID={2}", ReadTableName[i], AllReadValue.ByteValue.Info[j], j));
                                            }
                                        }
                                        break;
                                    case "ReadString":
                                        if (dt.Rows.Count != AllReadValue.StringValue.Value.Count)
                                        {
                                            SQL.Write(string.Format("Delete from {0}", ReadTableName[i]));
                                            for (int j = 0; j < AllReadValue.StringValue.Value.Count; j++)
                                            {
                                                SQL.Write(string.Format("insert into {0} values ({1},'{2}','{3}')", ReadTableName[i], j, AllReadValue.StringValue.Value[j], AllReadValue.StringValue.Info[j]));
                                            }
                                        }
                                        else
                                        {
                                            for (int j = 0; j < AllReadValue.StringValue.Value.Count; j++)
                                            {
                                                AllReadValue.StringValue.Value[j] = Class.Num.ToString(dt.Rows[j]["Data"]);
                                                SQL.Write(string.Format("update {0} Set Info='{1}' where ID={2}", ReadTableName[i], AllReadValue.StringValue.Info[j], j));
                                            }
                                        }
                                        break;
                                    case "ReadFloat":
                                        if (dt.Rows.Count != AllReadValue.FloatValue.Value.Count)
                                        {
                                            SQL.Write(string.Format("Delete from {0}", ReadTableName[i]));
                                            for (int j = 0; j < AllReadValue.FloatValue.Value.Count; j++)
                                            {
                                                SQL.Write(string.Format("insert into {0} values ({1},{2},'{3}')", ReadTableName[i], j, AllReadValue.FloatValue.Value[j], AllReadValue.FloatValue.Info[j]));
                                            }
                                        }
                                        else
                                        {
                                            for (int j = 0; j < AllReadValue.FloatValue.Value.Count; j++)
                                            {
                                                AllReadValue.FloatValue.Value[j] = Class.Num.ToFloat(dt.Rows[j]["Data"]);
                                                SQL.Write(string.Format("update {0} Set Info='{1}' where ID={2}", ReadTableName[i], AllReadValue.FloatValue.Info[j], j));
                                            }
                                        }
                                        break;
                                    case "ReadDouble":
                                        if (dt.Rows.Count != AllReadValue.DoubleValue.Value.Count)
                                        {
                                            SQL.Write(string.Format("Delete from {0}", ReadTableName[i]));
                                            for (int j = 0; j < AllReadValue.DoubleValue.Value.Count; j++)
                                            {
                                                SQL.Write(string.Format("insert into {0} values ({1},{2},'{3}')", ReadTableName[i], j, AllReadValue.DoubleValue.Value[j], AllReadValue.DoubleValue.Info[j]));
                                            }
                                        }
                                        else
                                        {
                                            for (int j = 0; j < AllReadValue.DoubleValue.Value.Count; j++)
                                            {
                                                AllReadValue.DoubleValue.Value[j] = Class.Num.ToDouble(dt.Rows[j]["Data"]);
                                                SQL.Write(string.Format("update {0} Set Info='{1}' where ID={2}", ReadTableName[i], AllReadValue.DoubleValue.Info[j], j));
                                            }
                                        }
                                        break;
                                    case "ReadInt":
                                        if (dt.Rows.Count != AllReadValue.IntValue.Value.Count)
                                        {
                                            SQL.Write(string.Format("Delete from {0}", ReadTableName[i]));
                                            for (int j = 0; j < AllReadValue.IntValue.Value.Count; j++)
                                            {
                                                SQL.Write(string.Format("insert into {0} values ({1},{2},'{3}')", ReadTableName[i], j, AllReadValue.IntValue.Value[j], AllReadValue.IntValue.Info[j]));
                                            }
                                        }
                                        else
                                        {
                                            for (int j = 0; j < AllReadValue.IntValue.Value.Count; j++)
                                            {
                                                AllReadValue.IntValue.Value[j] = Class.Num.ToInt(dt.Rows[j]["Data"]);
                                                SQL.Write(string.Format("update {0} Set Info='{1}' where ID={2}", ReadTableName[i], AllReadValue.IntValue.Info[j], j));
                                            }
                                        }
                                        break;
                                    case "ReadUshort":
                                        if (dt.Rows.Count != AllReadValue.UshortValue.Value.Count)
                                        {
                                            SQL.Write(string.Format("Delete from {0}", ReadTableName[i]));
                                            for (int j = 0; j < AllReadValue.UshortValue.Value.Count; j++)
                                            {
                                                SQL.Write(string.Format("insert into {0} values ({1},{2},'{3}')", ReadTableName[i], j, AllReadValue.UshortValue.Value[j], AllReadValue.UshortValue.Info[j]));
                                            }
                                        }
                                        else
                                        {
                                            for (int j = 0; j < AllReadValue.UshortValue.Value.Count; j++)
                                            {
                                                AllReadValue.UshortValue.Value[j] = Class.Num.ToUshort(dt.Rows[j]["Data"]);
                                                SQL.Write(string.Format("update {0} Set Info='{1}' where ID={2}", ReadTableName[i], AllReadValue.UshortValue.Info[j], j));
                                            }
                                        }
                                        break;
                                }
                            }


                            dt.Dispose();
                        }
                    }
                    catch (Exception e)
                    {
                        result = false;
                        Class.Error.Add(e);
                    }
                }
            }
            return result;
        }
        #endregion
        #region//加载通讯
        /// <summary>
        /// 从加载的XML文档中初始化通讯类
        /// </summary>
        /// <returns></returns>
        private bool LoadCommunicate()
        {
            bool result = true;
            if (AllCommuniteFile.Count <= 0)
            {
                return true;
            }
            int timeStart = Environment.TickCount;
            AllCommunite = new List<Communite>(AllCommuniteFile.Count);
            Communite tmpCommunite;
            All.Class.Reflex<Communite> tmpReflex;
            for (int i = 0; i < AllCommuniteFile.Count; i++)
            {
                if (!AllCommuniteFile[i].Attribute.ContainsKey("Class"))
                {
                    All.Class.Error.Add("初始化通讯错误,初始化字符串不包含反射类", Environment.StackTrace);
                    continue;
                }
                tmpReflex = new Class.Reflex<Communite>("All", AllCommuniteFile[i].Attribute["Class"]);
                tmpCommunite = (Communite)tmpReflex.Get();
                AllCommunite.Add(tmpCommunite);
                AllCommunite[i].Init(AllCommuniteFile[i].Attribute);
                AllCommunite[i].GetError += DataReadAndWrite_GetError;
                if (!AllCommuniteFile[i].Attribute.ContainsKey("Run") ||
                    All.Class.Num.ToBool(AllCommuniteFile[i].Attribute["Run"]))
                {
                    AllCommunite[i].Open();
                }

                All.Class.Log.Add(string.Format("LoadValueCount时间   ->    {0}ms", Environment.TickCount - timeStart));
                All.Class.Log.Add(string.Format("Load{0}时间          ->    {1}ms",AllCommunite[i].Text, Environment.TickCount - timeStart));
                timeStart = Environment.TickCount;
            }
            return result;
        }
        /// <summary>
        /// 通讯错误
        /// </summary>
        /// <param name="e"></param>
        private void DataReadAndWrite_GetError(Exception e)
        {
            if (GetError != null)
            {
                GetError(e);
            }
        }
        /// <summary>
        /// 从加载的XML文档中初始化设备类
        /// </summary>
        /// <returns></returns>
        private bool LoadMeter()
        {
            bool result = true;
            Meter.Meter tmpMeter;
            All.Class.Reflex<Meter.Meter> tmpReflex;

            for (int i = 0; i < AllCommunite.Count && i < AllCommuniteFile.Count; i++)
            {

                for (int j = 0; j < AllCommuniteFile[i].AllMeter.Count; j++)
                {
                    if (!AllCommuniteFile[i].AllMeter[j].Attribute.ContainsKey("Class"))
                    {
                        All.Class.Error.Add("初始化通讯错误,初始化字符串不包含反射类", Environment.StackTrace);
                        continue;
                    }
                    tmpReflex = new Class.Reflex<Meter.Meter>("All", AllCommuniteFile[i].AllMeter[j].Attribute["Class"]);
                    tmpMeter = (Meter.Meter)tmpReflex.Get();
                    if (tmpMeter == null)
                    {
                        All.Class.Error.Add(string.Format("从命令空间反射类失败，请检查反射名称：{0}是否正确", AllCommuniteFile[i].AllMeter[j].Attribute["Class"]));
                        continue;
                    }
                    tmpMeter.Parent = AllCommunite[i];
                    AllCommunite[i].Sons.Add(tmpMeter);
                }
            }
            return result;
        }
        #endregion
    }
}
