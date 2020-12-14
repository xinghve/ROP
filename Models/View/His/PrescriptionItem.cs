﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 开处方项目
    /// </summary>
    public class PrescriptionItem
    {
        private System.Int32 _specid;
        /// <summary>
        /// 规格ID
        /// </summary>
        public System.Int32 specid { get { return this._specid; } set { this._specid = value; } }

        private System.String _packid;
        /// <summary>
        /// 包ID
        /// </summary>
        public System.String packid { get { return this._packid; } set { this._packid = value?.Trim(); } }

        private System.String _content;
        /// <summary>
        /// 内容
        /// </summary>
        public System.String content { get { return this._content; } set { this._content = value?.Trim(); } }

        private System.Int16 _quantity;
        /// <summary>
        /// 数量
        /// </summary>
        public System.Int16 quantity { get { return this._quantity; } set { this._quantity = value; } }

        private System.Int16? _usageid;
        /// <summary>
        /// 用法ID
        /// </summary>
        public System.Int16? usageid { get { return this._usageid; } set { this._usageid = value ?? default(System.Int16); } }

        private System.String _usagename;
        /// <summary>
        /// 用法
        /// </summary>
        public System.String usagename { get { return this._usagename; } set { this._usagename = value?.Trim(); } }

        private System.Int16? _frequecyid;
        /// <summary>
        /// 频率ID
        /// </summary>
        public System.Int16? frequecyid { get { return this._frequecyid; } set { this._frequecyid = value ?? default(System.Int16); } }

        private System.String _frequecyname;
        /// <summary>
        /// 频率
        /// </summary>
        public System.String frequecyname { get { return this._frequecyname; } set { this._frequecyname = value?.Trim(); } }

        private System.Decimal? _sigle;
        /// <summary>
        /// 单量
        /// </summary>
        public System.Decimal? sigle { get { return this._sigle; } set { this._sigle = value ?? default(System.Decimal); } }

        private System.Int32? _groupid;
        /// <summary>
        /// 组ID
        /// </summary>
        public System.Int32? groupid { get { return this._groupid; } set { this._groupid = value ?? default(System.Int32); } }

        /// <summary>
        /// 规格名称
        /// </summary>
        public string specname { get; set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        public int itemid { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string itemname { get; set; }

        /// <summary>
        /// 医疗室ID
        /// </summary>
        public int room_id { get; set; }

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

        /// <summary>
        /// 项目规格列表
        /// </summary>
        public List<ItemSpecs> specList { get; set; }
    }
}
