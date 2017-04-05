using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
namespace All.Communite
{
    /// <summary>
    /// 通讯类
    /// </summary>
    public abstract class Communite
    {
        public Communite()
        {
            FlushTick = 1000;
            Text = "";
            Sons = new List<Meter.Meter>();
        }
        /// <summary>
        /// 通讯名称
        /// </summary>
        public abstract string Text
        { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="InitBuff"></param>
        public abstract void Init(Dictionary<string, string> InitBuff);
        /// <summary>
        /// 端口是否已打开 
        /// </summary>
        public abstract bool IsOpen
        { get; }
        /// <summary>
        /// 打开通讯端口
        /// </summary>
        public abstract void Open();
        /// <summary>
        /// 关闭通讯端口
        /// </summary>
        public abstract void Close();
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public abstract void Send<T>(T value);
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        public abstract void Read<T>(out T Value);
        /// <summary>
        /// 当前数据量
        /// </summary>
        public abstract int DataRecive
        { get;  }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <param name="SendBuff"></param>
        public abstract void Send<T>(T value, Dictionary<string, string> SendBuff);
        /// <summary>
        /// 所有通讯子类设备
        /// </summary>
        public List<Meter.Meter> Sons
        { get; set; }
        /// <summary>
        /// 此通讯刷新周期,单位ms
        /// </summary>
        public int FlushTick
        { get; set; }
        /// <summary>
        /// 通讯错误
        /// </summary>
        /// <param name="e">错误内容</param>
        public delegate void GetErrorHandle(Exception e);
        /// <summary>
        /// 通讯错误
        /// </summary>
        public abstract event GetErrorHandle GetError;
    }
}
