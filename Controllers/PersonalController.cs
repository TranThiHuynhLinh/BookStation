using BookStation.Data;
using BookStation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStation.Controllers
{
    public class PersonalController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PersonalController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        public IActionResult Index()
        {
            IEnumerable<FavouriteBook> objfavouriteBooks = _db.FavouriteBooks;
            IEnumerable<Review> objReviews = _db.Reviews;
            IEnumerable<SubUserInfo> objSubUserInfos = _db.SubUserInfos;
            var viewModel = new PersonalViewModel
            {
                FavouriteBooks = objfavouriteBooks,
                Reviews = objReviews,
                SubUserInfos = objSubUserInfos
            };
            return View(viewModel);
        }
        public IActionResult UploadAvatar()
        {
            return View();
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _db.FavouriteBooks == null)
            {
                return NotFound();
            }

            var fv = await _db.FavouriteBooks
                .FirstOrDefaultAsync(m => m.FavouriteBookId == id);
            if (fv == null)
            {
                return NotFound();
            }

            return View(fv);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_db.FavouriteBooks == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FavouriteBooks'  is null.");
            }
            var fv = await _db.FavouriteBooks.FindAsync(id);
            if (fv != null)
            {
                _db.FavouriteBooks.Remove(fv);
            }
            await _db.SaveChangesAsync();
            return Redirect("/Personal");
        }
    }
}
