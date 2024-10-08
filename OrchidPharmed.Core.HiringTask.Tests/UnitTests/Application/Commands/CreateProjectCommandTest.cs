using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OrchidPharmed.Core.HiringTask.Tests.UnitTests.Application.Commands
{
    public class CreateProjectCommandTest
    {
        private readonly Mock<API.Repositories.IUnitOfWork> _mockUOW;
        private readonly API.Core.Application.Project.Commands.CreateProjectCommand.Handler _handler;
        public CreateProjectCommandTest()
        {
            _mockUOW = new Mock<API.Repositories.IUnitOfWork>();
            _handler = new API.Core.Application.Project.Commands.CreateProjectCommand.Handler(_mockUOW.Object);
        }
        [Fact]
        public async Task Handle_ReturnsIdOfCreatedProject()
        {
            Guid pid = Guid.NewGuid();
            var project = new API.Core.Domain.Project()
            {
                Id = pid,
                Title = "TestProject1_Title",
                Description = "TestProject1_Description"
            };

            _mockUOW.Setup(uow => uow.ApplicationRepository.AddProjectAsync(It.IsAny<API.Core.Domain.Project>())).ReturnsAsync(project);
            _mockUOW.Setup(uow => uow.CommitAsync()).Verifiable();


            var result = await _handler.Handle(new API.Core.Application.Project.Commands.CreateProjectCommand.Command()
            {
                Title = "TestProject1_Title",
                Description = "TestProject1_Description",
            }, CancellationToken.None);

            Assert.Equal(project.Id, result);
        }
    }
}
