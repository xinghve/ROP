using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    public class LoginInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 是否管理员
        /// </summary>
        public bool is_admin { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string user_name { get; set; }

        /// <summary>
        /// 门店列表
        /// </summary>
        public List<LoginStoreList> stroeMark { get;set;}

        /// <summary>
        /// 到期天数
        /// </summary>
        public int day { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        public int org_id { get; set; }

        /// <summary>
        /// 人员性质
        /// </summary>
        public List<short> nature_list { get; set; }

        /// <summary>
        /// 医生科室
        /// </summary>
        public List<Dept> dept_list { get; set; }
    }
}
