using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Crm
{
    /// <summary>
    /// 会员信息
    /// </summary>
    public class Archives
    {
        /// <summary>
        /// 档案信息
        /// </summary>
        public c_archives archives { get; set; }

        /// <summary>
        /// 档案附页
        /// </summary>
        public c_archives_supplement archivesSupplement { get; set; }

        ///// <summary>
        ///// 档案信息扩展
        ///// </summary>
        //public c_archives_extend archivesExtend { get; set; }

        ///// <summary>
        ///// 标签
        ///// </summary>
        //public List<archives_tag> archivesTags { get; set; }

        ///// <summary>
        ///// 偏好
        ///// </summary>
        //public List<string> preference { get; set; }

        /// <summary>
        /// 是否修改密码
        /// </summary>
        public bool is_change_password { get; set; }

        /// <summary>
        /// 是否立即充值
        /// </summary>
        public int is_recharge { get; set; }

        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }
    }

    /// <summary>
    /// 标签
    /// </summary>
    public class archives_tag
    {
        private System.Int32 _tag_id;
        /// <summary>
        /// 标签编码
        /// </summary>
        public System.Int32 tag_id { get { return this._tag_id; } set { this._tag_id = value; } }

        private System.String _tag;
        /// <summary>
        /// 标签
        /// </summary>
        public System.String tag { get { return this._tag; } set { this._tag = value?.Trim(); } }
    }
}
