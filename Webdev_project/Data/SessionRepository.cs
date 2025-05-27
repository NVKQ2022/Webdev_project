using Microsoft.Data.SqlClient;
using Webdev_project.Interfaces;
using Webdev_project.Models;

namespace Webdev_project.Data
{
    public class SessionRepository :ISessionRepository
    {
        private static readonly string  _connectionString = Environment.GetEnvironmentVariable("ConnectionString__UserDatabase")?? throw new InvalidOperationException("Database connection string not configured");

        public SessionRepository(/*IConfiguration configuration*/)
        {
            //_connectionString = configuration.GetConnectionString("DatabaseConnectionString")
            //    ?? throw new InvalidOperationException("Database connection string not configured");
        }

        public void DeleteSession(string sessionId)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "@DELETE FROM session WHERE SessionId='@SessionId'";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@SessionId", sessionId);

            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public string CreateSession(User user, string ipAddress, DateTime requestTime, string userAgent)
        {
            string sessionId = SecurityHelper.GenerateSessionId();

            using var connection = new SqlConnection(_connectionString);
            string query = @"INSERT INTO session (UserId, UserName, LoginTime , IpAddress, UserAgent, SessionId, IsAdmin) 
                         VALUES (@UserId, @UserName, @LoginTime, @IpAddress, @UserAgent, @SessionId, @IsAdmin)";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", user.Id);
            cmd.Parameters.AddWithValue("@UserName", user.Username);
            cmd.Parameters.AddWithValue("@LoginTime", requestTime);
            cmd.Parameters.AddWithValue("@IpAddress", ipAddress);
            cmd.Parameters.AddWithValue("@UserAgent", userAgent);
            cmd.Parameters.AddWithValue("@SessionId", sessionId);
            cmd.Parameters.AddWithValue("@IsAdmin", user.IsAdmin);


            connection.Open();
            cmd.ExecuteNonQuery();

            return sessionId;
        }
         public string RetrieveIdFromSession(string? sessionId)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = @"SELECT UserId FROM session WHERE SessionId = @SessionId)";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@SessionId" , sessionId);

            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return reader.GetString(0);
            }
            return new string("user not found");
        }
        public User? RetrieveFromSession(string? sessionId)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = @"SELECT UserId, Username, IsAdmin FROM session WHERE SessionId = @SessionId  ";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@SessionId", sessionId);

            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new User
                {

                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    IsAdmin = reader.GetBoolean(2)

                };
            }
            return null;
        }
    }
}
