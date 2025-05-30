
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Webdev_project.Helpers
{
    public class SecurityHelper
    {
        private static readonly string myPepper = Environment.GetEnvironmentVariable("Security__Pepper");
        private static readonly string serverEmail = Environment.GetEnvironmentVariable("Security__Email");
        private static readonly string serverEmailPassword = Environment.GetEnvironmentVariable("Security__Password");
        public SecurityHelper(/*IConfiguration configuration*/)
        {
            //myPepper = configuration["Security:Pepper"];
        }

        public static string GenerateSessionId(int length = 32) // use when login
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var data = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(data);
            }

            var result = new StringBuilder(length);
            foreach (byte b in data)
            {
                result.Append(chars[b % chars.Length]);
            }

            return result.ToString();
        }

        public static string GenerateSalt(int length = 10) // use in register
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var saltBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            var saltChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                saltChars[i] = chars[saltBytes[i] % chars.Length];
            }

            return new string(saltChars);
        }

        public static string HashPassword(string password, string salt)
        {
            string combined = password + salt + myPepper;
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(combined));
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyPassword(string inputPassword, string storedSalt, string storedHash) // use when login
        {
            string newHash = HashPassword(inputPassword, storedSalt);
            return storedHash == newHash;
        }

        public static void SendOPTEmail(string targetEmail, string subject, string body)
        {
            string yourEmail = "nguyenvietkyquan@gmail.com";
            string yourPassword = "12345678";

            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(yourEmail);
                mail.To.Add(targetEmail);
                mail.Subject = subject;
                mail.Body = body;

                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
                smtpServer.Port = 587;
                smtpServer.Credentials = new NetworkCredential(yourEmail, yourPassword);
                smtpServer.EnableSsl = true;

                smtpServer.Send(mail);
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to send email. Error: " + ex.Message);
            }
        }
    }
}
