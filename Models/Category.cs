using System.ComponentModel.DataAnnotations;

namespace BookStation.Models;
public class Category
{
    [Key]
    public int CategoryId { get; set; }
    [Required]
    public string CategoryName { get; set; }
}

