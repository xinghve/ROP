using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 资产报损报溢明细
    /// </summary>
    public class bus_loss_overflow_detials
    {
        /// <summary>
        /// 资产报损报溢明细
        /// </summary>
        public bus_loss_overflow_detials()
        {
        }

        private System.String _no;
        /// <summary>
        /// 单号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        private System.String _remark;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remark { get { return this._remark; } set { this._remark = value?.Trim(); } }

        private System.String _assets_no;
        /// <summary>
        /// 固定资产编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String assets_no { get { return this._assets_no; } set { this._assets_no = value?.Trim(); } }
    }
}