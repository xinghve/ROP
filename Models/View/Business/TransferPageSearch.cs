using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 调拨分页查询
    /// </summary>
    public class TransferPageSearch : Search
    {
        /// <summary>
        /// 调拨单号
        /// </summary>
        public string bill_no { get; set; }

        /// <summary>
        /// 申请单号
        /// </summary>
        public string apply_no { get; set; }

        /// <summary>
        /// 类型ID（-1=所有；1=采购；2=其他）
        /// </summary>
        public short type_id { get; set; }

        /// <summary>
        /// 调出门店ID
        /// </summary>
        public int out_store_id { get; set; }

        /// <summary>
        /// 调入门店ID
        /// </summary>
        public int in_store_id { get; set; }

        /// <summary>
        /// 状态（-1=所有；26=待审核；36=审核中；28=未通过；34=待出库；41=调拨中；15=已完成；7=已取消；-2=已审核（34、41、15、28）；-3=申请页所有状态（26、36、34、41、15、7）；-4=调拨页所有状态（34、41、15、7））
        /// </summary>
        public short state { get; set; }

        /// <summary>
        /// 调入调出（-1=所有；1=调入；2=调出）
        /// </summary>
        public short in_or_out { get; set; }

        /// <summary>
        /// 登录门店ID
        /// </summary>
        public int store_id { get; set; }
    }
}
