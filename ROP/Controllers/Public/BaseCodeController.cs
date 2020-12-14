using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Service.Repository;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 基础编码（固定）
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BaseCodeController : ControllerBase
    {
        private readonly IBaseServer<b_basecode> _baseServer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseServer"></param>
        public BaseCodeController(IBaseServer<b_basecode> baseServer)
        {
            _baseServer = baseServer;
        }

        ///// <summary>
        ///// 获取状态
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("GetState")]
        //public async Task<List<basecode>> GetStateAsync()
        //{
        //    return await _baseServer.GetListAsync(w => w.baseid == 0 && w.stateid == 1, o => o.valueid, 0);
        //}

        /// <summary>
        /// 获取号别
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSignalNumber")]
        public async Task<List<b_basecode>> GetSignalNumberAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 1 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取医院收费等级
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetHospitalChargeLevel")]
        public async Task<List<b_basecode>> GetHospitalChargeLevelAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 2 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取人群分类
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPopClass")]
        public async Task<List<b_basecode>> GetPopClassAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 4 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取档案分类
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDocClass")]
        public async Task<List<b_basecode>> GetDocClassAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 5 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取人员属性
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPersonnelAttributes")]
        public async Task<List<b_basecode>> GetPersonnelAttributesAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 7 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取药品单据分类
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDrugDocClass")]
        public async Task<List<b_basecode>> GetDrugDocClassAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 8 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取部门类别
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDeptClass")]
        public async Task<List<b_basecode>> GetDeptClassAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 9 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取科室性质
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDeptNature")]
        public async Task<List<b_basecode>> GetDeptNatureAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 68 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取项目等级
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetProjectLevel")]
        public async Task<List<b_basecode>> GetProjectLevelAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 12 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取项目类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetProjectType")]
        public async Task<List<b_basecode>> GetProjectTypeAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 13 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取项目限制类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetProjectRestrictionType")]
        public async Task<List<b_basecode>> GetProjectRestrictionTypeAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 14 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取药品分类
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDrugClass")]
        public async Task<List<b_basecode>> GetDrugClassAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 15 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取检查项目-类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetInspectionItemsType")]
        public async Task<List<b_basecode>> GetInspectionItemsTypeAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 16 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取就医类别
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMedicalCategory")]
        public async Task<List<b_basecode>> GetMedicalCategoryAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 17 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取卫材类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTypesOfTimber")]
        public async Task<List<b_basecode>> GetTypesOfTimberAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 18 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取诊疗项目-治疗
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDtpt")]
        public async Task<List<b_basecode>> GetDtptAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 19 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取诊疗项目-护理
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDtpn")]
        public async Task<List<b_basecode>> GetDtpnAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 20 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取诊疗项目-检验
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDtii")]
        public async Task<List<b_basecode>> GetDtiiAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 21 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取诊疗项目-手术
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDtpo")]
        public async Task<List<b_basecode>> GetDtpoAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 22 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取诊疗项目-麻醉
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDtia")]
        public async Task<List<b_basecode>> GetDtiaAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 23 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取症状特征
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSymptomCharacteristics")]
        public async Task<List<b_basecode>> GetSymptomCharacteristicsAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 24 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取中药属性
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPocm")]
        public async Task<List<b_basecode>> GetPocmAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 27 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取服务商品分类
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSgc")]
        public async Task<List<b_basecode>> GetSgcAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 32 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取收费项目-护理等级
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCiloc")]
        public async Task<List<b_basecode>> GetCilocAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 37 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取诊疗项目-特殊
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDtps")]
        public async Task<List<b_basecode>> GetDtpsAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 40 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取收费项目-其他
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetChargeItemOther")]
        public async Task<List<b_basecode>> GetChargeItemOtherAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 50 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取收费项目-治疗类别
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCitc")]
        public async Task<List<b_basecode>> GetCitcAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 105 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取收费项目-中医类别
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCicotcm")]
        public async Task<List<b_basecode>> GetCicotcmAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 106 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取特殊给药方法
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSpecialAdministrationMethod")]
        public async Task<List<b_basecode>> GetSpecialAdministrationMethodAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 160 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取执业类别
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCategoryOfPractice")]
        public async Task<List<b_basecode>> GetCategoryOfPracticeAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 511 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取病历档案管理密级
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetClomrm")]
        public async Task<List<b_basecode>> GetClomrmAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 550 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取康复部门类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetKF")]
        public async Task<List<b_basecode>> GetKFAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 51 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取客户类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetArchivesType")]
        public async Task<List<b_basecode>> GetArchivesTypeAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 53 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取星座
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetConstellation")]
        public async Task<List<b_basecode>> GetConstellationAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 54 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取生肖
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetZodiac")]
        public async Task<List<b_basecode>> GetZodiacAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 55 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取文化程度
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetEducation")]
        public async Task<List<b_basecode>> GetEducationAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 56 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取职业
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetOccupation")]
        public async Task<List<b_basecode>> GetOccupationAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 57 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取ABO血型
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetABO")]
        public async Task<List<b_basecode>> GetABOAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 58 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取RH血型
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetRH")]
        public async Task<List<b_basecode>> GetRHAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 59 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取民族
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetNation")]
        public async Task<List<b_basecode>> GetNationAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 60 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取婚姻状况
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMaritalStatus")]
        public async Task<List<b_basecode>> GetMaritalStatusAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 61 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取人员性质
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetEmployeeNature")]
        public async Task<List<b_basecode>> GetEmployeeNatureAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 62 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取性别
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSex")]
        public async Task<List<b_basecode>> GetSexAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 63 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取关系
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetRelation")]
        public async Task<List<b_basecode>> GetRelationAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 64 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取专业职务
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetProDuties")]
        public async Task<List<b_basecode>> GetProDutiesAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 65 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取服务对象
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetService")]
        public async Task<List<b_basecode>> GetServiceAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 67 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取费别
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetFee")]
        public async Task<List<b_basecode>> GetFeeAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 66 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取性别限制
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSexLimitAsync")]
        public async Task<List<b_basecode>> GetSexLimitAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 73 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取费用等级
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetFeeLevelAsync")]
        public async Task<List<b_basecode>> GetFeeLevelAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 69 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取频率
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetFrequency")]
        public async Task<List<b_basecode>> GetFrequencyAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 77 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取客户分类
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetArchivesClass")]
        public async Task<List<b_basecode>> GetArchivesClassAsync()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 72 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取客户渠道
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetArchivesChannel")]
        public async Task<List<b_basecode>> GetArchivesChannel()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 79 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取物资类型属性
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMaterialType")]
        public async Task<List<b_basecode>> GetMaterialType()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 80 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取流程类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetProcessType")]
        public async Task<List<b_basecode>> GetProcessType()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 81 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取入库类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPutInStorageType")]
        public async Task<List<b_basecode>> GetPutInStorageType()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 82 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取请假类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetLeaveProcessType")]
        public async Task<List<b_basecode>> GetLeaveProcessType()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 83 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取会议类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMeetingType")]
        public async Task<List<b_basecode>> GetMeetingType()
        {
            return await _baseServer.GetListAsync(w => w.baseid == 84 && w.stateid == 1, o => o.valueid, 0);
        }

        /// <summary>
        /// 获取支付方式
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPayWay")]
        public async Task<List<b_basecode>> GetPayWay()
        {
            return await _baseServer.GetListAsync(w=>w.baseid==74&&w.stateid==1,o=>o.valueid,0);
        }
    }
}