using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 通知关联人员表
    /// </summary>
    public class oa_notice_employee
    {
        /// <summary>
        /// 通知关联人员表
        /// </summary>
        public oa_notice_employee()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 关联通知表
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32 _employee_id;
        /// <summary>
        /// 被通知人id
        /// </summary>
        public System.Int32 employee_id { get { return this._employee_id; } set { this._employee_id = value; } }

        private System.String _employee;
        /// <summary>
        /// 被通知人
        /// </summary>
        public System.String employee { get { return this._employee; } set { this._employee = value?.Trim(); } }

        private System.String _service_object;
        /// <summary>
        /// 服务对象
        /// </summary>
        public System.String service_object { get { return this._service_object; } set { this._service_object = value?.Trim(); } }

        private System.Int32? _service_object_id;
        /// <summary>
        /// 服务对象id
        /// </summary>
        public System.Int32? service_object_id { get { return this._service_object_id; } set { this._service_object_id = value ?? default(System.Int32); } }

        private System.Int16? _read_state;
        /// <summary>
        /// 是否已读
        /// </summary>
        public System.Int16? read_state { get { return this._read_state; } set { this._read_state = value ?? default(System.Int16); } }

        private System.String _img_url;
        /// <summary>
        /// 图片地址
        /// </summary>
        public System.String img_url { get { return this._img_url; } set { this._img_url = value?.Trim(); } }

        private System.String _service_object_phone;
        /// <summary>
        /// 服务对象电话
        /// </summary>
        public System.String service_object_phone { get { return this._service_object_phone; } set { this._service_object_phone = value?.Trim(); } }

       
    }
}