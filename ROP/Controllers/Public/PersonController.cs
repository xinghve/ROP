using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.Public;
using Service.Repository.Interfaces.Public;
using Tools;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 人员管理--ty
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;
        /// <summary>
        /// 人员构造
        /// </summary>
        /// <param name="personService"></param>
        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        /// <summary>
        /// 获取人员分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="status">状态（1=已启用 2=已停用 0=所有）</param>
        [HttpGet("GetPageAsync")]
        public async Task<Page<PersonDetials>> GetPageAsync([FromQuery]SearchModel entity, [FromQuery] int status = 0)
        {
            return await _personService.GetPageAsync(entity, status);
        }

        /// <summary>
        /// 根据Id获取人员信息
        /// </summary>
        /// <param name="roleEntity"></param>
        /// <returns></returns>
        [HttpGet("GetPersonById")]
        public async Task<dynamic> GetPersonById([FromQuery]p_employee_role roleEntity)
        {
            return await _personService.GetPersonById(roleEntity);
        }

        /// <summary>
        /// 添加人员信息、详情、部门
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<bool> Add([FromBody]PersonModel entity)
        {
            return await _personService.Add(entity);
        }

        /// <summary>
        /// 编辑人员信息、详情、部门
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<bool> Modify([FromBody]PersonModel entity)
        {
            return await _personService.Modify(entity);
        }

        /// <summary>
        /// 批量删除人员
        /// </summary>
        /// <param name="delList"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<int> DeleteAsync([FromBody]List<int> delList)
        {
            return await _personService.DeleteAsync(delList);
        }

        /// <summary>
        /// 批量启用人员
        /// </summary>
        /// <param name="useList"></param>
        /// <returns></returns>
        [HttpPut("Use")]
        public async Task<int> UseAsync([FromBody]List<int> useList)
        {
            return await _personService.UseAsync(useList);
        }

        /// <summary>
        /// 人员调度
        /// </summary>
        /// <param name="employeeDispatch">人员调度信息</param>
        /// <returns></returns>
        [HttpPut("Dispatch")]
        public async Task<bool> DispatchAsync([FromBody]EmployeeDispatch employeeDispatch)
        {
            return await _personService.DispatchAsync(employeeDispatch);
        }


        /// <summary>
        /// 根据人员ID获取人员门店列表
        /// </summary>
        /// <param name="id">人员ID</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<List<Store>> GetListAsync([FromQuery] int id)
        {
            return await _personService.GetListAsync(id);
        }


        /// <summary>
        /// 根据门店ID获取人员列表
        /// </summary>
        /// <param name="name">姓名/拼音/手机号/身份证</param>
        /// <param name="natureID">性质ID（默认-1代表所有）</param>
        /// <param name="storeID">门店ID（默认-1代表所有）</param>
        /// <param name="state">状态（-1=所有，0=已失效，1=正常）</param>
        /// <param name="dept_id">部门ID</param>
        /// <returns></returns>
        [HttpGet("GetEmployeeList")]
        public async Task<List<p_employee>> GetEmployeeListAsync([FromQuery] string name, [FromQuery] int natureID = -1, [FromQuery] int storeID = -1, [FromQuery] int state = -1, [FromQuery] int dept_id = -1)
        {
            return await _personService.GetEmployeeListAsync(name, natureID, storeID, state, dept_id);
        }


        /// <summary>
        /// 获取人员二维码
        /// </summary>
        /// <param name="storeID">门店ID</param>
        /// <param name="deptTypeCode">部门分类编码</param>
        /// <returns></returns>
        [HttpGet("GetQrCode")]
        public async Task<dynamic> GetQrCodeAsync([FromQuery] int storeID, [FromQuery] string deptTypeCode)
        {
            dynamic type = (new Program()).GetType();
            return await _personService.GetQrCodeAsync(storeID, deptTypeCode, type);
        }

        /// <summary>
        /// 根据科室获取医生
        /// </summary>
        /// <param name="deptId">科室ID</param>
        /// <param name="storeId">门店ID</param>
        /// <param name="name">医生名称</param>
        /// <returns></returns>
        [HttpGet("GetDoctorList")]
        public async Task<List<DoctorModel>> GetDoctorList([FromQuery]int deptId, [FromQuery] int storeId, [FromQuery] string name = "")
        {
            return await _personService.GetDoctorList(deptId, storeId, name);
        }

        /// <summary>
        /// 获取医生简介
        /// </summary>
        /// <param name="doctorId"></param>
        /// <param name="deptId"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        [HttpGet("GetDoctoIntroduce")]
        public async Task<DoctorDetailModel> GetDoctoIntroduce([FromQuery]int doctorId, [FromQuery]int deptId, [FromQuery]int storeId)
        {
            return await _personService.GetDoctorIntroduce(doctorId, deptId, storeId);
        }

        /// <summary>
        /// 收藏医生
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("CollectionDoctor")]
        public async Task<int> CollectionDoctor([FromBody]c_collection entity)
        {
            return await _personService.CollectionDoctor(entity);
        }

        /// <summary>
        /// 删除收藏
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("DeleteCollection")]
        public async Task<int> DeleteCollection([FromBody]c_collection entity)
        {
            return await _personService.DeleteCollection(entity);
        }

        /// <summary>
        /// 流程选定人员
        /// </summary>
        /// <param name="dept_id">部门id</param>
        /// <param name="role_id">角色id</param>
        /// <param name="searchVal">姓名或电话</param>
        /// <returns></returns>
        [HttpGet("GetProcessPerson")]
        public async Task<List<PersonProcessModel>> GetProcessPerson([FromQuery]int dept_id, [FromQuery]int role_id, [FromQuery]string searchVal)
        {
            return await _personService.GetProcessPerson(dept_id, role_id,searchVal);
        }

        /// <summary>
        /// 门店新增人员
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("StoreAddPerson")]
        public async Task<bool> StoreAddPerson([FromBody]StoreAddPersonModel entity)
        {
            return await _personService.StoreAddPerson(entity);
        }

        /// <summary>
        /// 门店移除角色
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("StoreRemovePerson")]
        public async Task<bool> StoreRemovePerson([FromBody]StoreRemovePersonModel entity)
        {
            return await _personService.StoreRemovePerson(entity);
        }
        /// <summary>
        /// 获取机构下人员
        /// </summary>
        /// <param name="dept_id"></param>
        /// <param name="role_id"></param>
        /// <returns></returns>
        [HttpGet("GetOrgPerson")]
        public async Task<List<PersonProcessModel>> GetOrgPerson([FromQuery]int dept_id,[FromQuery] int role_id)
        {
            return await _personService.GetOrgPerson(dept_id,role_id);
        }
    }
}