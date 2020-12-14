using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Mobile
{
    /// <summary>
    /// 会员信息
    /// </summary>
    public class MobileArchives
    {
        /// <summary>
        /// Id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 门店Id
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 负责人ID
        /// </summary>
        public int to_director_id { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        public string to_director { get; set; }

        /// <summary>
        /// 分销人员ID
        /// </summary>
        public int distributor_id { get; set; }

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
        /// 饮料
        /// </summary>
        public string drink { get; set; }

        /// <summary>
        /// 水果
        /// </summary>
        public string fruits { get; set; }

        /// <summary>
        /// 饮食
        /// </summary>
        public string foods { get; set; }

        /// <summary>
        /// 吸烟
        /// </summary>
        public string smoke { get; set; }

        /// <summary>
        /// 习惯
        /// </summary>
        public string habit { get; set; }

        /// <summary>
        /// 爱好
        /// </summary>
        public string hobby { get; set; }
    }
}
