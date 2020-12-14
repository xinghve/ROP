using Models.DB;
using Models.View.OA;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.OA
{
    /// <summary>
    /// 会议
    /// </summary>
    public interface IMeetingService
    {
        /// <summary>
        /// 添加会议
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> AddMeetingAsync(Meeting entity);

        /// <summary>
        /// 延期会议
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> DelayMeetingAsync(oa_meeting entity);

        /// <summary>
        /// 取消会议
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> CancelMeetingAsync(oa_meeting entity);

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<Meeting>> GetPageAsync(MeetingPageSearch entity);

        /// <summary>
        /// 获取会议记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<string> GetRecordAsync(int id);

        /// <summary>
        /// 修改会议记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> ModifyRecordAsync(oa_meeting_employee entity);
        
        /// <summary>
        /// 保存发言
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> SaveSpeakAsync(SpeakList entity);

        /// <summary>
        /// 保存决议
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> SaveResolutionAsync(ResolutionList entity);
    }
}
