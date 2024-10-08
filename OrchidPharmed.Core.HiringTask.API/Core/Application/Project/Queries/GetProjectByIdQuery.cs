using AutoMapper;
using MediatR;
using OrchidPharmed.Core.HiringTask.API.Repositories;
using OrchidPharmed.Core.HiringTask.API.Structure;
using OrchidPharmed.Core.HiringTask.API.Structure.DTO;

namespace OrchidPharmed.Core.HiringTask.API.Core.Application.Project.Queries
{
    public class GetProjectByIdQuery
    {
        public class Query : IRequest<ProjectDTO?>
        {
            public Guid Id { get; set; }

        }
        public class Handler : IRequestHandler<Query, ProjectDTO?>
        {
            private readonly IUnitOfWork _uow;
            private readonly IMapper _mapper;
            private readonly ICacheManager _cacheManager;
            public Handler(IUnitOfWork uow, IMapper mapper, Structure.ICacheManager cacheManager)
            {
                _uow = uow;
                _mapper = mapper;
                _cacheManager = cacheManager;
            }
            public async Task<ProjectDTO?> Handle(Query request, CancellationToken cancellationToken)
            {
                var projects = await _cacheManager.GetCacheAsync("ProjectList", _uow.ApplicationRepository.GetAllProjectsAsync);
                //var project = await _uow.ApplicationRepository.GetProjectByIdAsync(request.Id);
                var project = projects.SingleOrDefault(e => e.Id == request.Id);
                if (project == null)
                {
                    await Task.CompletedTask;
                    return null;
                }
                await Task.CompletedTask;
                return _mapper.Map<ProjectDTO>(project);
            }
        }
    }
}
