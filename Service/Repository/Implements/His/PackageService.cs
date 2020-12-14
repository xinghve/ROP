using Models.DB;
using Models.View.His;
using Service.Extensions;
using Service.Repository.Interfaces.His;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;
using static Tools.IdentityModels.GetUser;

namespace Service.Repository.Implements.His
{
    /// <summary>
    /// 套餐业务
    /// </summary>
    public class PackageService : DbContext, IPackageService
    {
        private readonly UserInfo userInfo = new Tools.IdentityModels.GetUser().userInfo;

        /// <summary>
        /// 套餐项目分页
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<PackageModel>> GetPageAsync(PackSearch entity)
        {
            if (entity.typeId <= 0)
            {
                throw new MessageException("请选择套餐类别！");
            }

            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            //分页
            var packList = await Db.Queryable<his_applycontent>()
                               .Where(w => w.org_id == userInfo.org_id && w.store_id == entity.storeId  && w.packid!="0" && w.type_id == entity.typeId)
                                .WhereIF(!string.IsNullOrEmpty(entity.name), w => w.content.Contains(entity.name))
                               .GroupBy(w => new { w.packid, w.content, w.type_id,w.applyid })
                               .Select(w => new PackageModel { packid = w.packid, content = w.content, type_id = w.type_id, applyid=w.applyid })
                               .OrderBy(entity.order + orderTypeStr)
                               .WithCache()
                               .ToPageAsync(entity.page, entity.limit);
            return packList;
        }

        /// <summary>
        /// 根据套餐获取项目
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<his_applycontent>> GetItemPageAsync(PackItemSearch entity)
        {
            if (string.IsNullOrEmpty(entity.packId))
            {
                throw new MessageException("请选择套餐！");
            }

            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            //分页
            var packList = await Db.Queryable<his_applycontent>()
                               .Where(w => w.org_id == userInfo.org_id && w.store_id == entity.storeId  && w.packid == entity.packId&&w.applyid==entity.applyid)
                                .WhereIF(!string.IsNullOrEmpty(entity.name), w => w.specname.Contains(entity.name))
                               .OrderBy(entity.order + orderTypeStr)
                               .WithCache()
                               .ToPageAsync(entity.page, entity.limit);
            return packList;
        }




        /// <summary>
        /// 获取套餐列表
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public async Task<List<PackageModel>> GetList(int typeId, int storeId)
        {
            if (typeId <= 0)
            {
                throw new MessageException("请选择套餐类别！");
            }

            //申请数据
            var applyList= await Db.Queryable<his_applycontent>()
                               .Where(w => w.org_id == userInfo.org_id && w.store_id == storeId && w.packid != "0" && w.type_id == typeId)
                               .WithCache()
                               .ToListAsync();
            //重复数据
            var packList = applyList
                               .GroupBy(w => new { w.packid, w.content, w.type_id })
                               .Select(w => new PackageModel { packid = w.First().packid, content = w.First().content, type_id = w.First().type_id, shouldamount=w.Select(s=>s.shouldamount).Sum() })
                               .OrderBy(w => w.packid)
                               .ToList();
            
            //返回数据
            var newList = packList.Select(s => new PackageModel { shouldamount=s.shouldamount, packid = s.packid, content = s.content, packList = applyList.Where(w => w.packid == s.packid && w.type_id == s.type_id && w.store_id == storeId).ToList() }).ToList();
            packList = newList;

            return packList;
        }

        /// <summary>
        /// 添加套餐
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> AddAsync(PackageModel entity)
        {
            if (entity.store_id <= 0)
            {
                throw new MessageException("门店未传入数据！");
            }
            if (entity.type_id <= 0)
            {
                throw new MessageException("未选择套餐类型！");
            }

            //查询是否有相同套餐名
            var isExist = await Db.Queryable<his_applycontent>()
                                .WithCache()
                                .AnyAsync(w => w.content == entity.content && w.store_id == entity.store_id && w.type_id == entity.type_id);
            if (isExist)
            {
                throw new MessageException("已存在此套餐名！");
            }

            //获取申请单最小值
            var minCode =await Db.Queryable<his_applycontent>() 
                                 .WithCache()
                                 .MinAsync(w => w.applyid);

            var applyId = -1;
            if (minCode<0)
            {
                applyId = minCode - 1;
            }

            var packEntity = new his_applycontent();
            packEntity.store_id = entity.store_id;
            packEntity.type_id = entity.type_id;
            packEntity.org_id = userInfo.org_id;
            packEntity.applyid = applyId;
            packEntity.actualamount = 0;
            packEntity.content = entity.content;
            packEntity.orderid = 0;
            packEntity.specid = 0;
            packEntity.quantity = 0;
            packEntity.cost = 0;
            packEntity.price = 0;
            packEntity.shouldamount = 0;
            packEntity.specname = "无";
            packEntity.packid =DateTime.Now.ToString("yyyyMMddhhmmssfff");
            packEntity.execdeptid = userInfo.id;
            packEntity.execdept = userInfo.name;
            packEntity.execstateid = 0;
            packEntity.execstate = "无";
            packEntity.unitname = "无";
            packEntity.modulus = 0;

            var isResult = await Db.Insertable<his_applycontent>(packEntity).ExecuteCommandAsync();
            redisCache.RemoveAll<his_applycontent>();
            return isResult;
        }

        /// <summary>
        /// 编辑套餐名
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(PackageUpdateModel entity)
        {
            if (entity.store_id <= 0||string.IsNullOrEmpty(entity.packid))
            {
                throw new MessageException("门店未传入数据！");
            }
            if (entity.type_id <= 0)
            {
                throw new MessageException("未选择套餐类型！");
            }

            //查询是否有相同套餐名
            var isExist = await Db.Queryable<his_applycontent>()
                                .WithCache()
                                .AnyAsync(w => w.content == entity.content && w.store_id == entity.store_id && w.type_id == entity.type_id && w.packid != entity.packid);
            if (isExist)
            {
                throw new MessageException("已存在此套餐名！");
            }
            

            return await Db.Updateable<his_applycontent>(entity)
                                   .SetColumns(w=>w.content==entity.content)
                                   .Where(w=>w.store_id == entity.store_id && w.type_id == entity.type_id&&w.packid==entity.packid&&w.org_id==userInfo.org_id)
                                   .EnableDiffLogEvent().RemoveDataCache().ExecuteCommandAsync();
          
        }

        /// <summary>
        /// 编辑套餐
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ModifyAsync(PackageEntity entity)
        {
            if (string.IsNullOrEmpty(entity.packid ))
            {
                throw new MessageException("未选择套餐数据！");
            }

            //获取商品规格详情
            var specids = entity.packList.Select(s => s.specid).ToList();

            var itemlist = Db.Queryable<h_itemspec, h_item, b_basecode>((isp, i, b) => new object[] { JoinType.Left, isp.itemid == i.item_id, JoinType.Left, isp.incomeid == b.valueid && b.baseid == 66 }).Where((isp, i, b) => specids.Contains(isp.specid)).Select((isp, i, b) => new { itemspec = isp, item = i, b.value }).WithCache().ToList();
            var items = itemlist.Select(s => new { s.itemspec, s.item, s.value, pre_entity = entity.packList.Where(w => w.specid == s.itemspec.specid).FirstOrDefault() }).ToList();
            
            var packList = items.Select((s, index) => new his_applycontent { applyid = entity.applyid, actualamount = 0, content = entity.content, orderid = short.Parse(index.ToString()), specid = s.itemspec.specid, specname = s.itemspec.specname, packid = entity.packid, quantity = s.pre_entity.quantity, price = s.itemspec.sale_price, cost = s.itemspec.buy_price, shouldamount = s.itemspec.sale_price * s.pre_entity.quantity, usageid =entity.type_id==5?-1: s.pre_entity.usageid, usagename = entity.type_id == 5 ? "" : s.pre_entity.usagename, frequecyname = entity.type_id == 5 ? "" : s.pre_entity.frequecyname, frequecyid = entity.type_id == 5 ? -1 : s.pre_entity.frequecyid, sigle = entity.type_id == 5 ? 0 : s.pre_entity.sigle, dosageunit = s.itemspec.dosageunit, execdeptid = userInfo.id, execdept = userInfo.name, execstateid = 0, execstate = "", unitname = s.itemspec.salseunit, modulus = s.itemspec.salsemodulus, groupid =  0, item_id = s.item.item_id, item_name = s.item.trade_name, org_id = userInfo.org_id, store_id = entity.store_id, type_id = entity.type_id }).ToList();

            var result = await Db.Ado.UseTranAsync(() => {
                //删除之前数据，重新添加
                var isResult = Db.Deleteable<his_applycontent>()
                  .Where(s => s.packid == entity.packid&&s.org_id==userInfo.org_id&&s.store_id==entity.store_id&&s.type_id==entity.type_id)
                  .EnableDiffLogEvent()
                  .RemoveDataCache()
                  .ExecuteCommand();
                if (isResult<=0)
                {
                    throw new MessageException("编辑删除未成功！");
                }


                Db.Insertable<his_applycontent>(packList).ExecuteCommand();
                redisCache.RemoveAll<his_applycontent>();
            });

            return result.IsSuccess;
        }

        /// <summary>
        /// 删除套餐
        /// </summary>
        /// <param name="packid"></param>
        /// <param name="type_id"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(string packid, int storeId,int type_id)
        {
            if (string.IsNullOrEmpty(packid)||type_id<=0||storeId<=0)
            {
                throw new MessageException("未获取到套餐");
            }
            return await Db.Deleteable<his_applycontent>()
                  .Where(s => s.packid == packid && s.org_id == userInfo.org_id  && s.type_id == type_id&&s.store_id== storeId)
                  .EnableDiffLogEvent()
                  .RemoveDataCache()
                  .ExecuteCommandAsync();
        }
    }
}
