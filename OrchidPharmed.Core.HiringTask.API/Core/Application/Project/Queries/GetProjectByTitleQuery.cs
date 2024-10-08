using AutoMapper;
using MediatR;
using OrchidPharmed.Core.HiringTask.API.Core.Domain;
using OrchidPharmed.Core.HiringTask.API.Repositories;
using OrchidPharmed.Core.HiringTask.API.Structure.DTO;
using System.Linq;

namespace OrchidPharmed.Core.HiringTask.API.Core.Application.Project.Queries
{
    public class GetProjectByTitleQuery
    {
        public class Query : IRequest<ProjectDTO[]>
        {
            public string Title { get; set; }

        }
        public class Handler : IRequestHandler<Query, ProjectDTO[]>
        {
            private readonly IUnitOfWork _uow;
            private readonly IMapper _mapper;
            public Handler(IUnitOfWork uow, IMapper mapper)
            {
                _uow = uow;
                _mapper = mapper;
            }
            public async Task<ProjectDTO[]> Handle(Query request, CancellationToken cancellationToken)
            {
                var projects = await _uow.ApplicationRepository.FindProjectsByTitleAsync(request.Title);
                await System.Threading.Tasks.Task.CompletedTask;
                return _mapper.Map<ProjectDTO[]>(projects); ;
            }
        }
    }
}
