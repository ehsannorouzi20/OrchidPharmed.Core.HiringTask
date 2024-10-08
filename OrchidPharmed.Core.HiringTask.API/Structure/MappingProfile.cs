using AutoMapper;

namespace OrchidPharmed.Core.HiringTask.API.Structure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Core.Domain.Project, Models.ProjectEntity>()
                .ForMember(dest => dest.Tasks, opt => opt.MapFrom(src => src.Tasks))
                    .AfterMap((src, dest) =>
                    {
                        foreach (var task in dest.Tasks)
                            task.ProjectId = dest.Id;
                    })
                     .ReverseMap();

            CreateMap<Core.Domain.Task, Models.TaskEntity>()
                .ForMember(dest => dest.ProjectId, opt => opt.Ignore())
                   .ReverseMap();


            CreateMap<Core.Domain.Project, Structure.DTO.ProjectDTO>()
                .ForMember(dest => dest.Tasks, opt => opt.MapFrom(src => src.Tasks))
                    .AfterMap((src, dest) =>
                    {
                        dest.PrepareValue();
                        foreach (var task in dest.Tasks)
                            task.PrepareValue();
                    });

            CreateMap<Core.Domain.Task, Structure.DTO.TaskDTO>()
                   .AfterMap((src, dest) =>
                   {
                       dest.PrepareValue();
                   });
        }
    }
}
