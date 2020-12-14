using Microsoft.International.Converters.PinYinConverter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tools
{
    public class ToSpell
    {
        /// <summary> 
        /// 汉字转化为拼音
        /// </summary> 
        /// <param name="str">汉字</param> 
        /// <returns>全拼</returns> 
        public static string GetPinyin(string str)
        {
            string r = string.Empty;
            foreach (char obj in str)
            {
                try
                {
                    ChineseChar chineseChar = new ChineseChar(obj);
                    string t = chineseChar.Pinyins[0].ToString();
                    r += t.Substring(0, t.Length - 1);
                }
                catch
                {
                    r += obj.ToString();
                }
            }
            return r;
        }

        /// <summary> 
        /// 汉字转化为拼音首字母
        /// </summary> 
        /// <param name="str">汉字</param> 
        /// <returns>首字母</returns> 
        //public static string GetFirstPinyin(string str)
        //{
        //    string r = string.Empty;
        //    foreach (char obj in str)
        //    {
        //        try
        //        {
        //            ChineseChar chineseChar = new ChineseChar(obj);
        //            string t = chineseChar.Pinyins[0].ToString();
        //            r += t.Substring(0, 1);
        //        }
        //        catch
        //        {
        //            r += obj.ToString();
        //        }
        //    }
        //    return r;
        //}

        public static string GetFirstPinyin(string str, int len = 10)
        {
            string r = string.Empty;
            if (len > str.Length) len = str.Length;
            for (int i = 0; i < len; i++)
            {
                var obj = str[i];
                try
                {
                    ChineseChar chineseChar = new ChineseChar(obj);
                    string t = chineseChar.Pinyins[0].ToString();
                    r += t.Substring(0, 1);
                }
                catch
                {
                    r += obj.ToString();
                }
            }
            return r;
        }

        // <summary> 
        /// 简体转换为繁体
        /// </summary> 
        /// <param name="str">简体字</param> 
        /// <returns>繁体字</returns> 
        //public static string GetTraditional(string str)
        //{
        //    string r = string.Empty;
        //    r = ChineseConverter.Convert(str, ChineseConversionDirection.SimplifiedToTraditional);
        //    return r;
        //}
        /// <summary> 
        /// 繁体转换为简体
        /// </summary> 
        /// <param name="str">繁体字</param> 
        /// <returns>简体字</returns> 
        //public static string GetSimplified(string str)
        //{
        //    string r = string.Empty;
        //    r = ChineseConverter.Convert(str, ChineseConversionDirection.TraditionalToSimplified);
        //    return r;
        //}
    }
}
