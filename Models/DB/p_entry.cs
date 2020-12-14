using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 词条
    /// </summary>
    public class p_entry
    {
        /// <summary>
        /// 词条
        /// </summary>
        public p_entry()
        {
        }

        private System.Int32 _employee_id;
        /// <summary>
        /// 人员ID
        /// </summary>
        public System.Int32 employee_id { get { return this._employee_id; } set { this._employee_id = value; } }

        private System.String _text;
        /// <summary>
        /// 词条内容
        /// </summary>
        public System.String text { get { return this._text; } set { this._text = value?.Trim(); } }

        private System.String _code;
        /// <summary>
        /// 编码
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String code { get { return this._code; } set { this._code = value?.Trim(); } }
    }
}