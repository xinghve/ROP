using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 挂号类别费用
    /// </summary>
    public class his_registertypefee
    {
        /// <summary>
        /// 挂号类别费用
        /// </summary>
        public his_registertypefee()
        {
        }

        private System.Int32 _typeid;
        /// <summary>
        /// 类别ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 typeid { get { return this._typeid; } set { this._typeid = value; } }

        private System.Int16 _specid;
        /// <summary>
        /// 规格ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int16 specid { get { return this._specid; } set { this._specid = value; } }

        private System.Int16 _quantiry;
        /// <summary>
        /// 次数
        /// </summary>
        public System.Int16 quantiry { get { return this._quantiry; } set { this._quantiry = value; } }

        private System.Int16 _orderflag;
        /// <summary>
        /// 预约标志
        /// </summary>
        public System.Int16 orderflag { get { return this._orderflag; } set { this._orderflag = value; } }
    }
}