using System.ComponentModel.DataAnnotations;

namespace BookStation.Models
{
    public class Vote
    {
        [Key]
        public int VoteId { get; set; }
        [Required]
        public int? ReviewId { get; set; }
        [Required]
        public string? UserId { get; set; }
        [Required]
        public int VoteValue { get; set; }
    }
}
