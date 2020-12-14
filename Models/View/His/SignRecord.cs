using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 签到记录
    /// </summary>
    public class SignRecord
    {
        /// <summary>
        /// 挂号ID或需做康复项目预约ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public int archives_id { get; set; }

        /// <summary>
        /// 客户姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 客户电话
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// 分类ID（1=门诊，2=康复）
        /// </summary>
        public short type_id { get; set; }

        /// <summary>
        /// 分类名称（1=门诊，2=康复）
        /// </summary>
        public string type_name { get; set; }

        /// <summary>
        /// 签到内容（如：预约就诊、挂号就诊、康复具体项目）
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 医生名称
        /// </summary>
        public string doctorname { get; set; }

        /// <summary>
        /// 负责人名称
        /// </summary>
        public string to_director { get; set; }

        /// <summary>
        /// 负责人电话
        /// </summary>
        public string to_director_phone { get; set; }

        /// <summary>
        /// 签到时间
        /// </summary>
        public DateTime? sign_time { get; set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        public int item_id { get; set; }

        /// <summary>
        /// 规格ID
        /// </summary>
        public int spec_id { get; set; }

        /// <summary>
        /// 设备标志
        /// </summary>
        public short? equipment { get; set; }

        /// <summary>
        /// 设备信息
        /// </summary>
        public string equipment_info { get; set; }

        /// <summary>
        /// 医疗室信息
        /// </summary>
        public string room_info { get; set; }
    }
}
