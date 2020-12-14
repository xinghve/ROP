using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.View.Mobile;
using Service.Repository.Interfaces.Public;

namespace ROP.Controllers.Mobile
{
    /// <summary>
    /// 人员
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IPersonService _personService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="personService"></param>
        public EmployeeController(IPersonService personService)
        {
            _personService = personService;
        }

        /// <summary>
        /// 根据Id获取人员头像
        /// </summary>
        /// <param name="id">人员ID</param>
        /// <returns></returns>
        [HttpGet("GetPhotoById")]
        public async Task<string> GetPhotoByIdAsync([FromQuery]int id)
        {
            return await _personService.GetPhotoByIdAsync(id);
        }

        /// <summary>
        /// 编辑人员
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<bool> Modify([FromBody]Employee entity)
        {
            return await _personService.Modify(entity);
        }

        /// <summary>
        /// 编辑人员头像
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("ModifyPhotoById")]
        public async Task<int> ModifyPhotoByIdAsync([FromBody]Photo entity)
        {
            return await _personService.ModifyPhotoByIdAsync(entity);
        }

        /// <summary>
        /// 获取分销二维码
        /// </summary>
        /// <param name="store_id"></param>
        /// <returns></returns>
        [HttpGet("GetDistributionQrCode")]
        public string GetDistributionQrCode([FromQuery]int store_id)
        {
            dynamic type = (new Program()).GetType();
            return _personService.GetDistributionQrCode(store_id, type);
        }
    }
}