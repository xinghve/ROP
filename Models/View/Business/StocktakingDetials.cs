using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 盘点明细
    /// </summary>
    public class StocktakingDetials : bus_stocktaking_detials
    {

        private System.Int16? _property_id;
        /// <summary>
        /// 属性ID
        /// </summary>
        public System.Int16? property_id { get { return this._property_id; } set { this._property_id = value ?? default(System.Int16); } }
    }
}
