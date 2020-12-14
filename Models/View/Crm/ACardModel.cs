using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Crm
{
    /// <summary>
    /// 会员卡实体
    /// </summary>
    public class ACardModel : SearchMl
    {
        private System.Int32 _levelId = 0;
        /// <summary>
        /// 等级id
        /// </summary>
        public System.Int32 levelId { get { return this._levelId; } set { this._levelId = value; } }

        private System.String _phone;
        /// <summary>
        /// 手机号
        /// </summary>
        public System.String phone { get { return this._phone; } set { this._phone = value?.Trim(); } }

        private System.String _startintegral;
        /// <summary>
        /// 剩余开始积分
        /// </summary>
        public System.String startintegral { get { return this._startintegral; } set { this._startintegral = value?.Trim(); } }

        private System.String _endintegral;
        /// <summary>
        /// 剩余结束积分
        /// </summary>
        public System.String endintegral { get { return this._endintegral; } set { this._endintegral = value?.Trim(); } }

        private bool _is_me = false;
        /// <summary>
        /// 自己
        /// </summary>
        public bool is_me { get { return this._is_me; } set { this._is_me = value; } }
    }

    /// <summary>
    /// 打印model
    /// </summary>
    public class PrintModel
    {
        private System.Int32 _levelId = 0;
        /// <summary>
        /// 等级id
        /// </summary>
        public System.Int32 levelId { get { return this._levelId; } set { this._levelId = value; } }

        private System.String _phone;
        /// <summary>
        /// 手机号
        /// </summary>
        public System.String phone { get { return this._phone; } set { this._phone = value?.Trim(); } }

        private System.String _name;
        /// <summary>
        /// 搜索名
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.DateTime? _startTime;
        /// <summary>
        /// 开始时间
        /// </summary>
        public System.DateTime? startTime { get { return this._startTime; } set { this._startTime = value; } }

        private System.DateTime? _endTime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public System.DateTime? endTime { get { return this._endTime; } set { this._endTime = value; } }

        private System.Int32 _storeId = 0;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32 storeId { get { return this._storeId; } set { this._storeId = value; } }


    }

    /// <summary>
    /// 充值记录分页
    /// </summary>
    public class Recharge : r_recharge
    {
        /// <summary>
        /// 虚拟卡
        /// </summary>
        public string virtual_no { get; set; }
    }

}
