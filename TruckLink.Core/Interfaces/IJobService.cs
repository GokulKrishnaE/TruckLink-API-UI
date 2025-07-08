using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TruckLink.Core.Entities;
using TruckLink.Core.models;

namespace TruckLink.Core.Interfaces
{
    public interface IJobService
    {
        Task<List<Job>> GetAvailableJobsAsync(JobFilter filter);

        Task AddJobAsync(Job job);

        Task<bool> AcceptJobAsync(Guid jobId, Guid driverId);

        Task<List<Job>> GetJobsByDriverAsync(Guid driverId);

        // Get all driver requests (interests) for a given poster (job creator)
        Task<List<JobInterest>> GetInterestsForPosterAsync(Guid posterId);

        // Drivers request a job by sending mobile number
        Task<bool> RequestJobAsync(Guid jobId, Guid driverId, string mobileNumber);

        Task<List<Job>> GetJobsDriverHasRequestedAsync(Guid driverId);

        Task<List<JobInterest>> GetJobInterestsByDriverAsync(Guid driverId);

        Task<List<Job>> GetJobsWithRequestsForPosterAsync(Guid posterId);

        Task<bool> UpdateJobAsync(Guid jobId, Job updatedJob, Guid posterId);

        Task<bool> DeleteJobAsync(Guid jobId, Guid posterId);

        Task<bool> MarkJobAsCompletedAsync(Guid jobId, Guid posterId);
    }
}
