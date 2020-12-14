using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.View.OA;
using Models.View.Public;
using Tools;
using Tools.Filter;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 上传
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UpLoadController : ControllerBase
    {
        /// <summary>
        /// 上传入口（营业执照）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<List<string>> UploadAsync()
        {
            var files = Request.Form.Files;
            dynamic type = (new Program()).GetType();
            List<string> result = await UpLoadFiles.FileSave("License", files, type);
            return result;
        }

        /// <summary>
        /// 上传入口（集团图片）
        /// </summary>
        /// <returns></returns>
        [HttpPost("UploadOrg")]
        public async Task<List<string>> UploadOrgAsync()
        {
            var files = Request.Form.Files;
            dynamic type = (new Program()).GetType();
            List<string> result = await UpLoadFiles.FileSave("Org", files, type);
            return result;
        }

        /// <summary>
        /// 上传入口（门店图片）
        /// </summary>
        /// <returns></returns>
        [HttpPost("UploadStore")]
        public async Task<List<string>> UploadStoreAsync()
        {
            var files = Request.Form.Files;
            dynamic type = (new Program()).GetType();
            List<string> result = await UpLoadFiles.FileSave("Store", files, type);
            return result;
        }

        /// <summary>
        /// 上传入口（部门图片）
        /// </summary>
        /// <returns></returns>
        [HttpPost("UploadDept")]
        public async Task<List<string>> UploadDeptAsync()
        {
            var files = Request.Form.Files;
            dynamic type = (new Program()).GetType();
            List<string> result = await UpLoadFiles.FileSave("Dept", files, type);
            return result;
        }

        /// <summary>
        /// 上传入口（人员图片）
        /// </summary>
        /// <returns></returns>
        [HttpPost("UploadEmp")]
        public async Task<List<string>> UploadEmpAsync()
        {
            var files = Request.Form.Files;
            dynamic type = (new Program()).GetType();
            List<string> result = await UpLoadFiles.FileSave("Employee", files, type);
            return result;
        }

        /// <summary>
        /// 上传入口（分销人员图片）
        /// </summary>
        /// <returns></returns>
        [HttpPost("UploadDistributor")]
        public async Task<List<string>> UploadDistributorAsync()
        {
            var files = Request.Form.Files;
            dynamic type = (new Program()).GetType();
            List<string> result = await UpLoadFiles.FileSave("Distributor", files, type);
            return result;
        }

        /// <summary>
        /// 上传入口（客户图片）
        /// </summary>
        /// <returns></returns>
        [HttpPost("UploadArchives")]
        public async Task<List<string>> UploadArchivesAsync()
        {
            var files = Request.Form.Files;
            dynamic type = (new Program()).GetType();
            List<string> result = await UpLoadFiles.FileSave("Archives", files, type);
            return result;
        }

        /// <summary>
        /// 上传入口（人员图片）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("UploadEmp64")]
        public async Task<string> UploadEmp64Async([FromBody]UpLoad entity)
        {
            dynamic type = (new Program()).GetType();
            var result = await UpLoadFiles.FileSave("Employee", entity.name, entity.str, type);
            return result;
        }

        /// <summary>
        /// 上传入口（客户图片）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("UploadArchives64")]
        public async Task<string> UploadArchives64Async([FromBody]UpLoad entity)
        {
            dynamic type = (new Program()).GetType();
            var result = await UpLoadFiles.FileSave("Archives", entity.name, entity.str, type);
            return result;
        }

        /// <summary>
        /// 上传入口（分销人员图片）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("UploadDistributor64")]
        public async Task<string> UploadDistributor64Async([FromBody]UpLoad entity)
        {
            dynamic type = (new Program()).GetType();
            var result = await UpLoadFiles.FileSave("Distributor", entity.name, entity.str, type);
            return result;
        }

        /// <summary>
        /// 上传入口（反馈与投诉）
        /// </summary>
        /// <returns></returns>
        [HttpPost("UploadFeedBack")]
        public async Task<List<string>> UploadFeedBackAsync()
        {
            var files = Request.Form.Files;
            dynamic type = (new Program()).GetType();
            List<string> result = await UpLoadFiles.FileSave("FeedBack", files, type);
            return result;
        }

        /// <summary>
        /// 上传入口（活动图片）
        /// </summary>
        /// <returns></returns>
        [HttpPost("UploadActivity")]
        public async Task<List<string>> UploadActivityAsync()
        {
            var files = Request.Form.Files;
            dynamic type = (new Program()).GetType();
            List<string> result = await UpLoadFiles.FileSave("Activity", files, type);
            return result;
        }

        /// <summary>
        /// 上传入口（客户图片）
        /// </summary>
        /// <returns></returns>
        [HttpPost("UploadCustomer")]
        public async Task<List<string>> UploadCustomerAsync()
        {
            var files = Request.Form.Files;
            dynamic type = (new Program()).GetType();
            List<string> result = await UpLoadFiles.FileSave("Customer", files, type);
            return result;
        }

        /// <summary>
        /// 上传入口（请假图片）
        /// </summary>
        /// <param name="leave_type_id">请假类型id,跟在url后</param>
        /// <returns></returns>
        [HttpPost("UploadLeave")]
        public async Task<List<string>> UploadLeavelAsync([FromQuery] int leave_type_id)
        {
            var files = Request.Form.Files;
            dynamic type = (new Program()).GetType();
            List<string> result = await UpLoadFiles.FileSave("Leave/"+ leave_type_id, files, type);
            return result;
        }

        /// <summary>
        /// 上传入口（会议附件）
        /// </summary>
        /// <returns></returns>
        [HttpPost("UploadMeeting")]
        public async Task<List<FilesInfo>> UploadMeetingAsync()
        {
            var files = Request.Form.Files;
            dynamic type = (new Program()).GetType();
            List<FilesInfo> result = await UpLoadFiles.FilesSave("Meeting", files, type);
            return result;
        }
    }
}