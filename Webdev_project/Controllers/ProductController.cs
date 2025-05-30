using Microsoft.AspNetCore.Mvc;
using Webdev_project.Data;
using Webdev_project.Models;
using Webdev_project.Interfaces;
namespace Webdev_project.Controllers
{
    [Route("Product")]
    public class ProductController : Controller
    {
       
        public readonly IProductRepository productRepository;

        public ProductController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> Detail(string id = null)
        {
            var product = await productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound("Product not found");
            }

            ViewBag.Product = product;
            return View();
        }

      


    }
}
