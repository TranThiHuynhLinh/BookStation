using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStation.Models
{
    public class SubUserInfo
    {
        [Key]
        public  int infoID { get; set; }
        [Required]
        public string UserID { get; set; }
        public string AvatarName { get; set; }
        public string Path { get; set;}
        [NotMapped]
        public IFormFile? FileUpload { get; set; }
    }
}
