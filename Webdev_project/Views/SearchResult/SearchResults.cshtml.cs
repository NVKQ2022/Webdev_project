using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Webdev_project.Data;
using Webdev_project.Models;

namespace Webdev_project.Views.SearchResult
{
    public class SearchResultsModel : PageModel
    {
        private readonly ProductRepository _repo;
        public List<Product> Products { get; set; } = new();

        public SearchResultsModel(ProductRepository repo)
        {
            _repo = repo;
        }

        public async Task OnGetAsync(string query)
        {
            if (!string.IsNullOrWhiteSpace(query))
            {
                Products = await _repo.SearchByNameAsync(query);
            }
        }
    }
}
