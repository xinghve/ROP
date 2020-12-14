using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 短信群发
    /// </summary>
    public class SendSMSGroup
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        public int templateId { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        public int storeID { get; set; }

        /// <summary>
        /// 是否所有
        /// </summary>
        public bool isAll { get; set; }

        /// <summary>
        /// 模板对象json（{"code":"1111"}）
        /// </summary>
        public string toValues { get; set; }

        /// <summary>
        /// 发送内容
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public List<string> phoneList { get; set; }
    }
}
