using Microsoft.AspNetCore.Mvc;
using Webdev_project.Helpers;
using Webdev_project.Interfaces;

namespace Webdev_project.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IProductRepository productRepository;

        public CategoryController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }
        public async Task<IActionResult> Products(string CateName, string sort = "default", string search = "")
        {
            ConverterHelper converterHelper = new ConverterHelper();
            var products = await productRepository.GetByCategoryAsync(CateName);

            // Tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Sắp xếp
            switch (sort)
            {
                case "price-asc":
                    products = products.OrderBy(p => p.Price).ToList();
                    break;
                case "price-desc":
                    products = products.OrderByDescending(p => p.Price).ToList();
                    break;
                case "name":
                    products = products.OrderBy(p => p.Name).ToList();
                    break;
                case "sold":
                    products = products.OrderByDescending(p => p.Sold).ToList();
                    break;
            }

            var products_zip = converterHelper.ConvertProductListToProductZipList(products);
            ViewBag.CategoryName = CateName;
            ViewBag.Sort = sort;
            ViewBag.Search = search;
            return View(products_zip);
        }


    }
}
