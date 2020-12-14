using Microsoft.AspNetCore.Http;
using Models.DB;
using Models.View.Crm;
using Models.View.Mobile;
using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Crm
{
    /// <summary>
    /// 会员卡接口
    /// </summary>
    public interface IACardService
    {
        /// <summary>
        /// 会员卡分页查询
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Page<CardPageModel>> GetPageAsync(ACardModel model, bool name_or_phone = false);
        /// <summary>
        /// 会员卡充值
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> Recharge(RechargeModel model);
        /// <summary>
        /// 充值记录分页查询
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Page<Recharge>> GetRechargePageAsync(RJLModel model, bool name_or_phone = false, bool is_me = false);

        /// <summary>
        /// 获取充值业绩
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<dynamic> GetRechargeAsync(short type);

        /// <summary>
        /// 获取充值业绩排行
        /// </summary>
        /// <param name="store_id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<dynamic> GetRechargeOrderAsync(int store_id, short type);

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<List<CardPageModel>> GetPrintAsync(PrintModel model);

        /// <summary>
        /// 获取账户信息（移动端）
        /// </summary>
        /// <param name="archives_id"></param>
        /// <returns></returns>
        Task<dynamic> GetAccountAsync(int archives_id);

        /// <summary>
        /// 获取档案账户信息（客户端）
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetArcAccountAsync();

        /// <summary>
        /// 获取门店总充值业绩排行
        /// </summary>
        /// <param name="store_id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<dynamic> GetTotalRechargeOrderAsync(int store_id, short type);

        /// <summary>
        /// 获取充值记录（客户端）
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetRechargeListAsync();

        /// <summary>
        /// 消费记录分页查询（客户端）
        /// </summary>
        /// <returns></returns>
        Task<Page<Spend>> GetSpendPagesAsync(Search entity);

        /// <summary>
        /// 消费记录（PC）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<ConsumerList>> GetConsumerPagesAsync(ConsumerSearch entity);

        /// <summary>
        /// 获取优惠券金额
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="no"></param>
        /// <returns></returns>
        Task<object> GetCouponMoney(int storeId,string no);

        /// <summary>
        /// 获取消费详情
        /// </summary>
        /// <returns></returns>
        Task<List<f_balancedetail>> GetConsumerDetail(int balance_id);
    }
}
