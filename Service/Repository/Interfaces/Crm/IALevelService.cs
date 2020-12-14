using Models.DB;
using Models.View.Crm;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Crm
{
    /// <summary>
    /// 会员等级接口
    /// </summary>
    public interface IALevelService
    {
        /// <summary>
        /// 获取等级分页数据
        /// </summary>
        /// <returns></returns>
        Task<Page<c_archives_level>> GetPageAsync(SearchMl model);

        /// <summary>
        /// 获取等级列表
        /// </summary>
        /// <param name="store_id"></param>
        /// <returns></returns>
        Task<List<c_archives_level>> GetList( int store_id);

        /// <summary>
        /// 添加会员等级
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Add(c_archives_level entity);

        /// <summary>
        /// 编辑会员等级
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Modify(c_archives_level entity);

        /// <summary>
        /// 删除会员等级
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> Delete(int id);
    }
}
