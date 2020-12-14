using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 收费结算分页数据
    /// </summary>
    public class ChargePages
    {
        private System.Int16 _orderid;
        /// <summary>
        /// 序号
        /// </summary>
        public System.Int16 orderid { get { return this._orderid; } set { this._orderid = value; } }

        private System.String _itemname;
        /// <summary>
        /// 项目名称
        /// </summary>
        public System.String itemname { get { return this._itemname; } set { this._itemname = value?.Trim(); } }

        private System.String _specname;
        /// <summary>
        /// 规格
        /// </summary>
        public System.String specname { get { return this._specname; } set { this._specname = value?.Trim(); } }

        private System.Decimal _quantity;
        /// <summary>
        /// 数量
        /// </summary>
        public System.Decimal quantity { get { return this._quantity; } set { this._quantity = value; } }

        private System.Decimal _price;
        /// <summary>
        /// 售价
        /// </summary>
        public System.Decimal price { get { return this._price; } set { this._price = value; } }

        private System.Decimal _shouldamount;
        /// <summary>
        /// 应收金额
        /// </summary>
        public System.Decimal shouldamount { get { return this._shouldamount; } set { this._shouldamount = value; } }

        private System.String _frequecyname;
        /// <summary>
        /// 频率
        /// </summary>
        public System.String frequecyname { get { return this._frequecyname; } set { this._frequecyname = value?.Trim(); } }

        private System.Decimal? _sigle;
        /// <summary>
        /// 单量
        /// </summary>
        public System.Decimal? sigle { get { return this._sigle; } set { this._sigle = value ?? default(System.Decimal); } }

        private System.String _unitname;
        /// <summary>
        /// 单位
        /// </summary>
        public System.String unitname { get { return this._unitname; } set { this._unitname = value?.Trim(); } }

        private System.Decimal _modulus;
        /// <summary>
        /// 系数
        /// </summary>
        public System.Decimal modulus { get { return this._modulus; } set { this._modulus = value; } }

        private System.Decimal? _discount_rate;
        /// <summary>
        /// 折扣率
        /// </summary>
        public System.Decimal? discount_rate { get { return this._discount_rate; } set { this._discount_rate = value ?? default(System.Decimal); } }
    }
}
