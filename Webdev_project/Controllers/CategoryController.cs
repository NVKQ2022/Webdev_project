using Microsoft.AspNetCore.Mvc;
using Webdev_project.Data;
using Webdev_project.Helpers;
using Webdev_project.Interfaces;
using Webdev_project.Models;

namespace Webdev_project.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly IProductRepository productRepository;
        private readonly IAuthenticationRepository authenticationRepository;

        public CategoryController(IProductRepository productRepository, IAuthenticationRepository authenticationRepository) : base(authenticationRepository)
        {
            this.productRepository = productRepository;
            this.authenticationRepository = authenticationRepository;
        }
        //public async Task<IActionResult> Products(string CateName, string sort = "default", string search = "")
        //{
        //    ConverterHelper converterHelper = new ConverterHelper();
        //    var products = await productRepository.GetByCategoryAsync(CateName);

        //    // Tìm kiếm
        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        //    }

        //    // Sắp xếp
        //    switch (sort)
        //    {
        //        case "price-asc":
        //            products = products.OrderBy(p => p.Price).ToList();
        //            break;
        //        case "price-desc":
        //            products = products.OrderByDescending(p => p.Price).ToList();
        //            break;
        //        case "name":
        //            products = products.OrderBy(p => p.Name).ToList();
        //            break;
        //        case "sold":
        //            products = products.OrderByDescending(p => p.Sold).ToList();
        //            break;
        //    }

        //    var products_zip = converterHelper.ConvertProductListToProductZipList(products);
        //    ViewBag.CategoryName = CateName;
        //    ViewBag.Sort = sort;
        //    ViewBag.Search = search;
        //    return View(products_zip);
        //}

        //public async Task<IActionResult> ProductsByCategory(string category, int page = 1)
        //{
        //    int pageSize = 42; // số sản phẩm trên 1 trang

        //    var products = string.IsNullOrEmpty(category) ?
        //                    new List<Product>() :
        //                    await productRepository.GetByCategoryAsync(category);

        //    ConverterHelper converterHelper = new ConverterHelper();
        //    List<Product_zip> product_Zips = converterHelper.ConvertProductListToProductZipList(products);

        //    int totalProducts = product_Zips.Count;
        //    int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

        //    var pagedProducts = product_Zips
        //                        .Skip((page - 1) * pageSize)
        //                        .Take(pageSize)
        //                        .ToList();

        //    ViewBag.product = pagedProducts;
        //    ViewBag.TotalPages = totalPages;
        //    ViewBag.CurrentPage = page;
        //    ViewBag.SelectedCategory = category;

        //    return View("~/Views/SearchResult/CategoryResults.cshtml", pagedProducts);
        //}

        public async Task<IActionResult> Products(string CateName, string sort = "default", string search = "", int page = 1)
        {
            const int pageSize = 42;

            // Get products by category
            var products = string.IsNullOrEmpty(CateName)
                ? new List<Product>()
                : await productRepository.GetByCategoryAsync(CateName);

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Sorting
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
                    // Add more sorting options if needed
            }

            // Convert to Product_zip list
            ConverterHelper converterHelper = new ConverterHelper();
            var product_Zips = converterHelper.ConvertProductListToProductZipList(products);

            // Pagination
            int totalProducts = product_Zips.Count;
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            var pagedProducts = product_Zips
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // ViewBags
            ViewBag.CategoryName = CateName;
            ViewBag.Sort = sort;
            ViewBag.Search = search;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.SelectedCategory = CateName;
            ViewBag.product = pagedProducts;

            return View("~/Views/SearchResult/CategoryResults.cshtml", pagedProducts);
        }


    }
}
