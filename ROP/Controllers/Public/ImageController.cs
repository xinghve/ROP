using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.Public;
using Service.Repository.Interfaces.Public;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 图片
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImagesService _imagesService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imagesService"></param>
        public ImageController(IImagesService imagesService)
        {
            _imagesService = imagesService;
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="realtionCode">关联编码</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<p_image>> Get([FromQuery]string realtionCode)
        {
            return await _imagesService.GetAsync(realtionCode);
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="item">图片</param>
        /// <returns>成功/失败</returns>
        [HttpPost]
        public async Task<bool> Post([FromBody] Image item)
        {
            dynamic type = (new Program()).GetType();
            return await _imagesService.SetAsync(item, type);
        }
    }
}