using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 供应商分类
    /// </summary>
    public class ManufactorClassModel
    {
        private System.Int32 _class_id;
        /// <summary>
        /// 分类id
        /// </summary>
        public System.Int32 class_id { get { return this._class_id; } set { this._class_id = value; } }

        /// <summary>
        /// 供应商List
        /// </summary>
        public List<ManufactoryId> manufactoryList { get; set; }
    }

    /// <summary>
    /// 供应商ids
    /// </summary>
    public class ManufactoryId
    {
        private System.Int32 _manufactor_id;
        /// <summary>
        /// 厂家id
        /// </summary>
        public System.Int32 manufactor_id { get { return this._manufactor_id; } set { this._manufactor_id = value; } }
    }

    /// <summary>
    /// 修改状态
    /// </summary>
    public class ManufactorSetState
    {
        /// <summary>
        /// 供应商id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 状态id
        /// </summary>
        public short state_id { get; set; }
    }
}
