using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 签到
    /// </summary>
    public class Sign
    {
        /// <summary>
        /// 客户ID
        /// </summary>
        public int archives_id { get; set; }

        /// <summary>
        /// 签到对应ID
        /// </summary>
        public int sign_id { get; set; }

        /// <summary>
        /// 分类ID（1=门诊，2=康复）
        /// </summary>
        public short type_id { get; set; }

        /// <summary>
        /// 医疗室ID
        /// </summary>
        public int room_id { get; set; }

        /// <summary>
        /// 医疗室名称
        /// </summary>
        public string room_name { get; set; }

        /// <summary>
        /// 医疗室位置
        /// </summary>
        public string room_address { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public int equipment_id { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string equipment_no { get; set; }
    }
}
