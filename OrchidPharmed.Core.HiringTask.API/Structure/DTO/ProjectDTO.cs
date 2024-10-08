namespace OrchidPharmed.Core.HiringTask.API.Structure.DTO
{
    public class ProjectDTO
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string? Description { get; set; }

        public TaskDTO[] Tasks { get; set; }
        public void PrepareValue()
        {
        }
    }
}
