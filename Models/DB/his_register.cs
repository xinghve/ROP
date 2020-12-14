using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 挂号记录表
    /// </summary>
    public class his_register
    {
        /// <summary>
        /// 挂号记录表
        /// </summary>
        public his_register()
        {
        }

        private System.Int32 _regid;
        /// <summary>
        /// 挂号ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 regid { get { return this._regid; } set { this._regid = value; } }

        private System.Int32 _orgid;
        /// <summary>
        /// 机构ID
        /// </summary>
        public System.Int32 orgid { get { return this._orgid; } set { this._orgid = value; } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店ID
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.Int32 _deptid;
        /// <summary>
        /// 部门ID
        /// </summary>
        public System.Int32 deptid { get { return this._deptid; } set { this._deptid = value; } }

        private System.String _deptname;
        /// <summary>
        /// 部门
        /// </summary>
        public System.String deptname { get { return this._deptname; } set { this._deptname = value?.Trim(); } }

        private System.Int32 _doctorid;
        /// <summary>
        /// 医生ID
        /// </summary>
        public System.Int32 doctorid { get { return this._doctorid; } set { this._doctorid = value; } }

        private System.String _doctorname;
        /// <summary>
        /// 医生
        /// </summary>
        public System.String doctorname { get { return this._doctorname; } set { this._doctorname = value?.Trim(); } }

        private System.Int32 _typeid;
        /// <summary>
        /// 类别ID
        /// </summary>
        public System.Int32 typeid { get { return this._typeid; } set { this._typeid = value; } }

        private System.Int32 _centerid;
        /// <summary>
        /// 档案ID
        /// </summary>
        public System.Int32 centerid { get { return this._centerid; } set { this._centerid = value; } }

        private System.Int32 _clinicid;
        /// <summary>
        /// 就诊ID
        /// </summary>
        public System.Int32 clinicid { get { return this._clinicid; } set { this._clinicid = value; } }

        private System.DateTime _recorddate;
        /// <summary>
        /// 记录时间
        /// </summary>
        public System.DateTime recorddate { get { return this._recorddate; } set { this._recorddate = value; } }

        private System.DateTime _regdate;
        /// <summary>
        /// 挂号时间
        /// </summary>
        public System.DateTime regdate { get { return this._regdate; } set { this._regdate = value; } }

        private System.Int32 _operatorid;
        /// <summary>
        /// 操作员ID
        /// </summary>
        public System.Int32 operatorid { get { return this._operatorid; } set { this._operatorid = value; } }

        private System.String _operator_name;
        /// <summary>
        /// 操作员
        /// </summary>
        public System.String operator_name { get { return this._operator_name; } set { this._operator_name = value?.Trim(); } }

        private System.String _cardno;
        /// <summary>
        /// 就诊卡
        /// </summary>
        public System.String cardno { get { return this._cardno; } set { this._cardno = value?.Trim(); } }

        private System.Decimal _shouldamount;
        /// <summary>
        /// 应收金额
        /// </summary>
        public System.Decimal shouldamount { get { return this._shouldamount; } set { this._shouldamount = value; } }

        private System.Decimal _actualamount;
        /// <summary>
        /// 实收金额
        /// </summary>
        public System.Decimal actualamount { get { return this._actualamount; } set { this._actualamount = value; } }

        private System.Int16 _paymentid;
        /// <summary>
        /// 支付方式ID
        /// </summary>
        public System.Int16 paymentid { get { return this._paymentid; } set { this._paymentid = value; } }

        private System.String _payment;
        /// <summary>
        /// 支付方式
        /// </summary>
        public System.String payment { get { return this._payment; } set { this._payment = value?.Trim(); } }

        private System.Int16 _settleflag;
        /// <summary>
        /// 结算标志
        /// </summary>
        public System.Int16 settleflag { get { return this._settleflag; } set { this._settleflag = value; } }

        private System.Int16 _orderflag;
        /// <summary>
        /// 预约标志
        /// </summary>
        public System.Int16 orderflag { get { return this._orderflag; } set { this._orderflag = value; } }

        private System.Int32? _roomid;
        /// <summary>
        /// 诊室ID
        /// </summary>
        public System.Int32? roomid { get { return this._roomid; } set { this._roomid = value ?? default(System.Int32); } }

        private System.String _room;
        /// <summary>
        /// 诊室
        /// </summary>
        public System.String room { get { return this._room; } set { this._room = value?.Trim(); } }

        private System.Int16 _canceled;
        /// <summary>
        /// 作废标志
        /// </summary>
        public System.Int16 canceled { get { return this._canceled; } set { this._canceled = value; } }

        private System.Int16 _stateid;
        /// <summary>
        /// 状态ID
        /// </summary>
        public System.Int16 stateid { get { return this._stateid; } set { this._stateid = value; } }

        private System.Int32 _checkoutid;
        /// <summary>
        /// 结账ID
        /// </summary>
        public System.Int32 checkoutid { get { return this._checkoutid; } set { this._checkoutid = value; } }

        private System.Int16 _billtypeid;
        /// <summary>
        /// 单据类型ID
        /// </summary>
        public System.Int16 billtypeid { get { return this._billtypeid; } set { this._billtypeid = value; } }

        private System.String _billtype;
        /// <summary>
        /// 单据类型
        /// </summary>
        public System.String billtype { get { return this._billtype; } set { this._billtype = value?.Trim(); } }

        private System.Int16 _sourceid;
        /// <summary>
        /// 来源ID
        /// </summary>
        public System.Int16 sourceid { get { return this._sourceid; } set { this._sourceid = value; } }

        private System.String _source;
        /// <summary>
        /// 来源
        /// </summary>
        public System.String source { get { return this._source; } set { this._source = value?.Trim(); } }

        private System.Int16 _clinicstateid;
        /// <summary>
        /// 就诊状态ID
        /// </summary>
        public System.Int16 clinicstateid { get { return this._clinicstateid; } set { this._clinicstateid = value; } }

        private System.String _summary;
        /// <summary>
        /// 摘要
        /// </summary>
        public System.String summary { get { return this._summary; } set { this._summary = value?.Trim(); } }
    }
}