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
        private readonly IOrderRepository orderRepository;

        public HomeController(ILogger<HomeController> logger, IAuthenticationRepository authenticationRepository, IProductRepository productRepository, ICategoryRepository categoryRepository, IUserDetailRepository userDetailRepository , IOrderRepository orderRepository) : base(authenticationRepository)
        {
            _logger = logger;
            this.authenticationRepository = authenticationRepository;
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
            this.userDetailRepository = userDetailRepository;
            this.orderRepository = orderRepository;
        }


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
        public async Task<IActionResult> Index(string category)
        {
            var categories = await productRepository.GetAllCategoriesAsync();
            var products = string.IsNullOrEmpty(category) ? new List<Product>() : await productRepository.GetByCategoryAsync(category);


            ViewBag.Products = products;
            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = category;



            return View();

        }

        
        [HttpGet]
        public async Task<IActionResult> Test(string category)
        {
            ConverterHelper converterHelper = new ConverterHelper();

            var categories = await productRepository.GetAllCategoriesAsync();
            var products = string.IsNullOrEmpty(category)
                ? new List<Product>()
                : await productRepository.GetByCategoryAsync(category);

            var categoryList = await categoryRepository.GetCategoriesSortedByBuyTimeAsync();
            var userDetails = await userDetailRepository.GetAllAsync();
            var orders = await orderRepository.GetAllOrdersAsync();
            var allProducts = await productRepository.GetAllAsync(); // tất cả sản phẩm
            var product_Zips = converterHelper.ConvertProductListToProductZipList(allProducts);

            // Top 5 sản phẩm bán chạy
            var topSellingProducts = product_Zips
            .OrderByDescending(p => p.QuantitySold)
            .Take(5)
            .ToList(); 


            ViewBag.TopSelling = topSellingProducts;
            ViewBag.AllProducts = allProducts;
            ViewBag.Categories = categories;
            ViewBag.Products = products;
            ViewBag.SelectedCategory = category;
            ViewBag.CategoryList = categoryList;
            ViewBag.UserDetails = userDetails;
            ViewBag.Orders = orders;

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






    }
}
