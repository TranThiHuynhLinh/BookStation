using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStation.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        [Required]
        public string? BookName { get; set; }
        [Required]
        public int AuthorId { get; set; }
        [Required]
        public int CategoryId1 { get; set; }
        [Required]
        public int CategoryId2 { get; set; }
        [Required]
        public int CategoryId3 { get; set; }
        [Required]
        public string? Summary { get; set; }

        public string? Path { get; set; }

        public string? ImageName { get; set; }

        [NotMapped]
        public IFormFile? FileUpload  { get; set; }
    }
}
