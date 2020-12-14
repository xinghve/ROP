using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 挂号费用记录
    /// </summary>
    public class his_registerfee
    {
        /// <summary>
        /// 挂号费用记录
        /// </summary>
        public his_registerfee()
        {
        }

        private System.Int32 _regid;
        /// <summary>
        /// 挂号ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 regid { get { return this._regid; } set { this._regid = value; } }

        private System.Int32 _specid;
        /// <summary>
        /// 规格ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 specid { get { return this._specid; } set { this._specid = value; } }

        private System.Decimal _shouldamount;
        /// <summary>
        /// 应收金额
        /// </summary>
        public System.Decimal shouldamount { get { return this._shouldamount; } set { this._shouldamount = value; } }

        private System.Decimal _actualamount;
        /// <summary>
        /// 实收金额
        /// </summary>
        public System.Decimal actualamount { get { return this._actualamount; } set { this._actualamount = value; } }
    }
}