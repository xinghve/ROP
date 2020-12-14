using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class p_dept_specialty
    {
        /// <summary>
        /// 
        /// </summary>
        public p_dept_specialty()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 
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

        private System.String _name;
        /// <summary>
        /// 科室专业名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _code;
        /// <summary>
        /// 科室专业编码
        /// </summary>
        public System.String code { get { return this._code; } set { this._code = value?.Trim(); } }

        private System.String _pinyin;
        /// <summary>
        /// 拼音码
        /// </summary>
        public System.String pinyin { get { return this._pinyin; } set { this._pinyin = value?.Trim(); } }

        private System.Int16? _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

        private System.Int16? _standard_mark;
        /// <summary>
        /// 标准标志
        /// </summary>
        public System.Int16? standard_mark { get { return this._standard_mark; } set { this._standard_mark = value ?? default(System.Int16); } }

        private System.Int32? _parentid;
        /// <summary>
        /// 父级id
        /// </summary>
        public System.Int32? parentid { get { return this._parentid; } set { this._parentid = value ?? default(System.Int32); } }

        private System.String _ncms_code;
        /// <summary>
        /// 新农合编码
        /// </summary>
        public System.String ncms_code { get { return this._ncms_code; } set { this._ncms_code = value?.Trim(); } }

        private System.String _medical_insurance_code;
        /// <summary>
        /// 医保编码
        /// </summary>
        public System.String medical_insurance_code { get { return this._medical_insurance_code; } set { this._medical_insurance_code = value?.Trim(); } }
    }
}