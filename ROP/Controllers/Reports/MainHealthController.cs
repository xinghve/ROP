using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.View.Reports;
using Service.Repository.Interfaces.Reports;

namespace ROP.Controllers.Reports
{
    /// <summary>
    /// 医疗板块报表--ty
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MainHealthController : ControllerBase
    {
        private readonly IMainHealthService _mainHealthService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainHealthService"></param>
        public MainHealthController(IMainHealthService mainHealthService)
        {
            _mainHealthService = mainHealthService;
        }

        /// <summary>
        /// 获取医疗基本信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetHealthAsync")]
        public async Task<MainHealthModel> GetHealthAsync([FromQuery]DoctorTopSearch entity)
        {
            return await _mainHealthService.GetHealthAsync(entity);
        }

        /// <summary>
        /// 获取医生排名
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetTopList")]
        public async Task<doctorList> GetTopList([FromQuery]DoctorTopSearch entity)
        {
            return await _mainHealthService.GetTopList(entity);
        }
    }
}