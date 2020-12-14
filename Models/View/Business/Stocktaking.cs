using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 盘点
    /// </summary>
    public class Stocktaking : bus_stocktaking
    {
        /// <summary>
        /// 盘点单明细
        /// </summary>
        public List<StocktakingDetials> stocktaking_Detials { get; set; }
    }
}
