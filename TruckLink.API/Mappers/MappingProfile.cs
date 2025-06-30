using AutoMapper;
using TruckLink.API.DTOs;
using TruckLink.Core.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Register -> User
        CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        // Login -> User
        CreateMap<LoginDto, User>();

        // JobDto <-> Job
        CreateMap<JobDto, Job>()
            .ForMember(dest => dest.IsAccepted, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted));

        CreateMap<Job, JobDto>()
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted));

        // JobInterest -> JobInterestDto (with driver's name)
        CreateMap<JobInterest, JobInterestDto>()
            .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.Driver.Name));

        // Job <-> JobDetailsDto (optional extended info)
        CreateMap<Job, JobDetailsDto>()
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted));
        CreateMap<JobDetailsDto, Job>()
            .ForMember(dest => dest.Interests, opt => opt.Ignore());

        // Enriched JobWithRequestsDto
        CreateMap<Job, JobWithRequestsDto>()
            .ForMember(dest => dest.JobId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted))
            .ForMember(dest => dest.Interests, opt => opt.MapFrom(src => src.Interests));
    }
}
