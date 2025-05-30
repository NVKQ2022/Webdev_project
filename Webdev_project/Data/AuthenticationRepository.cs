﻿using System.Data.SqlClient;
using Webdev_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Webdev_project.Interfaces;

//using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Webdev_project.Data
{
    public class AuthenticationRepository:IUserRepository
    {
        private static readonly string _connectionString = Environment.GetEnvironmentVariable("ConnectionString__UserDatabase") ?? throw new InvalidOperationException("Database connection string not configured");

        public AuthenticationRepository(/*IConfiguration configuration*/)
        {
            

        }
        //  Thêm User vào Database
        public void AddUser(User user)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO users (Email, Username, Password, Salt, IsAdmin) VALUES (@Email, @Username, @Password, @Salt, @IsAdmin)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Salt", user.Salt);
                    command.Parameters.AddWithValue("@IsAdmin", user.IsAdmin);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }


        public User? AuthenticateUser(string email, string password)// done
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, Email, Username , IsAdmin FROM users WHERE Email = @Email AND Password = @Password";

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
                        IsAdmin =reader.GetBoolean(3)

                        
                    };
                }
            }
            return null; // Trả về null nếu sai tài khoản/mật khẩu
        }

        public bool AdminAuthorize(string userId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT IsAdmin FROM users WHERE Id=@Id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", userId);


                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return reader.GetBoolean(0);
                }

                return false; // Trả về null nếu sai tài khoản/mật khẩu
            }
        }

    }
}