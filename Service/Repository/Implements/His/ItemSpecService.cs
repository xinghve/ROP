using Models.DB;
using Models.View.His;
using Service.Extensions;
using Service.Repository.Interfaces.His;
using SqlSugar;
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
    /// 项目规格业务
    /// </summary>
    public class ItemSpecService : DbContext, IItemSpecService
    {
        private UserInfo userInfo = new Tools.IdentityModels.GetUser().userInfo;
        /// <summary>
        /// 获取项目规格分页数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Page<h_itemspec>> GetPageAsync(ItemSpecSearch model)
        {
            var list = Db.Queryable<h_itemspec, h_item>((ite, item) => new object[] { JoinType.Left, ite.itemid == item.item_id })
                .Where((ite, item) => ite.itemid == model.item_id && item.org_id == userInfo.org_id)
                .WhereIF(!string.IsNullOrEmpty(model.name), (ite, item) => ite.specname.Contains(model.name));

            var orderTypeStr = " Asc";
            if (model.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            list = list.OrderBy(model.order + orderTypeStr);
            return await list.WithCache().ToPageAsync(model.page, model.limit);
        }

        /// <summary>
        /// 添加项目规格
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> AddAsync(h_itemspec entity)
        {
            if (entity.itemid <= 0)
            {
                throw new MessageException("请选择项目!");
            }

            //查询是否存在相同规格
            var name = await Db.Queryable<h_itemspec>().WithCache().AnyAsync(w => w.itemid == entity.itemid && w.specname == entity.specname);
            if (name)
            {
                throw new MessageException("已存在此规格!");
            }

            var itemEntity = new h_itemspec();
            itemEntity = entity;
            itemEntity.stateid = 1;
            itemEntity.salsemodulus = 1;
            itemEntity.packmodulus = 1;
            itemEntity.packunit = "次";
            itemEntity.buy_price = 0;
            var isSuccess = await Db.Insertable(itemEntity).ExecuteCommandAsync();
            redisCache.RemoveAll<h_itemspec>();

            return isSuccess;
        }

        /// <summary>
        ///编辑项目规格
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> ModifyAsync(h_itemspec entity)
        {
            if (entity.itemid <= 0)
            {
                throw new MessageException("请选择项目!");
            }
            if (entity.specid <= 0)
            {
                throw new MessageException("请选择规格!");
            }

            //查询是否存在相同规格
            var name = await Db.Queryable<h_itemspec>().WithCache().AnyAsync(w => w.itemid == entity.itemid && w.specname == entity.specname&&w.specid!=entity.specid);
            if (name)
            {
                throw new MessageException("已存在此规格!");
            }

            var isSuccess = await Db.Updateable(entity).SetColumns(it => new h_itemspec { specname = entity.specname, hint = entity.hint, mindosage = entity.mindosage, dosageunit = entity.dosageunit, minunit = entity.minunit, standardcode = entity.standardcode, feegrade = entity.feegrade, incomeid = entity.incomeid, salseunit = entity.salseunit, sale_price = entity.sale_price, billid = entity.billid , expiredate=entity.expiredate})
                       .Where(a => a.specid == entity.specid && a.itemid == entity.itemid)
                       .RemoveDataCache()
                       .EnableDiffLogEvent()
                       .ExecuteCommandAsync();
            return isSuccess;
        }

        /// <summary>
        /// 删除项目规格
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                throw new MessageException("请选择规格！");
            }
            //查询规格是否被使用中
            var IsSpec = await Db.Queryable<f_balancedetail>()
                             .WithCache()
                             .AnyAsync(s => s.specid==id);

            if (IsSpec)
            {
                throw new MessageException("此项目使用中，不能删除！");
            }

            return await Db.Deleteable<h_itemspec>().Where(w => w.specid == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="item_id">项目ID</param>
        /// <returns></returns>
        public async Task<dynamic> GetListAsync(int item_id)
        {
            return await Db.Queryable<h_itemspec>()
                .Where(w => w.itemid == item_id)
                .Select(s => new { s.sale_price, s.salseunit, s.specid, s.specname })
                .WithCache()
                .ToListAsync();
        }
    }
}
