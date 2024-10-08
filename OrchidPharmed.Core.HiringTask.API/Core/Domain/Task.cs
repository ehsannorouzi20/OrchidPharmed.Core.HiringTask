namespace OrchidPharmed.Core.HiringTask.API.Core.Domain
{
    public class Task
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskStatus Status { get; set; }
    }


    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Done
    }
}
