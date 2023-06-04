using System.ComponentModel.DataAnnotations;

namespace BookStation.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int BookId { get; set; }
        [Required]
        public string ReviewDate { get; set; }
        [Required]
        public int Rating { get; set; }
        [Required]
        public int UpVotes { get; set; }
        [Required]
        public int DownVotes { get; set; }
        [Required]
        public int TotalVotes { get; set; }
        [Required]
        public string Detail { get; set; }
    }
}
