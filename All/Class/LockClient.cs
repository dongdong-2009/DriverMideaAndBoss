using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace All.Class
{
    public class LockClient<T>
    {
        public delegate void SwitchOpenHandle();
        /// <summary>
        /// 允许执行，请切换必要的信号
        /// </summary>
        public event SwitchOpenHandle SwitchOpen;

        public delegate void SwitchCloseHandle();
		/// <summary>
		/// 执行完毕，将切换的信号切换回去
		/// </summary>
        public event SwitchCloseHandle SwitchClose;

        public delegate void GetRemotHostInfoHandle(LockMain<T>.GetStatueList statue,string value);
		/// <summary>
		/// 收到远程消息
		/// </summary>
        public event GetRemotHostInfoHandle GetRemotHostInfo;

        public delegate void GetRemotHostTestValueHandle(string value);
		/// <summary>
		/// 远程返回测试结果
		/// </summary>
        public event GetRemotHostTestValueHandle GetRemotHostTestValue;

        public delegate void GetRemotHostTestingValueHandle(string value);
        public event GetRemotHostTestingValueHandle GetRemotHostTestingValue;
        public enum SetStatueList:int
        {
            请求 = 0,
            删除,
            连接测试,
			执行要求完毕,
			动作完毕取消请求
        }
        SetStatueList nowStatueList = SetStatueList.连接测试;
		/// <summary>
		/// 连接状态
		/// </summary>
        public bool Connect
        { get; set; }
        Udp lockUdp;
        Thread thConnectRemot;
        bool getRemotValue = false;
        string remotHost = "127.0.0.1";
        T Index = default(T);
        int remotPort = 10000;

        bool isStart = false;
        bool isPlease = false;
        bool isStop = false;
        bool isDel = false;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="RemotHost">远程IP地址</param>
		/// <param name="index">本机端唯一识别地址</param>
        public LockClient(string RemotHost,int RemotPort,int LocalPort,T index)
        {
            Connect = false;
            this.remotPort = RemotPort;
            Index = index;
            remotHost = RemotHost;

            lockUdp = new Udp(LocalPort);
            lockUdp.GetStringArgs += lockUdp_GetStringArgs;
            lockUdp.Open();

            thConnectRemot = new Thread(() => Conn());
            thConnectRemot.IsBackground = true;
            thConnectRemot.Start();
        }
        private void Conn()
        {
            int timeOut = 0;
            while (true)
            {
                timeOut++;
                if (timeOut > 3 && !getRemotValue)
                {
                    Connect = false;
                }
                if (getRemotValue)
                {
                    Connect = true;
                    timeOut = 0;
                    getRemotValue = false;
                }
                lockUdp.Write(GetSendValue(SetStatueList.连接测试), remotHost, remotPort);
                Thread.Sleep(500);
                if (isPlease)
                {
                    lockUdp.Write(GetSendValue(SetStatueList.请求), remotHost, remotPort);
                }
                if (isStart)
                {
                    lockUdp.Write(GetSendValue(SetStatueList.执行要求完毕), remotHost, remotPort);
                }
                if (isStop)
                {
                    lockUdp.Write(GetSendValue(SetStatueList.动作完毕取消请求), remotHost, remotPort);
                }
                if (isDel)
                {
                    lockUdp.Write(GetSendValue(SetStatueList.删除), remotHost, remotPort);
                }
                Thread.Sleep(500);
            }
        }
        private void lockUdp_GetStringArgs(Udp sender, ReciveString reciveArgs)
        {
            string value = reciveArgs.Value;
            string[] buff = value.Split(new string[] { "~~~" }, StringSplitOptions.None);

            LockMain<T>.GetStatueList statue = (LockMain<T>.GetStatueList)Num.ToInt(buff[0]);
            switch (statue)
            {
                case LockMain<T>.GetStatueList.收到请求:
                    nowStatueList = SetStatueList.请求;
                    getRemotValue = true;
                    break;
				case LockMain<T>.GetStatueList.等待:
                    SetRemotInfo(statue, "远程主机已收到请求，请等待分配执行");
                    break;
                case LockMain<T>.GetStatueList.允许执行互锁要求:
                    SetRemotInfo(statue, "远程主机已允许执行互锁前的开关切换");
                    isPlease = false;
                    switch (nowStatueList)
                    {
                        case SetStatueList.请求:
                            if (SwitchOpen != null)
                            {
                                SwitchOpen();
                            }
                            nowStatueList = SetStatueList.执行要求完毕;
                            break;
                    }
                    break;
                case LockMain<T>.GetStatueList.正在执行互锁动作:

                    SetRemotInfo(statue, "远程主机正在执行互锁动作");
                    if (buff.Length > 1)
                    {
                        if (GetRemotHostTestingValue != null)
                        {
                            GetRemotHostTestingValue(buff[1]);
                        }
                    }
                    break;
				case LockMain<T>.GetStatueList.互锁动作执行完毕:
                    if (isStart)
                    {
                        isStart = false;
                        SetRemotInfo(statue, "远程操作完成");
                        if (GetRemotHostTestValue != null)
                        {
                            GetRemotHostTestValue(buff[1]);
                        }
                        if (SwitchClose != null)
                        {
                            SwitchClose();
                        }
                    }
                    break;
                case LockMain<T>.GetStatueList.互锁请求正常结束:
                    SetRemotInfo(statue, "远程主机已收到请求，互锁请求正常结束");
                    nowStatueList = SetStatueList.删除;
                    isStop = false;
                    isPlease = false;
                    isStart = false;
                    break;
                case LockMain<T>.GetStatueList.删除请求成功:
                    SetRemotInfo(statue, "远程主机已收到请求，互锁请求已删除");
                    nowStatueList = SetStatueList.删除;
                    isDel = false;
                    isPlease = false;
                    isStart = false;
                    isStop = false;
                    break;
            }
        }
        private void SetRemotInfo(LockMain<T>.GetStatueList statue, string value)
        {
            if (GetRemotHostInfo != null)
            {
                GetRemotHostInfo(statue, value);
            }
        }
        private string GetSendValue(SetStatueList statue)
        {
            return string.Format("{0}~~~{1}", (object)Index, (int)statue);
        }
		/// <summary>
		/// 请求测试
		/// </summary>
        public void Please()
        {
            isPlease = true;
        }
		/// <summary>
		/// 开始测试
		/// </summary>
        public void Start()
        {
            isStart = true;
        }
		/// <summary>
		/// 结束测试
		/// </summary>
        public void Stop()
        {
            isStop = true;
        }
		/// <summary>
		/// 删除测试
		/// </summary>
        public void Delete()
        {
            isDel = true;
        }
        ~LockClient()
        {
            if (thConnectRemot != null)
            {
                thConnectRemot.Abort();
            }
            thConnectRemot = null;
            lockUdp.Close();
            lockUdp = null;
        }

    }
}
