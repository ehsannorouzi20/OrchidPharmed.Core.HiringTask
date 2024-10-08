namespace OrchidPharmed.Core.HiringTask.API.Repositories
{
    public interface IRedisRepositiry
    {
        Task<Core.Domain.Project> AddProjectAsync(Core.Domain.Project project);
        Task DeleteProjectAsync(Guid id);
        Task<IEnumerable<Core.Domain.Project>> FindProjectsByTitleAsync(string title);
        Task<IEnumerable<Core.Domain.Project>> GetAllProjectsAsync();
        Task<Core.Domain.Project?> GetProjectByIdAsync(Guid id);
        Task UpdateProjectAsync(Core.Domain.Project project);
    }
}
