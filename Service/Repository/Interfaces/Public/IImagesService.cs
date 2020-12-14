using Models.DB;
using Models.View.Public;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Repository.Interfaces.Public
{
    /// <summary>
    /// 图片管理
    /// </summary>
    public interface IImagesService
    {
        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="image"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<bool> SetAsync(Image image, dynamic type);

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="relation_code">关联编码</param>
        /// <returns></returns>
        Task<List<p_image>> GetAsync(string relation_code);
    }
}
