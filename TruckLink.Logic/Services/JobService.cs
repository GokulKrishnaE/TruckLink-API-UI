using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TruckLink.Core.Entities;
using TruckLink.Core.Interfaces;
using TruckLink.Core.models;
using TruckLink.Infrastructure.Data;

namespace TruckLink.Logic.Services
{
    public class JobService : IJobService
    {
        private readonly TruckLinkDbContext _context;

        public JobService(TruckLinkDbContext context)
        {
            _context = context;
        }

        public async Task<List<Job>> GetAvailableJobsAsync(JobFilter filter)
        {
            var query = _context.Jobs
                .Include(j => j.Interests)
                .Where(j => !j.IsAccepted && !j.IsCompleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var searchTerm = filter.Search.ToLower();

                query = query.Where(j =>
                    j.LoadItem.ToLower().Contains(searchTerm) ||
                    j.Description.ToLower().Contains(searchTerm) ||
                    j.StartLocation.ToLower().Contains(searchTerm) ||
                    j.Destination.ToLower().Contains(searchTerm) ||
                    j.DistanceKm.ToString().Contains(searchTerm) ||   // if numeric fields
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

            return await query
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task AddJobAsync(Job job)
        {
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AcceptJobAsync(Guid jobId, Guid driverId)
        {
            var job = await _context.Jobs
                .Include(j => j.Interests)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null || job.IsAccepted)
                return false;

            var hasRequested = job.Interests.Any(i => i.DriverId == driverId);
            if (!hasRequested)
                return false;

            job.IsAccepted = true;
            job.AcceptedByDriverId = driverId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Job>> GetJobsByDriverAsync(Guid driverId)
        {
            return await _context.Jobs
                .Where(j => j.AcceptedByDriverId == driverId)
                .ToListAsync();
        }

        public async Task<List<JobInterest>> GetInterestsForPosterAsync(Guid posterId)
        {
            return await _context.JobInterests
                .Include(i => i.Driver)
                .Include(i => i.Job)
                .Where(i => i.Job.CreatedByUserId == posterId)
                .ToListAsync();
        }

        public async Task<bool> RequestJobAsync(Guid jobId, Guid driverId, string mobileNumber)
        {
            var job = await _context.Jobs
                .Include(j => j.Interests)
                .FirstOrDefaultAsync(j => j.Id == jobId);

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

            _context.JobInterests.Add(interest);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Job>> GetJobsDriverHasRequestedAsync(Guid driverId)
        {
            return await _context.JobInterests
                .Include(i => i.Job)
                .Where(i => i.DriverId == driverId)
                .Select(i => i.Job)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<JobInterest>> GetJobInterestsByDriverAsync(Guid driverId)
        {
            return await _context.JobInterests
                .Include(i => i.Job)
                .Where(i => i.DriverId == driverId)
                .ToListAsync();
        }

        public async Task<List<Job>> GetJobsWithRequestsForPosterAsync(Guid posterId)
        {
            return await _context.Jobs
                .Where(j => j.CreatedByUserId == posterId)
                .Include(j => j.Interests)
                    .ThenInclude(i => i.Driver)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateJobAsync(Guid jobId, Job updatedJob, Guid posterId)
        {
            var existingJob = await _context.Jobs
                .FirstOrDefaultAsync(j => j.Id == jobId && j.CreatedByUserId == posterId);

            if (existingJob == null || existingJob.IsAccepted || existingJob.IsCompleted)
                return false;

            existingJob.LoadItem = updatedJob.LoadItem;
            existingJob.StartLocation = updatedJob.StartLocation;
            existingJob.Destination = updatedJob.Destination;
            existingJob.Earnings = updatedJob.Earnings;
            existingJob.DistanceKm = updatedJob.DistanceKm;
            existingJob.MapUrl = updatedJob.MapUrl;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteJobAsync(Guid jobId, Guid posterId)
        {
            var job = await _context.Jobs
                .FirstOrDefaultAsync(j => j.Id == jobId && j.CreatedByUserId == posterId);

            if (job == null || job.IsAccepted || job.IsCompleted)
                return false;

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkJobAsCompletedAsync(Guid jobId, Guid posterId)
        {
            var job = await _context.Jobs
                .FirstOrDefaultAsync(j => j.Id == jobId && j.CreatedByUserId == posterId);

            if (job == null || !job.IsAccepted || job.IsCompleted)
                return false;

            job.IsCompleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
