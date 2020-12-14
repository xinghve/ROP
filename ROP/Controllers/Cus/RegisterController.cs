using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.His;
using Models.View.Mobile;
using Service.Repository.Interfaces.His;
using Tools;

namespace ROP.Controllers.Cus
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
        /// 获得分页列表（客户端）
        /// </summary>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        [HttpGet("GetPagesArc")]
        public async Task<Page<ArcRegisterRecord>> GetPagesArcAsync([FromQuery] string order, [FromQuery] int orderType, [FromQuery] int limit, [FromQuery] int page)
        {
            return await _registerService.GetPagesArcAsync(order, orderType, limit, page);
        }

        /// <summary>
        /// 获取一周日期信息（日期、星期、号数）和当前客户是否收藏
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetWeeks")]
        public async Task<dynamic> GetWeeks([FromQuery] int doctor_id)
        {
            return await _registerService.GetWeeks(doctor_id);
        }

        /// <summary>
        /// 根据科室和日期、医生获取排班
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="deptid">科室ID</param>
        /// <param name="date">指定日期</param>
        /// <param name="doctor_id">医生ID</param>
        /// <returns></returns>
        [HttpGet("GetByDeptAndDateAndDoctor")]
        public async Task<List<Scheduletimes>> GetByDeptAndDateAndDoctorAsync([FromQuery] int store_id, [FromQuery] int deptid, [FromQuery]  DateTime date, [FromQuery] int doctor_id)
        {
            return await _registerService.GetByDeptAndDateAndDoctorAsync(store_id, deptid, date, doctor_id);
        }

        /// <summary>
        /// 预约挂号
        /// </summary>
        /// <returns></returns>
        [HttpPost("CusAdd")]
        public async Task<bool> CusAddAsync([FromBody]CusRegister entity)
        {
            return await _registerService.CusAddAsync(entity);
        }
    }
}