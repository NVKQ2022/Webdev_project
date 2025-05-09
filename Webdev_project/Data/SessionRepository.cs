using Microsoft.Data.SqlClient;
using Webdev_project.Interfaces;
using Webdev_project.Models;

namespace Webdev_project.Data
{
    public class SessionRepository :ISessionRepository
    {
        private static readonly string  _connectionString = Environment.GetEnvironmentVariable("ConnectionString:UserDatabase")?? throw new InvalidOperationException("Database connection string not configured");

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
            string query = @"INSERT INTO session (UserId, UserName, LoginTime , IpAddress, UserAgent, SessionId) 
                         VALUES (@UserId, @UserName, @LoginTime, @IpAddress, @UserAgent, @SessionId)";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", user.Id);
            cmd.Parameters.AddWithValue("@UserName", user.Username);
            cmd.Parameters.AddWithValue("@LoginTime", requestTime);
            cmd.Parameters.AddWithValue("@IpAddress", ipAddress);
            cmd.Parameters.AddWithValue("@UserAgent", userAgent);
            cmd.Parameters.AddWithValue("@SessionId", sessionId);

            connection.Open();
            cmd.ExecuteNonQuery();

            return sessionId;
        }
    }
}
