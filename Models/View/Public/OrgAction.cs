using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 集团权限
    /// </summary>
    public class OrgAction
    {
        /// <summary>
        /// 集团id
        /// </summary>
        public int org_id { get; set; }
        /// <summary>
        /// 权限数组int[]
        /// </summary>
        public List<Action> actions { get; set; }
    }

    /// <summary>
    /// 功能菜单
    /// </summary>
    public class Action
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 菜单类型
        /// </summary>
        public short action_type { get; set; }
    }
}
