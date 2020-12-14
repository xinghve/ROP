using Models.DB;
using Models.View.His;
using Service.Extensions;
using Service.Repository.Interfaces.His;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;
using static Tools.IdentityModels.GetUser;

namespace Service.Repository.Implements.His
{
    /// <summary>
    /// 诊疗单据业务
    /// </summary>
     public class BillService:DbContext,IBillService
    {
        //获取用户
        private UserInfo userInfo = new Tools.IdentityModels.GetUser().userInfo;


        /// <summary>
        /// 获取诊疗单据分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<h_bill>> GetPageAsync(BillSearch entity)
        {
            var list = Db.Queryable<h_bill>()
             .Where(w => w.org_id == userInfo.org_id )
             .WhereIF(!string.IsNullOrEmpty(entity.name), w => w.bill_name.Contains(entity.name));

            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            list = list.OrderBy(entity.order + orderTypeStr);
            return await list.WithCache().ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 添加诊疗单据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> AddAsync(h_bill entity)
        {

            var isExtist = await Db.Queryable<h_bill>().WithCache().AnyAsync(s=>s.org_id==userInfo.org_id&&s.property_id==entity.property_id&&s.bill_name==entity.bill_name);
            if (isExtist)
            {
                throw new MessageException("已存在该属性的单据！");
            }
            entity.state_id = 1;
            entity.creater = userInfo.name;
            entity.creater_id = userInfo.id;
            entity.create_date = DateTime.Now;
            entity.org_id = userInfo.org_id;

            var isSuccess= await Db.Insertable(entity).ExecuteCommandAsync();
            redisCache.RemoveAll<h_bill>();

            return isSuccess;
        }

        /// <summary>
        /// 编辑诊疗单据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> ModifyAsync(h_bill entity)
        {

            var isExtist = await Db.Queryable<h_bill>().WithCache().AnyAsync(s => s.org_id == userInfo.org_id && s.property_id == entity.property_id && s.bill_name == entity.bill_name&&s.bill_id!=entity.bill_id);
            if (isExtist)
            {
                throw new MessageException("已存在该属性的单据！");
            }

            return await Db.Updateable(entity).SetColumns(it => new h_bill{ bill_name= entity.bill_name, property_id= entity.property_id,property= entity.property })
                    .Where(a => a.bill_id == entity.bill_id&&a.org_id==userInfo.org_id)
                    .RemoveDataCache()
                    .EnableDiffLogEvent()
                    .ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除诊疗单据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(int id)
        {
            var isUse = await Db.Queryable<h_itemspec>().WithCache().AnyAsync(s=>s.billid==id);
            if (isUse)
            {
                throw new MessageException("该单据使用中不能删除！");
            }

            return await Db.Deleteable<h_bill>().Where(s => s.bill_id == id && s.org_id == userInfo.org_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }
    }
}
