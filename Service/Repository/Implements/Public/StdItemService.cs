using Models.DB;
using Models.View.Public;
using Service.Extensions;
using Service.Repository.Interfaces.Public;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;

namespace Service.Repository.Implements.Public
{
    /// <summary>
    /// 基础物资
    /// </summary>
    public class StdItemService : DbContext, IStdItemService
    {
        /// <summary>
        /// 添加基础物资
        /// </summary>
        /// <param name="entity">基础物资实体</param>
        /// <returns></returns>
        public async Task<bool> AddAsync(StdItem entity)
        {
            //查询相同规格数量
            var only_list = new List<string>();
            entity.std_Item_Detials.ForEach(item =>
            {
                if (only_list.Contains(item.spec))
                {
                    throw new MessageException($"存在相同规格“{item.spec}”");
                }
                only_list.Add(item.spec);
            });

            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            //判断是否存在
            var isExisteName = await Db.Queryable<p_std_item>().WithCache().AnyAsync(a => a.org_id == userInfo.org_id && a.name == entity.name && a.type_id == entity.type_id);
            if (isExisteName)
            {
                throw new MessageException("已存在此物资基础信息");
            }
            var result = Db.Ado.UseTran(() =>
            {
                //新增
                var id = Db.Insertable(new p_std_item { code = entity.code, name = entity.name, org_id = userInfo.org_id, spell = ToSpell.GetFirstPinyin(entity.name), state = 1, type = entity.type, type_id = entity.type_id, min_num = entity.min_num, unit = entity.unit }).ExecuteReturnIdentity();
                redisCache.RemoveAll<p_std_item>();

                //添加明细
                var list = entity.std_Item_Detials.Where(w => !string.IsNullOrEmpty(w.spec)).Select(s => new p_std_item_detials { id = id, manufactor = s.manufactor, manufactor_id = s.manufactor_id, approval_no = s.approval_no, buy_multiple = s.buy_multiple, buy_price = s.buy_price, buy_unit = s.buy_unit, price = s.price, spec = s.spec }).ToList();
                Db.Insertable(list).ExecuteCommand();
                redisCache.RemoveAll<p_std_item_detials>();
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        public async Task<int> DeleteAsync(List<int> vs)
        {
            return await Db.Updateable<p_std_item>()
                 .SetColumns(s => new p_std_item { state = -1 })
                 .Where(w => vs.Contains(w.id))
                 .RemoveDataCache()
                 .EnableDiffLogEvent()
                 .ExecuteCommandAsync();
        }

        /// <summary>
        /// 根据基础物资ID获取基础物资信息
        /// </summary>
        /// <param name="id">基础物资ID</param>
        /// <returns></returns>
        public async Task<StdItem> GetAsync(int id)
        {
            var item = await Db.Queryable<p_std_item>().Where(w => w.id == id).WithCache().FirstAsync();
            var detials = await Db.Queryable<p_std_item_detials>().Where(w => w.id == id).WithCache().ToListAsync();
            return new StdItem { code = item.code, id = item.id, name = item.name, org_id = item.org_id, spell = item.spell, state = item.state, type = item.type, type_id = item.type_id, min_num = item.min_num, unit = item.unit, std_Item_Detials = detials };
        }

        /// <summary>
        /// 获取物资明细列表
        /// </summary>
        public async Task<List<p_std_item_detials>> GetDetialListAsync(int id)
        {
            return await Db.Queryable<p_std_item_detials>().Where(w => w.id == id).WithCache().ToListAsync();
        }

        /// <summary>
        /// 获取基础物资列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<StdItem>> GetListAsync(string name, short type_id)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var items = await Db.Queryable<p_std_item>()
                .Where(w => w.org_id == userInfo.org_id && w.state == 1)
                .WhereIF(type_id > -1, w => w.type_id == type_id)
                .WhereIF(!string.IsNullOrEmpty(name), w => w.name.Contains(name) || w.spell.Contains(name.ToUpper()))
                .WithCache()
                .ToListAsync();
            var ids = items.Select(s => s.id).ToList();
            var detials = await Db.Queryable<p_std_item_detials>().Where(w => ids.Contains(w.id)).WithCache().ToListAsync();
            var list = items.Select(s => new StdItem { code = s.code, id = s.id, name = s.name, org_id = s.org_id, spell = s.spell, state = s.state, type = s.type, type_id = s.type_id, min_num = s.min_num, unit = s.unit, std_Item_Detials = detials.Where(w => w.id == s.id).ToList() }).ToList();
            return list;
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<StdItem>> GetPageAsync(StdItemPageSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            var list = await Db.Queryable<p_std_item, p_std_item_detials, b_codebase>((si, sit, c) => new object[] { JoinType.Left, si.id == sit.id, JoinType.Left, si.type_id == c.id })
                .Where((si, sit, c) => si.org_id == userInfo.org_id)
                .WhereIF(entity.state == -1, (si, sit, c) => si.state >= 0)
                .WhereIF(entity.state > -1, (si, sit, c) => si.state == entity.state)
                .WhereIF(entity.catalog_type == 0 && entity.type_id > -1, (si, sit, c) => si.type_id == entity.type_id)
                .WhereIF(entity.catalog_type == 1, (si, sit, c) => c.catalog_id == entity.type_id)
                .WhereIF(entity.catalog_type == 2, (si, sit, c) => si.type_id == entity.type_id)
                .WhereIF(entity.manufactor_id > -1, (si, sit, c) => sit.manufactor_id == entity.manufactor_id)
                .WhereIF(!string.IsNullOrEmpty(entity.name), (si, sit, c) => si.name.Contains(entity.name))
                .GroupBy((si, sit, c) => new { si.code, si.id, si.name, si.org_id, si.spell, si.state, si.type, si.type_id, si.min_num, si.unit })
                .Select((si, sit, c) => new StdItem { code = si.code, id = si.id, name = si.name, org_id = si.org_id, spell = si.spell, state = si.state, type = si.type, type_id = si.type_id, min_num = si.min_num, unit = si.unit })
                .OrderBy(entity.order + orderTypeStr)
                .WithCache()
                .ToPageAsync(entity.page, entity.limit);
            var ids = list.Items.Select(s => s.id).ToList();
            var detials = await Db.Queryable<p_std_item_detials>().Where(w => ids.Contains(w.id)).WithCache().ToListAsync();
            list.Items = list.Items.Select(s => new StdItem { code = s.code, id = s.id, name = s.name, org_id = s.org_id, spell = s.spell, state = s.state, type = s.type, type_id = s.type_id, min_num = s.min_num, unit = s.unit, std_Item_Detials = detials.Where(w => w.id == s.id).ToList() }).ToList();
            return list;
        }

        /// <summary>
        /// 获取物资明细（规格对应明细）
        /// </summary>
        /// <param name="id">物资基础ID</param>
        /// <returns></returns>
        public async Task<List<StdItemDetials>> GetSpecDetialsAsync(int id)
        {
            var list = await Db.Queryable<p_std_item_detials>().Where(w => w.id == id).WithCache().ToListAsync();
            var list_spec = list.Select(s => s.spec).Distinct().ToList();
            return list_spec.Select(s => new StdItemDetials { spec = s, std_Item_Detials = list.Where(w => w.spec == s).ToList() }).ToList();
        }

        /// <summary>
        /// 编辑基础物资
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ModifyAsync(StdItem entity)
        {
            //查询相同规格数量
            var only_list = new List<string>();
            entity.std_Item_Detials.ForEach(item =>
            {
                if (only_list.Contains(item.spec))
                {
                    throw new MessageException($"存在相同规格“{item.spec}”");
                }
                only_list.Add(item.spec);
            });
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //判断是否存在
            var isExisteName = await Db.Queryable<p_std_item>().WithCache().AnyAsync(a => a.org_id == userInfo.org_id && a.name == entity.name && a.type_id == entity.type_id && a.id != entity.id);
            if (isExisteName)
            {
                throw new MessageException("已存在此物资基础信息");
            }
            entity.spell = ToSpell.GetFirstPinyin(entity.name);

            var result = Db.Ado.UseTran(() =>
            {
                //修改基础项目
                Db.Updateable<p_std_item>().SetColumns(it => new p_std_item { code = entity.code, name = entity.name, spell = entity.spell, type = entity.type, type_id = entity.type_id, min_num = entity.min_num, unit = entity.unit }).Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                redisCache.RemoveAll<p_std_item>();
                //删除明细
                Db.Deleteable<p_std_item_detials>().Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                if (entity.std_Item_Detials.Count > 0)
                {
                    //添加明细
                    var list = entity.std_Item_Detials.Where(w => !string.IsNullOrEmpty(w.spec)).Select(s => new p_std_item_detials { id = entity.id, manufactor = s.manufactor, manufactor_id = s.manufactor_id, approval_no = s.approval_no, buy_multiple = s.buy_multiple, buy_price = s.buy_price, buy_unit = s.buy_unit, price = s.price, spec = s.spec }).ToList();
                    Db.Insertable(list).ExecuteCommand();
                    redisCache.RemoveAll<p_std_item_detials>();
                }
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <returns></returns>
        public async Task<int> ModifyStatusAsync(p_std_item entity)
        {
            return await Db.Updateable<p_std_item>().SetColumns(it => new p_std_item { state = entity.state }).Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 根据分类获取物资
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<p_std_item>> GetStdByClass(string name, int id)
        {
            //获取当前用户信息
            var userinfo = new Tools.IdentityModels.GetUser().userInfo;

            return await Db.Queryable<p_std_item, b_codebase>((si, cb) => new object[] { JoinType.Left, si.type_id == cb.id })
                .Where((si, cb) => cb.property_id == id && si.state == 1 && si.org_id == userinfo.org_id)
                .WhereIF(!string.IsNullOrEmpty(name), (si, cb) => si.name.Contains(name) || si.spell.Contains(name.ToUpper()))
                .WithCache()
                .Select((si, cb) => new p_std_item { id = si.id, name = si.name, unit = si.unit, type = si.type, type_id = si.type_id })
                .ToListAsync();

        }

        /// <summary>
        /// 获取规格明细加可用数量
        /// </summary>
        /// <param name="id">基础id</param>
        /// <param name="store_id"></param>
        /// <returns></returns>
        public async Task<List<StdItemDetials>> GetSpecDetialsNumAsync(int id, int store_id)
        {
            //查询是否固资
            var isAsset = await Db.Queryable<p_std_item, b_codebase>((si, cb) => new object[] { JoinType.Left, si.type_id == cb.id })
                .Where((si, cb) => si.id == id && si.state == 1)
                .Select((si, cb) => new { cb.property_id, cb.property, si.id })
                .WithCache()
                .FirstAsync();

            if (isAsset == null || isAsset?.id <= 0)
            {
                throw new MessageException("此物资未分配属性！");
            }

            //固资
            if (isAsset.property_id == 1)
            {
                var list = await Db.Queryable<bus_assets, p_std_item_detials>((a, st) => new object[] { JoinType.Left, (st.id == a.std_item_id && st.spec == a.spec && st.manufactor_id == a.manufactor_id) })
                  .Where((a, st) => a.std_item_id == id && a.store_id == store_id && a.state == 30)
                  .GroupBy((a, st) => new { st.spec, a.std_item_id })
                  .Select((a, st) => new { num = SqlFunc.AggregateCount(a.id), st.spec })
                  .WithCache()
                  .ToListAsync();

                return list.Select(s => new StdItemDetials { spec = s.spec, num = s.num }).ToList();

            }
            else
            {
                var list = await Db.Queryable<p_std_item_detials, bus_storage, bus_storage_detials>((d, si, a) => new object[] { JoinType.Left, si.std_item_id == d.id, JoinType.Left, (d.id == a.std_item_id && d.spec == a.spec) })
                    .Where((d, si, a) => d.id == id && si.store_id == store_id && si.id == a.id)
                    .GroupBy((d, si, a) => new { d.spec, d.id })
                    .Select((d, si, a) => new { num = SqlFunc.AggregateSum(a.use_num), d.spec })
                    .WithCache()
                    .ToListAsync();

                return list.Select(s => new StdItemDetials { spec = s.spec, num = s.num }).ToList();
            }
        }
    }
}
