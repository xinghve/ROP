using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 申请单信息：包括检验、检查、西药、中药配方、治疗单等
    /// </summary>
    public class his_applybill
    {
        /// <summary>
        /// 申请单信息：包括检验、检查、西药、中药配方、治疗单等
        /// </summary>
        public his_applybill()
        {
        }

        private System.Int32 _applyid;
        /// <summary>
        /// 申请ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 applyid { get { return this._applyid; } set { this._applyid = value; } }

        private System.Int32 _orgid;
        /// <summary>
        /// 机构ID
        /// </summary>
        public System.Int32 orgid { get { return this._orgid; } set { this._orgid = value; } }

        private System.Int32 _clinicid;
        /// <summary>
        /// 就诊ID
        /// </summary>
        public System.Int32 clinicid { get { return this._clinicid; } set { this._clinicid = value; } }

        private System.Int32 _centerid;
        /// <summary>
        /// 档案ID
        /// </summary>
        public System.Int32 centerid { get { return this._centerid; } set { this._centerid = value; } }

        private System.Int16 _typeid;
        /// <summary>
        /// 单据类型ID
        /// </summary>
        public System.Int16 typeid { get { return this._typeid; } set { this._typeid = value; } }

        private System.String _typename;
        /// <summary>
        /// 单据类型
        /// </summary>
        public System.String typename { get { return this._typename; } set { this._typename = value?.Trim(); } }

        private System.Int32 _regid;
        /// <summary>
        /// 挂号ID
        /// </summary>
        public System.Int32 regid { get { return this._regid; } set { this._regid = value; } }

        private System.DateTime _recorddate;
        /// <summary>
        /// 记录时间
        /// </summary>
        public System.DateTime recorddate { get { return this._recorddate; } set { this._recorddate = value; } }

        private System.DateTime _applydate;
        /// <summary>
        /// 申请时间
        /// </summary>
        public System.DateTime applydate { get { return this._applydate; } set { this._applydate = value; } }

        private System.Int32 _doctorid;
        /// <summary>
        /// 开单医生ID
        /// </summary>
        public System.Int32 doctorid { get { return this._doctorid; } set { this._doctorid = value; } }

        private System.String _doctorname;
        /// <summary>
        /// 开单医生
        /// </summary>
        public System.String doctorname { get { return this._doctorname; } set { this._doctorname = value?.Trim(); } }

        private System.Int32 _deptid;
        /// <summary>
        /// 就诊科室ID
        /// </summary>
        public System.Int32 deptid { get { return this._deptid; } set { this._deptid = value; } }

        private System.String _deptname;
        /// <summary>
        /// 就诊科室
        /// </summary>
        public System.String deptname { get { return this._deptname; } set { this._deptname = value?.Trim(); } }

        private System.Int16 _stateid;
        /// <summary>
        /// 状态ID
        /// </summary>
        public System.Int16 stateid { get { return this._stateid; } set { this._stateid = value; } }

        private System.String _statename;
        /// <summary>
        /// 状态
        /// </summary>
        public System.String statename { get { return this._statename; } set { this._statename = value?.Trim(); } }

        private System.Int16 _issettle;
        /// <summary>
        /// 缴费标志
        /// </summary>
        public System.Int16 issettle { get { return this._issettle; } set { this._issettle = value; } }

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

        private System.Int16 _days;
        /// <summary>
        /// 天数
        /// </summary>
        public System.Int16 days { get { return this._days; } set { this._days = value; } }

        private System.String _diagnosiscode;
        /// <summary>
        /// 诊断编码
        /// </summary>
        public System.String diagnosiscode { get { return this._diagnosiscode; } set { this._diagnosiscode = value?.Trim(); } }

        private System.String _diagnosisname;
        /// <summary>
        /// 诊断
        /// </summary>
        public System.String diagnosisname { get { return this._diagnosisname; } set { this._diagnosisname = value?.Trim(); } }

        private System.Int16 _double_num;
        /// <summary>
        /// 付数
        /// </summary>
        public System.Int16 double_num { get { return this._double_num; } set { this._double_num = value; } }

        private System.String _summay;
        /// <summary>
        /// 摘要
        /// </summary>
        public System.String summay { get { return this._summay; } set { this._summay = value?.Trim(); } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店ID
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }
    }
}