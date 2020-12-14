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
    /// 就诊
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicRecordController : ControllerBase
    {
        private readonly IClinicRecordService _clinicRecordService;

        /// <summary>
        /// 
        /// </summary>
        public ClinicRecordController(IClinicRecordService clinicRecordService)
        {
            _clinicRecordService = clinicRecordService;
        }

        /// <summary>
        /// 签到
        /// </summary>
        /// <returns></returns>
        [HttpPost("Sign")]
        public async Task<bool> SignAsync([FromBody]Sign entity)
        {
            return await _clinicRecordService.SignAsync(entity);
        }

        /// <summary>
        /// 获得分页列表（签到）
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="state_id">状态（1=已签到，0=未签到，7=已取消）</param>
        /// <param name="type_id">分类（1=门诊，2=康复）</param>
        /// <param name="name">客户名称/手机号</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <param name="is_me">是否自己客户</param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<SignRecord>> GetPagesAsync([FromQuery]int store_id, [FromQuery]short state_id, [FromQuery]short type_id, [FromQuery] string name, [FromQuery]string order, [FromQuery]int orderType, [FromQuery]int limit, [FromQuery] int page, [FromQuery] bool is_me = false)
        {
            return await _clinicRecordService.GetPagesAsync(store_id, state_id, type_id, name, order, orderType, limit, page, is_me);
        }

        /// <summary>
        /// 获取就诊记录分页
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="name">客户名称/手机号</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <param name="start_regdate">就诊时间（开始）</param>
        /// <param name="end_regdate">就诊时间（结束）</param>
        /// <param name="is_me">是否自己客户</param>
        /// <returns></returns>
        [HttpGet("GetRecordPages")]
        public async Task<dynamic> GetRecordPagesAsync([FromQuery]int store_id, [FromQuery]string name, [FromQuery] string order, [FromQuery]int orderType, [FromQuery]int limit, [FromQuery]int page, [FromQuery]DateTime? start_regdate, [FromQuery] DateTime? end_regdate, [FromQuery]bool is_me = false)
        {
            return await _clinicRecordService.GetRecordPagesAsync(store_id, name, order, orderType, limit, page, start_regdate, end_regdate, is_me);
        }

        /// <summary>
        /// 根据医生获取待接诊人
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="state">状态（0=待接诊，1=已接诊，2=已完成）</param>
        /// <returns></returns>
        [HttpGet("GetByDoctor")]
        public async Task<dynamic> GetByDoctorAsync([FromQuery]int store_id, [FromQuery] short state)
        {
            return await _clinicRecordService.GetByDoctorAsync(store_id, state);
        }

        /// <summary>
        /// 接诊
        /// </summary>
        /// <returns></returns>
        [HttpPost("Clinic")]
        public async Task<int> ClinicAsync([FromBody]Clinic entity)
        {
            return await _clinicRecordService.ClinicAsync(entity);
        }

        /// <summary>
        /// 获取门诊病历
        /// </summary>
        /// <param name="clinic_id">就诊ID</param>
        /// <returns></returns>
        [HttpGet("GetMedicalRecord")]
        public async Task<his_clinic_mr> GetMedicalRecordAsync([FromQuery]int clinic_id)
        {
            return await _clinicRecordService.GetMedicalRecordAsync(clinic_id);
        }

        /// <summary>
        /// 获取用户最后一条诊疗记录
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        [HttpGet("GetMedicalUserRecordAsync")]
        public async Task<his_clinic_mr> GetMedicalUserRecordAsync([FromQuery]int user_id)
        {
            return await _clinicRecordService.GetMedicalUserRecordAsync(user_id);
        }

        /// <summary>
        /// 门诊病历
        /// </summary>
        /// <returns></returns>
        [HttpPut("MedicalRecord")]
        public async Task<int> MedicalRecordAsync([FromBody]his_clinic_mr entity)
        {
            return await _clinicRecordService.MedicalRecordAsync(entity);
        }

        /// <summary>
        /// 开处方
        /// </summary>
        /// <returns></returns>
        [HttpPost("Prescription")]
        public async Task<int> PrescriptionAsync([FromBody]Prescription entity)
        {
            return await _clinicRecordService.PrescriptionAsync(entity);
        }

        /// <summary>
        /// 获取处方
        /// </summary>
        /// <param name="clinic_id">就诊ID</param>
        /// <param name="typeid">单据类型ID</param>
        /// <returns></returns>
        [HttpGet("GetPrescription")]
        public async Task<List<Prescription>> GetPrescriptionAsync([FromQuery]int clinic_id, [FromQuery]int typeid)
        {
            return await _clinicRecordService.GetPrescriptionAsync(clinic_id, typeid);
        }

        /// <summary>
        /// 删除处方
        /// </summary>
        /// <param name="entity">挂号类别</param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<int> Delete([FromBody]his_applybill entity)
        {
            return await _clinicRecordService.DeleteAsync(entity.applyid);
        }

        /// <summary>
        /// 完成诊疗
        /// </summary>
        /// <param name="entity">传clinicid</param>
        /// <returns></returns>
        [HttpPost("Finish")]
        public async Task<bool> FinishAsync([FromBody]his_applybill entity)
        {
            return await _clinicRecordService.FinishAsync(entity.clinicid);
        }

        /// <summary>
        /// 获得病历列表
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="archives_id">会员ID</param>
        /// <param name="start_clinic_time">就诊时间（开始）</param>
        /// <param name="end_clinic_time">就诊时间（结束）</param>
        /// <returns></returns>
        [HttpGet("GetClinicMrList")]
        public async Task<List<ClinicMr>> GetClinicMrListAsync([FromQuery]int store_id, [FromQuery]short archives_id, [FromQuery]DateTime? start_clinic_time, [FromQuery] DateTime? end_clinic_time)
        {
            return await _clinicRecordService.GetClinicMrListAsync(store_id, archives_id, start_clinic_time, end_clinic_time);
        }
    }
}