using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 厂家查询
    /// </summary>
    public class ManufactorySearch
    {
        private System.String _name;
        /// <summary>
        /// 搜索名
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        /// <summary>
        /// 状态id
        /// </summary>
        public int state_id { get; set; }

        /// <summary>
        /// 分类id
        /// </summary>
        public int class_id { get; set; }

        private short _catalog_type = 0;
        /// <summary>
        /// 目录类型(0=所有；1=目录；2=类型)
        /// </summary>
        public short catalog_type { get { return this._catalog_type; } set { this._catalog_type = value; } }

        private System.String _order = "id";
        /// <summary>
        /// 排序字段
        /// </summary>
        public System.String order { get { return this._order; } set { this._order = value?.Trim(); } }

        private System.Int32 _orderType = 0;
        /// <summary>
        /// 排序方式
        /// </summary>
        public System.Int32 orderType { get { return this._orderType; } set { this._orderType = value; } }

        private System.Int32 _limit = 10;
        /// <summary>
        /// 单页数据条数
        /// </summary>
        public System.Int32 limit { get { return this._limit; } set { this._limit = value; } }

        private System.Int32 _page = 1;
        /// <summary>
        /// 查询第几页
        /// </summary>
        public System.Int32 page { get { return this._page; } set { this._page = value; } }

    }

    /// <summary>
    /// 分页数据
    /// </summary>
    public class ManufactoryModel :h_manufactor
    { 
        
        /// <summary>
        /// 分类
        /// </summary>
        public List<ClassModel> classEntity { get; set; }
    }

    /// <summary>
    /// 分类实体
    /// </summary>
    public class ClassModel
    {
        /// <summary>
        /// 分类名
        /// </summary>
        public string class_name { get; set; }
        /// <summary>
        /// 分类id
        /// </summary>
        public int class_id { get; set; }
    }

    /// <summary>
    /// 供应商List
    /// </summary>
    public class ManufactoryList
    {
        /// <summary>
        /// 供应商
        /// </summary>
        public string manufactor { get; set; }
        /// <summary>
        /// 供应商id
        /// </summary>
        public int manufactor_id { get; set; }
    }
}
