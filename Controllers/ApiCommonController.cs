using BookStation.Data;
using BookStation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookStation.Controllers
{

    [ApiController]
    public class ApiCommonController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ApiCommonController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // POST api/<ValuesController>
        [HttpPost]
        [Route("/api/review/upvote")]
        [Authorize(Roles = "user")]
        public async Task<int> UpVote()
        {

            string upVoteData = string.Empty;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                Task<string> data = reader.ReadToEndAsync();
                upVoteData = data.Result;
            }
            if (upVoteData != string.Empty)
            {
                Dictionary<string, string> upVoteDict =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(upVoteData)!;
                string userId = _userManager.GetUserId(User)!;
                if (userId == null)
                {
                    throw new Exception("User is not found");
                }
                var reviewId = Int32.Parse(upVoteDict["reviewId"].ToString());

                // check whether this user up/down voted for the review
                var voteForCheck = await _context.Votes.FirstOrDefaultAsync(
                    v => v.UserId == userId && v.ReviewId == reviewId);

                // This case is adding new Vote
                if (voteForCheck == null)
                {
                    await saveVote(reviewId, userId, 1);
                    return await upVoteChangeReviewById(reviewId, 1);
                }

                // In this case, user want to un-upvote 
                if (voteForCheck != null && ((Vote)voteForCheck).VoteValue == 1)
                {
                    _context.Remove(voteForCheck);
                    await _context.SaveChangesAsync();
                    return await upVoteChangeReviewById(reviewId, -1);
                }

                // This is not a case, we ignore it. Just return TotalVotes
                if (voteForCheck != null && ((Vote)voteForCheck).VoteValue == -1)
                {
                    Review review = await getReviewById(reviewId);
                    return review.TotalVotes;
                }
                throw new Exception("Cannot change");
            }
            throw new Exception("Input data is not correct");
        }

        [HttpPost]
        [Route("/api/review/downvote")]
        [Authorize(Roles = "user")]
        public async Task<int> DownVote()
        {
            string upVoteData = string.Empty;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                Task<string> data = reader.ReadToEndAsync();
                upVoteData = data.Result;
            }
            if (upVoteData != string.Empty)
            {
                Dictionary<string, string> upVoteDict =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(upVoteData)!;
                string userId = _userManager.GetUserId(User)!;
                if (userId == null)
                {
                    throw new Exception("User is not found");
                }

                int reviewId = Int32.Parse(upVoteDict["reviewId"].ToString());

                // check whether this user up/down voted for the review
                var voteForCheck = await _context.Votes.FirstOrDefaultAsync(
                    v => v.UserId == userId && v.ReviewId == reviewId);

                // This case is adding new Vote
                if (voteForCheck == null)
                {
                    await saveVote(reviewId, userId, -1);
                    return await downVoteChangeReviewById(reviewId, -1);
                }

                // In this case, user want to down-upvote
                if (voteForCheck != null && ((Vote)voteForCheck).VoteValue == -1)
                {
                    _context.Remove(voteForCheck);
                    await _context.SaveChangesAsync();
                    return await downVoteChangeReviewById(reviewId, 1);
                }

                // This is not a case, we ignore it. Just return TotalVotes
                if (voteForCheck != null && ((Vote)voteForCheck).VoteValue == 1)
                {
                    Review review = await getReviewById(reviewId);
                    return review.TotalVotes;
                }

                throw new Exception("Cannot change");
            }
            throw new Exception("Input data is not correct");
        }

        [HttpPost]
        [Route("/api/review/addfavourites")]
        [Authorize(Roles = "user")]
        public async Task<string> AddFavourites()
        {
            string addFavouritesData = string.Empty;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                Task<string> data = reader.ReadToEndAsync();
                addFavouritesData = data.Result;
            }
            if (addFavouritesData != string.Empty)
            {
                Dictionary<string, string> addFavouritesDict =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(addFavouritesData)!;
                string userId = _userManager.GetUserId(User)!;
                if (userId == null)
                {
                    throw new Exception("user is not found");
                }
                var bookID = Int32.Parse(addFavouritesDict["BookId"].ToString());

                var addForCheck = await _context.FavouriteBooks.FirstOrDefaultAsync(
                    v => v.UserId == userId && v.BookId == bookID);
                if (addForCheck == null)
                {
                    await saveFavouriteBook(userId, bookID);
                }
                else
                {
                    await unFavouriteBook(userId, bookID);
                }
                return "{\"result\":\"SUCCESS\"}";
            }
            throw new Exception("Input data is not correct");
        }

        [HttpPost]
        [Route("/api/review/addreports")]
        [Authorize(Roles = "user")]
        public async Task<string> AddReports()
        {
            string addReportsData = string.Empty;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                Task<string> data = reader.ReadToEndAsync();
                addReportsData = data.Result;
            }
            if (addReportsData != string.Empty)
            {
                Dictionary<string, string> addReportsDict =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(addReportsData)!;
                string userId = _userManager.GetUserId(User)!;
                if (userId == null)
                {
                    throw new Exception("user is not found");
                }
                var reviewId = Int32.Parse(addReportsDict["ReviewId"].ToString());
                var writerReview = await _context.Reviews.FirstOrDefaultAsync(
                    w => w.ReviewId == reviewId);
                var userIdWriteReview = writerReview.UserId;
                var addForCheck = await _context.Reports.FirstOrDefaultAsync(
                    v => v.UserIdReport == userId && v.ReviewId == reviewId);
                if (addForCheck == null)
                {
                    await saveReport(userId, reviewId, userIdWriteReview);
                }
                else
                {
                    await unReport(userId, reviewId, userIdWriteReview);
                }
                return "{\"result\":\"SUCCESS\"}";
            }
            throw new Exception("Input data is not correct");
        }

        private async Task saveVote(int reviewId, string userId, int voteValue)
        {
            Vote vote = new Vote();
            vote.UserId = userId;
            vote.ReviewId = reviewId;
            vote.VoteValue = voteValue;
            _context.Add(vote);
            await _context.SaveChangesAsync();
        }
        private async Task saveFavouriteBook(string userId, int bookID)
        {
            FavouriteBook favouriteBook = new FavouriteBook();
            favouriteBook.UserId = userId;
            favouriteBook.BookId = bookID;
            _context.Add(favouriteBook);
            await _context.SaveChangesAsync();
        }
        private async Task unFavouriteBook(string userId, int bookID)
        {
            var favouriteBook = await _context.FavouriteBooks.FirstOrDefaultAsync(fb => fb.UserId == userId && fb.BookId == bookID);

            if (favouriteBook != null)
            {
                _context.Remove(favouriteBook);
                await _context.SaveChangesAsync();
            }
        }
        private async Task saveReport(string userId, int reviewId, string userIdWriteReview)
        {
            Report report = new Report();
            report.UserIdReport = userId;
            report.ReviewId = reviewId;
            report.UserIdReported = userIdWriteReview;
            _context.Add(report);
            await _context.SaveChangesAsync();
        }
        private async Task unReport(string userId, int reviewId, string userIdWriteReview)
        {
            var report = await _context.Reports.FirstOrDefaultAsync(
                re => re.UserIdReport == userId && re.ReviewId == reviewId && re.UserIdReported == userIdWriteReview);

            if (report != null)
            {
                _context.Remove(report);
                await _context.SaveChangesAsync();
            }
        }
        private async Task<int> upVoteChangeReviewById(int reviewId, int voteValue)
        {
            Review review = (Review)(await _context.Reviews.FirstOrDefaultAsync(
                r => r.ReviewId == reviewId))!;

            if (review != null)
            {
                review.UpVotes = review.UpVotes + voteValue;
                review.TotalVotes = review.TotalVotes + voteValue;
                _context.Update(review);
                await _context.SaveChangesAsync();
                return review.TotalVotes;
            }
            throw new Exception("Not found Review");
        }

        private async Task<int> downVoteChangeReviewById(int reviewId, int voteValue)
        {
            Review review = (Review)(await _context.Reviews.FirstOrDefaultAsync(
                r => r.ReviewId == reviewId))!;

            if (review != null)
            {
                review.DownVotes = review.DownVotes - voteValue;
                review.TotalVotes = review.TotalVotes + voteValue;
                _context.Update(review);
                await _context.SaveChangesAsync();
                return review.TotalVotes;
            }
            throw new Exception("Not found Review");
        }


        private async Task<Review> getReviewById(int reviewId)
        {
            return (Review)(await _context.Reviews.FirstOrDefaultAsync(
                r => r.ReviewId == reviewId))!;
        }

    }
}
