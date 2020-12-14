using Models.DB;
using Models.View.Business;
using Service.Extensions;
using Service.Repository.Interfaces.Business;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Implements.Business
{
    /// <summary>
    /// 库存
    /// </summary>
    public class StorageService : DbContext, IStorageService
    {
        /// <summary>
        /// 获取库存明细（根据项目、规格、厂家）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<StorageDetials> GetDetialsAsync(StorageDetials entity)
        {
            entity.use_num = await Db.Queryable<bus_storage_detials, bus_storage>((sd, s) => new object[] { JoinType.Left, sd.id == s.id }).Where((sd, s) => sd.spec == entity.spec && sd.std_item_id == entity.std_item_id && sd.manufactor_id == entity.manufactor_id && s.store_id == entity.store_id).SumAsync(sd => sd.use_num);
            return entity;
        }

        /// <summary>
        /// 获取集团所有库存明细（根据项目、规格、厂家）
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<List<StorageDetials>> GetDetialsListAsync(List<StorageDetials> list)
        {
            var specs = list.Select(s => s.spec).ToList();
            var sdList = await Db.Queryable<bus_storage_detials, bus_storage>((sd, s) => new object[] { JoinType.Left, sd.id == s.id }).Where((sd, s) => specs.Contains(sd.spec) && s.store_id == 0).ToListAsync();
            var rtList = new List<StorageDetials>();

            list.ForEach(item =>
            {
                var use_num = sdList.Where(w => w.std_item_id == item.std_item_id && w.spec == item.spec && w.manufactor_id == item.manufactor_id).Sum(s => s.use_num);
                rtList.Add(new StorageDetials { std_item_id = item.std_item_id, manufactor_id = item.manufactor_id, spec = item.spec, use_num = use_num });
            });
            return rtList;
        }

        /// <summary>
        /// 获得分页列表（明细）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<bus_storage_detials>> GetDetialsPageAsync(StorageDetialsPageSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            return await Db.Queryable<bus_storage_detials>()
                            .Where(w => w.id == entity.id)
                            .WhereIF(entity.manufactor_id != -1, w => w.manufactor_id == entity.manufactor_id)
                            .OrderBy(entity.order + orderTypeStr)
                            .WithCache()
                            .ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<bus_storage>> GetPageAsync(StoragePageSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            return await Db.Queryable<bus_storage, b_codebase>((w, c) => new object[] { JoinType.Left, w.type_id == c.id })
                            .Where((w, c) => w.org_id == userInfo.org_id)
                            .WhereIF(entity.store_id != -1, (w, c) => w.store_id == entity.store_id)
                            .WhereIF(entity.catalog_type == 0 && entity.type_id != -1, (w, c) => w.type_id == entity.type_id)
                            .WhereIF(entity.catalog_type == 1, (w, c) => c.catalog_id == entity.type_id)
                            .WhereIF(entity.catalog_type == 2, (w, c) => w.type_id == entity.type_id)
                            .WhereIF(!string.IsNullOrEmpty(entity.name), (w, c) => w.name.Contains(entity.name))
                            .Select((w, c) => w)
                            .OrderBy(entity.order + orderTypeStr)
                            .WithCache()
                            .ToPageAsync(entity.page, entity.limit);
        }
    }
}
