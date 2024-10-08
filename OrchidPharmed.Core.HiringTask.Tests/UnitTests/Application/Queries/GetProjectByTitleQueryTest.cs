using AutoMapper;
using Moq;

namespace OrchidPharmed.Core.HiringTask.Tests.UnitTests.Application.Queries
{
    public class GetProjectByTitleQueryTest
    {
        private readonly Mock<API.Repositories.IUnitOfWork> _mockUOW;
        private readonly Mock<IMapper> _mockMapper;
        private readonly API.Core.Application.Project.Queries.GetProjectByTitleQuery.Handler _handler;
        public GetProjectByTitleQueryTest()
        {
            _mockMapper = new Mock<IMapper>();
            _mockUOW = new Mock<API.Repositories.IUnitOfWork>();
            _handler = new API.Core.Application.Project.Queries.GetProjectByTitleQuery.Handler(_mockUOW.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ReturnsListOfProjectsByProjectTitle()
        {
            var pid = Guid.NewGuid();
            var tid = Guid.NewGuid();
            var now = DateTime.UtcNow;
            string title = "TestProject1";
            var projects = new API.Core.Domain.Project[]
            {
                new API.Core.Domain.Project(){
                    Id = pid,
                    Title = "TestProject1_Title",
                    Description = "TestProject1_Description",
                    Tasks = new List<API.Core.Domain.Task>
                    {
                        new API.Core.Domain.Task()
                        {
                            Id = tid,
                            Title = "Task1_Title",
                            Description = "Task1_Description",
                            DueDate = now,
                            Status = API.Core.Domain.TaskStatus.ToDo
                        }
                    }
                }
            };
            var projectsDto = new API.Structure.DTO.ProjectDTO[]
            {
                new API.Structure.DTO.ProjectDTO()
                {
                    Id = pid,
                    Title = "TestProject1_Title",
                    Description = "TestProject1_Description",
                    Tasks = new API.Structure.DTO.TaskDTO[]
                    {
                        new API.Structure.DTO.TaskDTO()
                        {
                            Id = tid,
                            Title = "Task1_Title",
                            Description = "Task1_Description",
                            DueDate = now,
                            Status = API.Core.Domain.TaskStatus.ToDo
                        }
                    }
                }
            };

            _mockUOW.Setup(uow => uow.ApplicationRepository.FindProjectsByTitleAsync(title)).ReturnsAsync(projects);
            _mockMapper.Setup(mapper => mapper.Map<API.Structure.DTO.ProjectDTO[]>(projects)).Returns(projectsDto);
            var result = await _handler.Handle(new API.Core.Application.Project.Queries.GetProjectByTitleQuery.Query() { Title = title }, CancellationToken.None);
            Assert.Equal(projectsDto, result);
        }
    }
}
