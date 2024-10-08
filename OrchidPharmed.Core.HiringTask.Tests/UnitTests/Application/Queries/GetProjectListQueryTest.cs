using AutoMapper;
using Moq;
using OrchidPharmed.Core.HiringTask.API.Core.Domain;
using System.Security.Cryptography;

namespace OrchidPharmed.Core.HiringTask.Tests.UnitTests.Application.Queries
{
    public class GetProjectListQueryTest
    {
        private readonly Mock<API.Repositories.IUnitOfWork> _mockUOW;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<API.Structure.ICacheManager> _mockCacheManager;
        private readonly Mock<Microsoft.Extensions.Logging.ILogger<API.Core.Application.Project.Queries.GetProjectListQuery.Handler>> _mockLogger;
        private readonly API.Core.Application.Project.Queries.GetProjectListQuery.Handler _handler;

        private Guid _Pid;
        private Guid _Tid;
        private DateTime _Now;
        public GetProjectListQueryTest()
        {
            _mockMapper = new Mock<IMapper>();
            _mockUOW = new Mock<API.Repositories.IUnitOfWork>();
            _mockCacheManager = new Mock<API.Structure.ICacheManager>();
            _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<API.Core.Application.Project.Queries.GetProjectListQuery.Handler>>();
            _handler = new API.Core.Application.Project.Queries.GetProjectListQuery.Handler(_mockUOW.Object, _mockMapper.Object, _mockLogger.Object, _mockCacheManager.Object);
        }

        [Fact]
        public async System.Threading.Tasks.Task Handle_ReturnsListOfProjectDTOs()
        {
            _Pid = Guid.NewGuid();
            _Tid = Guid.NewGuid();
            _Now = DateTime.UtcNow;
            var projects = new API.Core.Domain.Project[]
            {
                new API.Core.Domain.Project(){
                    Id = _Pid,
                    Title = "TestProject1_Title",
                    Description = "TestProject1_Description",
                    Tasks = new List<API.Core.Domain.Task>
                    {
                        new API.Core.Domain.Task()
                        {
                            Id = _Tid,
                            Title = "Task1_Title",
                            Description = "Task1_Description",
                            DueDate = _Now,
                            Status = API.Core.Domain.TaskStatus.ToDo
                        }
                    }
                }
            };
            var projectsDto = new API.Structure.DTO.ProjectDTO[]
            {
                new API.Structure.DTO.ProjectDTO()
                {
                    Id = _Pid,
                    Title = "TestProject1_Title",
                    Description = "TestProject1_Description",
                    Tasks = new API.Structure.DTO.TaskDTO[]
                    {
                        new API.Structure.DTO.TaskDTO()
                        {
                            Id = _Tid,
                            Title = "Task1_Title",
                            Description = "Task1_Description",
                            DueDate = _Now,
                            Status = API.Core.Domain.TaskStatus.ToDo
                        }
                    }
                }
            };

            _mockUOW.Setup(uow => uow.ApplicationRepository.GetAllProjectsAsync()).ReturnsAsync(projects);
            Mock<Func<Task<IEnumerable<API.Core.Domain.Project>>>> mockfunction = new Mock<Func<Task<IEnumerable<Project>>>>();
            mockfunction.Setup(mf => mf()).ReturnsAsync(projects);
            _mockCacheManager.Setup(cm => cm.GetCacheAsync(It.IsAny<string>(), mockfunction.Object)).ReturnsAsync(projects);
            _mockMapper.Setup(mapper => mapper.Map<API.Structure.DTO.ProjectDTO[]>(projects)).Returns(projectsDto);
            var result = await _handler.Handle(new API.Core.Application.Project.Queries.GetProjectListQuery.Query(), CancellationToken.None);
            Assert.Equal(projectsDto, result);
        }
    }
}
