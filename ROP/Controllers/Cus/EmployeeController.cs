using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.View.Mobile;
using Service.Repository.Interfaces.Public;

namespace ROP.Controllers.Cus
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
        /// 获取收藏医生
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCollectionList")]
        public async Task<dynamic> GetCollectionListAsync()
        {
            return await _personService.GetCollectionListAsync();
        }
    }
}