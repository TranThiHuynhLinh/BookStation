using System.ComponentModel.DataAnnotations;

namespace BookStation.Models;
    public class Report
    {
        [Key]
        public int ReportId { get; set; }
        [Required]
        public int ReviewId { get; set; }
        [Required]
        public string UserIdReport { get; set; }
        [Required]
        public string UserIdReported { get; set; }

    }

