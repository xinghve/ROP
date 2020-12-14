using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class bus_grant_detail
    {
        /// <summary>
        /// 
        /// </summary>
        public bus_grant_detail()
        {
        }

        private System.String _bill_no;
        /// <summary>
        /// 发放单号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String bill_no { get { return this._bill_no; } set { this._bill_no = value?.Trim(); } }

        private System.Int32 _storage_id;
        /// <summary>
        /// 库存id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 storage_id { get { return this._storage_id; } set { this._storage_id = value; } }

        private System.String _storage_no;
        /// <summary>
        /// 库存明细编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String storage_no { get { return this._storage_no; } set { this._storage_no = value?.Trim(); } }

        private System.Int32 _std_item_id;
        /// <summary>
        /// 物资基础项目ID
        /// </summary>
        public System.Int32 std_item_id { get { return this._std_item_id; } set { this._std_item_id = value; } }

        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.Int16 _type_id;
        /// <summary>
        /// 类型ID
        /// </summary>
        public System.Int16 type_id { get { return this._type_id; } set { this._type_id = value; } }

        private System.String _type;
        /// <summary>
        /// 类型
        /// </summary>
        public System.String type { get { return this._type; } set { this._type = value?.Trim(); } }

        private System.String _spec;
        /// <summary>
        /// 规格
        /// </summary>
        public System.String spec { get { return this._spec; } set { this._spec = value?.Trim(); } }

        private System.Int32 _bill_num;
        /// <summary>
        /// 发放数量
        /// </summary>
        public System.Int32 bill_num { get { return this._bill_num; } set { this._bill_num = value; } }

        private System.String _unit;
        /// <summary>
        /// 发放单位
        /// </summary>
        public System.String unit { get { return this._unit; } set { this._unit = value?.Trim(); } }

        private System.Int32 _manufactor_id;
        /// <summary>
        /// 厂家ID
        /// </summary>
        public System.Int32 manufactor_id { get { return this._manufactor_id; } set { this._manufactor_id = value; } }

        private System.String _manufactor;
        /// <summary>
        /// 厂家
        /// </summary>
        public System.String manufactor { get { return this._manufactor; } set { this._manufactor = value?.Trim(); } }

        private System.Int32 _creater_id;
        /// <summary>
        /// 发放人ID
        /// </summary>
        public System.Int32 creater_id { get { return this._creater_id; } set { this._creater_id = value; } }

        private System.String _creater;
        /// <summary>
        /// 发放人
        /// </summary>
        public System.String creater { get { return this._creater; } set { this._creater = value?.Trim(); } }

        private System.DateTime _create_time;
        /// <summary>
        /// 发放时间
        /// </summary>
        public System.DateTime create_time { get { return this._create_time; } set { this._create_time = value; } }

        private System.Int32? _return_id;
        /// <summary>
        /// 归还操作人ID
        /// </summary>
        public System.Int32? return_id { get { return this._return_id; } set { this._return_id = value ?? default(System.Int32); } }

        private System.String _returner;
        /// <summary>
        /// 归还操作人
        /// </summary>
        public System.String returner { get { return this._returner; } set { this._returner = value?.Trim(); } }

        private System.DateTime? _return_time;
        /// <summary>
        /// 归还时间
        /// </summary>
        public System.DateTime? return_time { get { return this._return_time; } set { this._return_time = value ?? default(System.DateTime); } }

        private System.Int16? _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

        private System.String _remark;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remark { get { return this._remark; } set { this._remark = value?.Trim(); } }

        private System.String _no;
        /// <summary>
        /// 设备编号
        /// </summary>
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        private System.String _re_remark;
        /// <summary>
        /// 归还备注
        /// </summary>
        public System.String re_remark { get { return this._re_remark; } set { this._re_remark = value?.Trim(); } }


    }
}