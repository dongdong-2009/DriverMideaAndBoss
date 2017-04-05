using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace All.Meter
{
    public class MitsubishiFX : Meter
    {
        Dictionary<string, string> initParm;

        public override Dictionary<string, string> InitParm
        {
            get { return initParm; }
            set { initParm = value; }
        }
        public override void Init(Dictionary<string, string> initParm)
        {
            //throw new NotImplementedException();
            if (initParm.ContainsKey("ErrorCount"))
            {
                this.ErrorCount = All.Class.Num.ToInt(initParm["ErrorCount"]);
            }
        }
        public override bool Read<T>(out List<T> value, int start, int end)
        {
            bool result = true;
            value = new List<T>();
            if (typeof(T) == typeof(string))
            { }
            return result;
        }
        public override bool Read<T>(out T value, int start)
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
        public override bool Read<T>(out List<T> value, Dictionary<string, string> parm)
        {
            throw new NotImplementedException();
        }
        public override bool WriteInternal<T>(List<T> value, Dictionary<string, string> parm)
        {
            throw new NotImplementedException();
        }
    }
}
