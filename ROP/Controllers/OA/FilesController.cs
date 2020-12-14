using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.OA;
using Service.Repository.Interfaces.OA;
using Tools;

namespace ROP.Controllers.OA
{
    /// <summary>
    /// 文件管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFilesService _filesService;
        /// <summary>
        /// 文件管理构造
        /// </summary>
        /// <param name="filesService"></param>
        public FilesController(IFilesService filesService)
        {
            _filesService = filesService;
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("UploadFiles")]
        public async Task<List<oa_files>> UploadFilesAsync([FromQuery]Files entity)
        {
            var files = Request.Form.Files;
            dynamic type = (new Program()).GetType();
            var result = await _filesService.UploadFilesAsync(entity, files, type);
            return result;
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("CreateFolder")]
        public async Task<bool> CreateFolder([FromBody]Folder entity)
        {
            dynamic type = (new Program()).GetType();
            return await _filesService.CreateFolder(entity, type);
        }

        /// <summary>
        /// 文件分页
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPage")]
        public async Task<Page<FilesPageModel>> GetPageAsync([FromQuery]FilesPageSearch entity)
        {
            return await _filesService.GetPageAsync(entity);
        }

        /// <summary>
        /// 删除文件或文件夹
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<int> Delete([FromBody]oa_files entity)
        {
            dynamic type = (new Program()).GetType();
            return await _filesService.DeleteAsync(entity, type);
        }

        /// <summary>
        /// 修改文件或文件夹
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<bool> Modify([FromBody]UpdateFilesOrFolderName entity)
        {
            dynamic type = (new Program()).GetType();
            return await _filesService.ModifyAsync(entity, type);
        }

        /// <summary>
        /// 获取树结构（部门、职位、人员）
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <returns></returns>
        [HttpGet("GetTree")]
        public async Task<object> GetTreeAsync([FromQuery]int store_id)
        {
            return await _filesService.GetTreeAsync(store_id);
        }

        /// <summary>
        /// 设置文件权限
        /// </summary>
        /// <param name="filesAuthority"></param>
        /// <returns></returns>
        [HttpPost("SetAuthority")]
        public async Task<bool> SetAuthorityAsync([FromBody]FilesAuthority filesAuthority)
        {
            return await _filesService.SetAuthorityAsync(filesAuthority);
        }

        /// <summary>
        /// 获取选中文件权限
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetAuthority")]
        public async Task<object> GetAuthorityAsync([FromQuery]oa_files entity)
        {
            return await _filesService.GetAuthorityAsync(entity);
        }
    }
}