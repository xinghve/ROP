using Models.DB;
using Models.View.Mobile;
using Models.View.Public;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Public
{
    /// <summary>
    /// 人员管理接口
    /// </summary>
    public interface IPersonService
    {
        /// <summary>
        /// 人员分页信息
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="status">状态（1=已启用 2=已停用 0=所有）</param>
        /// <returns></returns>
        Task<Page<PersonDetials>> GetPageAsync(SearchModel entity, int status);

        /// <summary>
        /// 添加人员信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> Add(PersonModel entity);

        /// <summary>
        /// 编辑人员信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> Modify(PersonModel entity);

        /// <summary>
        /// 编辑人员信息（移动端）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> Modify(Employee entity);

        /// <summary>
        /// 根据Id获取人员
        /// </summary>
        /// <param name="roleEntity"></param>
        /// <returns></returns>
        Task<dynamic> GetPersonById(p_employee_role roleEntity);

        /// <summary>
        /// 批量删除人员
        /// </summary>
        /// <param name="delList"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(List<int> delList);

        /// <summary>
        /// 批量启用人员
        /// </summary>
        /// <param name="useList"></param>
        /// <returns></returns>
        Task<int> UseAsync(List<int> useList);

        /// <summary>
        /// 获取人员门店列表
        /// </summary>
        /// <param name="id">人员ID</param>
        /// <returns></returns>
        Task<List<Store>> GetListAsync(int id);

        /// <summary>
        /// 获取人员列表
        /// </summary>
        /// <param name="name">姓名/拼音/手机号/身份证</param>
        /// <param name="nature_id">性质ID</param>
        /// <param name="store_id">门店ID</param>
        /// <param name="state">状态（-1=所有，0=已失效，1=正常）</param>
        /// <param name="dept_id">部门ID</param>
        /// <returns></returns>
        Task<List<p_employee>> GetEmployeeListAsync(string name, int nature_id = -1, int store_id = -1, int state = -1, int dept_id = -1);

        /// <summary>
        /// 人员调度
        /// </summary>
        /// <param name="employeeDispatch">人员调度信息</param>
        /// <returns></returns>
        Task<bool> DispatchAsync(EmployeeDispatch employeeDispatch);

        /// <summary>
        /// 获取人员二维码
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="dept_type_code">部门分类编码</param>
        /// <returns></returns>
        Task<dynamic> GetQrCodeAsync(int store_id, string dept_type_code, dynamic type);

        /// <summary>
        /// 根据Id获取人员头像
        /// </summary>
        /// <param name="id">人员ID</param>
        /// <returns></returns>
        Task<string> GetPhotoByIdAsync(int id);

        /// <summary>
        /// 根据Id修改人员头像
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> ModifyPhotoByIdAsync(Photo entity);

        /// <summary>
        /// 获取分销二维码
        /// </summary>
        /// <param name="store_id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetDistributionQrCode(int store_id, dynamic type);

        /// <summary>
        /// 获取科室医生
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="storeId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<List<DoctorModel>> GetDoctorList(int deptId, int storeId, string name = "");
        /// <summary>
        /// 获取医生简介
        /// </summary>
        /// <param name="doctorId"></param>
        /// <param name="deptId"></param>
        /// <param name="storeId"></param>
        Task<DoctorDetailModel> GetDoctorIntroduce(int doctorId, int deptId, int storeId);

        /// <summary>
        /// 收藏医生
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> CollectionDoctor(c_collection entity);

        /// <summary>
        /// 移除收藏
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> DeleteCollection(c_collection entity);

        /// <summary>
        /// 获取收藏医生
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetCollectionListAsync();

        /// <summary>
        /// 流程选定人员
        /// </summary>
        /// <param name="dept_id">部门id</param>
        /// <param name="role_id">角色id</param>
        /// <param name="searchVal">姓名或电话</param>
        /// <returns></returns>
        Task<List<PersonProcessModel>> GetProcessPerson(int dept_id,int role_id,string searchVal);

        /// <summary>
        /// 门店新增人员
        /// </summary>
        /// <returns></returns>
        Task<bool> StoreAddPerson(StoreAddPersonModel entity);
        /// <summary>
        /// 门店移除角色
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> StoreRemovePerson(StoreRemovePersonModel entity);

        /// <summary>
        /// 获取机构下人员
        /// </summary>
        /// <param name="dept_id"></param>
        /// <param name="role_id"></param>
        /// <returns></returns>
        Task<List<PersonProcessModel>> GetOrgPerson(int dept_id, int role_id);
    }
}
