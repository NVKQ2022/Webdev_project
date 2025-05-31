using Microsoft.AspNetCore.Mvc;
//using NuGet.Protocol.Plugins;
using Webdev_project.Models;
using Webdev_project.Interfaces;
using Webdev_project.Helpers;
namespace Webdev_project.Controllers
{
    public class AuthenticateController : Controller
    {
        //public IConfiguration configuration;
        
        private readonly IUserRepository userRepository;
        private readonly ISessionRepository sessionRepository;
        private readonly IProductRepository productRepository;
        private readonly ICategoryRepository categoryRepository;
        public AuthenticateController(IUserRepository userRepository, ISessionRepository sessionRepository, IProductRepository productRepository,ICategoryRepository categoryRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
            this.productRepository = productRepository; 
            this.categoryRepository = categoryRepository;
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
        [HttpGet]
        public async Task<IActionResult> Profile(string category)
        {
            User? user=  sessionRepository.RetrieveFromSession(HttpContext.Request.Cookies["sessionId"]);
            ViewBag.User = user;
            if (user!= null && user.IsAdmin)
            {
                var categories = await productRepository.GetAllCategoriesAsync();
                var products = string.IsNullOrEmpty(category) ? new List<Product>() : await productRepository.GetByCategoryAsync(category);
                await categoryRepository.InsertCategoryAsync(category);
                ViewBag.User = user;
                ViewBag.Products = products;
                ViewBag.Categories = categories;
                ViewBag.SelectedCategory = category;

             
            }
            return View();
        }

        [HttpPost]
        public  IActionResult Profile()
        {
            var product = new Product();
            productRepository.AddAsync(product);
            return View();
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


        [HttpPost]
        public IActionResult MyRegister(string email, string username, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                ViewBag.Message = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Message = "Mật khẩu xác nhận không khớp.";
                return View();
            }

            string salt = SecurityHelper.GenerateSalt();
            string hashedPassword = SecurityHelper.HashPassword(password, salt);

            var user = new User
            {
                Email = email,
                Username = username,
                Password = hashedPassword,
                Salt = salt,
                IsAdmin = false
            };

            try
            {
                userRepository.AddUser(user);
                ViewBag.Message = "Đăng ký thành công! Bạn có thể đăng nhập.";
                return RedirectToAction("MyLogin");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Đăng ký thất bại: " + ex.Message;
                return View();
            }
        }
    }
}
