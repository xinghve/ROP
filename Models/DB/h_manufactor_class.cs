using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 厂家分类
    /// </summary>
    public class h_manufactor_class
    {
        /// <summary>
        /// 厂家分类
        /// </summary>
        public h_manufactor_class()
        {
        }

        private System.Int32 _manufactor_id;
        /// <summary>
        /// 厂家id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 manufactor_id { get { return this._manufactor_id; } set { this._manufactor_id = value; } }

        private System.Int32 _class_id;
        /// <summary>
        /// 分类id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 class_id { get { return this._class_id; } set { this._class_id = value; } }
    }
}