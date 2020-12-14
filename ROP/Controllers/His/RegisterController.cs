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
    /// 预约挂号
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterService _registerService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registerService"></param>
        public RegisterController(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        /// <summary>
        /// 根据科室和日期获取医生排班
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="deptid">科室ID</param>
        /// <param name="date">指定日期</param>
        /// <returns></returns>
        [HttpGet("GetByDeptAndDate")]
        public async Task<List<RegisterDoctor>> GetByDeptAndDateAsync([FromQuery]int store_id, [FromQuery]int deptid, [FromQuery]DateTime date)
        {
            return await _registerService.GetByDeptAndDateAsync(store_id, deptid, date);
        }

        /// <summary>
        /// 根据医生获取最近一周排班
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="doctor_id">医生ID</param>
        /// <returns></returns>
        [HttpGet("GetByDoctor")]
        public async Task<List<RegisterDate>> GetByDoctorAsync([FromQuery]int store_id, [FromQuery] int doctor_id)
        {
            return await _registerService.GetByDoctorAsync(store_id, doctor_id);
        }

        /// <summary>
        /// 预约挂号
        /// </summary>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<bool> AddAsync([FromBody]HisRegister entity)
        {
            return await _registerService.AddAsync(entity);
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="name_or_phone">姓名或手机号</param>
        /// <param name="store_id">门店ID</param>
        /// <param name="dept_id">科室ID</param>
        /// <param name="doctor_id">科室ID</param>
        /// <param name="type_id">类别ID</param>
        /// <param name="orderflag">预约标志</param>
        /// <param name="stateid">状态ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<RegisterRecord>> GetPagesAsync([FromQuery]string name_or_phone, [FromQuery]int store_id, [FromQuery]int dept_id, [FromQuery]int doctor_id, [FromQuery] int type_id, [FromQuery]int orderflag, [FromQuery]int stateid, [FromQuery] string order, [FromQuery] int orderType, [FromQuery] int limit, [FromQuery] int page)
        {
            return await _registerService.GetPagesAsync(name_or_phone, store_id, dept_id, doctor_id, type_id, orderflag, stateid, order, orderType, limit, page);
        }

        /// <summary>
        /// 更改挂号日期
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("ModifyDateAsync")]
        public async Task<bool> ModifyDateAsync([FromBody]ModifyDateModel entity)
        {
            return await _registerService.ModifyDateAsync(entity);
        }
    }
}