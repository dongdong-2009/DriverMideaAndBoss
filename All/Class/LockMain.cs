using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace All.Class
{
    public class LockMultiChannleMain<T>
    {
        public delegate void AllTestNeedOkHandle(int ChannelIndex,T Index);
        /// <summary>
        /// 所有准备工作完毕，执行互锁动作
        /// </summary>
        public event AllTestNeedOkHandle AllTestNeedOk;
        public delegate void AllTestCancelHandle(int ChannelIndex, T Index);
        public event AllTestCancelHandle AllTestCancel;
        public event AllTestCancelHandle AllTestOk;
        public enum GetStatueList:int
        {
            收到请求 = 0,
            等待,
            允许执行互锁要求,
            正在执行互锁动作,
            互锁动作执行完毕,
            删除请求成功,
            互锁请求正常结束
        }
        /// <summary>
        /// 每一路互锁
        /// </summary>
        public class Channel
        {
            /// <summary>
            /// 互锁中的客户唯一编码
            /// </summary>
            public List<T> EveryChannel
            { get; set; }
            public Channel()
            {
                EveryChannel = new List<T>();
            }
        }
        /// <summary>
        /// 多路互锁
        /// </summary>
        public List<Channel> AllChannel
        { get; set; }

        Udp lockUdp;

        string[] testValue;//每一路结果
        string[] testingValue;//每一路临时值 
        GetStatueList[] nowStatueList;//每一中状态
        Channel[] lockList;//每一路互锁
        public LockMultiChannleMain(int MainPort,int AllChannelCount)
        {
            AllChannel = new List<Channel>();
            nowStatueList = new GetStatueList[AllChannelCount];
            testValue = new string[AllChannelCount];
            lockList = new Channel[AllChannelCount];
            testingValue = new string[AllChannelCount];
            for (int i = 0; i < AllChannelCount; i++)
            {
                nowStatueList[i] = GetStatueList.收到请求;
                testValue[i] = "";
                lockList[i] = new Channel();
                testingValue[i] = "";
            }
            lockUdp = new Udp(MainPort);
            lockUdp.GetStringArgs += lockUpd_GetStringArgs;
            lockUdp.Open();
        }
        public void AllTestOver(T index, string value)
        {
            int i = GetChannelIndexFromEveryIndex(index);
            if (i >= 0)
            {
                testValue[i] = value;
                nowStatueList[i] = GetStatueList.互锁动作执行完毕;
            }
            else
            {
                All.Class.Error.Add(string.Format("当前结束返回时序号错误,当前序号不包含在互锁的子项中,当前项为{0}", index));
            }
        }
        public void AllTesting(int ChannelIndex, T index, string value)
        {
            try
            {
                if (lockList.Length > ChannelIndex && lockList[ChannelIndex].EveryChannel.Count > 0)
                {
                    if ((string)(object)index == (string)(object)lockList[ChannelIndex].EveryChannel[0])
                    {
                        testingValue[ChannelIndex] = value;
                    }
                }
            }
            catch (Exception e)
            {
                All.Class.Error.Add(e);
                All.Class.Error.Add(string.Format("当前状态值 ChannelIndex:{0},    index:{1},  value:{2}", ChannelIndex, index, value), Environment.StackTrace);
            }
        }
        private int GetChannelIndexFromEveryIndex(T index)
        {
            int result = -1;
            for (int i = 0; i < AllChannel.Count; i++)
            {
                for (int j = 0; j < AllChannel[i].EveryChannel.Count; j++)
                {
                    if ((string)(object)AllChannel[i].EveryChannel[j] == (string)(object)index)
                    {
                        result = i;
                        break;
                    }
                }
                if (result >= 0)
                {
                    break;
                }
            }
            return result;
        }
        private void lockUpd_GetStringArgs(Udp sender, ReciveString reciveArgs)
        {
            string value = reciveArgs.Value;
            string[] buff = value.Split(new string[] { "~~~" }, StringSplitOptions.None);
            T index = (T)(Object)(buff[0]);
            LockClient<T>.SetStatueList statue = (LockClient<T>.SetStatueList)Num.ToInt(buff[1]);

            int channel = GetChannelIndexFromEveryIndex(index);

            if (channel < 0)
            {
                All.Class.Error.Add(string.Format("当前请求的序号错误,当前序号不包含在互锁的子项中,当前项为{0}", index));
                return;
            }
            switch (statue)
            {
                case LockClient<T>.SetStatueList.连接测试:
                    lockUdp.Write(string.Format("{0}", (int)GetStatueList.收到请求), reciveArgs.RemoteHost, reciveArgs.RemotePort);
                    break;
                case LockClient<T>.SetStatueList.请求:
                    if (!lockList[channel].EveryChannel.Contains(index))
                    {
                        lockList[channel].EveryChannel.Add(index);
                    }
                    if (lockList[channel].EveryChannel.Count > 0)
                    {
                        if ((string)(object)lockList[channel].EveryChannel[0] == (string)(object)index)
                        {
                            lockUdp.Write(string.Format("{0}", (int)GetStatueList.允许执行互锁要求), reciveArgs.RemoteHost, reciveArgs.RemotePort);
                        }
                        else
                        {
                            lockUdp.Write(string.Format("{0}", (int)GetStatueList.等待), reciveArgs.RemoteHost, reciveArgs.RemotePort);
                        }
                    }
                    else
                    {
                        Error.Add("有添加的请求，但数组列为0", Environment.StackTrace);
                    }
                    break;
                case LockClient<T>.SetStatueList.执行要求完毕:
                    if (lockList[channel].EveryChannel.Count > 0)
                    {
                        if ((string)(object)lockList[channel].EveryChannel[0] != (string)(object)index)
                        {
                            Error.Add(string.Format("动作完成的工位和允许执行的工位不是同一个，大问题。允许工位为：{0}，执行工位为：{1}", lockList[0], index), Environment.StackTrace);
                            return;
                        }
                        switch (nowStatueList[channel])
                        {
                            case GetStatueList.收到请求:
                                if (AllTestNeedOk != null)
                                {
                                    AllTestNeedOk(channel,index);
                                }
                                nowStatueList[channel] = GetStatueList.正在执行互锁动作;
                                lockUdp.Write(string.Format("{0}", (int)GetStatueList.正在执行互锁动作), reciveArgs.RemoteHost, reciveArgs.RemotePort);
                                break;
                            case GetStatueList.正在执行互锁动作:
                                lockUdp.Write(string.Format("{0}~~~{1}", (int)GetStatueList.正在执行互锁动作,testingValue[channel]), reciveArgs.RemoteHost, reciveArgs.RemotePort);
                                break;
                            case GetStatueList.互锁动作执行完毕:
                                lockUdp.Write(string.Format("{0}~~~{1}", (int)GetStatueList.互锁动作执行完毕, testValue[channel]), reciveArgs.RemoteHost, reciveArgs.RemotePort);
                                break;
                        }
                    }
                    else
                    {
                        Error.Add("有返回执行的工位请求，但数组列为0", Environment.StackTrace);
                    }
                    break;
                case LockClient<T>.SetStatueList.动作完毕取消请求:
                    lockUdp.Write(string.Format("{0}", (int)GetStatueList.互锁请求正常结束), reciveArgs.RemoteHost, reciveArgs.RemotePort);
                    if (lockList[channel].EveryChannel.Count > 0)
                    {
                        if ((string)(object)lockList[channel].EveryChannel[0] == (string)(object)index)
                        {
                            nowStatueList[channel] = GetStatueList.收到请求;
                            testValue[channel] = "";
                            testingValue[channel] = "";
                            lockList[channel].EveryChannel.Remove(index);
                            if (AllTestOk != null)
                            {
                                AllTestOk(channel, index);
                            }
                        }
                        else
                        {
                            Error.Add(string.Format("动作完成的工位和完成请求的工位不是同一个，大问题。允许工位为：{0}，执行完成工位为：{1}", lockList[0], index), Environment.StackTrace);
                            return;
                        }
                    }
                    else
                    {
                        Error.Add("有返回完毕的工位请求，但数组列为0", Environment.StackTrace);
                    }
                    break;
                case LockClient<T>.SetStatueList.删除:

                    if (lockList[channel].EveryChannel.Count > 0)
                    {
                        if ((string)(object)lockList[channel].EveryChannel[0] == (string)(object)index)
                        {
                            nowStatueList[channel] = GetStatueList.收到请求;
                            testValue[channel] = "";
                            testingValue[channel] = "";
                            if (AllTestCancel != null)
                            {
                                AllTestCancel(channel, index);
                            }
                        }
                    }
                    if (lockList[channel].EveryChannel.Contains(index))
                    {
                        lockList[channel].EveryChannel.Remove(index);
                    }
                    lockUdp.Write(string.Format("{0}", (int)GetStatueList.删除请求成功), reciveArgs.RemoteHost, reciveArgs.RemotePort);
                    break;
            }
        }

        ~LockMultiChannleMain()
        {
            lockUdp.Close();
            lockUdp = null;
        }
    }
    public class LockMain<T>
    {
        public delegate void AllTestNeedOkHandle();
        /// <summary>
        /// 所有准备工作完毕，执行互锁动作
        /// </summary>
        public event AllTestNeedOkHandle AllTestNeedOk;
        public delegate void AllTestCancelHandle();
        public event AllTestCancelHandle AllTestCancel;

        public enum GetStatueList:int
        {
            收到请求 = 0,
            等待,
            允许执行互锁要求,
            正在执行互锁动作,
            互锁动作执行完毕,
            删除请求成功,
            互锁请求正常结束
        }
        List<T> lockList = new List<T>();
        Udp lockUdp;

        string testValue = "";
        string testingValue = "";
        GetStatueList nowStatueList = GetStatueList.收到请求;
        public LockMain(int MainPort)
        {
            lockUdp = new Udp(MainPort);
            lockUdp.GetStringArgs += lockUpd_GetStringArgs;
            lockUdp.Open();
        }
        public void AllTestOver(string value)
        {
            testValue = value;
            nowStatueList = GetStatueList.互锁动作执行完毕;
        }
        public void AllTesting( string value)
        {
            testingValue = value;
        }
        private void lockUpd_GetStringArgs(Udp sender, ReciveString reciveArgs)
        {
            string value = reciveArgs.Value;
            string[] buff = value.Split(new string[] { "~~~" }, StringSplitOptions.None);
            T index = (T)(Object)(buff[0]);
            LockClient<T>.SetStatueList statue = (LockClient<T>.SetStatueList)Num.ToInt(buff[1]);
            switch (statue)
            {
                case LockClient<T>.SetStatueList.连接测试:
                    lockUdp.Write(string.Format("{0}",(int) GetStatueList.收到请求), reciveArgs.RemoteHost, reciveArgs.RemotePort);
                    break;
                case LockClient<T>.SetStatueList.请求:
                    if (!lockList.Contains(index))
                    {
                        lockList.Add(index);
                    }
                    if (lockList.Count > 0)
                    {
                        if ((string)(object)lockList[0] == (string)(object)index)
                        {
                            lockUdp.Write(string.Format("{0}", (int)GetStatueList.允许执行互锁要求), reciveArgs.RemoteHost, reciveArgs.RemotePort);
                        }
                        else
                        {
                            lockUdp.Write(string.Format("{0}", (int)GetStatueList.等待), reciveArgs.RemoteHost, reciveArgs.RemotePort);
                        }
                    }
                    else
                    {
                        Error.Add("有添加的请求，但数组列为0", Environment.StackTrace);
                    }
                    break;
                case LockClient<T>.SetStatueList.执行要求完毕:
                    if (lockList.Count > 0)
                    {
                        if ((string)(object)lockList[0] != (string)(object)index)
                        {
                            Error.Add(string.Format("动作完成的工位和允许执行的工位不是同一个，大问题。允许工位为：{0}，执行工位为：{1}",lockList[0],index), Environment.StackTrace);
                            return ;
                        }
                        switch (nowStatueList)
                        {
                            case GetStatueList.收到请求:
                                if (AllTestNeedOk != null)
                                {
                                    AllTestNeedOk();
                                }
                                nowStatueList = GetStatueList.正在执行互锁动作;
                                lockUdp.Write(string.Format("{0}", (int)GetStatueList.正在执行互锁动作), reciveArgs.RemoteHost, reciveArgs.RemotePort);
                                break;
                            case GetStatueList.正在执行互锁动作:
                                lockUdp.Write(string.Format("{0}~~~{1}", (int)GetStatueList.正在执行互锁动作, testingValue), reciveArgs.RemoteHost, reciveArgs.RemotePort);
                                break;
                            case GetStatueList.互锁动作执行完毕:
                                lockUdp.Write(string.Format("{0}~~~{1}", (int)GetStatueList.互锁动作执行完毕, testValue), reciveArgs.RemoteHost, reciveArgs.RemotePort);
                                break;
                        }
                    }
                    else
                    {
                        Error.Add("有返回执行的工位请求，但数组列为0", Environment.StackTrace);
                    }
                    break;
                case LockClient<T>.SetStatueList.动作完毕取消请求:
                    lockUdp.Write(string.Format("{0}", (int)GetStatueList.互锁请求正常结束), reciveArgs.RemoteHost, reciveArgs.RemotePort);            
                    if (lockList.Count > 0)
                    {
                        if ((string)(object)lockList[0] == (string)(object)index)
                        {
                            nowStatueList = GetStatueList.收到请求;
                            testValue = "";
                            testingValue = "";
                            lockList.Remove(index);
                        }
                        else 
                        {
                            Error.Add(string.Format("动作完成的工位和完成请求的工位不是同一个，大问题。允许工位为：{0}，执行完成工位为：{1}",lockList[0],index), Environment.StackTrace);
                            return ;
                        }
                    }
                    else
                    {
                        Error.Add("有返回完毕的工位请求，但数组列为0", Environment.StackTrace);
                    }
                    break;
                case LockClient<T>.SetStatueList.删除:

                    if (lockList.Count > 0)
                    {
                        if ((string)(object)lockList[0] == (string)(object)index)
                        {
                            nowStatueList = GetStatueList.收到请求;
                            testValue = "";
                            testingValue = "";
                            if (AllTestCancel != null)
                            {
                                AllTestCancel();
                            }
                        }
                    }
                    if (lockList.Contains(index))
                    {
                        lockList.Remove(index);
                    }
                    lockUdp.Write(string.Format("{0}", (int)GetStatueList.删除请求成功), reciveArgs.RemoteHost, reciveArgs.RemotePort);
                    break;
            }
        }

        ~LockMain()
        {
            lockUdp.Close();
            lockUdp = null;
        }
    }
}
