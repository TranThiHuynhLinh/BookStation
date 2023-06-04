using BookStation.Data;
using BookStation.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStation.Controllers
{
    public class TopController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TopController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Book> objBookList = _db.Books;
            IEnumerable<Review> objReviewList = _db.Reviews;
            IEnumerable<Vote> objVoteList = _db.Votes;
            var viewModel = new FullDataViewModel
            {
                Books = objBookList,
                Reviews = objReviewList,
                Votes = objVoteList
            };
            return View(viewModel);
        }
    }
}
