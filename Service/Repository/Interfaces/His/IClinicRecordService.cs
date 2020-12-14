using Models.DB;
using Models.View.His;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.His
{
    /// <summary>
    /// 就诊
    /// </summary>
    public interface IClinicRecordService
    {
        /// <summary>
        /// 签到
        /// </summary>
        /// <returns></returns>
        Task<bool> SignAsync(Sign entity);

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
        Task<Page<SignRecord>> GetPagesAsync(int store_id, short state_id, short type_id, string name, string order, int orderType, int limit, int page, bool is_me = false);

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
        Task<dynamic> GetRecordPagesAsync(int store_id, string name, string order, int orderType, int limit, int page, DateTime? start_regdate, DateTime? end_regdate, bool is_me = false);

        /// <summary>
        /// 获取负责人客户签到列表
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="state_id">状态（1=已签到，0=未签到）</param>
        /// <returns></returns>
        Task<List<SignRecord>> GetSignListAsync(int store_id, short state_id);

        /// <summary>
        /// 根据医生获取待接诊人
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="state">状态（0=待接诊，1=已接诊，2=已完成）</param>
        /// <returns></returns>
        Task<dynamic> GetByDoctorAsync(int store_id, short state);

        /// <summary>
        /// 接诊
        /// </summary>
        /// <returns></returns>
        Task<int> ClinicAsync(Clinic entity);

        /// <summary>
        /// 获取门诊病历
        /// </summary>
        /// <param name="clinic_id">就诊ID</param>
        /// <returns></returns>
        Task<his_clinic_mr> GetMedicalRecordAsync(int clinic_id);

        /// <summary>
        /// 获取门诊病历用户数据
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        Task<his_clinic_mr> GetMedicalUserRecordAsync(int user_id);

        /// <summary>
        /// 门诊病历
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> MedicalRecordAsync(his_clinic_mr entity);

        /// <summary>
        /// 开处方
        /// </summary>
        /// <returns></returns>
        Task<int> PrescriptionAsync(Prescription entity);

        /// <summary>
        /// 获取处方
        /// </summary>
        /// <param name="clinic_id">就诊ID</param>
        /// <param name="typeid">单据类型ID</param>
        /// <returns></returns>
        Task<List<Prescription>> GetPrescriptionAsync(int clinic_id, int typeid);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        Task<int> DeleteAsync(int id);

        /// <summary>
        /// 完成诊疗
        /// </summary>
        /// <param name="clinic_id">就诊ID</param>
        /// <returns></returns>
        Task<bool> FinishAsync(int clinic_id);

        /// <summary>
        /// 获得病历列表
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="archives_id">会员ID</param>
        /// <param name="start_clinic_time">就诊时间（开始）</param>
        /// <param name="end_clinic_time">就诊时间（结束）</param>
        /// <returns></returns>
        Task<List<ClinicMr>> GetClinicMrListAsync(int store_id, short archives_id, DateTime? start_clinic_time, DateTime? end_clinic_time);
    }
}
