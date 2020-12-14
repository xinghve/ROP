using Microsoft.AspNetCore.Http;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tools
{
    public class Utils
    {
        /// <summary>
        /// 请求头参数
        /// </summary>
        public static IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();

        /// <summary>
        /// 生成随机字母字符串(数字字母混和)
        /// </summary>
        /// <param name="codeCount">待生成的位数</param>
        public static string GetCheckCode(int codeCount)
        {
            string str = string.Empty;
            int rep = 0;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codeCount; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }

        /// <summary>
        /// 将字符串转换为int类型数组
        /// </summary>
        /// <param name="str">如1,2,3,4,5</param>
        /// <returns></returns>
        public static List<int> StrToListInt(string str)
        {
            var list = new List<int>();
            if (!str.Contains(","))
            {
                list.Add(int.Parse(str));
                return list;
            }
            var slist = str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in slist)
            {
                list.Add(int.Parse(item));
            }
            return list;
        }

        /// <summary>
        /// 将字符串转换为string类型数组
        /// </summary>
        /// <param name="str">如1,2,3,4,5</param>
        /// <returns></returns>
        public static List<string> StrToListString(string str)
        {
            var list = new List<string>();
            if (!str.Contains(","))
            {
                list.Add(str);
                return list;
            }
            var slist = str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in slist)
            {
                list.Add(item);
            }
            return list;
        }

        /// <summary>
        /// 将字符串转换为数组
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>字符串数组</returns>
        public static string[] GetStrArray(string str)
        {
            return str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        #region 截取字符串
        public static string GetSubString(string pSrcString, int pLength, string pTailString)
        {
            return GetSubString(pSrcString, 0, pLength, pTailString);
        }
        public static string GetSubString(string pSrcString, int pStartIndex, int pLength, string pTailString)
        {
            string str = pSrcString;
            byte[] bytes = Encoding.UTF8.GetBytes(pSrcString);
            foreach (char ch in Encoding.UTF8.GetChars(bytes))
            {
                if (((ch > 'ࠀ') && (ch < '一')) || ((ch > 0xac00) && (ch < 0xd7a3)))
                {
                    if (pStartIndex >= pSrcString.Length)
                    {
                        return "";
                    }
                    return pSrcString.Substring(pStartIndex, ((pLength + pStartIndex) > pSrcString.Length) ? (pSrcString.Length - pStartIndex) : pLength);
                }
            }
            if (pLength < 0)
            {
                return str;
            }
            byte[] sourceArray = Encoding.Default.GetBytes(pSrcString);
            if (sourceArray.Length <= pStartIndex)
            {
                return str;
            }
            int length = sourceArray.Length;
            if (sourceArray.Length > (pStartIndex + pLength))
            {
                length = pLength + pStartIndex;
            }
            else
            {
                pLength = sourceArray.Length - pStartIndex;
                pTailString = "";
            }
            int num2 = pLength;
            int[] numArray = new int[pLength];
            byte[] destinationArray = null;
            int num3 = 0;
            for (int i = pStartIndex; i < length; i++)
            {
                if (sourceArray[i] > 0x7f)
                {
                    num3++;
                    if (num3 == 3)
                    {
                        num3 = 1;
                    }
                }
                else
                {
                    num3 = 0;
                }
                numArray[i] = num3;
            }
            if ((sourceArray[length - 1] > 0x7f) && (numArray[pLength - 1] == 1))
            {
                num2 = pLength + 1;
            }
            destinationArray = new byte[num2];
            Array.Copy(sourceArray, pStartIndex, destinationArray, 0, num2);
            return (Encoding.Default.GetString(destinationArray) + pTailString);
        }
        #endregion

        #region 截取字符长度
        /// <summary>
        /// 截取字符长度
        /// </summary>
        /// <param name="inputString">字符</param>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public static string CutString(string inputString, int len)
        {
            if (string.IsNullOrEmpty(inputString))
                return "";
            inputString = DropHtml(inputString);
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            string tempString = "";
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }

                try
                {
                    tempString += inputString.Substring(i, 1);
                }
                catch
                {
                    break;
                }

                if (tempLen > len)
                    break;
            }
            //如果截过则加上半个省略号 
            byte[] mybyte = System.Text.Encoding.Default.GetBytes(inputString);
            if (mybyte.Length > len)
                tempString += "…";
            return tempString;
        }

        public static string DropHtml(string htmlstring)
        {
            if (string.IsNullOrEmpty(htmlstring)) return "";
            //删除脚本  
            htmlstring = Regex.Replace(htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML  
            htmlstring = Regex.Replace(htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);

            htmlstring = Regex.Replace(htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            htmlstring = htmlstring.Replace("<", "");
            htmlstring = htmlstring.Replace(">", "");
            htmlstring = htmlstring.Replace("\r\n", "");
            //htmlstring = HttpContext.Current.Server.HtmlEncode(htmlstring).Trim(); 
            return htmlstring;
        }
        #endregion

        #region 得到一个汉字的拼音第一个字母，如果是一个英文字母则直接返回大写字母
        /// <summary>
        /// 得到一个汉字的拼音第一个字母，如果是一个英文字母则直接返回大写字母
        /// </summary>
        /// <param name="CnChar">单个汉字</param>
        /// <returns>单个大写字母</returns>
        public static string GetCharSpellCode(string CnChar)
        {
            long iCnChar;
            byte[] ZW = Encoding.Default.GetBytes(CnChar);

            //如果是字母，则直接返回首字母
            if (ZW.Length == 1)
            {
                return CutString(CnChar.ToUpper(), 1);
            }
            else
            {
                int i1 = (short)(ZW[0]);
                int i2 = (short)(ZW[1]);
                iCnChar = i1 * 256 + i2;
            }
            // iCnChar match the constant
            if ((iCnChar >= 45217) && (iCnChar <= 45252))
            {
                return "A";
            }
            else if ((iCnChar >= 45253) && (iCnChar <= 45760))
            {
                return "B";
            }
            else if ((iCnChar >= 45761) && (iCnChar <= 46317))
            {
                return "C";
            }
            else if ((iCnChar >= 46318) && (iCnChar <= 46825))
            {
                return "D";
            }
            else if ((iCnChar >= 46826) && (iCnChar <= 47009))
            {
                return "E";
            }
            else if ((iCnChar >= 47010) && (iCnChar <= 47296))
            {
                return "F";
            }
            else if ((iCnChar >= 47297) && (iCnChar <= 47613))
            {
                return "G";
            }
            else if ((iCnChar >= 47614) && (iCnChar <= 48118))
            {
                return "H";
            }
            else if ((iCnChar >= 48119) && (iCnChar <= 49061))
            {
                return "J";
            }
            else if ((iCnChar >= 49062) && (iCnChar <= 49323))
            {
                return "K";
            }
            else if ((iCnChar >= 49324) && (iCnChar <= 49895))
            {
                return "L";
            }
            else if ((iCnChar >= 49896) && (iCnChar <= 50370))
            {
                return "M";
            }
            else if ((iCnChar >= 50371) && (iCnChar <= 50613))
            {
                return "N";
            }
            else if ((iCnChar >= 50614) && (iCnChar <= 50621))
            {
                return "O";
            }
            else if ((iCnChar >= 50622) && (iCnChar <= 50905))
            {
                return "P";
            }
            else if ((iCnChar >= 50906) && (iCnChar <= 51386))
            {
                return "Q";
            }
            else if ((iCnChar >= 51387) && (iCnChar <= 51445))
            {
                return "R";
            }
            else if ((iCnChar >= 51446) && (iCnChar <= 52217))
            {
                return "S";
            }
            else if ((iCnChar >= 52218) && (iCnChar <= 52697))
            {
                return "T";
            }
            else if ((iCnChar >= 52698) && (iCnChar <= 52979))
            {
                return "W";
            }
            else if ((iCnChar >= 52980) && (iCnChar <= 53640))
            {
                return "X";
            }
            else if ((iCnChar >= 53689) && (iCnChar <= 54480))
            {
                return "Y";
            }
            else if ((iCnChar >= 54481) && (iCnChar <= 55289))
            {
                return "Z";
            }
            else
                return ("?");

        }
        #endregion

        #region 获得IP地址
        /// <summary>
        /// 获得IP地址
        /// </summary>
        /// <returns>字符串数组</returns>
        public static string GetIp()
        {
            var ip = httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].ToString();
            if (string.IsNullOrEmpty(ip))
            {
                ip = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }
        #endregion

        #region 获得当前访问的URL地址
        /// <summary>
        /// 获得当前访问的URL地址
        /// </summary>
        /// <returns>字符串数组</returns>
        public static string GetUrl()
        {
            //HttpContextAccessor _context = new HttpContextAccessor();
            return httpContextAccessor.HttpContext.Request.Path.ToString();
        }
        #endregion

        #region 分割字符串
        public static string[] SplitString(string strContent, char strSplit)
        {
            if (!string.IsNullOrEmpty(strContent))
            {
                return strContent.Split(new char[] { strSplit }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                return new string[0] { };
            }
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (!string.IsNullOrEmpty(strContent))
            {
                if (strContent.IndexOf(strSplit, StringComparison.Ordinal) < 0)
                    return new string[] { strContent };

                return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            else
                return new string[0] { };
        }
        #endregion

        #region 生成随机字母或数字

        private static readonly Random Random = new Random();

        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <returns></returns>
        public static string Number(int length)
        {
            return Number(length, false);
        }

        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <param name="sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns></returns>
        public static string Number(int length, bool sleep)
        {
            if (sleep)
                System.Threading.Thread.Sleep(2);
            string result = "";

            for (int i = 0; i < length; i++)
            {
                result += Random.Next(10).ToString();
            }
            return result;
        }


        /// <summary>
        /// 根据日期和随机码生成订单号
        /// </summary>
        /// <returns></returns>
        public static string GetOrderNumber()
        {
            string num = DateTime.Now.ToString("yyyyMMddHHmmssms"); //yyyyMMddHHmmssms
            return num + Number(2);
        }

        #endregion

        /// <summary>
        /// 时间戳转换为日期（时间戳单位秒）
        /// </summary>
        /// <param name="TimeStamp"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(long timeStamp)
        {
            var dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = new TimeSpan(timeStamp);
            return dtStart.Add(toNow);
            //var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            //return start.AddMilliseconds(timeStamp).AddHours(8);
        }
        /// <summary>
        /// 日期转换为时间戳（时间戳单位秒）
        /// </summary>
        /// <param name="TimeStamp"></param>
        /// <returns></returns>
        public static long ConvertToTimeStamp(DateTime time)
        {
            DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(time.AddHours(-8) - Jan1st1970).TotalMilliseconds;
        }

        #region 根据身份证获取信息

        /// <summary>
        /// 身份证号
        /// </summary>
        /// <param name="IDCard"></param>
        /// <returns></returns>
        public static IDCardInfo GetInfoFromIDCard(string IDCard)
        {
            var result = new IDCardInfo { IsSuccess = false };
            //获取得到输入的身份证号码
            string identityCard = IDCard.Trim();
            try
            {

                if (string.IsNullOrEmpty(identityCard))
                {
                    //身份证号码不能为空，如果为空返回
                    result.Message = "身份证号码不能为空";
                    return result;
                }
                else
                {
                    //身份证号码只能为15位或18位其它不合法
                    if (identityCard.Length != 15 && identityCard.Length != 18)
                    {
                        result.Message = "身份证号码为15位或18位，请检查";
                        return result;
                    }
                }

                string birthday = "";
                string sex = "";

                //处理18位的身份证号码从号码中得到生日和性别代码
                if (identityCard.Length == 18)
                {
                    birthday = identityCard.Substring(6, 4) + "-" + identityCard.Substring(10, 2) + "-" + identityCard.Substring(12, 2);
                    sex = identityCard.Substring(14, 3);
                }
                //处理15位的身份证号码从号码中得到生日和性别代码
                if (identityCard.Length == 15)
                {
                    birthday = "19" + identityCard.Substring(6, 2) + "-" + identityCard.Substring(8, 2) + "-" + identityCard.Substring(10, 2);
                    sex = identityCard.Substring(12, 3);
                }
                result.birthday = DateTime.Parse(birthday);
                //性别代码为偶数是女性奇数为男性
                if (int.Parse(sex) % 2 == 0)
                {
                    result.sex = "女";
                }
                else
                {
                    result.sex = "男";
                }

                //年龄
                DateTime nowDateTime = DateTime.Now;
                int age = nowDateTime.Year - result.birthday.Year;
                //再考虑月、天的因素
                if (nowDateTime.Month < result.birthday.Month || (nowDateTime.Month == result.birthday.Month && nowDateTime.Day < result.birthday.Day))
                {
                    age--;
                }
                result.age = short.Parse(age.ToString());

                var constellation = "";
                var birthstone = "";
                CalcConstellation(result.birthday, out constellation, out birthstone);
                result.constellation = constellation;
                result.zodiac = LunarYearAnimal(result.birthday);

                //月
                int month = nowDateTime.Month - result.birthday.Month;
                if (month < 0)
                {
                    month += 12;
                }
                //再考虑天的因素
                if (nowDateTime.Day < result.birthday.Day)
                {
                    month--;
                }
                result.month = short.Parse(month.ToString());

                //天
                int days = nowDateTime.Day - result.birthday.Day;
                if (days < 0)
                {
                    days += DateTime.DaysInMonth(nowDateTime.AddMonths(-1).Year, nowDateTime.AddMonths(-1).Month);
                }
                result.day = short.Parse(days.ToString());

                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = "身份证号码输入有误";
                return result;
            }
        }

        /// <summary>
        /// 根据公历获取农历日期
        /// </summary>
        /// <param name="datetime">公历日期</param>
        /// <returns></returns>
        public static string GetChineseDateTime(DateTime datetime)
        {
            System.Globalization.ChineseLunisolarCalendar chinseCaleander = new System.Globalization.ChineseLunisolarCalendar();
            int lyear = chinseCaleander.GetYear(datetime);
            int lmonth = chinseCaleander.GetMonth(datetime);
            int lday = chinseCaleander.GetDayOfMonth(datetime);

            //获取闰月， 0 则表示没有闰月
            int leapMonth = chinseCaleander.GetLeapMonth(lyear);

            bool isleap = false;

            if (leapMonth > 0)
            {
                if (leapMonth == lmonth)
                {
                    //闰月
                    isleap = true;
                    lmonth--;
                }
                else if (lmonth > leapMonth)
                {
                    lmonth--;
                }
            }

            return string.Concat(GetLunisolarYear(lyear), "年", isleap ? "闰" : string.Empty, GetLunisolarMonth(lmonth), "月", GetLunisolarDay(lday));
        }

        #region 获取节日信息

        /// <summary>
        /// 获取指定日期节日信息
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFestival(DateTime datetime)
        {
            var list = new List<string>();
            System.Globalization.ChineseLunisolarCalendar chinseCaleander = new System.Globalization.ChineseLunisolarCalendar();
            //int lyear = chinseCaleander.GetYear(datetime);
            int lmonth = chinseCaleander.GetMonth(datetime);
            int lday = chinseCaleander.GetDayOfMonth(datetime);

            var lunar_date = lmonth.ToString().PadLeft(2, '0') + lday.ToString().PadLeft(2, '0');
            var lunar_festival = lunar.Where(s => s.Substring(0, 4).Contains(lunar_date)).FirstOrDefault();
            if (!string.IsNullOrEmpty(lunar_festival))
            {
                list.Add(lunar_festival.Split(' ')[1]);
            }

            var solar_date = datetime.ToString("MMdd");
            var solar_festival = solar.Where(s => s.Substring(0, 4).Contains(solar_date)).FirstOrDefault();
            if (!string.IsNullOrEmpty(solar_festival))
            {
                list.Add(solar_festival.Split(' ')[1]);
            }
            return list;
        }

        /// <summary>
        /// 国历节日 *表示节假日
        /// </summary>
        public static readonly string[] solar = new string[]
        {
                "0101 元旦",
                "0214 情人节",
                "0308 妇女节",
                "0312 植树节",
                //"0315 消费者权益日",
                //"0321 世界森林日、世界儿歌日",
                //"0322 世界水日",
                //"0323 世界气象日",
                //"0324 世界防治结核病日",

                "0401 愚人节",
                //"0407 世界卫生日",
                //"0422 世界地球日",

                "0501 劳动节",
                "0504 青年节",
                //"0505 碘缺乏病防治日",
                //"0508 世界红十字日",
                //"0512 国际护士节",
                //"0515 国际家庭日",
                //"0517 世界电信日",
                //"0518 国际博物馆日",
                //"0520 全国学生营养日",
                //"0523 国际牛奶日",
                //"0531 世界无烟日",

                "0601 儿童节",
                "0605 世界环境日",
                "0606 全国爱眼日",
                //"0616 防治荒漠化和干旱日",
                //"0623 国际奥林匹克日",
                //"0625 全国土地日",
                //"0626 国际反毒品日",

                "0701 建党节",// 香港回归纪念 国际建筑日
                //"0707 中国人民抗日战争纪念日",
                //"0711 世界人口日",

                "0801 建军节",
                //"0808 父亲节",

                //"0908 国际扫盲日",
                //"0909 毛泽东逝世纪念",
                "0910 教师节",
                //"0916 国际臭氧层保护日",
                "0920 国际爱牙日",
                //"0927 世界旅游日",
                //"0928 孔子诞辰",

                "1001 国庆节",// 国际音乐日
                //"1004 世界动物日",
                //"1006 老人节",
                //"1008 全国高血压日 世界视觉日",
                //"1009 世界邮政日",
                //"1015 国际盲人节",
                //"1016 世界粮食日",
                //"1017 世界消除贫困日",
                //"1024 联合国日",

                //"1108 中国记者日",
                //"1109 消防宣传日",
                //"1112 孙中山诞辰纪念",
                //"1114 世界糖尿病日",
                //"1117 国际大学生节",

                //"1201 世界艾滋病日",
                //"1203 世界残疾人日",
                //"1209 世界足球日",
                //"1220 澳门回归纪念",
                "1224 平安夜",
                "1225 圣诞节",
                //"1226 毛泽东诞辰纪念",
                //"1229 国际生物多样性日"
                };

        /// <summary>
        /// 农历节日 *表示节假日
        /// </summary>
        public static readonly string[] lunar = new string[] { "0101 春节", "0115 元宵节", "0505 端午节", "0707 七夕情人节", "0815 中秋节", "0909 重阳节", "1208 腊八节", "1223 小年" };//, "0715 中元节"

        #endregion

        /// <summary>  
        /// 阴历年生肖  
        /// </summary>  
        public static string LunarYearAnimal(DateTime date)
        {
            System.Globalization.ChineseLunisolarCalendar chinseCaleander = new System.Globalization.ChineseLunisolarCalendar();
            string TreeYear = "鼠牛虎兔龙蛇马羊猴鸡狗猪";
            int intYear = chinseCaleander.GetSexagenaryYear(date);
            return TreeYear.Substring(chinseCaleander.GetTerrestrialBranch(intYear) - 1, 1);
        }

        #region 农历年

        /// <summary>
        /// 十天干
        /// </summary>
        private static string[] tiangan = { "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸" };

        /// <summary>
        /// 十二地支
        /// </summary>
        private static string[] dizhi = { "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };

        /// <summary>
        /// 十二生肖
        /// </summary>
        private static string[] shengxiao = { "鼠", "牛", "虎", "免", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };


        /// <summary>
        /// 返回农历天干地支年 
        /// </summary>
        /// <param name="year">农历年</param>
        /// <returns></returns>
        public static string GetLunisolarYear(int year)
        {
            if (year > 3)
            {
                int tgIndex = (year - 4) % 10;
                int dzIndex = (year - 4) % 12;

                return string.Concat(tiangan[tgIndex], dizhi[dzIndex], "[", shengxiao[dzIndex], "]");

            }

            throw new ArgumentOutOfRangeException("无效的年份!");
        }


        #endregion

        #region 农历月

        /// <summary>
        /// 农历月
        /// </summary>
        private static string[] months = { "正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二(腊)" };


        /// <summary>
        /// 返回农历月
        /// </summary>
        /// <param name="month">月份</param>
        /// <returns></returns>
        public static string GetLunisolarMonth(int month)
        {
            if (month < 13 && month > 0)
            {
                return months[month - 1];
            }

            throw new ArgumentOutOfRangeException("无效的月份!");
        }


        #endregion

        #region 农历日

        /// <summary>
        /// 
        /// </summary>
        private static string[] days1 = { "初", "十", "廿", "三" };

        /// <summary>
        /// 日
        /// </summary>
        private static string[] days = { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };


        /// <summary>
        /// 返回农历日
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static string GetLunisolarDay(int day)
        {
            if (day > 0 && day < 32)
            {
                if (day != 20 && day != 30)
                {
                    return string.Concat(days1[(day - 1) / 10], days[(day - 1) % 10]);
                }
                else
                {
                    return string.Concat(days[(day - 1) / 10], days1[1]);
                }
            }

            throw new ArgumentOutOfRangeException("无效的日!");
        }

        #endregion

        #region 根据指定阳历日期计算星座＆诞生石
        /// <summary>  
        /// 根据指定阳历日期计算星座＆诞生石  
        /// </summary>  
        /// <param name="date">指定阳历日期</param>  
        /// <param name="constellation">星座</param>  
        /// <param name="birthstone">诞生石</param>  
        public static void CalcConstellation(DateTime date, out string constellation, out string birthstone)
        {
            int i = Convert.ToInt32(date.ToString("MMdd"));
            int j;
            if (i >= 321 && i <= 419)
                j = 0;
            else if (i >= 420 && i <= 520)
                j = 1;
            else if (i >= 521 && i <= 621)
                j = 2;
            else if (i >= 622 && i <= 722)
                j = 3;
            else if (i >= 723 && i <= 822)
                j = 4;
            else if (i >= 823 && i <= 922)
                j = 5;
            else if (i >= 923 && i <= 1023)
                j = 6;
            else if (i >= 1024 && i <= 1121)
                j = 7;
            else if (i >= 1122 && i <= 1221)
                j = 8;
            else if (i >= 1222 || i <= 119)
                j = 9;
            else if (i >= 120 && i <= 218)
                j = 10;
            else if (i >= 219 && i <= 320)
                j = 11;
            else
            {
                constellation = "未知星座";
                birthstone = "未知诞生石";
                return;
            }
            constellation = Constellations[j];
            birthstone = BirthStones[j];
            #region 星座划分  
            //白羊座：   3月21日------4月19日     诞生石：   钻石     
            //金牛座：   4月20日------5月20日   诞生石：   蓝宝石     
            //双子座：   5月21日------6月21日     诞生石：   玛瑙     
            //巨蟹座：   6月22日------7月22日   诞生石：   珍珠     
            //狮子座：   7月23日------8月22日   诞生石：   红宝石     
            //处女座：   8月23日------9月22日   诞生石：   红条纹玛瑙     
            //天秤座：   9月23日------10月23日     诞生石：   蓝宝石     
            //天蝎座：   10月24日-----11月21日     诞生石：   猫眼石     
            //射手座：   11月22日-----12月21日   诞生石：   黄宝石     
            //摩羯座：   12月22日-----1月19日   诞生石：   土耳其玉     
            //水瓶座：   1月20日-----2月18日   诞生石：   紫水晶     
            //双鱼座：   2月19日------3月20日   诞生石：   月长石，血石    
            #endregion
        }

        /// <summary>
        /// 星座
        /// </summary>
        public static readonly string[] Constellations
        = new string[] { "白羊座", "金牛座", "双子座", "巨蟹座", "狮子座", "处女座", "天秤座", "天蝎座", "射手座", "摩羯座", "水瓶座", "双鱼座" };

        /// <summary>
        /// 诞生石
        /// </summary>
        public static readonly string[] BirthStones
            = new string[] { "钻石", "蓝宝石", "玛瑙", "珍珠", "红宝石", "红条纹玛瑙", "蓝宝石", "猫眼石", "黄宝石", "土耳其玉", "紫水晶", "月长石，血石" };
        #endregion

        /// <summary>
        /// 身份证信息
        /// </summary>
        public class IDCardInfo
        {
            /// <summary>
            /// 是否成功
            /// </summary>
            public bool IsSuccess { get; set; }

            /// <summary>
            /// 信息
            /// </summary>

            public string Message { get; set; }

            private System.String _sex;
            /// <summary>
            /// 性别
            /// </summary>
            public System.String sex { get { return this._sex; } set { this._sex = value?.Trim(); } }

            private System.Int16 _age;
            /// <summary>
            /// 年龄
            /// </summary>
            public System.Int16 age { get { return this._age; } set { this._age = value; } }

            private System.Int16 _month;
            /// <summary>
            /// 月
            /// </summary>
            public System.Int16 month { get { return this._month; } set { this._month = value; } }

            private System.Int16 _day;
            /// <summary>
            /// 日
            /// </summary>
            public System.Int16 day { get { return this._day; } set { this._day = value; } }

            /// <summary>
            /// 生日
            /// </summary>
            public System.DateTime birthday { get; set; }

            private System.String _constellation;
            /// <summary>
            /// 星座
            /// </summary>
            public System.String constellation { get { return this._constellation; } set { this._constellation = value?.Trim(); } }

            private System.String _zodiac;
            /// <summary>
            /// 生肖
            /// </summary>
            public System.String zodiac { get { return this._zodiac; } set { this._zodiac = value?.Trim(); } }
        }

        #endregion

        /// <summary>
        /// 集合转为字符串
        /// </summary>
        /// <param name="list">集合</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static string ListToString(List<string> list, char separator)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                sb.Append(list[i]).Append(separator);
            }
            return sb.ToString().TrimEnd(separator);
        }

        /// <summary>
        /// 返回星期
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static Week GetWeek(DateTime date)
        {
            var sxWeek = Convert.ToInt16(date.DayOfWeek);
            var Day = date.Day.ToString().PadLeft(2, '0');
            var Date = date.ToString("yyyy-MM-dd");
            var Week = new Week { WeekNo = sxWeek, day = Day, Date = Date };
            string WeekTxt;
            switch (sxWeek)
            {
                case 1:
                    WeekTxt = "星期一";
                    break;
                case 2:
                    WeekTxt = "星期二";
                    break;
                case 3:
                    WeekTxt = "星期三";
                    break;
                case 4:
                    WeekTxt = "星期四";
                    break;
                case 5:
                    WeekTxt = "星期五";
                    break;
                case 6:
                    WeekTxt = "星期六";
                    break;
                default:
                    WeekTxt = "星期日";
                    break;
            }
            Week.WeekTxt = WeekTxt;
            return Week;
        }

        public class Week
        {
            public string Date { get; set; }
            public string WeekTxt { get; set; }
            public short WeekNo { get; set; }
            public string day { get; set; }
        }

        #region 生成二维码

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="txt">值</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetQrCode(string txt, dynamic type)
        {
            var generator = new QRCodeGenerator();
            var codeData = generator.CreateQrCode(txt, QRCodeGenerator.ECCLevel.M, true);
            var qrcode = new QRCode(codeData);

            string currentDirectory = Path.GetDirectoryName(type.Assembly.Location);
            string file = Path.Combine(currentDirectory + @"\wwwroot\Ico\ico.ico");
            Bitmap icon = new Bitmap(file);
            var qrImage = qrcode.GetGraphic(100, Color.Black, Color.White, icon, 20, 10);
            var ms = new MemoryStream();

            //图片格式指定为png
            qrImage.Save(ms, ImageFormat.Jpeg);
            byte[] bytes = ms.GetBuffer();
            ms.Close();
            return Convert.ToBase64String(bytes);
        }

        #endregion

        /// <summary>
        /// 获取头像
        /// </summary>
        /// <param name="image_url"></param>
        /// <param name="sex"></param>
        /// <returns></returns>
        public static string GetImage_url(string image_url, string sex)
        {
            var url = image_url;
            if (string.IsNullOrEmpty(url))
            {
                if (sex == "女")
                {
                    url = "Public/woman.jpg";
                }
                else
                {
                    url = "Public/man.jpg";
                }
            }
            return url;
        }

        public static string ToBase64String(string json, bool add_str = false, string start_str = "")
        {
            var str = System.Text.Encoding.Default.GetBytes(json);
            var base64 = Convert.ToBase64String(str);
            if (add_str)
            {
                base64 = MetarnetRegex.Encrypt(start_str) + base64;
            }
            return base64;
        }
    }
}
