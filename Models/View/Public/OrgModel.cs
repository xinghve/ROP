using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 机构启用，禁用
    /// </summary>
    public class OrgModel
    {
        private System.Int16 _state = 0;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16 state { get { return this._state; } set { this._state = value; } }

        private System.Int32 _orgId = 0;
        /// <summary>
        /// 机构id
        /// </summary>
        public System.Int32 orgId { get { return this._orgId; } set { this._orgId = value; } }

      
    }

    /// <summary>
    /// 获取机构简介
    /// </summary>
    public class OrgIntroduceModel
    {
        /// <summary>
        /// 图片地址
        /// </summary>
        public List<string> imgUrlList { get; set; }

        /// <summary>
        /// 机构简介
        /// </summary>
        public string content { get; set; }
    }

    /// <summary>
    /// 提现上下线
    /// </summary>
    public class CashLower
    {
        /// <summary>
        /// 提现下限
        /// </summary>
        public decimal cash_lower { get; set; }

        /// <summary>
        /// 提现上限
        /// </summary>
        public decimal cash_uper { get; set; }
    }
    

}
