using Microsoft.Data.SqlClient;
using System.Net;
namespace BookStation.Models
{
    public class AuthorCategory : Book
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Author> Authors { get; set; }
        public IEnumerable<Book> Books { get; set; }
    }
}
