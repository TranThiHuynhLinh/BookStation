using BookStation.Data;
using BookStation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace BookStation.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ReviewController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _db.Categories;
            IEnumerable<Author> objAuthorList = _db.Authors;
            IEnumerable<Book> objBookList = _db.Books;
            IEnumerable<Review> objReviewList = _db.Reviews;
            IEnumerable<Block> objBlockList = _db.Blocks;
            var viewModel = new FullDataViewModel
            {
                Categories = objCategoryList,
                Authors = objAuthorList,
                Books = objBookList,
                Reviews = objReviewList,
                Blocks = objBlockList
            };
            return View(viewModel);
        }
        [Authorize(Roles = "admin, user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReviewId,UserId,BookId,ReviewDate,Rating,UpVotes,DownVotes,TotalVotes,Detail")] Review review)
        {
            if (review.UserId == "0")
            {
                return View(review);
            }
            if (review.Rating == 0)
            {
                return RedirectToAction("ErrorWriteReview");
            }
            if (string.IsNullOrWhiteSpace(review.Detail))
            {
                review.Detail = "I have no idea...";
            }
            _db.Add(review);
            await _db.SaveChangesAsync();
            if (HttpContext.Request.Headers.TryGetValue("Referer", out var referer))
            {
                return Redirect(referer);
            }
            return View(review);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _db.Reviews == null)
            {
                return NotFound();
            }

            var review = await _db.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            return View(review);
        }
        [Authorize(Roles = "admin, user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReviewId,UserId,BookId,ReviewDate,Rating,UpVotes,DownVotes,TotalVotes,Detail")] Review review)
        {
            if (id != review.ReviewId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(review);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.ReviewId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("/Review/Index?bookId=" + review.BookId);
            }
            return View(review);
        }
        private bool ReviewExists(int id)
        {
            return (_db.Reviews?.Any(e => e.ReviewId == id)).GetValueOrDefault();
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _db.Reviews == null)
            {
                return NotFound();
            }

            var review = await _db.Reviews
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_db.Reviews == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reviews'  is null.");
            }
            var review = await _db.Reviews.FindAsync(id);
            var bookId = review.BookId;
            if (review != null)
            {
                _db.Reviews.Remove(review);
            }

            await _db.SaveChangesAsync();
            return Redirect("/Review/Index?bookId=" + bookId);
        }
        public IActionResult Blocked()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult ErrorWriteReview()
        {
            return View();
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
