using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Tools.Aliyun
{
    /// <summary>
    /// 阿里云短信
    /// </summary>
    public class AliyunSms
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Post(string url, string data, Encoding encoding)
        {
            try
            {
                HttpWebRequest req = WebRequest.CreateHttp(new Uri(url));
                req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                req.Method = "POST";
                req.Accept = "text/xml,text/javascript";
                req.ContinueTimeout = 60000;

                byte[] postData = encoding.GetBytes(data);
                Stream reqStream = req.GetRequestStreamAsync().Result;
                reqStream.Write(postData, 0, postData.Length);
                reqStream.Dispose();

                var rsp = (HttpWebResponse)req.GetResponseAsync().Result;
                var result = GetResponseAsString(rsp, encoding);
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static T Post<T>(string url, string data, Encoding encoding)
        {
            try
            {
                var result = Post(url, data, encoding);
                return JsonConvert.DeserializeObject<T>(result);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string BuildQuery(IDictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return null;
            }

            StringBuilder query = new StringBuilder();
            bool hasParam = false;

            foreach (KeyValuePair<string, string> kv in parameters)
            {
                string name = kv.Key;
                string value = kv.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                {
                    if (hasParam)
                    {
                        query.Append("&");
                    }

                    query.Append(name);
                    query.Append("=");
                    query.Append(WebUtility.UrlEncode(value));
                    hasParam = true;
                }
            }

            return query.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rsp"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            Stream stream = null;
            StreamReader reader = null;

            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);
                return reader.ReadToEnd();
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Dispose();
                if (stream != null) stream.Dispose();
                if (rsp != null) rsp.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="secret"></param>
        /// <param name="signMethod"></param>
        /// <returns></returns>
        public static string GetAlidayuSign(IDictionary<string, string> parameters, string secret, string signMethod)
        {
            //把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters, StringComparer.Ordinal);

            //把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder();
            if (Constants.SIGN_METHOD_MD5.Equals(signMethod))
            {
                query.Append(secret);
            }
            foreach (KeyValuePair<string, string> kv in sortedParams)
            {
                if (!string.IsNullOrEmpty(kv.Key) && !string.IsNullOrEmpty(kv.Value))
                {
                    query.Append(kv.Key).Append(kv.Value);
                }
            }

            //使用MD5/HMAC加密
            if (Constants.SIGN_METHOD_HMAC.Equals(signMethod))
            {
                return Hmac(query.ToString(), secret);
            }
            else
            {
                query.Append(secret);
                return Md5(query.ToString());
            }
        }

        public static string Hmac(string value, string key)
        {
            byte[] bytes;
            using (var hmac = new HMACMD5(Encoding.UTF8.GetBytes(key)))
            {
                bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(value));
            }
            StringBuilder result = new StringBuilder();
            foreach (byte t in bytes)
            {
                result.Append(t.ToString("X2"));
            }
            return result.ToString();
        }

        public static string Md5(string value)
        {
            byte[] bytes;
            using (var md5 = MD5.Create())
            {
                bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
            }
            var result = new StringBuilder();
            foreach (byte t in bytes)
            {
                result.Append(t.ToString("X2"));
            }
            return result.ToString();
        }

        public static SmsResultAli SendSms(string url, string appKey, string appSecret, DateTime timestamp, Dictionary<string, string> parsms)
        {
            var txtParams = new SortedDictionary<string, string>();
            txtParams.Add(Constants.METHOD, "alibaba.aliqin.fc.sms.num.send");
            txtParams.Add(Constants.VERSION, "2.0");
            txtParams.Add(Constants.SIGN_METHOD, Constants.SIGN_METHOD_HMAC);
            txtParams.Add(Constants.APP_KEY, appKey);
            txtParams.Add(Constants.FORMAT, "json");
            txtParams.Add(Constants.TIMESTAMP, timestamp.ToString(Constants.DATE_TIME_FORMAT));
            txtParams.Add(Constants.SMS_TYPE, "normal");
            foreach (var item in parsms)
            {
                txtParams.Add(item.Key, item.Value);
            }

            txtParams.Add(Constants.SIGN, GetAlidayuSign(txtParams, appSecret, Constants.SIGN_METHOD_HMAC));
            var result = Post<SmsResultAli>(url, BuildQuery(txtParams), Encoding.UTF8);

            return result;
        }

        public static string SendSms(string accessKeyId,string accessKeySecret,string PhoneNumbers,string SignName,string TemplateCode, string values)
        {
            // *** 需用户填写部分 ***

            //fixme 必填: 请参阅 https://ak-console.aliyun.com/ 取得您的AK信息

            //string accessKeyId = "your access key id";//你的accessKeyId，参考本文档步骤2
            //string accessKeySecret = "your access key secret";//你的accessKeySecret，参考本文档步骤2

            Dictionary<string, string> smsDict = new Dictionary<string, string>();

            //fixme 必填: 短信接收号码
            smsDict.Add("PhoneNumbers", PhoneNumbers);

            //fixme 必填: 短信签名，应严格按"签名名称"填写，请参考: https://dysms.console.aliyun.com/dysms.htm#/develop/sign
            smsDict.Add("SignName", SignName);

            //fixme 必填: 短信模板Code，应严格按"模板CODE"填写, 请参考: https://dysms.console.aliyun.com/dysms.htm#/develop/template

            smsDict.Add("TemplateCode", TemplateCode);

            // fixme 可选: 设置模板参数, 假如模板中存在变量需要替换则为必填项
            //Dictionary<string, string> param = new Dictionary<string, string>();
            //if (values != null && values.Count > 0)
            //{
            //    foreach (var item in values)
            //    {
            //        param.Add(item.Key, item.Value);
            //    }
            //}
            smsDict.Add("TemplateParam", values);

            //什么？Newtonsoft.Json也觉得重，那拼字符串好了
            //smsDict.Add("TemplateParam", "{\"appname\":\"微关爱\",\"appstorename\":\"小黑\"}");



            // *** 以下内容无需修改 ***
            smsDict.Add("RegionId", "cn-hangzhou");
            smsDict.Add("Action", "SendSms");
            smsDict.Add("Version", "2017-05-25");

            string domain = "dysmsapi.aliyuncs.com";//短信API产品域名（接口地址固定，无需修改）

            // 初始化SignatureHelper实例用于设置参数，签名以及发送请求
            var singnature = new SignatureHelper();

            return singnature.Request(accessKeyId, accessKeySecret, domain, smsDict);

        }

    }

    public class SmsResultAli
    {
        public SmsResponseALi Alibaba_Aliqin_Fc_Sms_Num_Send_Response { get; set; }
    }

    public class SmsResponseALi
    {
        public string Request_Id { get; set; }
        public SmsResponseResultAli Result { get; set; }
    }

    public class SmsResponseResultAli
    {
        public string Err_Code { get; set; }

        public string Model { get; set; }

        public bool Success { get; set; }
    }
}