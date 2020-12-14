using Microsoft.AspNetCore.Http;
using Models.DB;
using Models.View.OA;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.OA
{
    /// <summary>
    /// 文件管理
    /// </summary>
    public interface IFilesService
    {
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="entity">文件夹路径</param>
        /// <param name="files"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<List<oa_files>> UploadFilesAsync(Files entity, IEnumerable<IFormFile> files, dynamic type);

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<bool> CreateFolder(Folder entity, dynamic type);

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<FilesPageModel>> GetPageAsync(FilesPageSearch entity);

        /// <summary>
        /// 删除选中
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(oa_files entity, dynamic type);

        /// <summary>
        /// 修改文件名称/文件夹名称
        /// </summary>
        /// <returns></returns>
        Task<bool> ModifyAsync(UpdateFilesOrFolderName entity, dynamic type);

        /// <summary>
        /// 获取树结构（部门、职位、人员）
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <returns></returns>
        Task<object> GetTreeAsync(int store_id);

        /// <summary>
        /// 设置文件权限
        /// </summary>
        /// <param name="filesAuthority"></param>
        /// <returns></returns>
        Task<bool> SetAuthorityAsync(FilesAuthority filesAuthority);

        /// <summary>
        /// 获取选中文件权限
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<object> GetAuthorityAsync(oa_files entity);
    }
}
