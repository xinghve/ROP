using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Crm
{
    /// <summary>
    /// 档案查询
    /// </summary>
    public class ArchivesSearch: c_archives_supplement
    {
        /// <summary>
        /// 名称/拼音码
        /// </summary>
        public string name { get; set; }

        ///// <summary>
        ///// 偏好
        ///// </summary>
        //public string preference { get; set; }

        /// <summary>
        /// 性别编码
        /// </summary>
        public string sexCode { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string idCard { get; set; }

        /// <summary>
        /// 年龄段开始
        /// </summary>
        public string ageStart { get; set; }

        /// <summary>
        /// 年龄段结束
        /// </summary>
        public string ageEnd { get; set; }

        /// <summary>
        /// 婚姻状况编码
        /// </summary>
        public string maritalStatusCode { get; set; }

        /// <summary>
        /// 生肖编码
        /// </summary>
        public string zodiacCode { get; set; }

        /// <summary>
        /// 星座编码
        /// </summary>
        public string constellationCode { get; set; }

        ///// <summary>
        ///// 诊疗方式编码
        ///// </summary>
        //public string favourWayCode { get; set; }

        ///// <summary>
        ///// 消费习惯编码
        ///// </summary>
        //public string consumeHabitCode { get; set; }

        /// <summary>
        /// 状态（1=正常，0=黑名单，-1=所有）
        /// </summary>
        public int state { get; set; }

        /// <summary>
        /// 是否本人负责
        /// </summary>
        public bool isMe { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string order { get; set; }

        /// <summary>
        /// 排序方式(0：升序 1：降序)
        /// </summary>
        public int orderType { get; set; }

        /// <summary>
        /// 单页数据条数
        /// </summary>
        public int limit { get; set; }

        /// <summary>
        /// 查询第几页（第一页为1）
        /// </summary>
        public int page { get; set; }

        private System.Int32 _storeId = -1;
        /// <summary>
        /// 门店ID
        /// </summary>
        public System.Int32 storeId { get { return this._storeId; } set { this._storeId = value; } }

        ///// <summary>
        /////标签（1,2,3...）
        ///// </summary>
        //public string tags { get; set; }

        private System.Int32? _to_director_id;
        /// <summary>
        /// 现负责人ID
        /// </summary>
        public System.Int32? to_director_id { get { return this._to_director_id; } set { this._to_director_id = value ?? default(System.Int32); } }
    }
}
