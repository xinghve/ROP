using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 厂家
    /// </summary>
    public class h_manufactor
    {
        /// <summary>
        /// 厂家
        /// </summary>
        public h_manufactor()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 自增id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32 _org_id;
        /// <summary>
        /// 机构id
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.String _name;
        /// <summary>
        /// 厂家名
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _spell;
        /// <summary>
        /// 拼音码
        /// </summary>
        public System.String spell { get { return this._spell; } set { this._spell = value?.Trim(); } }

        private System.String _permit_no;
        /// <summary>
        /// 许可证号
        /// </summary>
        public System.String permit_no { get { return this._permit_no; } set { this._permit_no = value?.Trim(); } }

        private System.DateTime? _permit_date;
        /// <summary>
        /// 许可日期
        /// </summary>
        public System.DateTime? permit_date { get { return this._permit_date; } set { this._permit_date = value; } }

        private System.Int16 _state_id;
        /// <summary>
        /// 状态id
        /// </summary>
        public System.Int16 state_id { get { return this._state_id; } set { this._state_id = value; } }

        private System.String _creater;
        /// <summary>
        /// 创建人
        /// </summary>
        public System.String creater { get { return this._creater; } set { this._creater = value?.Trim(); } }

        private System.Int32 _creater_id;
        /// <summary>
        /// 创建人id
        /// </summary>
        public System.Int32 creater_id { get { return this._creater_id; } set { this._creater_id = value; } }

        private System.DateTime _create_date;
        /// <summary>
        /// 创建日期
        /// </summary>
        public System.DateTime create_date { get { return this._create_date; } set { this._create_date = value; } }

        private System.String _modifier;
        /// <summary>
        /// 编辑人
        /// </summary>
        public System.String modifier { get { return this._modifier; } set { this._modifier = value?.Trim(); } }

        private System.DateTime? _modify_date;
        /// <summary>
        /// 编辑日期
        /// </summary>
        public System.DateTime? modify_date { get { return this._modify_date; } set { this._modify_date = value; } }

        private System.String _tel;
        /// <summary>
        /// 厂家电话
        /// </summary>
        public System.String tel { get { return this._tel; } set { this._tel = value?.Trim(); } }

        private System.String _link_man;
        /// <summary>
        /// 联系人
        /// </summary>
        public System.String link_man { get { return this._link_man; } set { this._link_man = value?.Trim(); } }

        private System.String _phone_no;
        /// <summary>
        /// 联系人电话
        /// </summary>
        public System.String phone_no { get { return this._phone_no; } set { this._phone_no = value?.Trim(); } }

        private System.String _address;
        /// <summary>
        /// 厂家地址
        /// </summary>
        public System.String address { get { return this._address; } set { this._address = value?.Trim(); } }

        private System.String _remarks;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remarks { get { return this._remarks; } set { this._remarks = value?.Trim(); } }

        private System.Int32 _modify_id;
        /// <summary>
        /// 编辑人id
        /// </summary>
        public System.Int32 modify_id { get { return this._modify_id; } set { this._modify_id = value; } }

        private System.String _bank;
        /// <summary>
        /// 银行
        /// </summary>
        public System.String bank { get { return this._bank; } set { this._bank = value?.Trim(); } }

        private System.String _bank_no;
        /// <summary>
        /// 银行账号
        /// </summary>
        public System.String bank_no { get { return this._bank_no; } set { this._bank_no = value?.Trim(); } }
    }
}