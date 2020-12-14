using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Mobile
{
    /// <summary>
    /// 会员信息
    /// </summary>
    public class RegisterArchives
    {
        /// <summary>
        /// 集团Id
        /// </summary>
        public int org_id { get; set; }

        /// <summary>
        /// 门店Id
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 负责人Id
        /// </summary>
        public int to_director_id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        public string id_card { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string code { get; set; }
    }
}
