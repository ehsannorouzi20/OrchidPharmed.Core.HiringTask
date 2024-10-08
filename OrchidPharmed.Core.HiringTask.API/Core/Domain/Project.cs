namespace OrchidPharmed.Core.HiringTask.API.Core.Domain
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public List<Domain.Task> Tasks { get; set; } = new List<Task>();
        public void AddTask(string title, string? description, DateTime duedate)
        {
            var t = new Domain.Task()
            {
                Description = description,
                Title = title,
                DueDate = duedate,
                Status = TaskStatus.ToDo
            };
            Tasks.Add(t);
        }
        public void RemoveAllTasks()
        {
            Tasks.Clear();
        }
        public void RemoveTask(Guid taskId)
        {
            var t = Tasks.SingleOrDefault(e => e.Id == taskId);
            if (t != null)
                Tasks.Remove(t);
        }
        public Task? GetTaskById(Guid taskId)
        {
            return Tasks.SingleOrDefault(e => e.Id == taskId);
        }
        public decimal GetPrograss()
        {
            return Tasks.Where(e => e.Status == TaskStatus.Done).Count() / Tasks.Count() * 100;
        }
        public void UpdateTaskStatus(Guid taskId, string status)
        {
            var t = Tasks.SingleOrDefault(e => e.Id == taskId);
            if (t != null)
                t.Status = (TaskStatus)Enum.Parse(typeof(TaskStatus), status);
        }
    }
}
