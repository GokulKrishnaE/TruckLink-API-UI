using System.Net;
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


        return Ok(ApiResponse<object>.Success(new { jobs = jobDtos }, "Jobs fetched successfully", (int)HttpStatusCode.OK));
    }

    [Authorize(Roles = "Poster")]
    [HttpPost]
    public async Task<IActionResult> AddJob([FromBody] JobDto jobDto)
    {
        if (jobDto == null)
            return BadRequest(ApiResponse<object>.Error("Job cannot be null", (int)HttpStatusCode.BadRequest));

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var posterId))
            return Unauthorized(ApiResponse<object>.Error("You are unauthorized", (int)HttpStatusCode.Unauthorized));

        var job = _mapper.Map<Job>(jobDto);
        job.CreatedByUserId = posterId;

        await _jobService.AddJobAsync(job);
        return CreatedAtAction(nameof(GetAvailableJobs), new { id = job.Id }, ApiResponse<object>.Success(job,"Job has been successfully created",(int) HttpStatusCode.Created));
    }

    [Authorize(Roles = "Driver")]
    [HttpPost("request/{jobId}")]
    public async Task<IActionResult> RequestJob(Guid jobId, [FromQuery] string mobileNumber)
    {
        var driverId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _jobService.RequestJobAsync(jobId, driverId, mobileNumber);

        if (!result)
            return BadRequest(ApiResponse<string>.Error("Already requested or job not available.", (int)HttpStatusCode.BadRequest));

        return Ok(ApiResponse<string>.Success("Interest sent.", "Success", (int)HttpStatusCode.OK));
    }

    [Authorize(Roles = "Poster")]
    [HttpPost("accept-request")]
    public async Task<IActionResult> AcceptRequest([FromBody] AcceptJobRequestDto requestDto)
    {
        var success = await _jobService.AcceptJobAsync(requestDto.JobId, requestDto.DriverId);
        if (!success)
            return BadRequest(ApiResponse<string>.Error("Unable to accept request. Possibly already accepted.",(int)HttpStatusCode.BadRequest));
        return Ok(ApiResponse<string>.Success(null, "Job accepted successfully", (int)HttpStatusCode.OK));
    }

    [Authorize(Roles = "Poster")]
    [HttpGet("requests/{jobId}")]
    public async Task<IActionResult> GetJobRequests(Guid jobId)
    {
        var interests = await _jobService.GetInterestsForPosterAsync(jobId);
        return Ok(ApiResponse<object>.Success(interests,"Interests fetched successfully",(int)HttpStatusCode.OK));
    }

    [Authorize(Roles = "Driver")]
    [HttpGet("driver/{driverId}")]
    public async Task<IActionResult> GetJobsForDriver(Guid driverId)
    {
        var jobs = await _jobService.GetJobsByDriverAsync(driverId);
        return Ok(ApiResponse<object>.Success(jobs, "Jobs fetched successfully", (int)HttpStatusCode.OK));
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

        return Ok(ApiResponse<object>.Success(new { jobs = interestedJobDtos }, "Jobs you've shown interest in.", (int)HttpStatusCode.OK));
    }

    [Authorize(Roles = "Poster")]
    [HttpGet("with-requests")]
    public async Task<IActionResult> GetJobsWithRequestsForPoster()
    {
        var posterId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var jobs = await _jobService.GetJobsWithRequestsForPosterAsync(posterId);
        var jobDtos = _mapper.Map<List<JobWithRequestsDto>>(jobs);

        return Ok(ApiResponse<object>.Success(new { jobs = jobDtos }, "Jobs with driver requests fetched successfully", (int)HttpStatusCode.OK));
    }

    [Authorize(Roles = "Poster")]
    [HttpPut("{jobId}")]
    public async Task<IActionResult> UpdateJob(Guid jobId, [FromBody] JobDto jobDto)
    {
        var posterId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var updatedJob = _mapper.Map<Job>(jobDto);

        var success = await _jobService.UpdateJobAsync(jobId, updatedJob, posterId);
        if (!success)
            return BadRequest(ApiResponse<string>.Error("You can only update your own unaccepted/non-completed jobs.",(int)HttpStatusCode.BadRequest));

        return Ok(ApiResponse<string>.Success(null, "Job updated successfully", (int)HttpStatusCode.OK));
    }

    [Authorize(Roles = "Poster")]
    [HttpDelete("{jobId}")]
    public async Task<IActionResult> DeleteJob(Guid jobId)
    {
        var posterId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var success = await _jobService.DeleteJobAsync(jobId, posterId);
        if (!success)
            return BadRequest(ApiResponse<string>.Error("You can only delete your own unaccepted jobs.",(int)HttpStatusCode.BadRequest));

        return Ok(ApiResponse<string>.Success(null, "Job deleted successfully", (int)HttpStatusCode.OK));
    }

    [Authorize(Roles = "Poster")]
    [HttpPost("{jobId}/complete")]
    public async Task<IActionResult> MarkJobAsCompleted(Guid jobId)
    {
        var posterId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var success = await _jobService.MarkJobAsCompletedAsync(jobId, posterId);
        if (!success)
            return BadRequest(ApiResponse<string>.Error("Job not found, not accepted yet, or already completed.",(int)HttpStatusCode.BadRequest));

        return Ok(ApiResponse<string>.Success(null, "Job marked as completed", (int)HttpStatusCode.OK));
    }
}
