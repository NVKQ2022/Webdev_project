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
        
        
        private readonly IAuthenticationRepository authenticationRepository;
        private readonly IProductRepository productRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IUserDetailRepository userDetailRepository;
        public AuthenticateController(IAuthenticationRepository authenticationRepository, IProductRepository productRepository,ICategoryRepository categoryRepository,IUserDetailRepository userDetailRepository)
        {
            
            this.productRepository= productRepository;
            this.authenticationRepository = authenticationRepository;
            this.categoryRepository = categoryRepository;
            this.userDetailRepository = userDetailRepository;
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


        [HttpGet]
        public IActionResult MyLogout()
        {
            string? sessionId = HttpContext.Request.Cookies["SessionId"];
            if (!string.IsNullOrEmpty(sessionId))
            {
                authenticationRepository.DeleteSession(sessionId);
            }

            // Delete cookies
            Response.Cookies.Delete("SessionId");
            Response.Cookies.Delete("Username");
            Response.Cookies.Delete("CartItemNumber");

            // Redirect to a fresh logout page that runs JS
            return RedirectToAction("LogoutSuccess", "Authenticate");
        }

        public IActionResult LogoutSuccess()
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
        public async Task<IActionResult> Admin(string category)
        {
            User? user=  authenticationRepository.RetrieveFromSession(HttpContext.Request.Cookies["sessionId"]);
            ViewBag.User = user;
            if (user != null && user.IsAdmin)
            {
                var categories = await productRepository.GetAllCategoriesAsync();
                var products = string.IsNullOrEmpty(category) ? new List<Product>() : await productRepository.GetByCategoryAsync(category);
                await categoryRepository.InsertCategoryAsync(category);
                ViewBag.User = user;
                ViewBag.Products = products;
                ViewBag.Categories = categories;
                ViewBag.SelectedCategory = category;

                if (category != null) 
                {
                    await userDetailRepository.UpdateCategoryScoreAsync(user.Id, category, UserAction.ClickCategory);
                }

                



            }
            return View();
        }

        //[HttpPost]
        //public  IActionResult Profile()
        //{
        //    var product = new Product();
        //    productRepository.AddAsync(product);
        //    return View();
        //}
        [HttpPost]
        public async Task<IActionResult>MyLogin(string email, string password)
        {
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            DateTime requestTime = DateTime.UtcNow;
            var userAgent =Request.Headers["User-Agent"].ToString();
            User? user = authenticationRepository.AuthenticateUser(email, password);
            if (user != null)
            {   
                HttpContext.Response.Cookies.Append("SessionId", authenticationRepository.CreateSession(user, ipAddress, requestTime, userAgent));
                HttpContext.Response.Cookies.Append("Username", user.Username);
                int cartItemNumber= await userDetailRepository.CountCartItems(user.Id);
                HttpContext.Response.Cookies.Append("CartItemNumber", cartItemNumber.ToString());
                
                return RedirectToAction("Index", "Home");

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
                authenticationRepository.AddUser(user);
                ViewBag.Message = "Đăng ký thành công! Bạn có thể đăng nhập.";
                return RedirectToAction("MyLogin");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Đăng ký thất bại: " + ex.Message;
                return View();
            }
        }























        public async Task<IActionResult> Profile()
        {
            var notifications = new List<OrderNotification>
            {
                new OrderNotification {
                    ImageUrl = "https://phucanhcdn.com/media/product/55711_laptop_asus_vivobook_15_oled_a1505va_ma469w_6.jpg",
                    Status = "Giao kiện hàng thành công",
                    PackageCode = "SPXVN057191684675",
                    OrderCode = "2505296R62JNTS",
                    Date = new DateTime(2025, 5, 31, 9, 23, 0),
                    Message = "đã giao thành công đến bạn.",
                    ButtonText = "Xem Chi Tiết"
                },
                new OrderNotification {
                    ImageUrl = "https://www.techspot.com/images/products/2023/keyboards/org/2023-02-16-product-3.jpg",
                    Status = "Giao kiện hàng thành công",
                    PackageCode = "SPXVN055448964355",
                    OrderCode = "2505296SDBF0D3",
                    Date = new DateTime(2025, 5, 31, 9, 23, 0),
                    Message = "đã giao thành công đến bạn.",
                    ButtonText = "Xem Chi Tiết"
                },
                new OrderNotification {
                    ImageUrl = "https://product.hstatic.net/200000637319/product/gearvn-webcam-razer-kiyo-x-1_f806112c1b8e4209a2b4f4f332b4471b_eaff436e2bd94171844c5cfce593ad5b.jpg",
                    Status = "Đơn hàng đã hoàn tất",
                    OrderCode = "250511HAHE4838",
                    Date = new DateTime(2025, 5, 16, 15, 10, 0),
                    Message = "đã hoàn thành. Bạn hãy đánh giá sản phẩm trước ngày 15-06-2025 để nhận 200 xu",
                    ButtonText = "Đánh Giá Sản Phẩm"
                }
            };

            // Get user from session
            var user = authenticationRepository.RetrieveFromSession(HttpContext.Request.Cookies["SessionId"]);
            if (user == null)
            {
                return RedirectToAction("MyLogin", "Authenticate");
            }

            // Get user details
            var userDetail = await userDetailRepository.GetUserDetailAsync(user.Id);

            ViewBag.User = user;
            ViewBag.UserDetail = userDetail;
            ViewBag.Notifications = notifications;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress(string name, string phone, string address)
        {
            try
            {
                // Get user from session
                var user = authenticationRepository.RetrieveFromSession(HttpContext.Request.Cookies["SessionId"]);
                if (user == null)
                {
                    return RedirectToAction("MyLogin", "Authenticate");
                }

                // Create new ReceiveInfo
                var newReceiveInfo = new ReceiveInfo
                {
                    Name = name,
                    Phone = phone,
                    Address = address
                };

                // Add to database
                await userDetailRepository.AddReceiveInfoAsync(user.Id, newReceiveInfo);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }



    public class OrderNotification
    {
        public string ImageUrl { get; set; }
        public string Message { get; set; }
        public string PackageCode { get; set; }
        public string OrderCode { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } // e.g. "Giao kiện hàng thành công"
        public string ButtonText { get; set; } // e.g. "Xem Chi Tiết", "Đánh Giá Sản Phẩm"
    }



}
