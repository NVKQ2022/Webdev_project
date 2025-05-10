using System.Data.SqlClient;
using Webdev_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

//using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Webdev_project.Data
{
    public class UserRepository
    {
        private readonly string _connectionString;
        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnectionString"); ////////// chua fix

        }


        // 🛠 Thêm User vào Database
        public void AddUser(User user)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO users (Email, Username, Password, UserType, Salt) VALUES (@Email, @Username, @Password, @UserType, @Salt)";

                SqlCommand command = new SqlCommand(query, connection);


                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@Salt", user.Salt);


                connection.Open();
                command.ExecuteNonQuery();
            }
        }


        public User? AuthenticateUser(string email, string password)// done
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, Email, Username FROM users WHERE Email = @Email AND Password = @Password";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password); // Nên hash mật khẩu trong thực tế!

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new User
                    {

                        Id = reader.GetInt32(0),
                        Email = reader.GetString(1),
                        Username = reader.GetString(2),
                        
                    };
                }
            }
            return null; // Trả về null nếu sai tài khoản/mật khẩu
        }

        public string SessionGenerator(User user, string ipAddress,  DateTime requestTime, string userAgent)
        {

            string sessionId = SecurityHelper.GenerateSessionId();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO session (UserId, UserName, LoginTime , IpAddress, UserAgent, SessionId) VALUES (@UserId, @UserName, @LoginTime, @IpAddress, @UserAgent, @SessionId )";

                SqlCommand cmd = new SqlCommand(query, connection);


                cmd.Parameters.AddWithValue("@UserId", user.Id);
                cmd.Parameters.AddWithValue("@UserName", user.Username);
                cmd.Parameters.AddWithValue("@LoginTime", requestTime);
                cmd.Parameters.AddWithValue("@IpAddress", ipAddress);
                cmd.Parameters.AddWithValue("@UserAgent", userAgent);
                cmd.Parameters.AddWithValue("@SessionId", sessionId);


                cmd.ExecuteNonQuery();
            }
            return sessionId;
        }

        public async Task<bool> IsAdminFromSessionId(string sessionId)
        {
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT IsAdmin  FROM SESSION WHERE SessionId = @SessionId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SessionId", sessionId);

                    connection.Open();
                    SqlDataReader reader = await command.ExecuteReaderAsync();
                    if (await reader.ReadAsync()) 
                    {
                        return reader.GetBoolean(0); 
                    }

                    return false;
                }
            }
        }

    }
}
