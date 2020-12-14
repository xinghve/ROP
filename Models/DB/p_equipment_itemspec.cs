using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 设备器械服务项目
    /// </summary>
    public class p_equipment_itemspec
    {
        /// <summary>
        /// 设备器械服务项目
        /// </summary>
        public p_equipment_itemspec()
        {
        }

        private System.Int32 _equipment_id;
        /// <summary>
        /// 设备ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 equipment_id { get { return this._equipment_id; } set { this._equipment_id = value; } }

        private System.Int32 _itemid;
        /// <summary>
        /// 项目ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 itemid { get { return this._itemid; } set { this._itemid = value; } }

        private System.String _tradename;
        /// <summary>
        /// 项目商品名
        /// </summary>
        public System.String tradename { get { return this._tradename; } set { this._tradename = value?.Trim(); } }

        private System.Int32 _specid;
        /// <summary>
        /// 规格ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 specid { get { return this._specid; } set { this._specid = value; } }

        private System.String _specname;
        /// <summary>
        /// 规格
        /// </summary>
        public System.String specname { get { return this._specname; } set { this._specname = value?.Trim(); } }

        private System.Int16? _work_times;
        /// <summary>
        /// 工作时长（分钟）
        /// </summary>
        public System.Int16? work_times { get { return this._work_times; } set { this._work_times = value ?? default(System.Int16); } }
    }
}