using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace All.Class
{
    public class TypeUse
    {
        /// <summary>
        /// 当前使用数据类型列表
        /// </summary>
        public enum TypeList:byte
        {
            UnKnow = 0,
            Bytes,
            Byte,
            String,
            Double,
            UShort,
            Int,
            Float,
            Boolean
        }
        /// <summary>
        /// 将数值转化读取类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TypeList GetType(byte value)
        {
            TypeList result = TypeList.UnKnow;
            if (value >= 0 && value < Enum.GetNames(typeof(TypeList)).Length)
            {
                result = (TypeList)value;
            }
            return result;
        }
        /// <summary>
        /// 将类型转化为读取类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static TypeList GetType<T>()
        {
            
            TypeList result = TypeList.UnKnow;
            switch (typeof(T).ToString())
            {
                case "System.Byte[]":
                    result=TypeList.Bytes;
                    break;
                case "System.Byte":
                    result = TypeList.Byte;
                    break;
                case "System.String":
                    result = TypeList.String;
                    break;
                case "System.Double":
                    result = TypeList.Double;
                    break;
                case "System.UInt16":
                    result = TypeList.UShort;
                    break;
                case "System.Int32":
                    result = TypeList.Int;
                    break;
                case "System.Single":
                    result = TypeList.Float;
                    break;
                case "System.Boolean":
                    result = TypeList.Boolean;
                    break;
            }
            return result;
        }
    }
}
