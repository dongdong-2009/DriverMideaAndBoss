using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace All.Meter
{

    public class Mcgs:Meter
    {
        Dictionary<string, string> initParm;
        public override Dictionary<string, string> InitParm
        {
            get
            {
                return initParm;
            }
            set
            {
                initParm = value;
            }
        }
        public override void Init(Dictionary<string, string> initParm)
        {
            this.initParm = initParm;
            if (!initParm.ContainsKey("RemotHost"))
            {
                All.Class.Error.Add("MCGS初始化参数中不包含远程地址", Environment.StackTrace);
                return;
            }
            if (!initParm.ContainsKey("RemotPort"))
            {
                All.Class.Error.Add("MCGS初始化参数中不包含远程端口", Environment.StackTrace);
                return;
            }
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
