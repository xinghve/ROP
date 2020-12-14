using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class bus_requisitions_detail 
    {
        /// <summary>
        /// 
        /// </summary>
        public bus_requisitions_detail ()
        {
        }

        private System.String _bill_no;
        /// <summary>
        /// 领用单号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String bill_no { get { return this._bill_no; } set { this._bill_no = value?.Trim(); } }

        private System.Int32 _std_item_id;
        /// <summary>
        /// 物资基础项目ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 std_item_id { get { return this._std_item_id; } set { this._std_item_id = value; } }

        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.Int16 _type_id;
        /// <summary>
        /// 物资基础分类ID
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
        [SugarColumn(IsPrimaryKey = true)]
        public System.String spec { get { return this._spec; } set { this._spec = value?.Trim(); } }

        private System.String _unit;
        /// <summary>
        /// 单位
        /// </summary>
        public System.String unit { get { return this._unit; } set { this._unit = value?.Trim(); } }

        private System.Int16? _num;
        /// <summary>
        /// 数量（单位）
        /// </summary>
        public System.Int16? num { get { return this._num; } set { this._num = value ?? default(System.Int16); } }

        private System.String _approval_no;
        /// <summary>
        /// 批准文号
        /// </summary>
        public System.String approval_no { get { return this._approval_no; } set { this._approval_no = value?.Trim(); } }

       
        private System.String _remark;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remark { get { return this._remark; } set { this._remark = value?.Trim(); } }

     
        private System.Int16? _relation_id;
        /// <summary>
        /// 关联id
        /// </summary>
        public System.Int16? relation_id { get { return this._relation_id; } set { this._relation_id = value; } }

        private System.String _estimate_time;
        /// <summary>
        /// 预计归还时间
        /// </summary>
        public System.String estimate_time { get { return this._estimate_time; } set { this._estimate_time = value; } }

        private System.Int16? _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value; } }


    }
}