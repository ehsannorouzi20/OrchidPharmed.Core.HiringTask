using AutoMapper;
using MediatR;
using OrchidPharmed.Core.HiringTask.API.Repositories;
using OrchidPharmed.Core.HiringTask.API.Structure.DTO;

namespace OrchidPharmed.Core.HiringTask.API.Core.Application.Project.Queries
{
    public class GetTaskListByProjectIdQuery
    {
        public class Query : IRequest<TaskDTO[]>
        {
            public Guid ProjectId { get; set; }

        }
        public class Handler : IRequestHandler<Query, TaskDTO[]>
        {
            private readonly IUnitOfWork _uow;
            private readonly IMapper _mapper;
            public Handler(IUnitOfWork uow, IMapper mapper)
            {
                _uow = uow;
                _mapper = mapper;
            }
            public async Task<TaskDTO[]> Handle(Query request, CancellationToken cancellationToken)
            {
                var project = await _uow.ApplicationRepository.GetProjectByIdAsync(request.ProjectId);
                if (project == null)
                {
                    await Task.CompletedTask;
                    throw new ArgumentException("Project Not found!");
                }
                return _mapper.Map<TaskDTO[]>(project.Tasks);
            }
        }
    }
}
