using Service.DtoModel;
using Service.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseServer<T> : DbContext, IBaseServer<T> where T : class, new()
    {
        #region 添加操作
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        public async Task<int> AddAsync(T parm, bool Async = true)
        {
            return Async ? await Db.Insertable<T>(parm).RemoveDataCache().ExecuteCommandAsync() : Db.Insertable<T>(parm).RemoveDataCache().ExecuteCommand();
        }

        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <param name="parm">List<T></param>
        /// <returns></returns>
        public async Task<int> AddListAsync(List<T> parm, bool Async = true)
        {
            redisCache.RemoveAll<T>();
            return Async ? await Db.Insertable<T>(parm).ExecuteCommandAsync() : Db.Insertable<T>(parm).ExecuteCommand();
        }
        #endregion

        #region 查询操作
        /// <summary>
        /// 获得一条数据
        /// </summary>
        /// <param name="where">Expression<Func<T, bool>></param>
        /// <returns></returns>
        public async Task<T> GetModelAsync(Expression<Func<T, bool>> where, bool Async = true)
        {
            return Async ? await Db.Queryable<T>().Where(where).WithCache().FirstAsync() ?? new T() { }
            : Db.Queryable<T>().Where(where).WithCache().First() ?? new T() { };
        }

        /// <summary>
        /// 获得一条数据
        /// </summary>
        /// <param name="parm">string</param>
        /// <returns></returns>
        public async Task<T> GetModelAsync(string parm, bool Async = true)
        {
            return Async ? await Db.Queryable<T>().Where(parm).WithCache().FirstAsync() ?? new T() { }
            : Db.Queryable<T>().Where(parm).WithCache().First() ?? new T() { };
        }

        /// <summary>
		/// 获得列表——分页
		/// </summary>
		/// <param name="parm">PageParm</param>
		/// <returns></returns>
        public async Task<Page<T>> GetPagesAsync(PageParm parm, bool Async = true)
        {
            return Async ? await Db.Queryable<T>()
                    .WithCache().ToPageAsync(parm.page, parm.limit) : Db.Queryable<T>()
                    .WithCache().ToPage(parm.page, parm.limit);
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="parm">分页参数</param>
        /// <param name="order">排序值</param>
        /// <param name="orderType">排序方式OrderByType</param>
        /// <returns></returns>
        public async Task<Page<T>> GetPagesAsync(PageParm parm,
            string order, int orderType, bool Async = true, bool isWhere1 = false, Expression<Func<T, bool>> where1 = null, bool isWhere2 = false, Expression<Func<T, bool>> where2 = null, bool isWhere3 = false, Expression<Func<T, bool>> where3 = null, bool isWhere4 = false, Expression<Func<T, bool>> where4 = null, bool isWhere5 = false, Expression<Func<T, bool>> where5 = null, bool isWhere6 = false, Expression<Func<T, bool>> where6 = null, bool isWhere7 = false, Expression<Func<T, bool>> where7 = null, bool isWhere8 = false, Expression<Func<T, bool>> where8 = null, bool isWhere9 = false, Expression<Func<T, bool>> where9 = null)
        {
            var query = Db.Queryable<T>()
                    .WhereIF(isWhere1, where1)
                    .WhereIF(isWhere2, where2)
                    .WhereIF(isWhere3, where3)
                    .WhereIF(isWhere4, where4)
                    .WhereIF(isWhere5, where5)
                    .WhereIF(isWhere6, where6)
                    .WhereIF(isWhere7, where7)
                    .WhereIF(isWhere8, where8)
                    .WhereIF(isWhere9, where9)
                    .OrderByIF(orderType == 0, order + " asc ")
                    .OrderByIF(orderType == 1, order + " desc ")
                    .WithCache();
            return Async ? await query.ToPageAsync(parm.page, parm.limit) : query.ToPage(parm.page, parm.limit);
        }

        public class WT
        {
            public bool isWhere { get; set; }

            public Expression<Func<T, bool>> where { get; set; }
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="parm">分页参数</param>
        /// <param name="where">条件</param>
        /// <param name="order">排序值</param>
        /// <param name="orderType">排序方式OrderByType</param>
        /// <returns></returns>
        public async Task<Page<T>> GetPagesAsync(PageParm parm, Expression<Func<T, bool>> where,
            Expression<Func<T, object>> order, int orderType, bool Async = true)
        {
            var query = Db.Queryable<T>()
                    .Where(where)
                    .OrderByIF(orderType == 0, order, SqlSugar.OrderByType.Asc)
                    .OrderByIF(orderType == 1, order, SqlSugar.OrderByType.Desc)
                    .WithCache();
            return Async ? await query.ToPageAsync(parm.page, parm.limit) : query.ToPage(parm.page, parm.limit);
        }

        /// <summary>
		/// 获得列表
		/// </summary>
		/// <returns></returns>
        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> where,
            Expression<Func<T, object>> order, int orderType, bool Async = true)
        {
            var query = Db.Queryable<T>()
                    .Where(where)
                    .OrderByIF(orderType == 0, order, SqlSugar.OrderByType.Asc)
                    .OrderByIF(orderType == 1, order, SqlSugar.OrderByType.Desc)
                    .WithCache();
            return Async ? await query.ToListAsync() : query.ToList();
        }

        /// <summary>
        /// 获得列表，不需要任何条件
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> GetListAsync(bool Async = true)
        {
            return Async ? await Db.Queryable<T>().WithCache().ToListAsync() : Db.Queryable<T>().WithCache().ToList();
        }
        #endregion

        #region 修改操作
        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(T parm, bool Async = true)
        {
            return Async ? await Db.Updateable<T>(parm).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync() : Db.Updateable<T>(parm).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(List<T> parm, bool Async = true)
        {
            redisCache.RemoveAll<T>();
            return Async ? await Db.Updateable<T>(parm).ExecuteCommandAsync() : Db.Updateable<T>(parm).ExecuteCommand();
        }

        /// <summary>
        /// 修改一条数据，可用作假删除
        /// </summary>
        /// <param name="columns">修改的列=Expression<Func<T,T>></param>
        /// <param name="where">Expression<Func<T,bool>></param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(Expression<Func<T, T>> columns,
            Expression<Func<T, bool>> where, bool Async = true)
        {
            return Async ? await Db.Updateable<T>().SetColumns(columns).Where(where).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync()
                : Db.Updateable<T>().SetColumns(columns).Where(where).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
        }
        #endregion

        #region 删除操作
        /// <summary>
        /// 删除一条或多条数据
        /// </summary>
        /// <param name="parm">string</param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(string parm, bool Async = true)
        {
            var list = Utils.StrToListString(parm);
            return Async ? await Db.Deleteable<T>().In(list.ToArray()).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync() : Db.Deleteable<T>().In(list.ToArray()).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
        }

        /// <summary>
        /// 删除一条或多条数据
        /// </summary>
        /// <param name="where">Expression<Func<T, bool>></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(Expression<Func<T, bool>> where, bool Async = true)
        {
            return Async ? await Db.Deleteable<T>().Where(where).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync() : Db.Deleteable<T>().Where(where).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
        }
        #endregion
    }
}
