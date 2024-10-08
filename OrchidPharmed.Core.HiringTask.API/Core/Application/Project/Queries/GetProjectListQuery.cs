using AutoMapper;
using MediatR;
using OrchidPharmed.Core.HiringTask.API.Repositories;
using OrchidPharmed.Core.HiringTask.API.Structure;
using OrchidPharmed.Core.HiringTask.API.Structure.DTO;

namespace OrchidPharmed.Core.HiringTask.API.Core.Application.Project.Queries
{
    public class GetProjectListQuery
    {
        public class Query : IRequest<ProjectDTO[]>
        {
        }
        public class Handler : IRequestHandler<Query, ProjectDTO[]>
        {
            private readonly IUnitOfWork _uow;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _Logger;
            private readonly ICacheManager _cacheManager;
            public Handler(IUnitOfWork uow, IMapper mapper, ILogger<Handler> logger, Structure.ICacheManager cacheManager)
            {
                _uow = uow;
                _mapper = mapper;
                _Logger = logger;
                _cacheManager = cacheManager;
            }
            public async Task<ProjectDTO[]> Handle(Query request, CancellationToken cancellationToken)
            {
                var projects = await _cacheManager.GetCacheAsync("ProjectList", _uow.ApplicationRepository.GetAllProjectsAsync);
                // var projects = await _uow.ApplicationRepository.GetAllProjectsAsync();
                await Task.CompletedTask;
                _Logger.LogInformation("GetProjectListQuery By Ehsan");
                return _mapper.Map<ProjectDTO[]>(projects);
            }
        }
    }
}
