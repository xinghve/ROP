using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Tools.Aliyun
{
    /// <summary>
    /// 短信
    /// </summary>
    public class SMS
    {
        #region 发送短信
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name = "accountEntity" > 账号信息 </param>
        /// < param name="code">模板code</param>
        /// <param name = "values" > 参数键值对 </param>
        /// < param name="recivers">接收者号码集(string[] recivers)</param>
        /// <param name = "message" > 异常描述/Code </param>
        /// <returns>成功/失败 ，true/false</returns>
        public static bool sendSMS(AccountEntity accountEntity, string code, string values, string recivers, out string message)
        {
            message = string.Empty;
            string ret = "false";
            
            if (accountEntity != null)
            {
                //ret = YunpianSMS.sendSMS(accountEntity.AccessKeyId, accountEntity.SecretAccessKey, accountEntity.EndPoint, accountEntity.TopicName, accountEntity.FreeSignName, templetEnity.CMbcode, recivers, values, out message);

                //var parms = new Dictionary<string, string>();
                //parms.Add(Constants.EXTEND, "短信验证码");
                //parms.Add(Constants.SMS_FREE_SIGN_NAME, accountEntity.FreeSignName);
                //parms.Add(Constants.REC_NUM, recivers.Trim());
                //Dictionary<string, string> param = new Dictionary<string, string>();
                //if (values != null && values.Count > 0)
                //{
                //    foreach (var item in values)
                //    {
                //        param.Add(item.Key, item.Value);
                //    }
                //}
                //parms.Add(Constants.SMS_PARAM, JsonConvert.SerializeObject(param));
                //parms.Add(Constants.SMS_TEMPLATE_CODE, templetEnity.CMbcode);

                //ret = AliyunSms.SendSms("http://192.168.20.124/api/Sms", "appKey", "appSecret", DateTime.Now, parms).Alibaba_Aliqin_Fc_Sms_Num_Send_Response.Result.Success;
                ret = AliyunSms.SendSms(accountEntity.AccessKeyId, accountEntity.SecretAccessKey, recivers, accountEntity.FreeSignName, code, values);
                var rt = JsonConvert.DeserializeObject<dynamic>(ret);
                if (rt["Message"] == "OK")
                {
                    //var sendCode = string.Empty;
                    //if (ucase == useCase.短信验证码 || ucase == useCase.机构注册验证码)
                    //{
                    //    message = values["code"];
                    //}
                    //SMS_FSJL smsRecord = new SMS_FSJL();
                    //smsRecord.C_SYCJ = ucase.ToString();
                    //smsRecord.C_FSSJH = recivers.Trim();
                    //smsRecord.D_FSSJ = DateTime.Now;
                    //smsRecord.C_CWMS = message;
                    //smsRecord.I_SMS_MBID = templetid;
                    //smsRecord.I_SMS_ZHID = accountid;
                    //smsRecord.I_ZT = 1;
                    //smsRecord.C_Code = code.Trim();
                    //smsRecord.I_JGID = I_JGID;
                    //HtmlController.DbSugar.Insertable(smsRecord).ExecuteCommand();
                    //foreach (var phone in recivers)
                    //{
                    //    SmsFsjl smsRecord = new SmsFsjl();
                    //    smsRecord.CSycj = ucase.ToString();
                    //    smsRecord.CFssjh = phone.Trim();
                    //    smsRecord.DFssj = DateTime.Now;
                    //    smsRecord.CCwms = message;
                    //    smsRecord.ISmsMbid = templetid;
                    //    smsRecord.ISmsZhid = accountid;
                    //    smsRecord.IZt = 1;
                    //    smsRecord.CCode = code.Trim();
                    //    smsRecord.IJgid = I_JGID;
                    //    BaseController.DbYYYPT.SmsFsjl.Add(smsRecord);
                    //    BaseController.DbYYYPT.SaveChanges();
                    //}
                }
                else
                {
                    message = rt["Message"];
                    return false;
                }

            }
            else
            {
                message = "参数不正确！";
            }
            return true;
        }

        #endregion

        #region 验证验证码
        /// <summary>
        /// 验证验证码
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="code">验证码</param>
        /// <param name="ucase">使用场景</param>
        /// <param name="expire">有效时间（单位：秒）</param>
        /// <returns></returns>
        //public static bool validSMS(string phone, string code, useCase ucase, int expire = 60)
        //{
        //    bool ret = true;
        //    string useCase = ucase.ToString();
        //    var entity = HtmlController.DbSugar.Queryable<SMS_FSJL>().Where(e => e.C_FSSJH.Trim() == phone.Trim() && e.C_Code == code.Trim() && e.C_SYCJ == useCase).OrderBy(o => o.D_FSSJ, OrderByType.Desc).First();
        //    if (entity == null)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        DateTime sendTime = entity.D_FSSJ;
        //        double second = (DateTime.Now.Ticks - sendTime.Ticks) / 10000000.0;
        //        if (second > expire || second < 0)
        //        {
        //            return false;
        //        }

        //    }
        //    return ret;
        //}

        #endregion

        #region 账号Model
        public class AccountEntity
        {
            public string AccessKeyId { get; set; }
            public string SecretAccessKey { get; set; }
            public string FreeSignName { get; set; }


        }
        #endregion
    }
}
