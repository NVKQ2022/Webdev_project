using Microsoft.AspNetCore.Mvc;
using Webdev_project.Data;
using Webdev_project.Models;
using Webdev_project.Interfaces;
namespace Webdev_project.Controllers
{
    [Route("Product")]
    public class ProductController : BaseController
    {
       
        public readonly IProductRepository productRepository;
        public readonly IUserDetailRepository userDetailRepository;
        
        public readonly IAuthenticationRepository authenticationRepository;
        public ProductController(IProductRepository productRepository, IAuthenticationRepository authenticationRepository, IUserDetailRepository userDetailRepository) : base(authenticationRepository)
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
