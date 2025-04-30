using Microsoft.AspNetCore.Mvc;
//using NuGet.Protocol.Plugins;
using Webdev_project.Data;
using Webdev_project.Models;
namespace Webdev_project.Controllers
{
    public class AuthenticateController : Controller
    {
        //public IConfiguration configuration;
        //private UserRepository _userRepository= new UserRepository(configuration);
        private readonly UserRepository _userRepository;

        public AuthenticateController(UserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
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
            User? user = _userRepository.AuthenticateUser(email, password);
            if (user != null)
            {   ViewBag.isLoggedIn=true;
                ViewBag.userName=user.Username;
                HttpContext.Response.Cookies.Append("SessionId", _userRepository.SessionGenerator(user,ipAddress, requestTime, userAgent));
                return View("~/Views/Home/Index.cshtml");// need to make index

            }
            ViewBag.Message = "Sai thông tin!";
            
            return View("MyLogin");
        }
    }
}
