using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace All.Meter
{
    [Obsolete("不可用，上下位机通讯请使用SSCommunicate")]
    public class SSClient:Meter
    {
        /// <summary>
        /// 用于应答的数据
        /// </summary>
        public byte[] SendByte
        { get; set; }
        /// <summary>
        /// 用于应答的数据
        /// </summary>
        public string[] SendString
        { get; set; }
        /// <summary>
        /// 用于应答的数据
        /// </summary>
        public double[] SendDouble
        { get; set; }
        /// <summary>
        /// 用于应答的数据
        /// </summary>
        public ushort[] SendUshort
        { get; set; }
        /// <summary>
        /// 用于应答的数据
        /// </summary>
        public int[] SendInt
        { get; set; }
        /// <summary>
        /// 用于应答的数据
        /// </summary>
        public float[] SendFloat
        { get; set; }
        /// <summary>
        /// 用于应答的数据
        /// </summary>
        public bool[] SendBool
        { get; set; }
        /// <summary>
        /// 读取到的数据
        /// </summary>
        public byte[] ReadByte
        { get; set; }
        /// <summary>
        /// 读取到的数据
        /// </summary>
        public string[] ReadString
        { get; set; }
        /// <summary>
        /// 读取到的数据
        /// </summary>
        public double[] ReadDouble
        { get; set; }
        /// <summary>
        /// 读取到的数据
        /// </summary>
        public ushort[] ReadUshort
        { get; set; }
        /// <summary>
        /// 读取到的数据
        /// </summary>
        public int[] ReadInt
        { get; set; }
        /// <summary>
        /// 读取到的数据
        /// </summary>
        public float[] ReadFloat
        { get; set; }
        /// <summary>
        /// 读取到的数据
        /// </summary>
        public bool[] ReadBool
        { get; set; }

        byte[] getString = new byte[0]; 

        Dictionary<string, string> initParm = new Dictionary<string, string>();

        public override Dictionary<string, string> InitParm
        {
            get { return initParm; }
            set { initParm = value; }
        }
        public override void Init(Dictionary<string, string> initParm)
        {
            this.InitParm = initParm;
            if (initParm.ContainsKey("Text"))
            {
                this.Text = initParm["Text"];
            }
            if (initParm.ContainsKey("TimeOut"))
            {
                this.TimeOut = All.Class.Num.ToInt(initParm["TimeOut"]);
            }
            if (this.Parent is All.Communite.Com
                || this.Parent is All.Communite.Http
                || this.Parent is All.Communite.TcpClient)
            {
                All.Class.Error.Add(new string[] { "出错设备" }, new string[] { this.Text });
                All.Class.Error.Add("通讯类只能使用UDP的连接方式,其他方式暂时不行", Environment.StackTrace);
            }
            else
            {
                All.Communite.Udp parent = (All.Communite.Udp)this.Parent;
                parent.UdpClient.GetStringArgs += UdpClient_GetStringArgs;
            }

            ReadByte = new byte[0];
            ReadString = new string[0];
            ReadDouble = new double[0];
            ReadUshort = new ushort[0];
            ReadInt = new int[0];
            ReadFloat = new float[0];
            ReadBool = new bool[0];

            SendByte = new byte[0];
            SendString = new string[0];
            SendDouble = new double[0];
            SendUshort = new ushort[0];
            SendInt = new int[0];
            SendFloat = new float[0];
            SendBool = new bool[0]; 
        }
        private void UdpClient_GetStringArgs(Class.Udp sender, Class.ReciveString reciveArgs)
        {
            string value = reciveArgs.Value;
            string[] title = value.Split(All.Class.Num.SplitChar);
            
        }
        private byte[] GetLockValue<T>(int start, int end)
        {
            List<byte> result = new List<byte>();
            byte[] tmpBuff = new byte[0];
            byte tmpByte = 0;
            string tmpString = "";
            double tmpDouble = 0;
            ushort tmpUshort = 0;
            int tmpInt = 0;
            float tmpFloat = 0;
            bool tmpBool = false;
            switch (Class.TypeUse.GetType<T>())
            {
                case All.Class.TypeUse.TypeList.Byte:
                    for (int i = start; i <= end; i++)
                    {
                        if (i < ReadByte.Length)
                        {
                            tmpByte = ReadByte[i];
                        }
                        else
                        {
                            tmpByte = 0;
                        }
                        result.Add(tmpByte);
                    }
                    break;
                case All.Class.TypeUse.TypeList.String:
                    for (int i = start; i <= end; i++)
                    {
                        if (i < ReadString.Length)
                        {
                            tmpString = ReadString[i];
                        }
                        else
                        {
                            tmpString = "";
                        }
                        tmpBuff = Encoding.UTF8.GetBytes(tmpString);
                        result.Add((byte)((tmpBuff.Length >> 8) & 0xFF));
                        result.Add((byte)((tmpBuff.Length >> 0) & 0xFF));
                        for (int j = 0; j < tmpBuff.Length; j++)
                        {
                            result.Add(tmpBuff[i]);
                        }
                    }
                    break;
                case All.Class.TypeUse.TypeList.Double:
                    for (int i = start; i <= end; i++)
                    {
                        if (i < ReadDouble.Length)
                        {
                            tmpDouble = ReadDouble[i];
                        }
                        else
                        {
                            tmpDouble = 0;
                        }
                        tmpBuff = BitConverter.GetBytes(tmpDouble);
                        for (int j = 0; j < tmpBuff.Length; j++)
                        {
                            result.Add(tmpBuff[j]);
                        }
                    }
                    break;
                case All.Class.TypeUse.TypeList.UShort:
                    for (int i = start; i <= end; i++)
                    {
                        if (i < ReadUshort.Length)
                        {
                            tmpUshort = ReadUshort[i];
                        }
                        else
                        {
                            tmpUshort = 0;
                        }
                        tmpBuff = BitConverter.GetBytes(tmpUshort);
                        for (int j = 0; j < tmpBuff.Length; j++)
                        {
                            result.Add(tmpBuff[j]);
                        }
                    }
                    break;
                case All.Class.TypeUse.TypeList.Int:
                    for (int i = start; i <= end; i++)
                    {
                        if (i < ReadInt.Length)
                        {
                            tmpInt = ReadInt[i];
                        }
                        else
                        {
                            tmpInt = 0;
                        }
                        tmpBuff = BitConverter.GetBytes(tmpInt);
                        for (int j = 0; j < tmpBuff.Length; j++)
                        {
                            result.Add(tmpBuff[j]);
                        }
                    }
                    break;
                case All.Class.TypeUse.TypeList.Float:
                    for (int i = start; i <= end; i++)
                    {
                        if (i < ReadFloat.Length)
                        {
                            tmpFloat = ReadFloat[i];
                        }
                        else
                        {
                            tmpFloat = 0;
                        }
                        tmpBuff = BitConverter.GetBytes(tmpFloat);
                        for (int j = 0; j < tmpBuff.Length; j++)
                        {
                            result.Add(tmpBuff[j]);
                        }
                    }
                    break;
                case All.Class.TypeUse.TypeList.Boolean:
                    for (int i = start; i <= end; i++)
                    {
                        if (i < ReadBool.Length)
                        {
                            tmpBool = ReadBool[i];
                        }
                        else
                        {
                            tmpBool = false;
                        }
                        result.Add(tmpBool ? (byte)1 : (byte)0);
                    }
                    break;
            }
            return result.ToArray();
        }
        public override bool Read<T>(out List<T> value, Dictionary<string, string> parm)
        {
            throw new NotImplementedException();
        }
        public override bool Read<T>(out List<T> value, int start, int end)
        {
            throw new NotImplementedException();
        }
        public override bool Read<T>(out T value, int start)
        {
            throw new NotImplementedException();
        }
        public override bool WriteInternal<T>(List<T> value, Dictionary<string, string> parm)
        {
            throw new NotImplementedException();
        }
        public override bool WriteInternal<T>(List<T> value, int start, int end)
        {
            throw new NotImplementedException();
        }
        public override bool WriteInternal<T>(T value, int start)
        {
            throw new NotImplementedException();
        }
    }
}
