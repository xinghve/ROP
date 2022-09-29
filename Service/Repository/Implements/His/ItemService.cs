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
    /// 项目业务
    /// </summary>
    public class ItemService : DbContext, IItemService
    {
        //获取用户
        private UserInfo userInfo = new Tools.IdentityModels.GetUser().userInfo;

        /// <summary>
        /// 项目分页查询
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<h_item>> GetItemAsync(ItemSearch entity)
        {
            var list = Db.Queryable<h_item>()
               .Where(w => w.org_id == userInfo.org_id && w.dir_id == entity.dir_id)
               .WhereIF(!string.IsNullOrEmpty(entity.name), w => w.trade_name.Contains(entity.name) || w.common_name.Contains(entity.name));

            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            list = list.OrderBy(entity.order + orderTypeStr);
            return await list.WithCache().ToPageAsync(entity.page, entity.limit);

        }

        /// <summary>
        /// 添加项目
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> AddItemAsync(ItemModel entity)
        {
            if (entity.dir_id <= 0)
            {
                throw new MessageException("请选择目录!");
            }
            //查询是否存在相同项目
            var name = await Db.Queryable<h_item>().WithCache().AnyAsync(w => w.org_id == userInfo.org_id && w.dir_id == entity.dir_id && w.trade_name == entity.trade_name && w.type_id == entity.type_id);
            if (name)
            {
                throw new MessageException("已存在此项目!");
            }

            var ItemEntity = new h_item();
            ItemEntity = entity;
            ItemEntity.type_id = 1;
            ItemEntity.org_id = userInfo.org_id;
            ItemEntity.code = entity.code;//目录编码
            ItemEntity.short_code = ToSpell.GetFirstPinyin(entity.trade_name);//简码
            ItemEntity.state_id = 1;
            ItemEntity.create_date = DateTime.Now;
            ItemEntity.creater = userInfo.name;
            ItemEntity.creater_id = userInfo.id;
            var isSuccess = await Db.Insertable(ItemEntity).ExecuteCommandAsync();
            redisCache.RemoveAll<h_item>();

            return isSuccess;
        }

        /// <summary>
        /// 编辑项目
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ModifyItemAsync(ItemModel entity)
        {
            if (entity.dir_id <= 0)
            {
                throw new MessageException("请选择目录!");
            }
            if (entity.item_id <= 0)
            {
                throw new MessageException("请选择项目!");
            }
            var result = Db.Ado.UseTran(() =>
            {
                //查询是否存在相同项目
                var name = Db.Queryable<h_item>().WithCache().Any(w => w.org_id == userInfo.org_id && w.dir_id == entity.dir_id && w.trade_name == entity.trade_name && w.type_id == entity.type_id && w.item_id != entity.item_id);
            if (name)
            {
                throw new MessageException("已存在此项目!");
            }
            var ItemEntity = new h_item();
            ItemEntity = entity;
            ItemEntity.short_code = ToSpell.GetFirstPinyin(entity.trade_name);//简码

            //查询此项目信息
            var isEquiment = Db.Queryable<h_item>()
                                   .Where(ie => ie.item_id == entity.item_id && ie.org_id == userInfo.org_id && ie.dir_id == entity.dir_id)
                                   .WithCache()
                                   .First();

            //设备标志是否改变,改变则查看是否有排班
            if (isEquiment!=null&&isEquiment?.equipment!=entity.equipment)
            {
                if (isEquiment.equipment==2)
                {
                    var isEquimentSchedul = Db.Queryable<his_equipment_scheduling>()
                                                  .WithCache()
                                                  .Any(es => es.itemid == entity.item_id && es.org_id == userInfo.org_id);
                    if (isEquimentSchedul)
                    {
                        throw new MessageException("此设备项目已有排班，不能改成非设备标志!");
                    }
                }
            }     
           
                //修改项目
                Db.Updateable(ItemEntity).IgnoreColumns(it => new { it.item_id, it.dir_id, it.type_id, it.org_id, it.code, it.state_id, it.create_date, it.creater, it.creater_id })
                    .Where(a => a.item_id == ItemEntity.item_id)
                    .RemoveDataCache()
                    .EnableDiffLogEvent()
                    .ExecuteCommand();

                //删除对应设备或治疗室项目
                if (ItemEntity.equipment == 2)//设备
                {
                    //删除设备对应项目
                    Db.Deleteable<p_equipment_itemspec>().Where(w => w.itemid == entity.item_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                }
                else
                {
                    //删除医疗室对应项目
                    Db.Deleteable<p_room_itemspec>().Where(w => w.itemid == entity.item_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                }
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 根据费别获取类别列表
        /// </summary>
        /// <param name="valueId"></param>
        /// <returns></returns>
        public async Task<object> GetFeeList(int valueId)
        {
            //挂号
            if (valueId == 1)
            {
                return await Db.Queryable<b_basecode>().Where(w => w.baseid == 1 && w.stateid == 1).GroupBy(w => new { w.baseid, w.valueid, w.value }).Select(s => new b_basecode { baseid = s.baseid, valueid = s.valueid, value = s.value }).OrderBy(s => s.baseid).WithCache().ToListAsync();
            }
            else if (valueId == 2)
            {
                return await Db.Queryable<b_basecode>().Where(w => w.baseid == 105 && w.stateid == 1).GroupBy(w => new { w.baseid, w.valueid, w.value }).Select(s => new b_basecode { baseid = s.baseid, valueid = s.valueid, value = s.value }).OrderBy(s => s.baseid).WithCache().ToListAsync();
            }
            //手术
            else if (valueId == 3)
            {
                return await Db.Queryable<b_basecode>().Where(w => w.baseid == 22 && w.stateid == 1).GroupBy(w => new { w.baseid, w.valueid, w.value }).Select(s => new b_basecode { baseid = s.baseid, valueid = s.valueid, value = s.value }).OrderBy(s => s.baseid).WithCache().ToListAsync();
            }
            //检查
            else if (valueId == 4)
            {
                return await Db.Queryable<b_basecode>().Where(w => w.baseid == 16 && w.stateid == 1).GroupBy(w => new { w.baseid, w.valueid, w.value }).Select(s => new b_basecode { baseid = s.baseid, valueid = s.valueid, value = s.value }).OrderBy(s => s.baseid).WithCache().ToListAsync();
            }
            //护理
            else if (valueId == 6)
            {
                return await Db.Queryable<b_basecode>().Where(w => w.baseid == 20 && w.stateid == 1).GroupBy(w => new { w.baseid, w.valueid, w.value }).Select(s => new b_basecode { baseid = s.baseid, valueid = s.valueid, value = s.value }).OrderBy(s => s.baseid).WithCache().ToListAsync();
            }
            //膳食
            else if (valueId == 8)
            {
                return await Db.Queryable<b_basecode>().Where(w => w.baseid == 4 && w.stateid == 1).GroupBy(w => new { w.baseid, w.valueid, w.value }).Select(s => new b_basecode { baseid = s.baseid, valueid = s.valueid, value = s.value }).OrderBy(s => s.baseid).WithCache().ToListAsync();
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 获取项目规格List
        /// </summary>
        /// <param name="feeType"></param>
        /// <param name="name">项目名称</param>
        /// <returns></returns>
        public async Task<List<ItemSpecModel>> GetItemSpecAsync(int feeType, string name)
        {
            //查询项目
            var itemList = await Db.Queryable<h_item>()
                .Where(s => s.org_id == userInfo.org_id && s.state_id == 1 && s.fee_id == feeType)
                .WhereIF(!string.IsNullOrEmpty(name), w => w.trade_name.Contains(name))
                .GroupBy(s => new { s.fee_id, s.fee_name, s.trade_name, s.item_id, s.type_id, s.equipment }).Select(s => new ItemSpecModel { fee_id = s.fee_id, fee_name = s.fee_name, trade_name = s.trade_name, item_id = s.item_id, type_id = s.type_id, equipment = s.equipment }).WithCache().ToListAsync();

            //查询规格
            var specList = await Db.Queryable<h_item, h_itemspec>((i, its) => new object[] { JoinType.Left, i.item_id == its.itemid }).Select((i, its) => new { its.itemid, its.expiredate, its.feegrade, its.hint, its.mindosage, its.minunit, its.sale_price, its.salseunit, its.specid, its.specname }).WithCache().ToListAsync();

            var newList = itemList.Select(s => new ItemSpecModel { fee_id = s.fee_id, fee_name = s.fee_name, trade_name = s.trade_name, item_id = s.item_id, type_id = s.type_id, specList = specList.Where(its => its.itemid == s.item_id).Select(its => new h_itemspec { itemid = its.itemid, expiredate = its.expiredate, feegrade = its.feegrade, hint = its.hint, mindosage = its.mindosage, minunit = its.minunit, sale_price = its.sale_price, salseunit = its.salseunit, specid = its.specid, specname = its.specname }).ToList(), equipment = s.equipment }).ToList();
            itemList = newList;
            return itemList;

        }

        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(int id)
        {
            //规格如被使用不能删除
            if (id <= 0)
            {
                throw new MessageException("请选择项目！");
            }
            var result = Db.Ado.UseTran(() =>
            {
                //查询该项目下的项目规格
                var specIdList = Db.Queryable<h_item, h_itemspec>((item, spec) => new object[] { JoinType.Left, spec.itemid == item.item_id })
                                   .Where((item, spec) => item.item_id == id && item.org_id == userInfo.org_id)
                                   .Select((item, spec) => spec.specid)
                                   .WithCache()
                                   .ToList();
            if (specIdList.Count > 0)
            {
                //查询结算中是否有此目录下的规格
                var IsSpec = Db.Queryable<f_balancedetail>()
                                 .WithCache()
                                 .Any(s => specIdList.Contains(s.specid));

                if (IsSpec)
                {
                    throw new MessageException("此项目使用中，不能删除！");
                }
            }            
           
                //删除项目规格
                Db.Deleteable<h_itemspec>().Where(w => w.itemid == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                //删除项目
                Db.Deleteable<h_item>().Where(w => w.item_id == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 项目分页查询
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<h_item>> GetItemPageAsync(ItemPage entity)
        {
            var list = Db.Queryable<h_item>()
               .Where(w => w.org_id == userInfo.org_id && w.fee_id == entity.feeId)
               .WhereIF(!string.IsNullOrEmpty(entity.name), w => w.trade_name.Contains(entity.name) || w.common_name.Contains(entity.name));

            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            list = list.OrderBy(entity.order + orderTypeStr);
            return await list.WithCache().ToPageAsync(entity.page, entity.limit);

        }

        /// <summary>
        /// 获取设备项目
        /// </summary>
        /// <returns></returns>
        public async Task<Page<EquipmentItem>> GetEquipmentList(ItemPage entity)
        {
            var list = Db.Queryable<h_item>()
                           .Where(w => w.org_id == userInfo.org_id && w.state_id == 1 && w.equipment == entity.equipment)
                           .WhereIF(entity.feeId > 0, w => w.fee_id != 1 && w.fee_id != 3 && w.fee_id != 4)
                           .WhereIF(!string.IsNullOrEmpty(entity.name), w => w.trade_name.Contains(entity.name) || w.common_name.Contains(entity.name))
                           .Select(w => new EquipmentItem { item_id = w.item_id, item_name = w.trade_name, common_name = w.common_name });

            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            list = list.OrderBy(entity.order + orderTypeStr);
            if (entity.page == 0)
            {
                entity.page = 1;
                entity.limit = System.Int32.MaxValue;
            }
            return await list.WithCache().ToPageAsync(entity.page, entity.limit);
            
           
        }
    }
}
