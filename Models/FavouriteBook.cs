using System.ComponentModel.DataAnnotations;

namespace BookStation.Models
{
    public class FavouriteBook
    {
        [Key]
        public int FavouriteBookId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int BookId { get; set; }
    }
}
