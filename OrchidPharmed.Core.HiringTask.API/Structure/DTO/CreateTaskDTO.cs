namespace OrchidPharmed.Core.HiringTask.API.Structure.DTO
{
    public class CreateTaskDTO
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
    }
}
