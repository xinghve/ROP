using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 器械设备分页查询
    /// </summary>
    public class EquipmentPagesSearch : Search
    {
        private System.String _name;
        /// <summary>
        /// 设备名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _model;
        /// <summary>
        /// 型号
        /// </summary>
        public System.String model { get { return this._model; } set { this._model = value?.Trim(); } }

        private System.String _supplier;
        /// <summary>
        /// 供货商
        /// </summary>
        public System.String supplier { get { return this._supplier; } set { this._supplier = value?.Trim(); } }

        private System.String _manufactor;
        /// <summary>
        /// 厂家
        /// </summary>
        public System.String manufactor { get { return this._manufactor; } set { this._manufactor = value?.Trim(); } }

        private System.Int32 _store_id = 0;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        /// <summary>
        /// 设备启用状态
        /// </summary>
        public int state { get; set; } = 1;
    }
}
