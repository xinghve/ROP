using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 就诊评价
    /// </summary>
    public class r_evaluate
    {
        /// <summary>
        /// 就诊评价
        /// </summary>
        public r_evaluate()
        {
        }

        private System.String _id;
        /// <summary>
        /// ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String id { get { return this._id; } set { this._id = value?.Trim(); } }

        private System.Int32? _clinic_id;
        /// <summary>
        /// 就诊ID
        /// </summary>
        public System.Int32? clinic_id { get { return this._clinic_id; } set { this._clinic_id = value ?? default(System.Int32); } }

        private System.DateTime? _evaluate_time;
        /// <summary>
        /// 评价时间
        /// </summary>
        public System.DateTime? evaluate_time { get { return this._evaluate_time; } set { this._evaluate_time = value ?? default(System.DateTime); } }

        private System.Int16? _doctors_level;
        /// <summary>
        /// 医护人员星级
        /// </summary>
        public System.Int16? doctors_level { get { return this._doctors_level; } set { this._doctors_level = value ?? default(System.Int16); } }

        private System.String _doctor_context;
        /// <summary>
        /// 医护人员评价内容
        /// </summary>
        public System.String doctor_context { get { return this._doctor_context; } set { this._doctor_context = value?.Trim(); } }

        private System.Int16? _manager_level;
        /// <summary>
        /// 健康管家星级
        /// </summary>
        public System.Int16? manager_level { get { return this._manager_level; } set { this._manager_level = value ?? default(System.Int16); } }

        private System.String _manager_context;
        /// <summary>
        /// 健康管家评价内容
        /// </summary>
        public System.String manager_context { get { return this._manager_context; } set { this._manager_context = value?.Trim(); } }

        private System.Int16? _other_level;
        /// <summary>
        /// 其他医疗环节星级
        /// </summary>
        public System.Int16? other_level { get { return this._other_level; } set { this._other_level = value ?? default(System.Int16); } }

        private System.String _other_context;
        /// <summary>
        /// 其他医疗环节评价内容
        /// </summary>
        public System.String other_context { get { return this._other_context; } set { this._other_context = value?.Trim(); } }
    }
}