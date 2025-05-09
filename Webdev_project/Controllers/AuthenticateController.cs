using Microsoft.AspNetCore.Mvc;
//using NuGet.Protocol.Plugins;
using Webdev_project.Data;
using Webdev_project.Models;
using Webdev_project.Interfaces;
namespace Webdev_project.Controllers
{
    public class AuthenticateController : Controller
    {
        //public IConfiguration configuration;
        
        private readonly IUserRepository userRepository;
        private readonly ISessionRepository sessionRepository;
        public AuthenticateController(IUserRepository userRepository, ISessionRepository sessionRepository)
        {
            this.userRepository=userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        }
        [HttpGet]
        public IActionResult MyLogin()
        {
            return View();
        }
        public IActionResult MyRegister()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(string name, string email)// havent made yet
        {
            var user = new User { Username = name, Email = email };
            return View(user);
        }

        [HttpPost]
        public IActionResult MyLogin(string email, string password)
        {
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            DateTime requestTime = DateTime.UtcNow;
            var userAgent =Request.Headers["User-Agent"].ToString();
            User? user = userRepository.AuthenticateUser(email, password);
            if (user != null)
            {   ViewBag.isLoggedIn=true;
                ViewBag.userName=user.Username;
                HttpContext.Response.Cookies.Append("SessionId", sessionRepository.CreateSession(user,ipAddress, requestTime, userAgent));
                return View("~/Views/Home/Index.cshtml");// need to make index

            }
            ViewBag.Message = "Sai thông tin!";
            
            return View("MyLogin");
        }
    }
}
