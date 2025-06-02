using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Webdev_project.Interfaces;
using Webdev_project.Models;

namespace Webdev_project.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository userRepository;
        private readonly ISessionRepository sessionRepository;
        private readonly IProductRepository productRepository;

        public HomeController(
            ILogger<HomeController> logger,
            IUserRepository userRepository,
            ISessionRepository sessionRepository,
            IProductRepository productRepository
        ) : base(userRepository)
        {
            _logger = logger;
            this.userRepository = userRepository;
            this.sessionRepository = sessionRepository;
            this.productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await productRepository.GetAllCategoriesAsync();
            ViewBag.Categories = categories;
            return View();
        }

        [HttpGet("Index/{category}")]
        public async Task<IActionResult> Index(string category)
        {
            var categories = await productRepository.GetAllCategoriesAsync();
            var products = string.IsNullOrEmpty(category)
                ? new List<Product>()
                : await productRepository.GetByCategoryAsync(category);

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
