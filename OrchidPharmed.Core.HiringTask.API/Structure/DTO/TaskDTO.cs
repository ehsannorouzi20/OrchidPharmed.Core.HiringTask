namespace OrchidPharmed.Core.HiringTask.API.Structure.DTO
{
    public class TaskDTO : IDTO
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string? Description { get; set; }

        public DateTime DueDate { get; set; }
        public string CultureDueDate { get; set; }

        public Core.Domain.TaskStatus Status { get; set; }
        public string CultureStatus { get; set; }
        
        public void PrepareValue()
        {
            CultureDueDate = DueDate.ToPersianDateTime();
            switch (Status)
            {
                case Core.Domain.TaskStatus.ToDo:
                    CultureStatus = "منتظر انجام";
                    break;
                case Core.Domain.TaskStatus.InProgress:
                    CultureStatus = "در حال انجام";
                    break;
                case Core.Domain.TaskStatus.Done:
                    CultureStatus = "انجام شده";
                    break;
                default:
                    break;
            }
        }
    }
}
