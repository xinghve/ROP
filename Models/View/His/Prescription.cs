using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 开处方
    /// </summary>
    public class Prescription
    {
        /// <summary>
        /// 申请单ID
        /// </summary>
        public int applyid { get; set; }
        /// <summary>
        /// 就诊ID
        /// </summary>
        public int clinicid { get; set; }

        /// <summary>
        /// 档案ID
        /// </summary>
        public int archives_id { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 应收金额
        /// </summary>
        public decimal shouldamount { get; set; }

        /// <summary>
        /// 天数
        /// </summary>
        public short days { get; set; }

        private System.String _diagnosiscode;
        /// <summary>
        /// 诊断编码
        /// </summary>
        public System.String diagnosiscode { get { return this._diagnosiscode; } set { this._diagnosiscode = value?.Trim(); } }

        private System.String _diagnosisname;
        /// <summary>
        /// 诊断
        /// </summary>
        public System.String diagnosisname { get { return this._diagnosisname; } set { this._diagnosisname = value?.Trim(); } }

        private System.Int16 _double_num;
        /// <summary>
        /// 付数
        /// </summary>
        public System.Int16 double_num { get { return this._double_num; } set { this._double_num = value; } }

        private System.Int16 _typeid;
        /// <summary>
        /// 单据类型ID
        /// </summary>
        public System.Int16 typeid { get { return this._typeid; } set { this._typeid = value; } }

        private System.String _typename;
        /// <summary>
        /// 单据类型
        /// </summary>
        public System.String typename { get { return this._typename; } set { this._typename = value?.Trim(); } }

        /// <summary>
        /// 摘要
        /// </summary>
        public string summay { get; set; }

        /// <summary>
        /// 结算标志
        /// </summary>
        public int issettle { get; set; }

        /// <summary>
        /// 处方项目
        /// </summary>
        public List<PrescriptionItem> prescriptionItems { get; set; }
    }
}
