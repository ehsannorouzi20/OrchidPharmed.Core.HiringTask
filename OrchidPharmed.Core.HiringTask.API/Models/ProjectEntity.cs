using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace OrchidPharmed.Core.HiringTask.API.Models
{
    public class ProjectEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required, Column(TypeName = "nvarchar(200)"), MaxLength(200)]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(1000)"), MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime CDT { get; set; }

        public DateTime? MDT { get; set; }

        public List<TaskEntity> Tasks { get; set; }
    }
}
