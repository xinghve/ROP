using Models.DB;
using Models.View.His;
using Models.View.Mobile;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;
using static Tools.Utils;

namespace Service.Repository.Interfaces.His
{
    /// <summary>
    /// 预约挂号
    /// </summary>
    public interface IRegisterService
    {
        /// <summary>
        /// 预约挂号
        /// </summary>
        /// <returns></returns>
        Task<bool> AddAsync(HisRegister entity);

        /// <summary>
        /// 预约挂号
        /// </summary>
        /// <returns></returns>
        Task<bool> CusAddAsync(CusRegister entity);

        /// <summary>
        /// 根据科室和日期获取医生排班
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="deptid">科室ID</param>
        /// <param name="date">指定日期</param>
        /// <returns></returns>
        Task<List<RegisterDoctor>> GetByDeptAndDateAsync(int store_id, int deptid, DateTime date);

        /// <summary>
        /// 根据医生获取最近一周排班
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="doctor_id">医生ID</param>
        /// <returns></returns>
        Task<List<RegisterDate>> GetByDoctorAsync(int store_id, int doctor_id);

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="name_or_phone">姓名或手机号</param>
        /// <param name="store_id">门店ID</param>
        /// <param name="dept_id">科室ID</param>
        /// <param name="doctor_id">医生ID</param>
        /// <param name="type_id">类别ID</param>
        /// <param name="orderflag">预约标志</param>
        /// <param name="stateid">状态ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        Task<Page<RegisterRecord>> GetPagesAsync(string name_or_phone, int store_id, int dept_id, int doctor_id, int type_id, int orderflag, int stateid, string order, int orderType, int limit, int page);

        /// <summary>
        /// 修改挂号日期
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> ModifyDateAsync(ModifyDateModel entity);

        /// <summary>
        /// 获得分页列表（客户端）
        /// </summary>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        Task<Page<ArcRegisterRecord>> GetPagesArcAsync(string order, int orderType, int limit, int page);

        /// <summary>
        /// 获取一周日期信息（日期、星期、号数）和当前客户是否收藏
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetWeeks(int doctor_id);

        /// <summary>
        /// 根据科室和日期、医生获取排班
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="deptid">科室ID</param>
        /// <param name="date">指定日期</param>
        /// <param name="doctor_id">医生ID</param>
        /// <returns></returns>
        Task<List<Scheduletimes>> GetByDeptAndDateAndDoctorAsync(int store_id, int deptid, DateTime date, int doctor_id);
    }
}
