using TTL.HR.Application.Modules.Common.Entities;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Recruitment.Entities;
using TTL.HR.Application.Modules.Recruitment.Interfaces;
using TTL.HR.Application.Modules.Recruitment.Models;

namespace TTL.HR.Application.Modules.Recruitment
{
    public interface IRecruitmentApplication
    {
        Task<IEnumerable<JobDetail>> GetActiveJobPostingsAsync();
        Task<JobDetail?> GetJobDetailsAsync(string id);
        Task<bool> PostNewJobAsync(JobDetail job);
        Task<bool> UpdateJobPostingAsync(string id, JobDetail job);
        Task<bool> DeleteJobPostingAsync(string id);
        
        Task<IEnumerable<ApplicantItem>> GetApplicantsAsync(string jobId);
        Task<bool> AddApplicantAsync(string jobId, ApplicantRequest applicant);
        Task<bool> UpdateApplicantStatusAsync(string applicantId, string status, string? note = null);
        Task<bool> ScheduleInterviewAsync(string applicantId, InterviewScheduleModel schedule);
        Task<bool> DeleteApplicantAsync(string applicantId);
    }

    public class RecruitmentApplication : IRecruitmentApplication
    {
        private readonly IRecruitmentService _recruitmentService;

        public RecruitmentApplication(IRecruitmentService recruitmentService)
        {
            _recruitmentService = recruitmentService;
        }

        public async Task<IEnumerable<JobDetail>> GetActiveJobPostingsAsync()
        {
            var jobs = await _recruitmentService.GetJobsAsync();
            return jobs.Where(j => j.Status == "Active" || j.Status == "Published" || string.IsNullOrEmpty(j.Status));
        }

        public async Task<JobDetail?> GetJobDetailsAsync(string id)
        {
            return await _recruitmentService.GetJobAsync(id);
        }

        public async Task<bool> PostNewJobAsync(JobDetail job)
        {
            if (string.IsNullOrWhiteSpace(job.Title)) return false;
            
            var result = await _recruitmentService.CreateJobAsync(job);
            return result != null;
        }

        public async Task<bool> UpdateJobPostingAsync(string id, JobDetail job)
        {
            try 
            {
                await _recruitmentService.UpdateJobAsync(id, job);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteJobPostingAsync(string id)
        {
            try 
            {
                await _recruitmentService.DeleteJobAsync(id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<ApplicantItem>> GetApplicantsAsync(string jobId)
        {
            // Assuming the service has a method to get applicants by job ID
            // If not, I'll need to update IRecruitmentService too
            return await _recruitmentService.GetApplicantsAsync(jobId);
        }

        public async Task<bool> AddApplicantAsync(string jobId, ApplicantRequest applicant)
        {
            return await _recruitmentService.AddApplicantAsync(jobId, applicant);
        }

        public async Task<bool> UpdateApplicantStatusAsync(string applicantId, string status, string? note = null)
        {
            return await _recruitmentService.UpdateApplicantStatusAsync(applicantId, status, note);
        }

        public async Task<bool> ScheduleInterviewAsync(string applicantId, InterviewScheduleModel schedule)
        {
            return await _recruitmentService.ScheduleInterviewAsync(applicantId, schedule);
        }

        public async Task<bool> DeleteApplicantAsync(string applicantId)
        {
            return await _recruitmentService.DeleteApplicantAsync(applicantId);
        }
    }
}
