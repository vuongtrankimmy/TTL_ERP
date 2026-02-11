using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Recruitment.Models;

namespace TTL.HR.Application.Modules.Recruitment.Interfaces
{
    public interface IRecruitmentService
    {
        Task<IEnumerable<JobDetail>> GetJobsAsync();
        Task<JobDetail?> GetJobAsync(string id);
        Task<JobDetail?> CreateJobAsync(JobDetail job);
        Task UpdateJobAsync(string id, JobDetail job);
        Task DeleteJobAsync(string id);

        Task<IEnumerable<ApplicantItem>> GetApplicantsAsync(string jobId);
        Task<bool> AddApplicantAsync(string jobId, ApplicantRequest applicant);
        Task<bool> UpdateApplicantStatusAsync(string applicantId, string status, string? note = null);
        Task<bool> ScheduleInterviewAsync(string applicantId, InterviewScheduleModel schedule);
        Task<bool> DeleteApplicantAsync(string applicantId);
    }
}
