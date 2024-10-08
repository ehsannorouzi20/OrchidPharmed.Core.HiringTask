using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OrchidPharmed.Core.HiringTask.API.Core.Application.Project.Commands;
using OrchidPharmed.Core.HiringTask.API.Core.Application.Project.Queries;
using OrchidPharmed.Core.HiringTask.API.Structure.DTO;

namespace OrchidPharmed.Core.HiringTask.API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    [ApiVersion("2")]
    [ServiceFilter(typeof(Filters.ExceptionFilter))]
    public class ProjectController : Controller
    {
        private readonly IMediator _mediator;
        private readonly FluentValidation.IValidator<CreateProjectDTO> _createProjectValidator;
        private readonly FluentValidation.IValidator<CreateTaskDTO> _createTaskValidator;
        public ProjectController(IMediator mediator, FluentValidation.IValidator<CreateProjectDTO> pvalidator, FluentValidation.IValidator<CreateTaskDTO> tvalidator)
        {
            _mediator = mediator;
            _createProjectValidator = pvalidator;
            _createTaskValidator = tvalidator;
        }

        [HttpGet,MapToApiVersion("1")]
        public async Task<ProjectDTO[]> GetAllProjects()
        {
            return await _mediator.Send(
                new GetProjectListQuery.Query
                {
                }
            );
        }

        [HttpGet("{id}"),MapToApiVersion("2")]
        public async Task<ProjectDTO?> GetProjectById(Guid id)
        {
            return await _mediator.Send(
                new GetProjectByIdQuery.Query
                {
                    Id = id
                }
            );
        }

        [HttpGet("title/{title}")]
        public async Task<ProjectDTO[]> GetProjectByTitle(string title)
        {
            return await _mediator.Send(
                new GetProjectByTitleQuery.Query
                {
                    Title = title
                }
            );
        }

        [HttpPost]
        public async Task<Guid> CreateProject([FromBody] CreateProjectDTO model)
        {
            var validationresult = await _createProjectValidator.ValidateAsync(model);
            if (!validationresult.IsValid)
                throw new Exception(Newtonsoft.Json.JsonConvert.SerializeObject(new { Error = validationresult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) }));

            return await _mediator.Send(
                new CreateProjectCommand.Command
                {
                    Title = model.Title,
                    Description = model.Description,
                }
            );
        }

        [HttpDelete]
        public async System.Threading.Tasks.Task DeleteProject(Guid id)
        {
            await _mediator.Send(
                new DeleteProjectCommand.Command
                {
                    Id = id
                }
            );
        }

        [HttpPost("{projectId}/tasks")]
        public async System.Threading.Tasks.Task AddTaskToProject(Guid projectId, [FromBody] CreateTaskDTO model)
        {
            var validationresult = await _createTaskValidator.ValidateAsync(model);
            if (!validationresult.IsValid)
                throw new Exception(Newtonsoft.Json.JsonConvert.SerializeObject(new { Error = validationresult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) }));

            await _mediator.Send(
               new CreateTaskCommand.Command
               {
                   ProjectId = projectId,
                   Title = model.Title,
                   Description = model.Description,
                   DueDate = model.DueDate
               }
           );
        }

        [HttpDelete("{projectId}/tasks/{taskId}")]
        public async System.Threading.Tasks.Task DeleteTaskFromProject(Guid projectId, Guid taskId)
        {
            await _mediator.Send(
                new DeleteTaskCommand.Command
                {
                    ProjectId = projectId,
                    TaskId = taskId
                }
            );
        }

        [HttpGet("{projectId}/tasks")]
        public async Task<TaskDTO[]> GetAllTasksByProjectId(Guid projectId)
        {
            return await _mediator.Send(
                new GetTaskListByProjectIdQuery.Query
                {
                    ProjectId = projectId
                }
            );
        }

        [HttpPut("{projectId}/tasks/{taskId}")]
        public async System.Threading.Tasks.Task UpdateTaskInProject(Guid projectId, Guid taskId, [FromBody] UpdateTaskDTO model)
        {
            await _mediator.Send(
                new UpdateTaskCommand.Command
                {
                    ProjectId = projectId,
                    TaskId = taskId,
                    Status = model.Status
                }
            );
        }
    }
}
