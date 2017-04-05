using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace All.Meter
{
    /// <summary>
    /// 异步写入操作
    /// </summary>
    public class WriteInvoke
    {
        /// <summary>
        /// 异步写入单点
        /// </summary>
        public class WritePoint
        {
            /// <summary>
            /// 写入值
            /// </summary>
            public object Value
            { get; set; }
            /// <summary>
            /// 写入开始位置
            /// </summary>
            public int Start
            { get; set; }
            /// <summary>
            /// 写入类型
            /// </summary>
            public Type T
            { get; set; }
            /// <summary>
            /// 异步写入单点
            /// </summary>
            /// <param name="value"></param>
            /// <param name="start"></param>
            /// <param name="t"></param>
            public WritePoint(object value, int start, Type t)
            {
                this.Value = value;
                this.Start = start;
                this.T = t;
            }
        }
        /// <summary>
        /// 异步写入多点
        /// </summary>
        public class WriteList
        {
            /// <summary>
            /// 异步写入值
            /// </summary>
            public List<object> Value
            { get; set; }
            /// <summary>
            /// 写入开始点
            /// </summary>
            public int Start
            { get; set; }
            /// <summary>
            /// 写入结束点
            /// </summary>
            public int End
            { get; set; }
            /// <summary>
            /// 写入类型
            /// </summary>
            public Type T
            { get; set; }
            /// <summary>
            /// 异步写入多点
            /// </summary>
            /// <param name="value"></param>
            /// <param name="start"></param>
            /// <param name="end"></param>
            /// <param name="t"></param>
            public WriteList(List<object> value, int start, int end, Type t)
            {
                this.Value = value;
                this.Start = start;
                this.End = end;
                this.T = t;
            }
        }
        /// <summary>
        /// 异步写入特殊值
        /// </summary>
        public class WriteOther
        {
            /// <summary>
            /// 写入数据列表
            /// </summary>
            public List<object> Value
            { get; set; }
            /// <summary>
            /// 写入特殊值
            /// </summary>
            public Dictionary<string, string> Parm
            { get; set; }
            /// <summary>
            /// 写入数据类型
            /// </summary>
            public Type T
            { get; set; }
            public WriteOther(List<object> value,Dictionary<string, string> parm,Type t)
            {
                this.Value = value;
                this.Parm = parm;
                this.T = t;
            }
        }
        public WritePoint PointValue
        { get; set; }
        public WriteList ListValue
        { get; set; }
        public WriteOther OtherValue
        { get; set; }
        /// <summary>
        /// 异步写入单点
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Start"></param>
        /// <param name="T"></param>
        public WriteInvoke(object Value, int Start,Type T)
        {
            PointValue = new WritePoint(Value, Start, T);
        }
        /// <summary>
        /// 异步写入多点
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="T"></param>
        public WriteInvoke(List<object> Value, int Start, int End,Type T)
        {
            ListValue = new WriteList(Value, Start, End, T);
        }
        /// <summary>
        /// 异步写入特殊值
        /// </summary>
        /// <param name="Value"></param>
        public WriteInvoke(List<object> Value,Dictionary<string, string> Parm,Type T)
        {
            OtherValue = new WriteOther(Value,Parm,T);
        }
    }
}
