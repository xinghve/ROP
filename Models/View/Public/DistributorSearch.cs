using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 分销人员查询
    /// </summary>
    public class DistributorSearch : Search
    {
        /// <summary>
        /// 查询条件（姓名/拼音/身份证/手机号）
        /// </summary>
        public string search_condition { get; set; }

        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string sex_name { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string director { get; set; }

        private System.DateTime? _startTime;
        /// <summary>
        /// 开始时间
        /// </summary>
        public System.DateTime? startTime { get { return this._startTime; } set { this._startTime = value ?? default(System.DateTime); } }

        private System.DateTime? _endTime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public System.DateTime? endTime { get { return this._endTime; } set { this._endTime = value; } }

    }
    /// <summary>
    /// 分销分页实体
    /// </summary>
    public class DistributorPageModel:p_distributor
    {
        /// <summary>
        /// 门店名
        /// </summary>
        public string store_name { get; set; }

        /// <summary>
        /// 负责人手机号
        /// </summary>
        public string dir_phone { get; set; }
        
    }

    /// <summary>
    /// 启用禁用分销人员实体
    /// </summary>
    public class DistributorModify
    {
        private System.Int32 _id;
        /// <summary>
        /// 分销人员id
        /// </summary>
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店Id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        /// <summary>
        /// 分销人员状态0禁用1启用
        /// </summary>
        public int distributor_state { get; set; }
    }
}
