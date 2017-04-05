using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace All.Class
{
    public class Process
    {
        /// <summary>
        /// 关闭指定程序
        /// </summary>
        /// <param name="exeName"></param>
        public static void Kill(string exeName)
        {
            System.Diagnostics.Process[] allProcess = System.Diagnostics.Process.GetProcesses();
            for (int i = 0; i < allProcess.Length; i++)
            {
                if (allProcess[i].ProcessName.ToUpper() == exeName.ToUpper()
                    || allProcess[i].ProcessName.ToUpper() == exeName.ToUpper().Replace(".EXE", ""))
                {
                    allProcess[i].Kill();
                }
            }
        }
    }
}
