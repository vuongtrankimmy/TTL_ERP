using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Training.Models;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.Training.Interfaces
{
    public interface ITrainingService
    {
        Task<IEnumerable<CourseModel>> GetCoursesAsync();
        Task<CourseModel?> GetCourseAsync(string id);
        Task<bool> CreateCourseAsync(CourseModel course);
        Task<bool> UpdateCourseAsync(string id, CourseModel course);
        Task<bool> DeleteCourseAsync(string id);
        Task<bool> RegisterCourseAsync(string courseId);
        Task<IEnumerable<ParticipantModel>> GetParticipantsAsync(string courseId);
        Task<bool> RemoveParticipantAsync(string courseId, string employeeId);
        Task<ApiResponse<bool>> RegisterParticipantsAsync(string courseId, List<string> employeeIds);
        Task<TrainingAnalyticsModel?> GetAnalyticsAsync();
    }
}
