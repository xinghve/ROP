using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 分页返回套餐model
    /// </summary>
    public class PackageModel
    {
        private System.Int32 _applyid;
        /// <summary>
        /// 申请id
        /// </summary>
        public System.Int32 applyid { get { return this._applyid; } set { this._applyid = value; } }

        private System.Int32? _store_id;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32? store_id { get { return this._store_id; } set { this._store_id = value ?? default(System.Int32); } }

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

        private System.Int32? _type_id;
        /// <summary>
        /// 套餐类型id
        /// </summary>
        public System.Int32? type_id { get { return this._type_id; } set { this._type_id = value ?? default(System.Int32); } }

        /// <summary>
        /// 包
        /// </summary>
        public List<his_applycontent> packList { get; set; }

        private System.Decimal _shouldamount;
        /// <summary>
        /// 应收金额
        /// </summary>
        public System.Decimal shouldamount { get { return this._shouldamount; } set { this._shouldamount = value; } }
    }

    /// <summary>
    /// 编辑套餐名
    /// </summary>
    public class PackageUpdateModel
    {
        private System.Int32? _store_id;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32? store_id { get { return this._store_id; } set { this._store_id = value ?? default(System.Int32); } }

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

        private System.Int32? _type_id;
        /// <summary>
        /// 套餐类型id
        /// </summary>
        public System.Int32? type_id { get { return this._type_id; } set { this._type_id = value ?? default(System.Int32); } }
    }

    /// <summary>
    /// 套餐entity
    /// </summary>
    public class PackageEntity
    {
        private System.String _packid;
        /// <summary>
        /// 包ID
        /// </summary>
        public System.String packid { get { return this._packid; } set { this._packid = value?.Trim(); } }

        private System.Int32 _applyid;
        /// <summary>
        /// 申请id(编辑时)
        /// </summary>
        public System.Int32 applyid { get { return this._applyid; } set { this._applyid = value; } }

        private System.Int32? _type_id;
        /// <summary>
        /// 套餐类型id
        /// </summary>
        public System.Int32? type_id { get { return this._type_id; } set { this._type_id = value ?? default(System.Int32); } }

        private System.Int32? _store_id;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32? store_id { get { return this._store_id; } set { this._store_id = value ?? default(System.Int32); } }

        private System.String _content;
        /// <summary>
        /// 套餐名
        /// </summary>
        public System.String content { get { return this._content; } set { this._content = value?.Trim(); } }

        /// <summary>
        /// 包
        /// </summary>
        public List<PackList> packList { get; set; }


    }

    /// <summary>
    /// 包
    /// </summary>
    public class PackList
    {
        private System.Int32 _specid;
        /// <summary>
        /// 规格ID
        /// </summary>
        public System.Int32 specid { get { return this._specid; } set { this._specid = value; } }

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
    }

}
