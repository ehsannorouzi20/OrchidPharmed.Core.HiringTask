using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrchidPharmed.Core.HiringTask.API.Models;

namespace OrchidPharmed.Core.HiringTask.API.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _Mapper;

        public ApplicationRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _Mapper = mapper;
        }

        public async Task<Core.Domain.Project> AddProjectAsync(Core.Domain.Project project)
        {
            var projectentity = _Mapper.Map<ProjectEntity>(project);
            projectentity.CDT = DateTime.Now;
            var result = await _dbContext.Projects.AddAsync(projectentity);
            return _Mapper.Map<Core.Domain.Project>(result.Entity);
        }

        public async Task<IEnumerable<Core.Domain.Project>> GetAllProjectsAsync()
        {
            var result = await _dbContext.Projects.Include(p => p.Tasks).ToArrayAsync();
            return _Mapper.Map<IEnumerable<Core.Domain.Project>>(result);
        }

        public async Task<IEnumerable<Core.Domain.Project>> FindProjectsByTitleAsync(string title)
        {
            var result = await _dbContext.Projects.Where(e => e.Title.Contains(title))
                .Include(p => p.Tasks).ToArrayAsync();
            return _Mapper.Map<IEnumerable<Core.Domain.Project>>(result);
        }


        public async Task<Core.Domain.Project?> GetProjectByIdAsync(Guid id)
        {
            var result = await _dbContext.Projects.Where(e => e.Id == id).Include(p => p.Tasks).AsNoTracking().SingleOrDefaultAsync();
            return _Mapper.Map<Core.Domain.Project>(result);
        }
        public async Task DeleteProjectAsync(Guid id)
        {
            var project = await _dbContext.Projects.FindAsync(id);
            if (project != null)
            {
                _dbContext.Projects.Remove(project);
                await Task.CompletedTask;
            }
        }
        public async Task UpdateProjectAsync(Core.Domain.Project project)
        {
            var projectEntity = _Mapper.Map<ProjectEntity>(project);

            var dbtasks = await _dbContext.Tasks.Where(e => e.ProjectId == projectEntity.Id).AsNoTracking().ToArrayAsync();
            foreach (var task in dbtasks)
            {
                if (!projectEntity.Tasks.Any(e => e.Id == task.Id))
                    _dbContext.Tasks.Remove(task);
            }

            foreach (var task in projectEntity.Tasks)
            {
                if (task.Id == Guid.Empty)
                {
                    task.Id = Guid.NewGuid();
                    task.CDT = DateTime.Now;
                    _dbContext.Entry(task).State = EntityState.Added;
                }
                else
                {
                    var t = dbtasks.Single(t => t.Id == task.Id);
                    task.MDT = DateTime.Now;
                    task.CDT = t.CDT;
                    _dbContext.Entry(task).State = EntityState.Modified;
                }
            }
            projectEntity.MDT = DateTime.Now;
            _dbContext.Projects.Update(projectEntity);
            await Task.CompletedTask;
        }
    }
}
