using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrchidPharmed.Core.HiringTask.API.Models
{
    public class TaskEntity
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("ProjectEntity")]
        public Guid ProjectId { get; set; }
        public ProjectEntity Project { get; set; }

        [Required, Column(TypeName = "nvarchar(200)"), MaxLength(200)]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(1000)"), MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public Core.Domain.TaskStatus Status { get; set; } = Core.Domain.TaskStatus.ToDo;

        [Required]
        public DateTime CDT { get; set; }

        public DateTime? MDT { get; set; }
    }

}
