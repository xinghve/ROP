using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    ///项目跟规格List
    /// </summary>
     public class ItemSpecModel
    {
        /// <summary>
        /// 项目
        /// </summary>
        private System.Int32 _item_id;
        /// <summary>
        /// 项目ID
        /// </summary>
        public System.Int32 item_id { get { return this._item_id; } set { this._item_id = value; } }


        private System.Int16 _type_id;
        /// <summary>
        /// 类别ID
        /// </summary>
        public System.Int16 type_id { get { return this._type_id; } set { this._type_id = value; } }

        private System.String _trade_name;
        /// <summary>
        /// 商品名
        /// </summary>
        public System.String trade_name { get { return this._trade_name; } set { this._trade_name = value?.Trim(); } }

        private System.Int16 _fee_id;
        /// <summary>
        /// 费别ID
        /// </summary>
        public System.Int16 fee_id { get { return this._fee_id; } set { this._fee_id = value; } }

        private System.String _fee_name;
        /// <summary>
        /// 费别
        /// </summary>
        public System.String fee_name { get { return this._fee_name; } set { this._fee_name = value?.Trim(); } }

        /// <summary>
        /// 规格List
        /// </summary>
        public List<h_itemspec> specList { get; set; }

        /// <summary>
        /// 设备标志
        /// </summary>
        public short? equipment { get; set; }
    }

    /// <summary>
    /// 设备项目
    /// </summary>
    public class EquipmentItem 
    {
        /// <summary>
        /// 项目
        /// </summary>
        private System.Int32 _item_id;
        /// <summary>
        /// 项目ID
        /// </summary>
        public System.Int32 item_id { get { return this._item_id; } set { this._item_id = value; } }

        private System.String _item_name;
        /// <summary>
        /// 项目名
        /// </summary>
        public System.String item_name { get { return this._item_name; } set { this._item_name = value?.Trim(); } }


        private System.String _common_name;
        /// <summary>
        /// 通用名
        /// </summary>
        public System.String common_name { get { return this._common_name; } set { this._common_name = value?.Trim(); } }

    }


}
