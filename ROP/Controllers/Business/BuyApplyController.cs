using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.Business;
using Service.Repository.Interfaces.Business;
using Tools;

namespace ROP.Controllers.Business
{
    /// <summary>
    /// 采购申请--ty
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BuyApplyController : ControllerBase
    {
        private readonly IBuyApplyService _buyApplyService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buyApplyService"></param>
        public BuyApplyController(IBuyApplyService buyApplyService)
        {
            _buyApplyService = buyApplyService;
        }

        /// <summary>
        /// 采购申请分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPageAsync")]
        public async Task<Page<bus_buy_apply>> GetPageAsync([FromQuery]BuyApplySearch entity)
        {
            return await _buyApplyService.GetPageAsync(entity);
        }

        /// <summary>
        /// 采购申请明细分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPageDetailAsync")]
        public async Task<Page<bus_buy_apply_detials>> GetPageDetailAsync([FromQuery]BuyApplyDetailSearch entity)
        {
            return await _buyApplyService.GetPageDetailAsync(entity);
        }

        /// <summary>
        /// 新增采购单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("AddBuyApply")]
        public async Task<bool> AddBuyApply([FromBody]BuyApplyAddModel entity)
        {
            return await _buyApplyService.AddBuyApply(entity);
        }
        /// <summary>
        /// 编辑采购单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("ModifyApply")]
        public async Task<bool> ModifyApply([FromBody]BuyApplyModifyModel entity)
        {
            return await _buyApplyService.ModifyApply(entity);
        }

        /// <summary>
        /// 提交采购单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("CommitApply")]
        public async Task<bool> CommitApply([FromBody]CommitModel entity)
        {
            return await _buyApplyService.CommitApply(entity);
        }

        /// <summary>
        /// 作废采购单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("DeleteApply")]
        public async Task<bool> DeleteApply([FromBody]CommitModel entity)
        {
            return await _buyApplyService.DeleteApply(entity);
        }

        /// <summary>
        /// 获得分页列表（已审核）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetApplyPage")]
        public async Task<Page<ApplyBill>> GetApplyPageAsync([FromQuery]ApplyBillPageSearch entity)
        {
            return await _buyApplyService.GetApplyPageAsync(entity);
        }

        /// <summary>
        ///  立即提交采购申请
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("ImmediatelyCommit")]
        public async Task<bool> ImmediatelyCommit([FromBody]BuyApplyAddModel entity)
        {
            return await _buyApplyService.ImmediatelyCommit(entity);
        }

        /// <summary>
        /// 获取明细及审核信息
        /// </summary>
        /// <param name="apply_no">采购申请单号</param>
        /// <returns></returns>
        [HttpGet("GetDetialsAndVerify")]
        public async Task<BuyApplyDetials> GetDetialsAndVerifyAsync([FromQuery]string apply_no)
        {
            return await _buyApplyService.GetDetialsAndVerifyAsync(apply_no);
        }

        /// <summary>
        /// 根据申请单号集合获取对应明细
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpGet("GetDetials")]
        public async Task<List<bus_buy_apply_detials>> GetDetialsAsync([FromQuery]string list)
        {
            return await _buyApplyService.GetDetialsAsync(list.Split(",").ToList());
        }

        /// <summary>
        /// 删除草稿
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("DeleteDrafyAsync")]
        public async Task<bool> DeleteDrafyAsync([FromBody]CommitModel entity)
        {
            return await _buyApplyService.DeleteDrafyAsync(entity);
        }

        /// <summary>
        /// 导出到Excel
        /// </summary>
        /// <returns></returns>
        [HttpGet("Export")]
        public async Task<IActionResult> ExportExcel([FromQuery]string No)
        {
            var excelFilePath = await _buyApplyService.ExportAsync(No);

            //下边这个是返回文件需要前端处理   
            return File(new FileStream(excelFilePath, FileMode.Open), "application/octet-stream", "采购申请单.xlsx");
            //下边这种是返回下载连接,可通过浏览器直接访问下载
            //return excelFilePath;

        }
    }
}