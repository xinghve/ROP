using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Reports
{
    /// <summary>
    /// 主页报表
    /// </summary>
    public class MainPageModel
    {
        private System.Decimal? _recharge_money;
        /// <summary>
        /// 总充值金额
        /// </summary>
        public System.Decimal? recharge_money { get { return this._recharge_money; } set { this._recharge_money = value ?? default(System.Decimal); } }

        private System.Decimal? _give_total_money;
        /// <summary>
        /// 赠送总金额
        /// </summary>
        public System.Decimal? give_total_money { get { return this._give_total_money; } set { this._give_total_money = value ?? default(System.Decimal); } }

        private System.Int32 _his_register;
        /// <summary>
        ///挂号数
        /// </summary>
        public System.Int32 his_registerCount { get { return this._his_register; } set { this._his_register = value; } }

        private System.Int32 _sign_in;
        /// <summary>
        ///签到数
        /// </summary>
        public System.Int32 sign_inCount { get { return this._sign_in; } set { this._sign_in = value; } }

        private System.Int32 _order;
        /// <summary>
        ///成单数
        /// </summary>
        public System.Int32 orderCount { get { return this._order; } set { this._order = value; } }

        private System.Int32 _archives;
        /// <summary>
        ///新增客户
        /// </summary>
        public System.Int32 archivesCount { get { return this._archives; } set { this._archives = value; } }

        /// <summary>
        /// 门店销售比
        /// </summary>
        public List<storeList> storeList { get; set; }
    }
    /// <summary>
    /// 客户增长数量
    /// </summary>
    public class archivesList
    {
        private System.Int32 _archives;
        /// <summary>
        ///新增客户数量
        /// </summary>
        public System.Int32 archivesCount { get { return this._archives; } set { this._archives = value; } }

        private System.Int32 _month;
        /// <summary>
        /// 月
        /// </summary>
        public System.Int32 month { get { return this._month; } set { this._month = value; } }
    }

    /// <summary>
    /// 业绩
    /// </summary>
    public class rechargeList
    {
        /// <summary>
        /// 业绩List
        /// </summary>
        public List<rechargeMoneyList> rechargeMoneyList { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public Decimal? moneyTotal { get; set; }
    }

    /// <summary>
    ///  业绩
    /// </summary>
    public class rechargeMoneyList
    {
        private System.Decimal? _recharge_money;
        /// <summary>
        /// 总充值金额
        /// </summary>
        public System.Decimal? recharge_money { get { return this._recharge_money; } set { this._recharge_money = value ?? default(System.Decimal); } }

        private System.Int32 _month;
        /// <summary>
        /// 月
        /// </summary>
        public System.Int32 month { get { return this._month; } set { this._month = value; } }
        
    }

    /// <summary>
    /// 挂号数List
    /// </summary>
    public class his_registerList
    {
        private System.Int32 _his_register;
        /// <summary>
        ///挂号数量
        /// </summary>
        public System.Int32 his_register { get { return this._his_register; } set { this._his_register = value; } }

        private System.Int32 _month;
        /// <summary>
        /// 月
        /// </summary>
        public System.Int32 month { get { return this._month; } set { this._month = value; } }
    }

    /// <summary>
    /// 签到数List
    /// </summary>
    public class signList
    {
        private System.Int32 _sign;
        /// <summary>
        ///签到数量
        /// </summary>
        public System.Int32 sign { get { return this._sign; } set { this._sign = value; } }

        private System.Int32 _month;
        /// <summary>
        /// 月
        /// </summary>
        public System.Int32 month { get { return this._month; } set { this._month = value; } }
    }

    /// <summary>
    /// 成单数量List
    /// </summary>
    public class orderList
    {
        private System.Int32 _orderCount;
        /// <summary>
        ///成单数量
        /// </summary>
        public System.Int32 orderCount { get { return this._orderCount; } set { this._orderCount = value; } }

        private System.Int32 _month;
        /// <summary>
        /// 月
        /// </summary>
        public System.Int32 month { get { return this._month; } set { this._month = value; } }
    }

    /// <summary>
    /// 门店销售比
    /// </summary>
    public class storeList
    {
        /// <summary>
        /// 门店名
        /// </summary>
        public string name { get; set; }

        private System.Decimal? _sale_money=0;
        /// <summary>
        /// 门店销售总金额
        /// </summary>
        public System.Decimal? sale_money { get { return this._sale_money; } set { this._sale_money = value ?? default(System.Decimal); } }

    }
    /// <summary>
    /// 整体情况
    /// </summary>
    public class totalList
    {
        /// <summary>
        /// 客户增长数List
        /// </summary>
        public List<archivesList> archivesList { get; set; }


        /// <summary>
        /// 挂号数量List
        /// </summary>
        public List<his_registerList> his_registerList { get; set; }
        
        /// <summary>
        /// 签到数量List
        /// </summary>
        public List<signList> signList { get; set; }

        /// <summary>
        /// 成单数量List
        /// </summary>
        public List<orderList> orderList { get; set; }
        
        /// <summary>
        /// 获取年List
        /// </summary>
        public List<yearList> yearList { get; set; }

        /// <summary>
        /// 客户增长总数
        /// </summary>
        public int archivesCounts { get; set; }



    }

    /// <summary>
    /// 医护板块
    /// </summary>
    public class doctorModel
    {
        private System.Int32 _his_register;
        /// <summary>
        ///挂号数
        /// </summary>
        public System.Int32 his_registerCount { get { return this._his_register; } set { this._his_register = value; } }

        private System.Int32 _sign_in;
        /// <summary>
        ///签到数
        /// </summary>
        public System.Int32 sign_inCount { get { return this._sign_in; } set { this._sign_in = value; } }
        
    }

    /// <summary>
    /// 获取年月份List
    /// </summary>
    public class yearList
    {
        public  int year { get; set; }
        /// <summary>
        /// 获取月
        /// </summary>
        public List<int> months { get; set; }
    }






}
