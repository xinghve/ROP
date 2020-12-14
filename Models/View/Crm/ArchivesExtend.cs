using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Crm
{
    /// <summary>
    /// 档案信息扩展
    /// </summary>
    public class ArchivesExtend : c_archives
    {
        private System.String _drink;
        /// <summary>
        /// 饮料
        /// </summary>
        public System.String drink { get { return this._drink; } set { this._drink = value?.Trim(); } }

        private System.String _fruits;
        /// <summary>
        /// 水果
        /// </summary>
        public System.String fruits { get { return this._fruits; } set { this._fruits = value?.Trim(); } }

        private System.String _foods;
        /// <summary>
        /// 饮食
        /// </summary>
        public System.String foods { get { return this._foods; } set { this._foods = value?.Trim(); } }

        private System.String _smoke;
        /// <summary>
        /// 吸烟
        /// </summary>
        public System.String smoke { get { return this._smoke; } set { this._smoke = value?.Trim(); } }

        private System.String _habit;
        /// <summary>
        /// 习惯
        /// </summary>
        public System.String habit { get { return this._habit; } set { this._habit = value?.Trim(); } }

        private System.String _hobby;
        /// <summary>
        /// 爱好
        /// </summary>
        public System.String hobby { get { return this._hobby; } set { this._hobby = value?.Trim(); } }

        //private System.String _favour_way_code;
        ///// <summary>
        ///// 偏爱诊疗方式编码
        ///// </summary>
        //public System.String favour_way_code { get { return this._favour_way_code; } set { this._favour_way_code = value?.Trim(); } }

        //private System.String _favour_way;
        ///// <summary>
        ///// 偏爱诊疗方式
        ///// </summary>
        //public System.String favour_way { get { return this._favour_way; } set { this._favour_way = value?.Trim(); } }

        //private System.String _consume_habit_code;
        ///// <summary>
        ///// 消费习惯编码
        ///// </summary>
        //public System.String consume_habit_code { get { return this._consume_habit_code; } set { this._consume_habit_code = value?.Trim(); } }

        //private System.String _consume_habit;
        ///// <summary>
        ///// 消费习惯
        ///// </summary>
        //public System.String consume_habit { get { return this._consume_habit; } set { this._consume_habit = value?.Trim(); } }

        //private System.String _blood_type_code;
        ///// <summary>
        ///// 血型编码
        ///// </summary>
        //public System.String blood_type_code { get { return this._blood_type_code; } set { this._blood_type_code = value?.Trim(); } }

        //private System.String _blood_type;
        ///// <summary>
        ///// 血型
        ///// </summary>
        //public System.String blood_type { get { return this._blood_type; } set { this._blood_type = value?.Trim(); } }

        ///// <summary>
        ///// 标签
        ///// </summary>
        //public List<archives_tag> archivesTags { get; set; }

        ///// <summary>
        ///// 偏爱
        ///// </summary>
        //public List<string> archivesPreference { get; set; }

        private System.String _store_name;
        /// <summary>
        /// 门店名称
        /// </summary>
        public System.String store_name { get { return this._store_name; } set { this._store_name = value?.Trim(); } }
    }
}
