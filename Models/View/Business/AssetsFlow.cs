using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 资产流向
    /// </summary>
    public class AssetsFlow
    {
        /// <summary>
        /// 日期时间
        /// </summary>
        public string datetime { get; set; }

        /// <summary>
        /// 类型（1=入库；2=调出；3=调入；4=领用；5=归还；6=维修；7=报废）
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 门店
        /// </summary>
        public string store { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string employee { get; set; }

        /// <summary>
        /// 调拨类型
        /// </summary>
        public string transfer_type { get; set; }

        /// <summary>
        /// 关联单号
        /// </summary>
        public string realted_no { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 领用部门
        /// </summary>
        public string dept { get; set; }

        /// <summary>
        /// 领用人/使用人
        /// </summary>
        public string requisitions_employee { get; set; }

        /// <summary>
        /// 维修内容
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 维修结果
        /// </summary>
        public string result { get; set; }
    }
}
