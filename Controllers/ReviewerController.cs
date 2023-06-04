using BookStation.Data;
using BookStation.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStation.Controllers
{
    public class ReviewerController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ReviewerController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _db.Categories;
            IEnumerable<Author> objAuthorList = _db.Authors;
            IEnumerable<Book> objBookList = _db.Books;
            IEnumerable<Review> objReviewList = _db.Reviews;
            var viewModel = new FullDataViewModel
            {
                Categories = objCategoryList,
                Authors = objAuthorList,
                Books = objBookList,
                Reviews = objReviewList
            };
            return View(viewModel);
        }
    }
}
