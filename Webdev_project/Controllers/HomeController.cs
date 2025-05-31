using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using Webdev_project.Helpers;
using Webdev_project.Interfaces;
using Webdev_project.Models;

namespace Webdev_project.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository userRepository;
        private readonly ISessionRepository sessionRepository;
        private readonly IProductRepository productRepository;

        public HomeController(ILogger<HomeController> logger, IUserRepository userRepository, ISessionRepository sessionRepository, IProductRepository productRepository)
        {
            _logger = logger;
            this.userRepository = userRepository;
            this.sessionRepository = sessionRepository;
            this.productRepository = productRepository;
        }

        //[HttpGet("Index/{category}")]


        public async Task<IActionResult> Index()
        {
            ConverterHelper converterHelper = new ConverterHelper();   
            var categories = await productRepository.GetAllCategoriesAsync();
            List<Product_zip> product_Zips = converterHelper.ConvertProductListToProductZipList(await productRepository.GetAllAsync());
            ViewBag.product = product_Zips;
            ViewBag.Categories = categories;
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
