using BookStation.Data;
using BookStation.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookStation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _db.Categories;
            IEnumerable<Author> objAuthorList = _db.Authors;
            IEnumerable<Book> objBookList = _db.Books;
            IEnumerable<Review> Reviews = _db.Reviews;
            IEnumerable<FavouriteBook> FavouriteBooks = _db.FavouriteBooks;
            IEnumerable<Vote> Votes = _db.Votes;
            IEnumerable<Block> Blocks = _db.Blocks;
            var viewModel = new FullDataViewModel
            {
                Categories = objCategoryList,
                Authors = objAuthorList,
                Books = objBookList,
                Reviews = Reviews,
                FavouriteBooks = FavouriteBooks,
                Votes = Votes,
                Blocks = Blocks
            };
            return View(viewModel);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}