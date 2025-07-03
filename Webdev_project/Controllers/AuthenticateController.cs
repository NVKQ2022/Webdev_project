using Microsoft.AspNetCore.Mvc;
//using NuGet.Protocol.Plugins;
using Webdev_project.Models;
using Webdev_project.Interfaces;
using Webdev_project.Helpers;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Webdev_project.Data;
using MongoDB.Bson.Serialization.IdGenerators;
namespace Webdev_project.Controllers
{
    public class AuthenticateController : Controller
    {
        //public IConfiguration configuration;
        
        
        private readonly IAuthenticationRepository authenticationRepository;
        private readonly IProductRepository productRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IUserDetailRepository userDetailRepository;
        private readonly IOrderRepository orderRepository;
        public AuthenticateController(IAuthenticationRepository authenticationRepository, IProductRepository productRepository,ICategoryRepository categoryRepository,IUserDetailRepository userDetailRepository, IOrderRepository orderRepository)
        {
            
            this.productRepository= productRepository;
            this.authenticationRepository = authenticationRepository;
            this.categoryRepository = categoryRepository;
            this.userDetailRepository = userDetailRepository;
            this.orderRepository = orderRepository;
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
                    
                   // await userDetailRepository.UpdateCategoryScoreAsync(user.Id, category, UserAction.ClickCategory);
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
        public async Task<IActionResult> MyLogin(string email, string password, bool remember)
        {
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            DateTime requestTime = DateTime.UtcNow;
            var userAgent = Request.Headers["User-Agent"].ToString();

       
            string? salt = await authenticationRepository.GetSaltByEmailAsync(email);
            if (salt == null)
            {
                ViewBag.Message = "Sai thông tin!";
                return View("MyLogin");
            }

          
            string hashPassword = SecurityHelper.HashPassword(password, salt);
            User? user = authenticationRepository.AuthenticateUser(email, hashPassword);

          
            if (user != null)
            { 
                var sessionId = authenticationRepository.CreateSession(user, ipAddress, requestTime, userAgent);

               
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    //Secure = true, // Bảo mật cookie, yêu cầu HTTPS( need HTTPs)
                    SameSite = SameSiteMode.Strict,
                    Expires = remember ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddSeconds(0) // Cookie sẽ hết hạn sau 7 ngày nếu chọn "Remember Me", 1 giờ nếu không
                };

                // Lưu thông tin người dùng và session vào cookie
                HttpContext.Response.Cookies.Append("SessionId", sessionId, cookieOptions);
                HttpContext.Response.Cookies.Append("Username", user.Username, cookieOptions);

               
                int cartItemNumber = await userDetailRepository.CountCartItems(user.Id);
                HttpContext.Response.Cookies.Append("CartItemNumber", cartItemNumber.ToString(), cookieOptions);

               
                return RedirectToAction("Index", "Home");
            }

          
            ViewBag.Message = "Sai thông tin!";
            return View("MyLogin");
        }


        [HttpPost]
        public async Task<IActionResult> MyRegister(string email, string username, string password, string confirmPassword)
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
                Id = authenticationRepository.GetCurrentUserId() + 1, // get current user id from database then increase it by 1
                Email = email,
                Username = username,
                Password = hashedPassword,
                Salt = salt,
                IsAdmin = false
            };

            var userDetail = new UserDetail
            {
                UserId = authenticationRepository.GetCurrentUserId()+1,
                Avatar = String.Empty,
                Category = new Dictionary<string, int>(),
                Cart = new List<CartItem>(),
                ReceiveInfo = new List<ReceiveInfo>(),
                PhoneNumber = string.Empty,
                Gender = string.Empty,
                Birthday = new DateTime(),
                Banking = new Banking()

            };


            try
            {
                var categories = await productRepository.GetAllCategoriesAsync();
                authenticationRepository.AddUser(user); // add user in to database
                authenticationRepository.UpdateCurrentUserId();// update current user id in database
                await userDetailRepository.AddUserDetailAsync(userDetail);// add the detail of user in to database
                await userDetailRepository.InsertUserCategoriesAsync(userDetail.UserId, categories);
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


            // Get user from session
            var user = authenticationRepository.RetrieveFromSession(HttpContext.Request.Cookies["SessionId"]);
            //var categories = await productRepository.GetAllCategoriesAsync();
            //await userDetailRepository.InsertUserCategoriesAsync(user.Id, categories);
            if (user == null)
            {
                return RedirectToAction("MyLogin", "Authenticate");
            }

            // Get user details
            var userDetail = await userDetailRepository.GetUserDetailAsync(user.Id);
            var receiveInfo = await userDetailRepository.GetReceiveInfoAsync(user.Id);
            var orders = await orderRepository.GetOrdersByUserAsync(user.Id);
            ViewBag.User = user;
            ViewBag.UserDetail = userDetail;
            ViewBag.Orders =orders;
            return View();
        }
        public async Task<IActionResult> ProfileOld()
        {


            // Get user from session
            var user = authenticationRepository.RetrieveFromSession(HttpContext.Request.Cookies["SessionId"]);

            if (user == null)
            {
                return RedirectToAction("MyLogin", "Authenticate");
            }

            // Get user details
            var userDetail = await userDetailRepository.GetUserDetailAsync(user.Id);
            var receiveInfo = await userDetailRepository.GetReceiveInfoAsync(user.Id);
            var orders = await orderRepository.GetOrdersByUserAsync(user.Id);
            ViewBag.User = user;
            ViewBag.UserDetail = userDetail;
            ViewBag.Orders = orders;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromBody] UserInfo model)
        {

            if (model == null)
                return BadRequest("Invalid data");
            var user = authenticationRepository.RetrieveFromSession(HttpContext.Request.Cookies["SessionId"]);
            
            bool success = await userDetailRepository.UpdateUserInfo(user.Id, model.Name, model.PhoneNumber, model.Gender, model.Birthday);
            if (success)
                return Ok(new { message = "Cập nhật thành công" });
            else
                return NotFound(new { message = $"Không tìm thấy người dùng với id {user.Id}" });
        }


        public class UserInfo
        {
            
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string Gender { get; set; }
            public DateTime Birthday { get; set; }
        }












        [HttpPost]
        public async Task<IActionResult> AddAddress(string name, string phone, string address)
        {
            // Create new ReceiveInfo
            ReceiveInfo newReceiveInfo = new ReceiveInfo
            {
                Name = name,
                Phone = phone,
                Address = address
            };
            try
            {
                // Get user from session
                var user = authenticationRepository.RetrieveFromSession(HttpContext.Request.Cookies["SessionId"]);
                if (user == null)
                {
                    return RedirectToAction("MyLogin", "Authenticate");
                }

                

                // Add to database
                await userDetailRepository.AddReceiveInfoAsync(user.Id, newReceiveInfo);

                return Json(new { success = true , ReceiveInfo = newReceiveInfo });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message ,ReceiveInfo = newReceiveInfo});
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
