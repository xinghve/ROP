using Models.DB;
using Models.View.Business;
using Newtonsoft.Json;
using Service.Extensions;
using Service.Repository.Interfaces.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;

namespace Service.Repository.Implements.Business
{
    /// <summary>
    /// 物资分类
    /// </summary>
    public class GoodsTypeService : DbContext, IGoodsTypeService
    {
        /// <summary>
        /// 添加物资分类
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> AddAsync(b_codebase entity)
        {
            //判断是否存在
            var isExisteName = await Db.Queryable<b_codebase>().WithCache().AnyAsync(a => a.category_id == 16 && (a.text == entity.text || a.code == entity.code));
            if (isExisteName)
            {
                throw new MessageException("已存在此物资分类（名称或code）");
            }
            entity.category_id = 16;
            entity.pinyin = ToSpell.GetFirstPinyin(entity.text);

            //查询父目录
            var catalog = await Db.Queryable<b_catalog>().Where(w => w.id == entity.catalog_id).WithCache().FirstAsync();
            entity.property_id = catalog.property_id;
            entity.property = catalog.property;

            redisCache.RemoveAll<b_codebase>();
            return await Db.Insertable(entity).ExecuteReturnIdentityAsync();
        }

        /// <summary>
        /// 添加物资目录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> AddCatalogAsync(b_catalog entity)
        {
            //判断是否存在
            var isExisteName = await Db.Queryable<b_catalog>().WithCache().AnyAsync(a => a.category_id == 16 && a.text == entity.text);
            if (isExisteName)
            {
                throw new MessageException("已存在此物资目录");
            }
            entity.category_id = 16;
            entity.pinyin = ToSpell.GetFirstPinyin(entity.text);
            redisCache.RemoveAll<b_catalog>();
            return await Db.Insertable(entity).ExecuteReturnIdentityAsync();
        }

        /// <summary>
        /// 删除物资分类
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(b_codebase entity)
        {
            return await Db.Deleteable<b_codebase>().Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除物资目录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> DeleteCatalogAsync(b_catalog entity)
        {
            return await Db.Deleteable<b_catalog>().Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取目录列表
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetCatalogListAsync()
        {
            var catalog_list = await Db.Queryable<b_catalog>().WithCache().ToListAsync();
            var catalog_ids = catalog_list.Select(s => s.id).ToList();
            var type_list = await Db.Queryable<b_codebase>().Where(w => catalog_ids.Contains(w.catalog_id)).ToListAsync();

            var str = "[";

            catalog_list.ForEach(item =>
            {
                str += "{";
                str += $"'label':'{item.text}',";
                str += $"'value':{item.id},";
                str += $"'children':[";

                var types = type_list.Where(w => w.catalog_id == item.id).ToList();

                types.ForEach(type =>
                {
                    str += "{";
                    str += $"'label':'{type.text}',";
                    str += $"'value':{type.id}";
                    str += "},";
                });
                str = str.TrimEnd(',');

                str += $"]";
                str += "},";
            });

            str = str.TrimEnd(',');

            str += "]";
            return JsonConvert.DeserializeObject(str);
        }

        /// <summary>
        /// 物资目录分页查询
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<b_catalog>> GetCatalogPagesAsync(CatalogPageSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            return await Db.Queryable<b_catalog>()
                            .Where(w => w.category_id == 16)
                            .WhereIF(entity.property_id > 0, w => w.property_id == entity.property_id)
                            .WhereIF(!string.IsNullOrEmpty(entity.text), w => w.text.Contains(entity.text))
                            .OrderBy(entity.order + orderTypeStr)
                            .WithCache()
                            .ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<b_codebase>> GetListAsync(int catalog_id)
        {
            return await Db.Queryable<b_codebase>().Where(w => w.category_id == 16 && w.catalog_id == catalog_id).WithCache().ToListAsync();
        }

        /// <summary>
        /// 物资分类分页查询
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<b_codebase>> GetPagesAsync(GoodsTypePageSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            return await Db.Queryable<b_codebase>()
                            .Where(w => w.category_id == 16)
                            .WhereIF(entity.catalog_id > 0, w => w.catalog_id == entity.catalog_id)
                            .WhereIF(!string.IsNullOrEmpty(entity.text), w => w.text.Contains(entity.text))
                            .OrderBy(entity.order + orderTypeStr)
                            .WithCache()
                            .ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 编辑物资分类
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> ModifyAsync(b_codebase entity)
        {
            //判断是否存在
            var isExisteName = await Db.Queryable<b_codebase>().WithCache().AnyAsync(a => a.category_id == 16 && (a.text == entity.text || a.code == entity.code) && a.id != entity.id);
            if (isExisteName)
            {
                throw new MessageException("已存在此物资分类（名称或code）");
            }
            entity.pinyin = ToSpell.GetFirstPinyin(entity.text);

            return await Db.Updateable<b_codebase>().SetColumns(s => new b_codebase { code = entity.code, introduce = entity.introduce, text = entity.text, pinyin = entity.pinyin }).Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 编辑物资目录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> ModifyCatalogAsync(b_catalog entity)
        {
            //判断是否存在
            var isExisteName = await Db.Queryable<b_catalog>().WithCache().AnyAsync(a => a.category_id == 16 && a.text == entity.text && a.id != entity.id);
            if (isExisteName)
            {
                throw new MessageException("已存在此物资目录");
            }
            entity.pinyin = ToSpell.GetFirstPinyin(entity.text);

            //修改分类属性
            await Db.Updateable<b_codebase>().SetColumns(s => new b_codebase { property_id = entity.property_id, property = entity.property }).Where(w => w.catalog_id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();

            return await Db.Updateable<b_catalog>().SetColumns(s => new b_catalog { introduce = entity.introduce, text = entity.text, pinyin = entity.pinyin, property_id = entity.property_id, property = entity.property }).Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }
    }
}
