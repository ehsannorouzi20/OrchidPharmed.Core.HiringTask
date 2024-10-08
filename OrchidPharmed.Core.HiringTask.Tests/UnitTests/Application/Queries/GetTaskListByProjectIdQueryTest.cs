using AutoMapper;
using Moq;

namespace OrchidPharmed.Core.HiringTask.Tests.UnitTests.Application.Query
{
    public class GetTaskListByProjectIdQueryTest
    {
        private readonly Mock<API.Repositories.IUnitOfWork> _mockUOW;
        private readonly Mock<IMapper> _mockMapper;
        private readonly API.Core.Application.Project.Queries.GetTaskListByProjectIdQuery.Handler _handler;
        public GetTaskListByProjectIdQueryTest()
        {
            _mockMapper = new Mock<IMapper>();
            _mockUOW = new Mock<API.Repositories.IUnitOfWork>();
            _handler = new API.Core.Application.Project.Queries.GetTaskListByProjectIdQuery.Handler(_mockUOW.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ReturnsListOfProjectTaskByProjectId()
        {
            var pid = Guid.NewGuid();
            var tid1 = Guid.NewGuid();
            var tid2 = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var project = new API.Core.Domain.Project()
            {
                Id = pid,
                Title = "TestProject1_Title",
                Description = "TestProject1_Description",
                Tasks = new List<API.Core.Domain.Task>
                    {
                        new API.Core.Domain.Task()
                        {
                            Id = tid1,
                            Title = "Task1_Title",
                            Description = "Task1_Description",
                            DueDate = now,
                            Status = API.Core.Domain.TaskStatus.ToDo
                        },
                         new API.Core.Domain.Task()
                        {
                            Id = tid2,
                            Title = "Task2_Title",
                            Description = "Task2_Description",
                            DueDate = now,
                            Status = API.Core.Domain.TaskStatus.InProgress
                        }
                    }
            };
            var projectDto = new API.Structure.DTO.ProjectDTO()
            {
                Id = pid,
                Title = "TestProject1_Title",
                Description = "TestProject1_Description",
                Tasks = new API.Structure.DTO.TaskDTO[]
                    {
                        new API.Structure.DTO.TaskDTO()
                        {
                            Id = tid1,
                            Title = "Task1_Title",
                            Description = "Task1_Description",
                            DueDate = now,
                            Status = API.Core.Domain.TaskStatus.ToDo
                        },
                         new API.Structure.DTO.TaskDTO()
                        {
                            Id = tid2,
                            Title = "Task2_Title",
                            Description = "Task2_Description",
                            DueDate = now,
                            Status = API.Core.Domain.TaskStatus.InProgress
                        }
                    }
            };
            _mockUOW.Setup(uow => uow.ApplicationRepository.GetProjectByIdAsync(pid)).ReturnsAsync(project);
            _mockMapper.Setup(mapper => mapper.Map<API.Structure.DTO.TaskDTO[]>(project.Tasks)).Returns(projectDto.Tasks);
            var result = await _handler.Handle(new API.Core.Application.Project.Queries.GetTaskListByProjectIdQuery.Query() { ProjectId = pid }, CancellationToken.None);
            Assert.Equal(projectDto.Tasks, result);
        }
    }
}
