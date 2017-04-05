using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
namespace All.Class
{
    public static class Api
    {
        /// <summary>
        /// 按下键盘
        /// </summary>
        public const uint WM_KEYDOWN = 0x100;
        /// <summary>
        /// 键盘弹起
        /// </summary>
        public const uint WM_KEYUP = 0x101;
        /// <summary>
        /// 鼠标激活
        /// </summary>
        public const uint WM_MouseActivate = 0x21;
        /// <summary>
        /// 不激活
        /// </summary>
        public const uint MA_NoActivate = 3;

        public const uint WA_InActive = 0;
        public const uint WM_NcActivate = 0x86;
        /// <summary>
        /// 内存拷贝
        /// </summary>
        public const int ROP_SrcCopy = 0xCC0020;

        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr handle);
        [DllImport("gdi32.dll")]
        public static extern bool StretchBlt
        (
            IntPtr hdcDest, //目标设备的句柄 
            int nXDest, // 目标对象的左上角的X坐标 
            int nYDest, // 目标对象的左上角的Y坐标 
            int nWidth, // 目标对象的矩形的宽度 
            int nHeight, // 目标对象的矩形的长度 
            IntPtr hdcSrc, // 源设备的句柄 
            int nXSrc, // 源对象的左上角的X坐标 
            int nYSrc, // 源对象的左上角的Y坐标 
            int nWidthSrc,//源对象的宽度
            int nHeightSrc,//源对象的高度
            int dwRop // 光栅的操作值 
            );
        /// <summary>
        /// 图像在内存的COPY
        /// </summary>
        /// <param name="hdcDest"></param>
        /// <param name="nXDest"></param>
        /// <param name="nYDest"></param>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        /// <param name="hdcSrc"></param>
        /// <param name="nXSrc"></param>
        /// <param name="nYSrc"></param>
        /// <param name="dwRop"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(
            IntPtr hdcDest, //目标设备的句柄 
            int nXDest, // 目标对象的左上角的X坐标 
            int nYDest, // 目标对象的左上角的Y坐标 
            int nWidth, // 目标对象的矩形的宽度 
            int nHeight, // 目标对象的矩形的长度 
            IntPtr hdcSrc, // 源设备的句柄 
            int nXSrc, // 源对象的左上角的X坐标 
            int nYSrc, // 源对象的左上角的Y坐标 
            int dwRop // 光栅的操作值 
            );
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern IntPtr DeleteObject(IntPtr hObj);

        /// <summary>
        /// 注销
        /// </summary>
        public const int LogOff = 0;
        /// <summary>
        /// 重启
        /// </summary>
        public const int Reboot = 2;
        /// <summary>
        /// 关机常量
        /// </summary>
        public const int ShutDown = 1;
        /// <summary>
        /// 关机，重启等命令
        /// </summary>
        /// <param name="uFlags"></param>
        /// <param name="dwReserved"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern long ExitWindowsEx(long uFlags, long dwReserved);

    }
}
