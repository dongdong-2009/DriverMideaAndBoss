using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
namespace All.Communite
{
    public class Http:Communite
    {
        string RemotHost = "";
        public override int DataRecive
        {
            get { throw new NotImplementedException(); }
        }
        public override string Text
        {
            get;
            set;
        }
        public override bool IsOpen
        {
            get { throw new NotImplementedException(); }
        }
        public override event Communite.GetErrorHandle GetError;
        public override void Init(Dictionary<string, string> InitBuff)
        {
            if (InitBuff.ContainsKey("Text"))
            {
                this.Text = InitBuff["Text"];
            }
            if (InitBuff.ContainsKey("RemotHost"))
            {
                All.Class.Error.Add(string.Format("初始化Http失败,初始化字符串不包括服务器地址,设备名称:{0}", this.Text), Environment.StackTrace);
                return;
            }
            else
            {
                RemotHost = InitBuff["RemotHost"];
            }
            if (InitBuff.ContainsKey("FlushTick"))
            {
                this.FlushTick = Class.Num.ToInt(InitBuff["FlushTick"]);
            }
        }
        public override void Open()
        {
            try
            { }
            catch (Exception e)
            {
                if (GetError != null)
                {
                    GetError(e);
                }
                All.Class.Error.Add(e);
            }
        }
        public override void Close()
        {
            throw new NotImplementedException();
        }
        public override void Read<T>(out T Value)
        {
            throw new NotImplementedException();
        }
        public override void Send<T>(T value)
        {
            throw new NotImplementedException();
        }
        public override void Send<T>(T value, Dictionary<string, string> SendBuff)
        {
            throw new NotImplementedException();
        }
    }
}
