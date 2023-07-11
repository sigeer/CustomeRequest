using System.ComponentModel.DataAnnotations;

namespace RequestProcessor
{
    public class JobTableViewModel: JobTableEditoModel
    {
        public int Status { get; set; }
    }

    public class JobStoreStatusControl
    {
        public int JobStoreId { get; set; }
        public int Status { get; set; }
    }

    public class JobTableEditoModel
    {
        [Required]
        public string Name { get; set; } = null!;
        public string? Remark { get; set; }
        [StringLength(36, MinimumLength = 3)]
        [Required(ErrorMessage = "JobId Required")]
        public string JobId { get; set; } = null!;
        [Required]
        public string Cron { get; set; } = null!;
        public int? JobStoreId { get; set; }
        [Required]
        public int JobDetailId { get; set; }
    }
}
