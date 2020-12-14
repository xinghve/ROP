using Service.DtoModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository
{
    /// <summary>
    /// 定义基本服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseServer<T> where T : class, new()
    {
        #region 添加操作
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="parm">cms_advlist</param>
        /// <returns></returns>
        Task<int> AddAsync(T parm, bool Async = true);

        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <param name="parm">List<T></param>
        /// <returns></returns>
        Task<int> AddListAsync(List<T> parm, bool Async = true);

        #endregion

        #region 查询操作
        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="where">Expression<Func<T, bool>></param>
        /// <param name="order">Expression<Func<T, object>></param>
        /// <param name="orderType">DbOrderEnum</param>
        /// <returns></returns>
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> where,
            Expression<Func<T, object>> order, int orderType, bool Async = true);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <returns></returns>
        Task<List<T>> GetListAsync(bool Async = true);

        /// <summary>
		/// 获得列表——分页
		/// </summary>
		/// <param name="parm">PageParm</param>
		/// <returns></returns>
		Task<Page<T>> GetPagesAsync(PageParm parm, bool Async = true);

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="parm">分页参数</param>
        /// <param name="order">排序值</param>
        /// <param name="orderType">排序方式OrderByType</param>
        /// <returns></returns>
        Task<Page<T>> GetPagesAsync(PageParm parm,
            string order, int orderType, bool Async = true, bool isWhere1 = false, Expression<Func<T, bool>> where1 = null, bool isWhere2 = false, Expression<Func<T, bool>> where2 = null, bool isWhere3 = false, Expression<Func<T, bool>> where3 = null, bool isWhere4 = false, Expression<Func<T, bool>> where4 = null, bool isWhere5 = false, Expression<Func<T, bool>> where5 = null, bool isWhere6 = false, Expression<Func<T, bool>> where6 = null, bool isWhere7 = false, Expression<Func<T, bool>> where7 = null, bool isWhere8 = false, Expression<Func<T, bool>> where8 = null, bool isWhere9 = false, Expression<Func<T, bool>> where9 = null);
        
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="parm">分页参数</param>
        /// <param name="where">条件</param>
        /// <param name="order">排序值</param>
        /// <param name="orderType">排序方式OrderByType</param>
        /// <returns></returns>
        Task<Page<T>> GetPagesAsync(PageParm parm, Expression<Func<T, bool>> where,
            Expression<Func<T, object>> order, int orderType, bool Async = true);

        /// <summary>
        /// 获得一条数据
        /// </summary>
        /// <param name="parm">string</param>
        /// <returns></returns>
        Task<T> GetModelAsync(string parm, bool Async = true);

        /// <summary>
        /// 获得一条数据
        /// </summary>
        /// <param name="where">Expression<Func<T, bool>></param>
        /// <returns></returns>
        Task<T> GetModelAsync(Expression<Func<T, bool>> where, bool Async = true);
        #endregion

        #region 修改操作
        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        Task<int> UpdateAsync(T parm, bool Async = true);

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        Task<int> UpdateAsync(List<T> parm, bool Async = true);

        /// <summary>
        /// 修改一条数据，可用作假删除
        /// </summary>
        /// <param name="columns">修改的列=Expression<Func<T,T>></param>
        /// <param name="where">Expression<Func<T,bool>></param>
        /// <returns></returns>
        Task<int> UpdateAsync(Expression<Func<T, T>> columns,
            Expression<Func<T, bool>> where, bool Async = true);
        #endregion

        #region 删除操作
        /// <summary>
        /// 删除一条或多条数据
        /// </summary>
        /// <param name="parm">string</param>
        /// <returns></returns>
        Task<int> DeleteAsync(string parm, bool Async = true);

        /// <summary>
        /// 删除一条或多条数据
        /// </summary>
        /// <param name="where">Expression<Func<T, bool>></param>
        /// <returns></returns>
        Task<int> DeleteAsync(Expression<Func<T, bool>> where, bool Async = true);

        #endregion

    }
}
