using System.ComponentModel.DataAnnotations;

namespace OrchidPharmed.Core.HiringTask.API.Structure.DTO
{
    public class CreateProjectDTO
    {
     //   [Required, MaxLength(200)]
        public string Title { get; set; }

      //  [MaxLength(1000)]
        public string? Description { get; set; }
    }
}
