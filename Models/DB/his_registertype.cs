using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 挂号类别
    /// </summary>
    public class his_registertype
    {
        /// <summary>
        /// 挂号类别
        /// </summary>
        public his_registertype()
        {
        }

        private System.Int32 _typeid;
        /// <summary>
        /// 类别ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 typeid { get { return this._typeid; } set { this._typeid = value; } }

        private System.String _typename;
        /// <summary>
        /// 类别名称
        /// </summary>
        public System.String typename { get { return this._typename; } set { this._typename = value?.Trim(); } }

        private System.Int16 _orderflag;
        /// <summary>
        /// 预约标志
        /// </summary>
        public System.Int16 orderflag { get { return this._orderflag; } set { this._orderflag = value; } }

        private System.Int16 _stateid;
        /// <summary>
        /// 状态ID
        /// </summary>
        public System.Int16 stateid { get { return this._stateid; } set { this._stateid = value; } }

        private System.Decimal _amount;
        /// <summary>
        /// 总金额
        /// </summary>
        public System.Decimal amount { get { return this._amount; } set { this._amount = value; } }

        private System.Int32 _orgid;
        /// <summary>
        /// 机构ID
        /// </summary>
        public System.Int32 orgid { get { return this._orgid; } set { this._orgid = value; } }
    }
}