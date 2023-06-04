using System.ComponentModel.DataAnnotations;

namespace BookStation.Models;
public class Author
{
    [Key]
    public int AuthorId { get; set; }
    [Required]
    public string AuthorName { get; set; }
}

