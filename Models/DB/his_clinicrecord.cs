using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 就诊记录
    /// </summary>
    public class his_clinicrecord
    {
        /// <summary>
        /// 就诊记录
        /// </summary>
        public his_clinicrecord()
        {
        }

        private System.Int32 _clinicid;
        /// <summary>
        /// 就诊ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 clinicid { get { return this._clinicid; } set { this._clinicid = value; } }

        private System.Int32 _centerid;
        /// <summary>
        /// 档案ID
        /// </summary>
        public System.Int32 centerid { get { return this._centerid; } set { this._centerid = value; } }

        private System.Int32 _orgid;
        /// <summary>
        /// 机构ID
        /// </summary>
        public System.Int32 orgid { get { return this._orgid; } set { this._orgid = value; } }

        private System.Int32 _outid;
        /// <summary>
        /// 门诊ID
        /// </summary>
        public System.Int32 outid { get { return this._outid; } set { this._outid = value; } }

        private System.Int32? _inhosid;
        /// <summary>
        /// 住院ID
        /// </summary>
        public System.Int32? inhosid { get { return this._inhosid; } set { this._inhosid = value ?? default(System.Int32); } }

        private System.String _patienttype;
        /// <summary>
        /// 病人类型
        /// </summary>
        public System.String patienttype { get { return this._patienttype; } set { this._patienttype = value?.Trim(); } }

        private System.String _age;
        /// <summary>
        /// 年龄
        /// </summary>
        public System.String age { get { return this._age; } set { this._age = value?.Trim(); } }

        private System.String _marriageid;
        /// <summary>
        /// 婚姻ID
        /// </summary>
        public System.String marriageid { get { return this._marriageid; } set { this._marriageid = value?.Trim(); } }

        private System.String _marriage;
        /// <summary>
        /// 婚姻
        /// </summary>
        public System.String marriage { get { return this._marriage; } set { this._marriage = value?.Trim(); } }

        private System.String _educationid;
        /// <summary>
        /// 文化程度ID
        /// </summary>
        public System.String educationid { get { return this._educationid; } set { this._educationid = value?.Trim(); } }

        private System.String _education;
        /// <summary>
        /// 文化程度
        /// </summary>
        public System.String education { get { return this._education; } set { this._education = value?.Trim(); } }

        private System.String _vocationid;
        /// <summary>
        /// 职业ID
        /// </summary>
        public System.String vocationid { get { return this._vocationid; } set { this._vocationid = value?.Trim(); } }

        private System.String _vocation;
        /// <summary>
        /// 职业
        /// </summary>
        public System.String vocation { get { return this._vocation; } set { this._vocation = value?.Trim(); } }

        private System.String _address;
        /// <summary>
        /// 联系地址
        /// </summary>
        public System.String address { get { return this._address; } set { this._address = value?.Trim(); } }

        private System.String _contactno;
        /// <summary>
        /// 联系电话
        /// </summary>
        public System.String contactno { get { return this._contactno; } set { this._contactno = value?.Trim(); } }

        private System.String _emergencyno;
        /// <summary>
        /// 应急电话
        /// </summary>
        public System.String emergencyno { get { return this._emergencyno; } set { this._emergencyno = value?.Trim(); } }

        private System.String _contancts;
        /// <summary>
        /// 联系人
        /// </summary>
        public System.String contancts { get { return this._contancts; } set { this._contancts = value?.Trim(); } }

        private System.String _contactsno;
        /// <summary>
        /// 联系人电话
        /// </summary>
        public System.String contactsno { get { return this._contactsno; } set { this._contactsno = value?.Trim(); } }

        private System.String _relationid;
        /// <summary>
        /// 联系人关系ID
        /// </summary>
        public System.String relationid { get { return this._relationid; } set { this._relationid = value?.Trim(); } }

        private System.String _relation;
        /// <summary>
        /// 联系人关系
        /// </summary>
        public System.String relation { get { return this._relation; } set { this._relation = value?.Trim(); } }

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

        private System.Int32 _doctorid;
        /// <summary>
        /// 接诊医生ID
        /// </summary>
        public System.Int32 doctorid { get { return this._doctorid; } set { this._doctorid = value; } }

        private System.String _doctorname;
        /// <summary>
        /// 接诊医生
        /// </summary>
        public System.String doctorname { get { return this._doctorname; } set { this._doctorname = value?.Trim(); } }

        private System.Int32? _nurseid;
        /// <summary>
        /// 责任护士ID
        /// </summary>
        public System.Int32? nurseid { get { return this._nurseid; } set { this._nurseid = value ?? default(System.Int32); } }

        private System.String _nursename;
        /// <summary>
        /// 责任护士
        /// </summary>
        public System.String nursename { get { return this._nursename; } set { this._nursename = value?.Trim(); } }

        private System.Int32? _salsemanid;
        /// <summary>
        /// 销售人ID
        /// </summary>
        public System.Int32? salsemanid { get { return this._salsemanid; } set { this._salsemanid = value ?? default(System.Int32); } }

        private System.String _salseman;
        /// <summary>
        /// 销售人
        /// </summary>
        public System.String salseman { get { return this._salseman; } set { this._salseman = value?.Trim(); } }

        private System.Int32? _customerid;
        /// <summary>
        /// 客服人ID
        /// </summary>
        public System.Int32? customerid { get { return this._customerid; } set { this._customerid = value ?? default(System.Int32); } }

        private System.String _customer;
        /// <summary>
        /// 客服人
        /// </summary>
        public System.String customer { get { return this._customer; } set { this._customer = value?.Trim(); } }

        private System.DateTime _regdate;
        /// <summary>
        /// 登记时间
        /// </summary>
        public System.DateTime regdate { get { return this._regdate; } set { this._regdate = value; } }

        private System.DateTime _begindate;
        /// <summary>
        /// 接诊开始时间
        /// </summary>
        public System.DateTime begindate { get { return this._begindate; } set { this._begindate = value; } }

        private System.DateTime? _enddate;
        /// <summary>
        /// 接诊结束时间
        /// </summary>
        public System.DateTime? enddate { get { return this._enddate; } set { this._enddate = value ?? default(System.DateTime); } }

        private System.Int16 _stateid;
        /// <summary>
        /// 状态ID
        /// </summary>
        public System.Int16 stateid { get { return this._stateid; } set { this._stateid = value; } }

        private System.String _diagnosiscode;
        /// <summary>
        /// 诊断编码
        /// </summary>
        public System.String diagnosiscode { get { return this._diagnosiscode; } set { this._diagnosiscode = value?.Trim(); } }

        private System.String _diagnosisname;
        /// <summary>
        /// 诊断名称
        /// </summary>
        public System.String diagnosisname { get { return this._diagnosisname; } set { this._diagnosisname = value?.Trim(); } }

        private System.Decimal _amount;
        /// <summary>
        /// 总费用
        /// </summary>
        public System.Decimal amount { get { return this._amount; } set { this._amount = value; } }

        private System.String _summary;
        /// <summary>
        /// 摘要
        /// </summary>
        public System.String summary { get { return this._summary; } set { this._summary = value?.Trim(); } }
    }
}