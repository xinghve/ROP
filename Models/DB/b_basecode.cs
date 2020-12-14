using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 负责记录各个表中的标志性\系统性的基础编码值，比如：状态ID 0.停用 1.启用等
    /// </summary>
    public class b_basecode
    {
        /// <summary>
        /// 负责记录各个表中的标志性\系统性的基础编码值，比如：状态ID 0.停用 1.启用等
        /// </summary>
        public b_basecode()
        {
        }

        private System.Int32 _baseid;
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 baseid { get { return this._baseid; } set { this._baseid = value; } }

        private System.String _name;
        /// <summary>
        /// 
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.Int16 _valueid;
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int16 valueid { get { return this._valueid; } set { this._valueid = value; } }

        private System.String _valuecode;
        /// <summary>
        /// 
        /// </summary>
        public System.String valuecode { get { return this._valuecode; } set { this._valuecode = value?.Trim(); } }

        private System.String _value;
        /// <summary>
        /// 
        /// </summary>
        public System.String value { get { return this._value; } set { this._value = value?.Trim(); } }

        private System.Int16 _stateid;
        /// <summary>
        /// 
        /// </summary>
        public System.Int16 stateid { get { return this._stateid; } set { this._stateid = value; } }

        private System.Int16 _propertyid;
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int16 propertyid { get { return this._propertyid; } set { this._propertyid = value; } }

        private System.String _property;
        /// <summary>
        /// 
        /// </summary>
        public System.String property { get { return this._property; } set { this._property = value?.Trim(); } }
    }
}