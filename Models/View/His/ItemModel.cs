using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 项目
    /// </summary>
    public class ItemModel:h_item
    {
        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.Int16 _typeid;
        /// <summary>
        /// 类别ID
        /// </summary>
        public System.Int16 typeid { get { return this._typeid; } set { this._typeid = value; } }

    
    }

    /// <summary>
    /// 项目的所有model
    /// </summary>
    public class ItemAllModel:h_itemspec
    {
        /// <summary>
        /// 目录名
        /// </summary>
        public string dir_name { get; set; }

        /// <summary>
        /// 目录类型名
        /// </summary>
        public string dir_type_name { get; set; }

        /// <summary>
        /// 项目名
        /// </summary>
        public string trade_name { get; set; }

        /// <summary>
        /// 通用名
        /// </summary>
        public string common_name { get; set; }

        /// <summary>
        /// 费别名
        /// </summary>
        public string fee_name { get; set; }

        /// <summary>
        /// 服务对象
        /// </summary>
        public string service_name { get; set; }

        /// <summary>
        /// 性别限制
        /// </summary>
        public string sex { get; set; }

        /// <summary>
        /// 物价编码
        /// </summary>
        public string price_code { get; set; }

        /// <summary>
        /// 是否公示
        /// </summary>
        public string publisher { get; set; }

        /// <summary>
        /// 是否外检
        /// </summary>
        public string external { get; set; }

        /// <summary>
        /// 是否折扣
        /// </summary>
        public string discount { get; set; }

        /// <summary>
        /// 是否设备
        /// </summary>
        public string equipment { get; set; }

        /// <summary>
        /// 收入
        /// </summary>
        public string income { get; set; }

        /// <summary>
        /// 单据
        /// </summary>
        public string bill { get; set; }
    }
}
