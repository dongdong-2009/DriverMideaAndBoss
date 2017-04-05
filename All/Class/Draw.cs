using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
namespace All.Class
{
    public class Draw
    {
        #region//外部API函数
        /// <summary>
        /// 绘画
        /// </summary>
        /// <param name="hdcDest">目标设备的句柄</param>
        /// <param name="nXDest">目标对象左上角X坐标</param>
        /// <param name="nYDest">目标对象左上角Y坐标</param>
        /// <param name="nWidth">目标对象的矩形宽度</param>
        /// <param name="nHeight">目标对象的矩形高度</param>
        /// <param name="hdcSrc">源设备的句柄</param>
        /// <param name="nXSrc">源设备的左上角的X坐标</param>
        /// <param name="nYSrc">源设备的左上角的Y坐标</param>
        /// <param name="dwRop">光栅的操作值</param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(
            IntPtr hdcDest, //目标设备的句柄 
            int nXDest, // 目标对象的左上角的X坐标 
            int nYDest, // 目标对象的左上角的X坐标 
            int nWidth, // 目标对象的矩形的宽度 
            int nHeight, // 目标对象的矩形的长度 
            IntPtr hdcSrc, // 源设备的句柄 
            int nXSrc, // 源对象的左上角的X坐标 
            int nYSrc, // 源对象的左上角的X坐标 
            int dwRop // 光栅的操作值 
            );
        public const int ROP_SrcCopy = 0xCC0020;
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        #endregion
    }
}
