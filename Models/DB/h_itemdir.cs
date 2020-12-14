using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 项目目录
    /// </summary>
    public class h_itemdir
    {
        /// <summary>
        /// 项目目录
        /// </summary>
        public h_itemdir()
        {
        }

        private System.Int32 _dirid;
        /// <summary>
        /// 目录ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 dirid { get { return this._dirid; } set { this._dirid = value; } }

        private System.Int32 _orgid;
        /// <summary>
        /// 机构ID
        /// </summary>
        public System.Int32 orgid { get { return this._orgid; } set { this._orgid = value; } }

        private System.Int32 _parentid;
        /// <summary>
        /// 上级ID
        /// </summary>
        public System.Int32 parentid { get { return this._parentid; } set { this._parentid = value; } }

        private System.String _code;
        /// <summary>
        /// 编码
        /// </summary>
        public System.String code { get { return this._code; } set { this._code = value?.Trim(); } }

        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.Int16 _typeid;
        /// <summary>
        /// 类别ID
        /// </summary>
        public System.Int16 typeid { get { return this._typeid; } set { this._typeid = value; } }

        private System.String _type_name;
        /// <summary>
        /// 类别名称
        /// </summary>
        public System.String type_name { get { return this._type_name; } set { this._type_name = value?.Trim(); } }

        private System.Int16? _type_parent_id;
        /// <summary>
        /// 类别父级ID
        /// </summary>
        public System.Int16? type_parent_id { get { return this._type_parent_id; } set { this._type_parent_id = value ?? default(System.Int16); } }
    }
}