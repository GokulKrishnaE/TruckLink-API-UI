using TruckLink.Core.Entities;
using TruckLink.Core.Interfaces;
using TruckLink.Core.models;

public class JobService : IJobService
{
    private readonly IRepository<Job> _jobRepo;
    private readonly IRepository<JobInterest> _jobInterestRepo;

    public JobService(
        IRepository<Job> jobRepo,
        IRepository<JobInterest> jobInterestRepo)
    {
        _jobRepo = jobRepo;
        _jobInterestRepo = jobInterestRepo;
    }

    public async Task<List<Job>> GetAvailableJobsAsync(JobFilter filter)
    {
        var jobs = await _jobRepo.GetAsync(
            j => !j.IsAccepted && !j.IsCompleted,
            orderBy: q => q.OrderByDescending(j => j.CreatedAt),
            j => j.Interests
        );

        var query = jobs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var searchTerm = filter.Search.ToLower();

            query = query.Where(j =>
                j.LoadItem.ToLower().Contains(searchTerm) ||
                j.Description.ToLower().Contains(searchTerm) ||
                j.StartLocation.ToLower().Contains(searchTerm) ||
                j.Destination.ToLower().Contains(searchTerm) ||
                j.DistanceKm.ToString().Contains(searchTerm) ||
                j.Earnings.ToString().Contains(searchTerm)
            );
        }

        if (!string.IsNullOrWhiteSpace(filter.StartPlace))
        {
            query = query.Where(j => j.StartLocation == filter.StartPlace);
        }

        if (!string.IsNullOrWhiteSpace(filter.EndPlace))
        {
            query = query.Where(j => j.Destination == filter.EndPlace);
        }

        // Distance range logic (same as your code)
        if (!string.IsNullOrEmpty(filter.Distance))
        {
            int? minDistance = null;
            int? maxDistance = null;

            if (filter.Distance.Contains('+'))
            {
                var minStr = filter.Distance.Replace("+", "");
                if (int.TryParse(minStr, out int minVal))
                    minDistance = minVal;
            }
            else if (filter.Distance.Contains('-'))
            {
                var parts = filter.Distance.Split('-');
                if (parts.Length == 2)
                {
                    if (int.TryParse(parts[0], out int minVal)) minDistance = minVal;
                    if (int.TryParse(parts[1], out int maxVal)) maxDistance = maxVal;
                }
            }

            if (minDistance.HasValue)
                query = query.Where(j => j.DistanceKm >= minDistance.Value);

            if (maxDistance.HasValue)
                query = query.Where(j => j.DistanceKm <= maxDistance.Value);
        }

        return query.ToList();
    }

    public async Task AddJobAsync(Job job)
    {
        await _jobRepo.AddAsync(job);
        await _jobRepo.SaveChangesAsync();
    }

    public async Task<bool> AcceptJobAsync(int jobId, int driverId)
    {
        var job = await _jobRepo.GetFirstAsync(
            j => j.Id == jobId,
            j => j.Interests
        );

        if (job == null || job.IsAccepted)
            return false;

        var hasRequested = job.Interests.Any(i => i.DriverId == driverId);
        if (!hasRequested)
            return false;

        job.IsAccepted = true;
        job.AcceptedByDriverId = driverId;

        _jobRepo.Update(job);
        await _jobRepo.SaveChangesAsync();

        return true;
    }

    public async Task<List<Job>> GetJobsByDriverAsync(int driverId)
    {
        return await _jobRepo.GetAsync(j => j.AcceptedByDriverId == driverId);
    }

    public async Task<List<JobInterest>> GetInterestsForPosterAsync(int posterId)
    {
        return await _jobInterestRepo.GetAsync(
            i => i.Job.CreatedByUserId == posterId,
            null,
            i => i.Driver, i => i.Job
        );
    }

    public async Task<bool> RequestJobAsync(int jobId, int driverId, string mobileNumber)
    {
        var job = await _jobRepo.GetFirstAsync(
            j => j.Id == jobId,
            j => j.Interests
        );

        if (job == null || job.IsAccepted)
            return false;

        bool alreadyRequested = job.Interests.Any(i => i.DriverId == driverId);
        if (alreadyRequested)
            return false;

        var interest = new JobInterest
        {
            JobId = jobId,
            DriverId = driverId,
            MobileNumber = mobileNumber,
            RequestedAt = DateTime.UtcNow,
            IsAccepted = false
        };

        await _jobInterestRepo.AddAsync(interest);
        await _jobInterestRepo.SaveChangesAsync();

        return true;
    }

    public async Task<List<Job>> GetJobsDriverHasRequestedAsync(int driverId)
    {
        var interests = await _jobInterestRepo.GetAsync(
            i => i.DriverId == driverId,
            includes: i => i.Job
        );

        return interests.Select(i => i.Job).Distinct().ToList();
    }

    public async Task<List<JobInterest>> GetJobInterestsByDriverAsync(int driverId)
    {
        return await _jobInterestRepo.GetAsync(
            i => i.DriverId == driverId,
            includes: i => i.Job
        );
    }

    public async Task<List<Job>> GetJobsWithRequestsForPosterAsync(int posterId)
    {
        return await _jobRepo.GetAsync(
            j => j.CreatedByUserId == posterId,
            orderBy: q => q.OrderByDescending(j => j.CreatedAt),
            j => j.Interests,
            j => j.Interests.Select(i => i.Driver)
        );
    }

    public async Task<bool> UpdateJobAsync(int jobId, Job updatedJob, int posterId)
    {
        var existingJob = await _jobRepo.GetFirstAsync(
            j => j.Id == jobId && j.CreatedByUserId == posterId
        );

        if (existingJob == null || existingJob.IsAccepted || existingJob.IsCompleted)
            return false;

        existingJob.LoadItem = updatedJob.LoadItem;
        existingJob.StartLocation = updatedJob.StartLocation;
        existingJob.Destination = updatedJob.Destination;
        existingJob.Earnings = updatedJob.Earnings;
        existingJob.DistanceKm = updatedJob.DistanceKm;
        existingJob.MapUrl = updatedJob.MapUrl;

        _jobRepo.Update(existingJob);
        await _jobRepo.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteJobAsync(int jobId, int posterId)
    {
        var job = await _jobRepo.GetFirstAsync(
            j => j.Id == jobId && j.CreatedByUserId == posterId
        );

        if (job == null || job.IsAccepted || job.IsCompleted)
            return false;

        _jobRepo.Delete(job);
        await _jobRepo.SaveChangesAsync();

        return true;
    }

    public async Task<bool> MarkJobAsCompletedAsync(int jobId, int posterId)
    {
        var job = await _jobRepo.GetFirstAsync(
            j => j.Id == jobId && j.CreatedByUserId == posterId
        );

        if (job == null || !job.IsAccepted || job.IsCompleted)
            return false;

        job.IsCompleted = true;

        _jobRepo.Update(job);
        await _jobRepo.SaveChangesAsync();

        return true;
    }
}
