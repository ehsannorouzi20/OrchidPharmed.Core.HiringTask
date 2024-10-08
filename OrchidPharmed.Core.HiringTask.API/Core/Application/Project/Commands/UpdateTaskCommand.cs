using AutoMapper;
using MediatR;
using OrchidPharmed.Core.HiringTask.API.Core.Domain;
using OrchidPharmed.Core.HiringTask.API.Repositories;

namespace OrchidPharmed.Core.HiringTask.API.Core.Application.Project.Commands
{
    public class UpdateTaskCommand
    {
        public class Command : IRequest
        {
            public Guid ProjectId { get; set; }
            public Guid TaskId { get; set; }
            public string Status { get; set; }
        }
        public class Handler : IRequestHandler<Command>
        {
            private readonly IUnitOfWork _uow;
            private readonly IMapper _mapper;
            public Handler(IUnitOfWork uow, IMapper mapper)
            {
                _uow = uow;
                _mapper = mapper;
            }
            public async System.Threading.Tasks.Task Handle(Command request, CancellationToken cancellationToken)
            {
                var project = await _uow.ApplicationRepository.GetProjectByIdAsync(request.ProjectId);
                if (project == null)
                {
                    await System.Threading.Tasks.Task.CompletedTask;
                    throw new ArgumentException("Project Not found!");
                }
                project.UpdateTaskStatus(request.TaskId, request.Status);
                await _uow.ApplicationRepository.UpdateProjectAsync(project);
                await _uow.CommitAsync();
            }
        }
    }
}
