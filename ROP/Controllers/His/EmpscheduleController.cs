using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.View.His;
using Service.Repository.Interfaces.His;

namespace ROP.Controllers.His
{
    /// <summary>
    /// 排班
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EmpscheduleController : ControllerBase
    {
        private readonly IEmpscheduleService _empscheduleService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="empscheduleService"></param>
        public EmpscheduleController(IEmpscheduleService empscheduleService)
        {
            _empscheduleService = empscheduleService;
        }

        /// <summary>
        /// 获取排班
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="deptid">部门ID</param>
        /// <param name="orderbegin">开始时间</param>
        /// <param name="orderend">结束时间</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Empschedule> Get([FromQuery]int store_id, [FromQuery]int deptid, [FromQuery]DateTime orderbegin, [FromQuery]DateTime orderend)
        {
            return await _empscheduleService.GetAsync(store_id, deptid, orderbegin, orderend);
        }

        /// <summary>
        /// 设置排班
        /// </summary>
        /// <param name="item">排班</param>
        /// <returns>成功/失败</returns>
        [HttpPost]
        public async Task<string> Post([FromBody] Empschedule item)
        {
            return await _empscheduleService.Set(item);
        }

        /// <summary>
        /// 获取存在排班的年份
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="deptid">部门ID</param>
        /// <returns></returns>
        [HttpGet("GetYear")]
        public async Task<List<int>> GetYearAsync([FromQuery]int store_id, [FromQuery] int deptid)
        {
            return await _empscheduleService.GetYearAsync(store_id, deptid);
        }

        /// <summary>
        /// 获取排班时间段
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="deptid">部门ID</param>
        /// <param name="year">年份</param>
        /// <returns></returns>
        [HttpGet("GetDate")]
        public async Task<List<EmpscheduleDate>> GetDateAsync([FromQuery]int store_id, [FromQuery]int deptid, [FromQuery]int year)
        {
            return await _empscheduleService.GetDateAsync(store_id, deptid, year);
        }
    }
}