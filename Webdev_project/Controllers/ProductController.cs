using Microsoft.AspNetCore.Mvc;
using Webdev_project.Data;
using Webdev_project.Models;
using Webdev_project.Interfaces;
using Webdev_project.Helpers;
namespace Webdev_project.Controllers
{
    [Route("Product")]
    public class ProductController : Controller
    {
       
        public readonly IProductRepository productRepository;
        public readonly IUserDetailRepository userDetailRepository;
        
        public readonly IAuthenticationRepository authenticationRepository;
        public ProductController(IProductRepository productRepository, IAuthenticationRepository authenticationRepository, IUserDetailRepository userDetailRepository)
        {
            this.productRepository = productRepository;
            this.authenticationRepository = authenticationRepository;
            this .userDetailRepository = userDetailRepository;
        }

        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> Detail(string id)
        {
            var product = await productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound("Product not found");
            }
            
            ViewBag.Product = product;
            //return NotFound(ViewBag.Product);
            return View();
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search(string keyword, int page = 1)
        {
            int pageSize = 42;
            ConverterHelper converterHelper = new ConverterHelper();

            var searchResults = await productRepository.SearchAsync(keyword);
            List<Product_zip> product_Zips = converterHelper.ConvertProductListToProductZipList(searchResults);

            int totalProducts = product_Zips.Count;
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            var pagedProducts = product_Zips.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.product = pagedProducts;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.Keyword = keyword;

            return View("~/Views/SearchResult/SearchResults.cshtml", pagedProducts);

        }


        //[HttpGet("Detail2/{id}")]
        //public async Task<IActionResult> Detail2(string id = null)
        //{
        //    var product = await productRepository.GetByIdAsync(id);

        //    if (product == null)
        //    {
        //        return NotFound("Product not found");
        //    }

        //    ViewBag.Product = product;
        //    //return NotFound(ViewBag.Product);
        //    return View();
        //}

        //[HttpPost("Detail/{id}")] //not yet
        //public async Task<IActionResult> PutInCart(string id = null)
        //{

        //    CartItem cartitem = new CartItem();
        //    await userDetailRepository.AddCartItemAsync((authenticationRepository.RetrieveFromSession(HttpContext.Request.Cookies["SessionId"])).Id,cartitem);
        //    return Ok(cartitem);
        //}




    }
}
