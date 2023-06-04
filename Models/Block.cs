using System.ComponentModel.DataAnnotations;

namespace BookStation.Models
{
    public class Block
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int NumOfReportedRv { get; set; }
        [Required]
        public int IsBlocked { get; set; }
    }
}
