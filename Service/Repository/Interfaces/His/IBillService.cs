using Models.DB;
using Models.View.His;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.His
{
    /// <summary>
    /// 诊疗单据接口
    /// </summary>
    public interface IBillService
    {
        /// <summary>
        /// 获取诊疗单据分页数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Page<h_bill>> GetPageAsync(BillSearch model);

        /// <summary>
        /// 添加诊疗单据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> AddAsync(h_bill entity);

        /// <summary>
        /// 编辑诊疗单据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> ModifyAsync(h_bill entity);

        /// <summary>
        /// 删除诊疗单据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(int id);
    }
}
