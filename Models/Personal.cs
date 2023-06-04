using Microsoft.Data.SqlClient;

namespace BookStation.Models
{
    public class PersonalViewModel : FavouriteBook
    {
        public IEnumerable<FavouriteBook> FavouriteBooks { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
        public IEnumerable<SubUserInfo> SubUserInfos { get; set; }
        public string CountReviews(string userId)
        {
            if (string.IsNullOrEmpty(userId) || Reviews == null)
            {
                return "0";
            }
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(ReviewId) AS count FROM Reviews WHERE UserId = @userId", conn);
                cmd.Parameters.AddWithValue("@userId", userId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader["count"].ToString();
                    }
                    else return "0";
                }
            }

        }
        public string GetHighestReview(string userID)
        {
            if (string.IsNullOrEmpty(userID) || Reviews == null)
            {
                return "You haven't reviewed anything!";
            }

            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT TOP(1) Detail AS detail FROM Reviews WHERE userId =  @userID ORDER BY TotalVotes DESC", conn);
                cmd.Parameters.AddWithValue("@userID", userID);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader["detail"].ToString();
                    }
                    else return "You haven't reviewed anything!";
                }
            }

        }
        public string SelectText(string FullText, int Limit)
        {
            if (FullText.Length > Limit)
            {
                return "\"" + FullText.Substring(0, Limit) + "...." + "\"";
            }
            else
            {
                return FullText;
            }
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
                        var sum = reader["sum"];
                        if (sum != null && sum != DBNull.Value && !string.IsNullOrEmpty(sum.ToString()))
                        {
                            up += Convert.ToInt32(sum);
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
                        var sum = reader["sum"];
                        if (sum != null && sum != DBNull.Value && !string.IsNullOrEmpty(sum.ToString()))
                        {
                            down += Convert.ToInt32(sum);
                        }
                    }
                }
            }
            return down;
        }
        public string GetAvatarID(string UserID)
        {
            if (string.IsNullOrEmpty(UserID) || Reviews == null)
            {
                return "";
            }
            using (var conn = new SqlConnection("Server=.;Database=Store;Trusted_Connection=True;Encrypt=false"))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT AvatarName FROM SubUserInfos WHERE UserID = @userID", conn);
                cmd.Parameters.AddWithValue("@userID", UserID);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader["AvatarName"].ToString();
                    }
                    else return "";
                }
            }

        }
    }
}
