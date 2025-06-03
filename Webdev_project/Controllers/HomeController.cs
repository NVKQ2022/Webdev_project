using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using Webdev_project.Helpers;
using Webdev_project.Interfaces;
using Webdev_project.Models;

namespace Webdev_project.Controllers
{

    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAuthenticationRepository authenticationRepository;
        private readonly IProductRepository productRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IUserDetailRepository userDetailRepository;

        public HomeController(ILogger<HomeController> logger, IAuthenticationRepository authenticationRepository, IProductRepository productRepository, ICategoryRepository categoryRepository,IUserDetailRepository userDetailRepository) : base(authenticationRepository)
        {
            _logger = logger;
            this.authenticationRepository = authenticationRepository;
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
            this.userDetailRepository = userDetailRepository;
        }

        //[HttpGet("Index/{category}")]


        //public async Task<IActionResult> Index()
        //{
        //    ConverterHelper converterHelper = new ConverterHelper();
        //    //var categories = await productRepository.GetAllCategoriesAsync();
        //    List<Product_zip> product_Zips;
        //    List<Category> categories;
        //    var sessionId = HttpContext.Request.Cookies["SessionId"];
        //    if (sessionId != null)
        //    {
        //        var user = authenticationRepository.RetrieveFromSession(sessionId);

        //        if (user != null)
        //        {
        //            List<Product> allProducts = new List<Product>();
        //            List<string> categoriesFromUser = await userDetailRepository.GetCategoriesByPointDescending(user.Id);
        //            foreach (var category in categoriesFromUser)
        //            {
        //                List<Product> productsInCategory = await productRepository.GetByCategoryAsync(category);
        //                allProducts.AddRange(productsInCategory);
        //            }

        //            categories = await categoryRepository.GetCategoriesSortedByBuyTimeAsync();
        //            product_Zips = converterHelper.ConvertProductListToProductZipList(allProducts);
        //            ViewBag.Categories = categories;
        //            ViewBag.product = product_Zips;
        //            return View();


        //        }
        //    }
        //    product_Zips = converterHelper.ConvertProductListToProductZipList(await productRepository.GetAllAsync());
        //    ViewBag.product = product_Zips;
        //    categories = await categoryRepository.GetCategoriesSortedByBuyTimeAsync();
        //    ViewBag.Categories = categories;
        //    return View();
        //}



        //public async Task<IActionResult> Index2(int page = 1)
        //{
        //    int pageSize = 42; // Hiển thị 7 dòng x 6 cột
        //    ConverterHelper converterHelper = new ConverterHelper();
        //    var categories = await categoryRepository.GetCategoriesSortedByBuyTimeAsync();

        //    List<Product_zip> product_Zips = converterHelper.ConvertProductListToProductZipList(await productRepository.GetAllAsync());

        //    int totalProducts = product_Zips.Count;
        //    int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

        //    var pagedProducts = product_Zips
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToList();

        //    ViewBag.product = pagedProducts;
        //    ViewBag.Categories = categories;
        //    ViewBag.TotalPages = totalPages;
        //    ViewBag.CurrentPage = page;

        //    return View();
        //}

        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 42; // 7 rows × 6 columns
            ConverterHelper converterHelper = new ConverterHelper();

            List<Product> allProducts = new List<Product>();
            List<Category> categories = await categoryRepository.GetCategoriesSortedByBuyTimeAsync();

            var sessionId = HttpContext.Request.Cookies["SessionId"];
            if (sessionId != null)
            {
                var user = authenticationRepository.RetrieveFromSession(sessionId);
                if (user != null)
                {
                    List<string> categoriesFromUser = await userDetailRepository.GetCategoriesByPointDescending(user.Id);
                    foreach (var category in categoriesFromUser)
                    {
                        List<Product> productsInCategory = await productRepository.GetByCategoryAsync(category);
                        allProducts.AddRange(productsInCategory);
                    }
                }
            }

            // If not logged in or user has no personalized categories, fallback to all products
            if (!allProducts.Any())
            {
                allProducts = await productRepository.GetAllAsync();
            }

            List<Product_zip> product_Zips = converterHelper.ConvertProductListToProductZipList(allProducts);

            int totalProducts = product_Zips.Count;
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            var pagedProducts = product_Zips
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.product = pagedProducts;
            ViewBag.Categories = categories;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View();
        }



        [HttpGet("Index/{category}")]
        public async  Task<IActionResult> Index(string category)
        {
            var categories = await productRepository.GetAllCategoriesAsync();
            var products = string.IsNullOrEmpty(category) ? new List<Product>() : await productRepository.GetByCategoryAsync(category);

            
            ViewBag.Products = products;
            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = category;

           

            return View();

        }





        public IActionResult Policy()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }







        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }





        public async Task<IActionResult> Test()
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
