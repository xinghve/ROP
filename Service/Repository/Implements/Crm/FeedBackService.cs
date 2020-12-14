using Models.DB;
using Models.View.Crm;
using Service.Extensions;
using Service.Repository.Interfaces.Crm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;

namespace Service.Repository.Implements.Crm
{
    /// <summary>
    /// 投诉反馈业务
    /// </summary>
    public class FeedBackService : DbContext, IFeedBackService
    {
        /// <summary>
        /// 投诉反馈分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<r_feedback>> GetPageAsync(FeedBackModel entity)
        {
            var fdList = Db.Queryable<r_feedback>()
                 //.WhereIF(entity.type!=-1,w=>w.type==entity.type)
                 .WhereIF(entity.startTime != null, w => w.date >= entity.startTime)
                .WhereIF(entity.endTime != null, w => w.date <= entity.endTime)
                .WhereIF(entity.storeId >= 0, w => w.store_id <= entity.storeId)
                .WhereIF(entity.orgId >= 0, w => w.org_id <= entity.orgId);

            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            fdList = fdList.OrderBy(entity.order + orderTypeStr);
            return await fdList.ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 添加投诉反馈信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> Add(r_feedback entity)
        {
            var fbEntity = new r_feedback();
            fbEntity = entity;
            fbEntity.date = DateTime.Now;
            var isSuccess = await Db.Insertable(fbEntity).ExecuteCommandAsync();
            if (isSuccess <= 0)
            {
                throw new MessageException("未添加成功!");
            }
            return isSuccess;
        }

        /// <summary>
        /// 编辑投诉反馈信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> Modify(r_feedback entity)
        {
            var isSuccess = await Db.Updateable(entity).SetColumns(s => s.content == entity.content).ExecuteCommandAsync();
            if (isSuccess <= 0)
            {
                throw new MessageException("未编辑成功!");
            }
            return isSuccess;
        }
        /// <summary>
        /// 批量删除投诉反馈信息
        /// </summary>
        /// <param name="delList"></param>
        /// <returns></returns>
        public async Task<int> Delete(List<int> delList)
        {
            return await Db.Deleteable<r_feedback>(delList).ExecuteCommandAsync();
        }
    }
}
