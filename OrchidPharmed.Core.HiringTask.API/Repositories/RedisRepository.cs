using StackExchange.Redis;

namespace OrchidPharmed.Core.HiringTask.API.Repositories
{
    public class RedisRepository : IRedisRepositiry
    {
        private readonly IUnitOfWork _uow;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IConfiguration _configuration;

        public RedisRepository(IUnitOfWork uow, IConnectionMultiplexer connectionMultiplexer, IConfiguration configuration)
        {
            _uow = uow;
            _connectionMultiplexer = connectionMultiplexer;
            _configuration = configuration;
            _InitRedisDB();
        }
        private IDatabase _GetDB()
        {
            return _connectionMultiplexer.GetDatabase();
        }
        private async void _InitRedisDB()
        {
            var projects = await _uow.ApplicationRepository.GetAllProjectsAsync();
            foreach (var project in projects)
                await _StoreProjectAsync(project);
        }
        private async Task _StoreProjectAsync(Core.Domain.Project project)
        {
            var db = _GetDB();
            var projectentries = new HashEntry[]
            {
                new HashEntry("Id", project.Id.ToString()),
                new HashEntry("Title", project.Title),
                new HashEntry("Description", project.Description)
            };
            await db.HashSetAsync($"project:{project.Id}", projectentries);
            List<HashEntry> taskentrieslist = new List<HashEntry>();
            foreach (var task in project.Tasks)
            {
                var te = new HashEntry[] {
                    new HashEntry("Id", task.Id.ToString()),
                    new HashEntry("Title", task.Title),
                    new HashEntry("Description", task.Description),
                    new HashEntry("DueDate", task.DueDate.ToString()),
                    new HashEntry("Status", task.Status.ToString()),
                };
                await db.HashSetAsync($"project:{project.Id}:task:{task.Id}", te);
            }
        }

        public Task<Core.Domain.Project> AddProjectAsync(Core.Domain.Project project)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProjectAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Core.Domain.Project>> FindProjectsByTitleAsync(string title)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Core.Domain.Project>> GetAllProjectsAsync()
        {
            var db = _GetDB();
            var server = _connectionMultiplexer.GetServer(_configuration["RedisServer:Address"]);
            var keys = server.Keys(pattern: "project:*");

            List<Core.Domain.Project> projects = new List<Core.Domain.Project>();
            foreach (var key in keys)
            {
                var project = await GetProjectByIdAsync(Guid.Parse(key.ToString().Split(':')[1]));
                if (project != null)
                    projects.Add(project);
            }
            return projects;
        }

        public async Task<Core.Domain.Project?> GetProjectByIdAsync(Guid id)
        {
            var db = _connectionMultiplexer.GetDatabase();
            var hasentries = await db.HashGetAllAsync($"project:{id}");
            if (hasentries == null)
                return null;
            var project = new Core.Domain.Project()
            {
                Id = Guid.Parse(hasentries.FirstOrDefault(entry => entry.Name == "Id").Value.ToString()),
                Title = hasentries.FirstOrDefault(entry => entry.Name == "Title").Value,
                Description = hasentries.FirstOrDefault(entry => entry.Name == "Description").Value,
                Tasks = await GetTasksByProjectIdAsync(id)
            };
            return project;
        }

        private async Task<List<Core.Domain.Task>> GetTasksByProjectIdAsync(Guid id)
        {
            var db = _GetDB();
            var server = _connectionMultiplexer.GetServer(_configuration["RedisServer:Address"]);
            var keys = server.Keys(pattern: $"project:{id}:task:*");

            List<Core.Domain.Task> tasks = new List<Core.Domain.Task>();
            foreach (var key in keys)
            {
                var taskentries = await db.HashGetAllAsync($"project:{id}:task:{key}");
                if (taskentries.Any())
                {
                    tasks.Add(new Core.Domain.Task()
                    {
                        Id = Guid.Parse(taskentries.FirstOrDefault(entry => entry.Name == "Id").Value.ToString()),
                        Title = taskentries.FirstOrDefault(entry => entry.Name == "Title").Value,
                        Description = taskentries.FirstOrDefault(entry => entry.Name == "Description").Value,
                        DueDate = DateTime.Parse(taskentries.FirstOrDefault(entry => entry.Name == "DueDate").Value),
                        Status = (Core.Domain.TaskStatus)Enum.Parse(typeof(Core.Domain.TaskStatus), taskentries.FirstOrDefault(entry => entry.Name == "Status").Value)
                    });
                }
            }
            return tasks;
        }

        public Task UpdateProjectAsync(Core.Domain.Project project)
        {
            throw new NotImplementedException();
        }
    }
}
