using Microsoft.Data.SqlClient;
using System.Net;
using Microsoft.AspNetCore.Http;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System;
using Microsoft.AspNetCore.Identity;

namespace BookStation.Models
{
    public class FullDataViewModel : Review
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Author> Authors { get; set; }
        public IEnumerable<Book> Books { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
        public IEnumerable<FavouriteBook> FavouriteBooks { get; set; }
        public IEnumerable<Vote> Votes { get; set; }
        public IEnumerable<Report> Reports { get; set; }
        public IEnumerable<Block> Blocks { get; set; }

        public int countTotalReviews(int bookId)
        {
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM Reviews WHERE BookId = @BookId ;", conn);
                cmd.Parameters.AddWithValue("@BookId", bookId);
                var total = (int)cmd.ExecuteScalar();
                Console.WriteLine("total=" + total);
                return total;
            }
        }
        public List<Review> PageReviews(int currentPage, int itemsPerPage, int bookId)
        {
            Console.Write("currentPage = " + currentPage);

            List<Review> reviews = new List<Review>();
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Reviews WHERE BookId = @BookId ORDER BY ReviewId OFFSET @OffSet ROWS FETCH NEXT @FetchRows ROWS ONLY;", conn);
                cmd.Parameters.AddWithValue("@BookId", bookId);
                cmd.Parameters.AddWithValue("@OffSet", currentPage * itemsPerPage);
                cmd.Parameters.AddWithValue("@FetchRows", itemsPerPage);
               
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Review review = new Review();
                        review.UserId = reader["UserId"].ToString();
                        review.ReviewId = Int32.Parse(reader["ReviewId"].ToString());
                        review.BookId = Int32.Parse(reader["BookId"].ToString());
                        review.ReviewDate = reader["ReviewDate"].ToString();
                        review.Rating = Int32.Parse(reader["Rating"].ToString());
                        review.UpVotes = Int32.Parse(reader["UpVotes"].ToString());
                        review.DownVotes = Int32.Parse(reader["DownVotes"].ToString());
                        review.TotalVotes = Int32.Parse(reader["TotalVotes"].ToString());
                        review.Detail = reader["Detail"].ToString();
                        reviews.Add(review);
                    }
                }
            }
            Console.WriteLine("size=" + reviews.Count);
            return reviews;
        }
        public List<int> ReviewIdListOfBook(int bookId)
        {

            List<int> listReviewId = new List<int>();
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT ReviewId FROM Reviews WHERE BookId = @BookId;", conn);
                cmd.Parameters.AddWithValue("@BookId", bookId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Int32.Parse(reader["ReviewId"].ToString());
                        listReviewId.Add(id);
                    }
                }
            }
            return listReviewId;
        }
        
        public int InteractionBook(List<int> ListReviewId)
        {
            int count = 0;
            int sum = ListReviewId.Count;
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                foreach (var id in ListReviewId)
                {
                    var cmd = new SqlCommand("SELECT COUNT(DISTINCT UserId) FROM Votes WHERE ReviewId = @ReviewId;", conn);
                    cmd.Parameters.AddWithValue("@ReviewId", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            count = reader.GetInt32(0);
                        }
                    }
                    sum += count;
                }                                    
            }                        
            if(sum == 0)
            {
                return -1;
            }
            else
            {
                return sum;
            }
        }
        public int ReportedNumber(int reviewId, IEnumerable<Report> reports)
        {
            int count = 0;
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(ReviewId) FROM Reports WHERE ReviewId = @ReviewId;", conn);
                cmd.Parameters.AddWithValue("@ReviewId", reviewId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        count = reader.GetInt32(0);
                    }
                }
            }
            return count;
        }
        public List<int> ReportedReviewIds(IEnumerable<Report> reports)
        {
            List<int> ReportedReviews = new List<int>();
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT ReviewId, COUNT(ReviewId) AS count FROM Reports GROUP BY ReviewId ORDER BY count DESC;", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Int32.Parse(reader["ReviewId"].ToString());
                        ReportedReviews.Add(id);
                    }
                }
            }
            return ReportedReviews;
        }
        public string FindAuthor(int AuthorId, IEnumerable<Author> Authors)
        {
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT AuthorName FROM Authors WHERE AuthorId = @AuthorId", conn);
                cmd.Parameters.AddWithValue("@AuthorId", AuthorId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader["AuthorName"].ToString();
                    }
                }
            }
            return "No Name";
        }
        public int CheckExistedVote(string UserId, int ReviewId, IEnumerable<Vote> votes)
        {
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Votes WHERE UserId = @UserId AND ReviewId = @ReviewId", conn);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@ReviewId", ReviewId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Int32.Parse(reader["VoteValue"].ToString());
                    }
                }
            }
            return 0;
        }
        public bool CheckExistedFavourite(int BookId, string UserId, IEnumerable<FavouriteBook> favouriteBooks)
        {
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM FavouriteBooks WHERE UserId = @UserId AND BookId = @BookId", conn);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@BookId", BookId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool CheckExistedReport(int ReviewId, string UserIdReport, IEnumerable<Report> reports)
        {
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Reports WHERE UserIdReport = @UserIdReport AND ReviewId = @ReviewId", conn);
                cmd.Parameters.AddWithValue("@UserIdReport", UserIdReport);
                cmd.Parameters.AddWithValue("@ReviewId", ReviewId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public string FindCategory(int CategoryId, IEnumerable<Category> Categories)
        {
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT CategoryName FROM Categories WHERE CategoryId = @CategoryId", conn);
                cmd.Parameters.AddWithValue("@CategoryId", CategoryId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader["CategoryName"].ToString();
                    }
                }
            }
            return "No Category";
        }

        public Book FindBook(int BookId, IEnumerable<Book> Books)
        {
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Books WHERE BookId = @BookId", conn);
                cmd.Parameters.AddWithValue("@BookId", BookId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Book book = new Book();
                        book.BookId = BookId;
                        book.BookName = reader["BookName"].ToString();
                        book.AuthorId = Int32.Parse(reader["AuthorId"].ToString());
                        book.CategoryId1 = Int32.Parse(reader["CategoryId1"].ToString());
                        book.CategoryId2 = Int32.Parse(reader["CategoryId2"].ToString());
                        book.CategoryId3 = Int32.Parse(reader["CategoryId3"].ToString());
                        book.ImageName = reader["ImageName"].ToString();
                        book.Path = reader["Path"].ToString();
                        book.Summary = reader["Summary"].ToString();
                        return book;
                    }
                }
            }
            return null;
        }
        public Vote VoteStatus(int ReviewId, string UserId, IEnumerable<Vote> votes)
        {
            if (UserId == null)
            {
                return null;
            }
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Votes WHERE ReviewId = @ReviewId AND UserId = @UserId", conn);
                cmd.Parameters.AddWithValue("@ReviewId", ReviewId);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Vote vote = new Vote();
                        vote.VoteId = (int) reader["VoteId"];
                        vote.ReviewId = Int32.Parse(reader["ReviewId"].ToString());
                        vote.UserId = reader["UserId"].ToString();
                        vote.VoteValue = (int) reader["VoteValue"];
                        return vote;
                    }
                }
            }
            return null;
        }
        public string GetAverageRating(int BookId)
        {
            if (BookId == 0)
            {
                return "0.0";
            }
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT AVG(CAST(Rating AS FLOAT)) AS 'avg' FROM Reviews WHERE BookId = @BookId", conn);
                cmd.Parameters.AddWithValue("@BookId", BookId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read() && !reader.IsDBNull(reader.GetOrdinal("avg")))
                    {
                        return Convert.ToDouble(reader["avg"]).ToString("N1");
                    }
                }
            }
            return "0.0";
        }
        public List<Book> ListHighestRatedBook(int num, IEnumerable<Book> Books)
        {
            List<Book> list = new List<Book>();
            foreach(var b in Books)
            {
                list.Add(b);
            }
            list.Sort((b1, b2) => Convert.ToDouble(GetAverageRating(b2.BookId)).CompareTo(Convert.ToDouble(GetAverageRating(b1.BookId))));
            return list.Take(num).ToList();
        }
        public bool IsExitedComment(int BookId, string UserId, IEnumerable<Review> Reviews)
        {
            if (UserId.Length < 10)
            {
                return false;
            }
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Reviews WHERE BookId = @BookId AND UserId = @UserId", conn);
                cmd.Parameters.AddWithValue("@BookId", BookId);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public Review GetReview(int BookId, string UserId, IEnumerable<Review> Reviews)
        {
            if (UserId.Length < 10)
            {
                return null;
            }
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Reviews WHERE BookId = @BookId AND UserId = @UserId", conn);
                cmd.Parameters.AddWithValue("@BookId", BookId);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Review review = new Review();
                        review.ReviewId = Int32.Parse(reader["ReviewId"].ToString());
                        review.UserId = UserId;
                        review.BookId = BookId;
                        review.ReviewDate = reader["ReviewDate"].ToString();
                        review.Rating = Int32.Parse(reader["Rating"].ToString());
                        review.UpVotes = Int32.Parse(reader["UpVotes"].ToString());
                        review.DownVotes = Int32.Parse(reader["DownVotes"].ToString());
                        review.TotalVotes = Int32.Parse(reader["TotalVotes"].ToString());
                        review.Detail = reader["Detail"].ToString();
                        return review;
                    }
                }
            }
            return null;
        }
        public List<Review> ListUserReviews(string userId)
        {
            List<Review> listRev = new List<Review>();
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Reviews WHERE UserId = @userId;", conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Review review = new Review();
                        review.ReviewId = Int32.Parse(reader["ReviewId"].ToString());
                        review.UserId = userId;
                        review.BookId = Int32.Parse(reader["BookId"].ToString());
                        review.ReviewDate = reader["ReviewDate"].ToString();
                        review.Rating = Int32.Parse(reader["Rating"].ToString());
                        review.UpVotes = Int32.Parse(reader["UpVotes"].ToString());
                        review.DownVotes = Int32.Parse(reader["DownVotes"].ToString());
                        review.TotalVotes = Int32.Parse(reader["TotalVotes"].ToString());
                        review.Detail = reader["Detail"].ToString();
                        listRev.Add(review);
                    }
                }
            }
            return listRev;
        }
        public int GetUpVotes(string userId) 
        {
            int up = 0;
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT SUM(UpVotes) AS sum FROM Reviews WHERE UserId = @userId;", conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("sum")))
                        {
                            up += Int32.Parse(reader["sum"].ToString());
                        }
                    }
                }
            }
            return up;
        }
        public int GetDownVotes(string userId)
        {
            int down = 0;
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT SUM(DownVotes) AS sum FROM Reviews WHERE UserId = @userId;", conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("sum")))
                        {
                            down += Int32.Parse(reader["sum"].ToString());
                        }
                    }
                }
            }
            return down;
        }
        public int GetTotalVotes(string userId)
        {
            int total = 0;
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT SUM(TotalVotes) AS sum FROM Reviews WHERE UserId = @userId;", conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("sum")))
                        {
                            total += Int32.Parse(reader["sum"].ToString());
                        }
                    }
                }
            }
            return total;
        }
        public List<string> ListHighestVoteReviewer(int num, UserManager<IdentityUser> userManager)
        {
            var userIds = userManager.Users.Select(user => user.Id).ToList();
            for (int i = 0; i < userIds.Count; i++)
            {
                for (int j = i + 1; j < userIds.Count; j++)
                {
                    if (GetTotalVotes(userIds[i]) < GetTotalVotes(userIds[j]))
                    {
                        string temp = userIds[i];
                        userIds[i] = userIds[j];
                        userIds[j] = temp;
                    }
                }
            }
            return userIds.Take(num).ToList();
        }
        public List<int> ListPopularBookId(int num)
        {
            List<int> listPopularBookId = new List<int>();
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT TOP (@num) BookId, COUNT(*) AS ReviewCount " +
                                         "FROM Reviews GROUP BY BookId ORDER BY ReviewCount DESC;", conn);
                cmd.Parameters.AddWithValue("@num", num);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listPopularBookId.Add(Int32.Parse(reader["BookId"].ToString()));
                    }
                }
            }
            return listPopularBookId;
        }
        public int CountReviews(string userId)
        {
            int total = 0;
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(ReviewId) AS count FROM Reviews WHERE UserId = @userId", conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("count")))
                        {
                            total += Int32.Parse(reader["count"].ToString());
                        }
                    }
                }
            }
            return total;
        }
        public List<string> ListEnthusiasticReviewer(int num, UserManager<IdentityUser> userManager)
        {
            var userIds = userManager.Users.Select(user => user.Id).ToList();
            for (int i = 0; i < userIds.Count; i++)
            {
                for (int j = i + 1; j < userIds.Count; j++)
                {
                    if (CountReviews(userIds[i]) < CountReviews(userIds[j]))
                    {
                        string temp = userIds[i];
                        userIds[i] = userIds[j];
                        userIds[j] = temp;
                    }
                }
            }
            return userIds.Take(num).ToList();
        }

        public string SelectText(string FullText, int Limit)
        {
            if (FullText.Length > Limit)
            {
                return FullText.Substring(0, Limit);
            }
            else
            {
                return FullText;
            }
        }
        public Boolean isBlocked(string userId)
        {
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Blocks WHERE UserId = @userId AND IsBlocked = 1;", conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

}
