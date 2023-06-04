using BookStation.Data;
using BookStation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace BookStation.Controllers
{
    public class ReportedReviewsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ReportedReviewsController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {   
            IEnumerable<Report> objReports = _db.Reports;
            IEnumerable<Book> objBookList = _db.Books;
            IEnumerable<Review> objReviewList = _db.Reviews;
            var viewModel = new FullDataViewModel
            {
                Reports = objReports,
                Books = objBookList,
                Reviews = objReviewList
            };
            return View(viewModel);
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

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_db.Reviews == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reviews'  is null.");
            }
            var review = await _db.Reviews.FindAsync(id);
            var voteToRemove = await _db.Votes.Where(v => v.ReviewId == id).ToListAsync();
            var reportToRemove = await _db.Reports.Where(r => r.ReviewId == id).ToListAsync();
            if (review != null)
            {
                _db.Reviews.Remove(review);
                var exitedUser = await _db.Blocks.FirstOrDefaultAsync(
                    v => v.UserId == review.UserId);
                if (exitedUser == null)
                {
                    Block block = new Block();
                    block.UserId = review.UserId;
                    block.NumOfReportedRv = 1;
                    block.IsBlocked = 0;
                    _db.Add(block);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    exitedUser.NumOfReportedRv++;
                    _db.Update(exitedUser);
                    await _db.SaveChangesAsync();
                }
            }
            if(voteToRemove != null)
            {
                _db.RemoveRange(voteToRemove);
                await _db.SaveChangesAsync();
            }
            if(reportToRemove != null)
            {
                _db.RemoveRange(reportToRemove); 
                await _db.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}
