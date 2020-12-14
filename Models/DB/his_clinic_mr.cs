using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 门诊病历
    /// </summary>
    public class his_clinic_mr
    {
        /// <summary>
        /// 门诊病历
        /// </summary>
        public his_clinic_mr()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 自增主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32 _org_id;
        /// <summary>
        /// 集团Id
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店Id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.Int32 _clinicid;
        /// <summary>
        /// 就诊ID
        /// </summary>
        public System.Int32 clinicid { get { return this._clinicid; } set { this._clinicid = value; } }

        private System.DateTime _clinic_time;
        /// <summary>
        /// 就诊时间
        /// </summary>
        public System.DateTime clinic_time { get { return this._clinic_time; } set { this._clinic_time = value; } }

        private System.String _complaint;
        /// <summary>
        /// 主诉
        /// </summary>
        public System.String complaint { get { return this._complaint; } set { this._complaint = value?.Trim(); } }

        private System.String _present_history;
        /// <summary>
        /// 现病史
        /// </summary>
        public System.String present_history { get { return this._present_history; } set { this._present_history = value?.Trim(); } }

        private System.String _past_history;
        /// <summary>
        /// 既往史
        /// </summary>
        public System.String past_history { get { return this._past_history; } set { this._past_history = value?.Trim(); } }

        private System.String _phisical_exam;
        /// <summary>
        /// 体格检查
        /// </summary>
        public System.String phisical_exam { get { return this._phisical_exam; } set { this._phisical_exam = value?.Trim(); } }

        private System.Decimal _t;
        /// <summary>
        /// 体格检查：体温
        /// </summary>
        public System.Decimal t { get { return this._t; } set { this._t = value; } }

        private System.Int16 _p;
        /// <summary>
        /// 体格检查：脉搏
        /// </summary>
        public System.Int16 p { get { return this._p; } set { this._p = value; } }

        private System.Int16 _r;
        /// <summary>
        /// 体格检查：呼吸
        /// </summary>
        public System.Int16 r { get { return this._r; } set { this._r = value; } }

        private System.Int16 _bp1;
        /// <summary>
        /// 体格检查：血压
        /// </summary>
        public System.Int16 bp1 { get { return this._bp1; } set { this._bp1 = value; } }

        private System.Int16 _bp2;
        /// <summary>
        /// 体格检查：血压
        /// </summary>
        public System.Int16 bp2 { get { return this._bp2; } set { this._bp2 = value; } }

        private System.Decimal _height;
        /// <summary>
        /// 体格检查：身高
        /// </summary>
        public System.Decimal height { get { return this._height; } set { this._height = value; } }

        private System.Decimal _weight;
        /// <summary>
        /// 体格检查：体重
        /// </summary>
        public System.Decimal weight { get { return this._weight; } set { this._weight = value; } }

        private System.Decimal _bmi;
        /// <summary>
        /// 体格检查：BMI（体重/身高米数的平方）
        /// </summary>
        public System.Decimal bmi { get { return this._bmi; } set { this._bmi = value; } }

        private System.String _diagnosis;
        /// <summary>
        /// 诊断
        /// </summary>
        public System.String diagnosis { get { return this._diagnosis; } set { this._diagnosis = value?.Trim(); } }

        private System.String _treatment;
        /// <summary>
        /// 处理措施
        /// </summary>
        public System.String treatment { get { return this._treatment; } set { this._treatment = value?.Trim(); } }

        private System.Int32? _archives_id;
        /// <summary>
        /// 档案ID
        /// </summary>
        public System.Int32? archives_id { get { return this._archives_id; } set { this._archives_id = value ?? default(System.Int32); } }
    }
}