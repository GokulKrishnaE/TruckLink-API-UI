using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TruckLink.API.DTOs;
using TruckLink.Core.Entities;
using TruckLink.Core.Interfaces;
using TruckLink.Core.models;

namespace TruckLink.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;
    private readonly IMapper _mapper;

    public JobsController(IJobService jobService, IMapper mapper)
    {
        _jobService = jobService;
        _mapper = mapper;
    }
    [HttpGet]
    public async Task<IActionResult> GetAvailableJobs([FromQuery] JobFilterDto dto)
    {
        var filter = new JobFilter
        {
            Search = dto.Search,
            Distance = dto.Distance,
            StartPlace = dto.StartPlace,
            EndPlace = dto.EndPlace
        };

        var jobs = await _jobService.GetAvailableJobsAsync(filter);
        var jobDtos = _mapper.Map<List<JobDto>>(jobs);

        return Ok(new ApiResponse<object>
        {
            IsSuccess = true,
            Message = "Jobs fetched successfully",
            Code = 200,
            Data = new { jobs = jobDtos }
        });
    }

    [Authorize(Roles = "Poster")]
    [HttpPost]
    public async Task<IActionResult> AddJob([FromBody] JobDto jobDto)
    {
        if (jobDto == null)
            return BadRequest("Job cannot be null.");

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var posterId))
            return Unauthorized();

        var job = _mapper.Map<Job>(jobDto);
        job.CreatedByUserId = posterId;

        await _jobService.AddJobAsync(job);
        return CreatedAtAction(nameof(GetAvailableJobs), new { id = job.Id }, job);
    }

    [Authorize(Roles = "Driver")]
    [HttpPost("request/{jobId}")]
    public async Task<IActionResult> RequestJob(Guid jobId, [FromQuery] string mobileNumber)
    {
        var driverId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _jobService.RequestJobAsync(jobId, driverId, mobileNumber);

        if (!result)
            return BadRequest("Already requested or job not available.");

        return Ok("Interest sent.");
    }

    [Authorize(Roles = "Poster")]
    [HttpPost("accept-request")]
    public async Task<IActionResult> AcceptRequest([FromBody] AcceptJobRequestDto requestDto)
    {
        var success = await _jobService.AcceptJobAsync(requestDto.JobId, requestDto.DriverId);
        if (!success)
            return BadRequest("Unable to accept request. Possibly already accepted.");

        return Ok("Job accepted successfully.");
    }

    [Authorize(Roles = "Poster")]
    [HttpGet("requests/{jobId}")]
    public async Task<IActionResult> GetJobRequests(Guid jobId)
    {
        var interests = await _jobService.GetInterestsForPosterAsync(jobId);
        return Ok(interests);
    }

    [Authorize(Roles = "Driver")]
    [HttpGet("driver/{driverId}")]
    public async Task<IActionResult> GetJobsForDriver(Guid driverId)
    {
        var jobs = await _jobService.GetJobsByDriverAsync(driverId);
        return Ok(jobs);
    }

    [Authorize(Roles = "Driver")]
    [HttpGet("interested")]
    public async Task<IActionResult> GetJobsDriverIsInterestedIn()
    {
        var driverId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var interests = await _jobService.GetJobInterestsByDriverAsync(driverId);

        var interestedJobDtos = interests.Select(i => new InterestedJobDto
        {
            Job = _mapper.Map<JobDto>(i.Job),
            IsAcceptedForDriver = i.Job.IsAccepted && i.Job.AcceptedByDriverId == driverId
        }).ToList();

        return Ok(new ApiResponse<object>
        {
            IsSuccess = true,
            Message = "Jobs you've shown interest in.",
            Code = 200,
            Data = new { jobs = interestedJobDtos }
        });
    }

    [Authorize(Roles = "Poster")]
    [HttpGet("with-requests")]
    public async Task<IActionResult> GetJobsWithRequestsForPoster()
    {
        var posterId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var jobs = await _jobService.GetJobsWithRequestsForPosterAsync(posterId);
        var jobDtos = _mapper.Map<List<JobWithRequestsDto>>(jobs);

        return Ok(new ApiResponse<object>
        {
            IsSuccess = true,
            Message = "Jobs with driver requests fetched successfully",
            Code = 200,
            Data = new { jobs = jobDtos }
        });
    }

    [Authorize(Roles = "Poster")]
    [HttpPut("{jobId}")]
    public async Task<IActionResult> UpdateJob(Guid jobId, [FromBody] JobDto jobDto)
    {
        var posterId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var updatedJob = _mapper.Map<Job>(jobDto);

        var success = await _jobService.UpdateJobAsync(jobId, updatedJob, posterId);
        if (!success)
            return BadRequest("You can only update your own unaccepted/non-completed jobs.");

        return Ok("Job updated successfully.");
    }

    [Authorize(Roles = "Poster")]
    [HttpDelete("{jobId}")]
    public async Task<IActionResult> DeleteJob(Guid jobId)
    {
        var posterId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var success = await _jobService.DeleteJobAsync(jobId, posterId);
        if (!success)
            return BadRequest("You can only delete your own unaccepted jobs.");

        return Ok("Job deleted successfully.");
    }

    [Authorize(Roles = "Poster")]
    [HttpPost("{jobId}/complete")]
    public async Task<IActionResult> MarkJobAsCompleted(Guid jobId)
    {
        var posterId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var success = await _jobService.MarkJobAsCompletedAsync(jobId, posterId);
        if (!success)
            return BadRequest("Job not found, not accepted yet, or already completed.");

        return Ok("Job marked as completed.");
    }
}
