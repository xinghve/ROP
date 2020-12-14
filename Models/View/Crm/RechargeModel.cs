using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Crm
{
    /// <summary>
    /// 充值实体
    /// </summary>
    public class RechargeModel
    {
        private System.Int32 _Id = 0;
        /// <summary>
        /// id
        /// </summary>
        public System.Int32 Id { get { return this._Id; } set { this._Id = value; } }

        private System.Int32? _archives_id;
        /// <summary>
        /// 档案ID
        /// </summary>
        public System.Int32? archives_id { get { return this._archives_id; } set { this._archives_id = value ?? default(System.Int32); } }

        private System.String _archives;
        /// <summary>
        /// 档案姓名
        /// </summary>
        public System.String archives { get { return this._archives; } set { this._archives = value?.Trim(); } }

        private System.String _archives_phone;
        /// <summary>
        /// 档案手机
        /// </summary>
        public System.String archives_phone { get { return this._archives_phone; } set { this._archives_phone = value?.Trim(); } }

        private System.String _card_no;
        /// <summary>
        /// 卡号
        /// </summary>
        public System.String card_no { get { return this._card_no; } set { this._card_no = value?.Trim(); } }

        private System.Decimal _money;
        /// <summary>
        /// 充值金额
        /// </summary>
        public System.Decimal money { get { return this._money; } set { this._money = value; } }

        private System.String _way_code;
        /// <summary>
        /// 支付方式编码
        /// </summary>
        public System.String way_code { get { return this._way_code; } set { this._way_code = value?.Trim(); } }

        private System.String _way;
        /// <summary>
        /// 支付方式
        /// </summary>
        public System.String way { get { return this._way; } set { this._way = value?.Trim(); } }

        private System.Int32? _level_id;
        /// <summary>
        /// 等级ID
        /// </summary>
        public System.Int32? level_id { get { return this._level_id; } set { this._level_id = value ?? default(System.Int32); } }

        private System.String _level;
        /// <summary>
        /// 等级
        /// </summary>
        public System.String level { get { return this._level; } set { this._level = value?.Trim(); } }

        private System.Int32 _store_id = 0;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.Int32 _org_id;
        /// <summary>
        /// 机构id
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Decimal? _give_money;
        /// <summary>
        /// 赠送金额
        /// </summary>
        public System.Decimal? give_money { get { return this._give_money; } set { this._give_money = value ?? default(System.Decimal); } }

        private System.String _no;
        /// <summary>
        /// 单据号
        /// </summary>
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        /// <summary>
        /// 券码
        /// </summary>
        public string couponCode { get; set; }
        /// <summary>
        /// 活动id
        /// </summary>
        public int activity_id { get; set; }

        /// <summary>
        /// 优惠券id
        /// </summary>
        public int coupon_id { get; set; }
    }
}
