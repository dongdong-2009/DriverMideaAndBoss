using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
namespace All.Class
{
    public class FlushAll
    {
        /// <summary>
        /// 所有要刷新数据
        /// </summary>
        List<FlushMethor> flushData = new List<FlushMethor>();

        Thread[] autoRun = new Thread[0];

        /// <summary>
        /// 添加要自动刷新的方法
        /// </summary>
        /// <param name="fm"></param>
        public void Add(FlushMethor flush)
        {
            if (!flushData.Contains(flush))
            {
                flushData.Add(flush);
            }
        }
        /// <summary>
        /// 开始系统刷新
        /// </summary>
        public void Run()
        {
            if (flushData.Count <= 0)
            {
                return;
            }
            autoRun = new Thread[flushData.Count];
            for (int i = 0; i < autoRun.Length; i++)
            {
                int index = i;
                autoRun[i] = new Thread(() => FlushHandle(index));
                autoRun[i].IsBackground = true;
                autoRun[i].Start();
            }
        }
        ~FlushAll()
        {
            if (autoRun != null)
            {
                if (autoRun.Length > 0)
                {
                    for (int i = 0; i < autoRun.Length; i++)
                    {
                        autoRun[i].Abort();
                        autoRun[i] = null;
                    }
                }
            }
            autoRun = null;
        }

        private void FlushHandle(int index)
        {
            flushData[index].Load();
            object lockObject = new object();
            while (true)
            {
                if ((Environment.TickCount - flushData[index].FlushStart) > flushData[index].FlushTick)
                {
                    flushData[index].FlushStart = Environment.TickCount;
                    flushData[index].Flush();
                }
                Thread.Sleep(20);
            }
        }
        /// <summary>
        /// 自动刷新方法
        /// </summary>
        public abstract class FlushMethor
        {
            int flushTick = 1000;
            /// <summary>
            /// 刷新间隔
            /// </summary>
            public int FlushTick
            {
                get { return flushTick; }
                set { flushTick = value; }
            }
            int flushStart = Environment.TickCount;
            /// <summary>
            /// 刷新开始时间 
            /// </summary>
            public int FlushStart
            {
                get { return flushStart; }
                set { flushStart = value; }
            }
            /// <summary>
            /// 刷新
            /// </summary>
            public abstract void Flush();
            /// <summary>
            /// 加载
            /// </summary>
            public abstract void Load();
        }
    }
}
