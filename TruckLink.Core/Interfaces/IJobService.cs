using System.Collections.Generic;
using System.Threading.Tasks;
using TruckLink.Core.Entities;

namespace TruckLink.Core.Interfaces
{
    public interface IJobService
    {
        Task<List<Job>> GetAvailableJobsAsync();

        Task AddJobAsync(Job job);

        Task<bool> AcceptJobAsync(int jobId, int driverId);

        Task<List<Job>> GetJobsByDriverAsync(int driverId);

        // Get all driver requests (interests) for a given poster (job creator)
        Task<List<JobInterest>> GetInterestsForPosterAsync(int posterId);

        // Drivers request a job by sending mobile number
        Task<bool> RequestJobAsync(int jobId, int driverId, string mobileNumber);

        Task<List<Job>> GetJobsDriverHasRequestedAsync(int driverId);

        Task<List<JobInterest>> GetJobInterestsByDriverAsync(int driverId);

        Task<List<Job>> GetJobsWithRequestsForPosterAsync(int posterId);

        Task<bool> UpdateJobAsync(int jobId, Job updatedJob, int posterId);
        Task<bool> DeleteJobAsync(int jobId, int posterId);

        Task<bool> MarkJobAsCompletedAsync(int jobId, int posterId);
    }
}
