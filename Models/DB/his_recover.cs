using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 康复预约
    /// </summary>
    public class his_recover
    {
        /// <summary>
        /// 康复预约
        /// </summary>
        public his_recover()
        {
        }

        private System.Int32 _recoverid;
        /// <summary>
        /// 康复预约ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 recoverid { get { return this._recoverid; } set { this._recoverid = value; } }

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
        /// 预约时间
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

        private System.String _summary;
        /// <summary>
        /// 摘要
        /// </summary>
        public System.String summary { get { return this._summary; } set { this._summary = value?.Trim(); } }

        private System.Int32? _applyid;
        /// <summary>
        /// 申请单id
        /// </summary>
        public System.Int32? applyid { get { return this._applyid; } set { this._applyid = value ?? default(System.Int32); } }

        private System.Int32 _specid;
        /// <summary>
        /// 规格ID
        /// </summary>
        public System.Int32 specid { get { return this._specid; } set { this._specid = value; } }

        private System.Int32 _item_id;
        /// <summary>
        /// 项目ID
        /// </summary>
        public System.Int32 item_id { get { return this._item_id; } set { this._item_id = value; } }

        private System.String _typename;
        /// <summary>
        /// 单据类型
        /// </summary>
        public System.String typename { get { return this._typename; } set { this._typename = value?.Trim(); } }

        private System.String _specname;
        /// <summary>
        /// 规格
        /// </summary>
        public System.String specname { get { return this._specname; } set { this._specname = value?.Trim(); } }

        private System.String _item_name;
        /// <summary>
        /// 项目名
        /// </summary>
        public System.String item_name { get { return this._item_name; } set { this._item_name = value?.Trim(); } }

        private System.DateTime? _sign_time;
        /// <summary>
        /// 签到时间
        /// </summary>
        public System.DateTime? sign_time { get { return this._sign_time; } set { this._sign_time = value ?? default(System.DateTime); } }
    }
}