using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.His;
using Service.Repository.Interfaces.His;
using Tools;

namespace ROP.Controllers.His
{
    /// <summary>
    /// 诊疗单据
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IBillService _billService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="billService"></param>
        public BillController(IBillService billService)
        {
            _billService = billService;
        }

        /// <summary>
        /// 查询诊疗单据分页数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetPageAsync")]
        public async Task<Page<h_bill>> GetPageAsync([FromQuery]BillSearch model)
        {
            return await _billService.GetPageAsync(model);
        }

        /// <summary>
        /// 添加诊疗单据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("AddAsync")]
        public async Task<int> AddAsync([FromBody]h_bill entity)
        {
            return await _billService.AddAsync(entity);
        }

        /// <summary>
        /// 编辑诊疗单据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("ModifyAsync")]
        public async Task<int> ModifyAsync([FromBody]h_bill entity)
        {
            return await _billService.ModifyAsync(entity);
        }

        /// <summary>
        /// 删除诊疗单据
        /// </summary>
        /// <returns></returns>
        [HttpDelete("DeleteAsync")]
        public async Task<int> DeleteAsync([FromBody]h_bill entity)
        {
            return await _billService.DeleteAsync(entity.bill_id);
        }

    }
}