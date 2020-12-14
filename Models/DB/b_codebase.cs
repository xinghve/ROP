using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 基础编码表
    /// </summary>
    public class b_codebase
    {
        /// <summary>
        /// 基础编码表
        /// </summary>
        public b_codebase()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 自增主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32 _category_id;
        /// <summary>
        /// 目录Id
        /// </summary>
        public System.Int32 category_id { get { return this._category_id; } set { this._category_id = value; } }

        private System.String _code;
        /// <summary>
        /// 编码
        /// </summary>
        public System.String code { get { return this._code; } set { this._code = value?.Trim(); } }

        private System.String _text;
        /// <summary>
        /// 文本
        /// </summary>
        public System.String text { get { return this._text; } set { this._text = value?.Trim(); } }

        private System.String _pinyin;
        /// <summary>
        /// 拼音
        /// </summary>
        public System.String pinyin { get { return this._pinyin; } set { this._pinyin = value?.Trim(); } }

        private System.String _introduce;
        /// <summary>
        /// 介绍说明
        /// </summary>
        public System.String introduce { get { return this._introduce; } set { this._introduce = value?.Trim(); } }

        private System.Int16? _property_id;
        /// <summary>
        /// 属性ID
        /// </summary>
        public System.Int16? property_id { get { return this._property_id; } set { this._property_id = value ?? default(System.Int16); } }

        private System.String _property;
        /// <summary>
        /// 属性
        /// </summary>
        public System.String property { get { return this._property; } set { this._property = value?.Trim(); } }

        private System.Int16 _year;
        /// <summary>
        /// 年限
        /// </summary>
        public System.Int16 year { get { return this._year; } set { this._year = value; } }

        private System.Int32 _catalog_id;
        /// <summary>
        /// 附目录ID
        /// </summary>
        public System.Int32 catalog_id { get { return this._catalog_id; } set { this._catalog_id = value; } }
    }
}