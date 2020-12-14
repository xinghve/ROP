using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 门店
    /// </summary>
    public class Store
    {
        /// <summary>
        /// 门店ID
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 门店名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 门店名称
        /// </summary>
        public bool is_admin { get; set; }
    }

    /// <summary>
    /// 门店简介model
    /// </summary>
    public class StoreIntroduceModel
    {
        /// <summary>
        /// 图片地址
        /// </summary>
        public List<string> imgUrlList { get; set; }

        /// <summary>
        /// 门店简介
        /// </summary>
        public string introduce { get; set; }
        /// <summary>
        /// 门店地址
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string tel { get; set; }

        /// <summary>
        /// 工作时间
        /// </summary>
        public string worktime { get; set; }
    }
}
