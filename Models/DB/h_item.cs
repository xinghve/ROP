using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class h_item
    {
        /// <summary>
        /// 
        /// </summary>
        public h_item()
        {
        }

        private System.Int32 _item_id;
        /// <summary>
        /// 项目ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 item_id { get { return this._item_id; } set { this._item_id = value; } }

        private System.Int32 _dir_id;
        /// <summary>
        /// 目录ID
        /// </summary>
        public System.Int32 dir_id { get { return this._dir_id; } set { this._dir_id = value; } }

        private System.Int16 _type_id;
        /// <summary>
        /// 类别ID
        /// </summary>
        public System.Int16 type_id { get { return this._type_id; } set { this._type_id = value; } }

        private System.Int32 _org_id;
        /// <summary>
        /// 机构ID
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.String _code;
        /// <summary>
        /// 编码
        /// </summary>
        public System.String code { get { return this._code; } set { this._code = value?.Trim(); } }

        private System.String _common_name;
        /// <summary>
        /// 通用名
        /// </summary>
        public System.String common_name { get { return this._common_name; } set { this._common_name = value?.Trim(); } }

        private System.String _trade_name;
        /// <summary>
        /// 商品名
        /// </summary>
        public System.String trade_name { get { return this._trade_name; } set { this._trade_name = value?.Trim(); } }

        private System.String _short_code;
        /// <summary>
        /// 简码
        /// </summary>
        public System.String short_code { get { return this._short_code; } set { this._short_code = value?.Trim(); } }

        private System.Int16 _fee_id;
        /// <summary>
        /// 费别ID
        /// </summary>
        public System.Int16 fee_id { get { return this._fee_id; } set { this._fee_id = value; } }

        private System.String _fee_name;
        /// <summary>
        /// 费别
        /// </summary>
        public System.String fee_name { get { return this._fee_name; } set { this._fee_name = value?.Trim(); } }

        private System.Int16 _property_id;
        /// <summary>
        /// 属性ID
        /// </summary>
        public System.Int16 property_id { get { return this._property_id; } set { this._property_id = value; } }

        private System.String _property_name;
        /// <summary>
        /// 属性
        /// </summary>
        public System.String property_name { get { return this._property_name; } set { this._property_name = value?.Trim(); } }

        private System.Int16 _state_id;
        /// <summary>
        /// 状态ID
        /// </summary>
        public System.Int16 state_id { get { return this._state_id; } set { this._state_id = value; } }

        private System.Int16 _publisher;
        /// <summary>
        /// 公示标志
        /// </summary>
        public System.Int16 publisher { get { return this._publisher; } set { this._publisher = value; } }

        private System.Int16 _external;
        /// <summary>
        /// 外检标志
        /// </summary>
        public System.Int16 external { get { return this._external; } set { this._external = value; } }

        private System.Int16 _discount;
        /// <summary>
        /// 折扣标志
        /// </summary>
        public System.Int16 discount { get { return this._discount; } set { this._discount = value; } }

        private System.DateTime _create_date;
        /// <summary>
        /// 建档时间
        /// </summary>
        public System.DateTime create_date { get { return this._create_date; } set { this._create_date = value; } }

        private System.String _creater;
        /// <summary>
        /// 建档人
        /// </summary>
        public System.String creater { get { return this._creater; } set { this._creater = value?.Trim(); } }

        private System.String _price_code;
        /// <summary>
        /// 物价编码
        /// </summary>
        public System.String price_code { get { return this._price_code; } set { this._price_code = value?.Trim(); } }

        private System.Int16 _sexid;
        /// <summary>
        /// 性别ID
        /// </summary>
        public System.Int16 sexid { get { return this._sexid; } set { this._sexid = value; } }

        private System.Int16 _serviceid;
        /// <summary>
        /// 服务对象ID
        /// </summary>
        public System.Int16 serviceid { get { return this._serviceid; } set { this._serviceid = value; } }

        private System.Int32 _creater_id;
        /// <summary>
        /// 创建人id
        /// </summary>
        public System.Int32 creater_id { get { return this._creater_id; } set { this._creater_id = value; } }

        private System.Int16? _equipment;
        /// <summary>
        /// 设备标志
        /// </summary>
        public System.Int16? equipment { get { return this._equipment; } set { this._equipment = value ?? default(System.Int16); } }
    }
}