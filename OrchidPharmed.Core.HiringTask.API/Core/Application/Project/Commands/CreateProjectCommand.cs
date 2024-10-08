using AutoMapper;
using MediatR;
using OrchidPharmed.Core.HiringTask.API.Repositories;

namespace OrchidPharmed.Core.HiringTask.API.Core.Application.Project.Commands
{
    public class CreateProjectCommand
    {
        public class Command : IRequest<Guid>
        {
            public string Title { get; set; }
            public string? Description { get; set; }
        }
        public class Handler : IRequestHandler<Command, Guid>
        {
            private readonly IUnitOfWork _uow;
            public Handler(IUnitOfWork uow)
            {
                _uow = uow;
            }
            public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
            {
                var newproject = await _uow.ApplicationRepository.AddProjectAsync(new Domain.Project()
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Description = request.Description,
                });
                await _uow.CommitAsync();
                return newproject.Id;
            }
        }
    }
}
