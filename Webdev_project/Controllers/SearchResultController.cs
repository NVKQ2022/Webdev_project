using Microsoft.AspNetCore.Mvc;
using Webdev_project.Data;
using Webdev_project.Models;

namespace Webdev_project.Controllers
{
    public class SearchResultController : Controller
    {
        private readonly ProductRepository _repo;
        public SearchResultController(ProductRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> SearchResults(string query)
        {
            var products = string.IsNullOrWhiteSpace(query)
                ? new List<Product>()
                : await _repo.SearchByNameAsync(query);

            ViewBag.Query = query;
            return View(products);
        }
    }
}
