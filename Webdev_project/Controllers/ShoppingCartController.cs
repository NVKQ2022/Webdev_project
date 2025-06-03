

using Microsoft.AspNetCore.Mvc;
using Webdev_project.Models;
using Webdev_project.Helpers;
using System.Collections.Generic;
using System.Linq;
using Webdev_project.Interfaces;
using Webdev_project.Data;

namespace Webdev_project.Controllers
{
    public class ShoppingCartController : BaseController
    {
        private readonly IUserDetailRepository userDetailRepository;
        private readonly IAuthenticationRepository authenticationRepository;
        public ShoppingCartController(IUserDetailRepository userDetailRepository, IAuthenticationRepository authenticationRepository) : base(authenticationRepository)
        {
            this.userDetailRepository = userDetailRepository;
            this.authenticationRepository = authenticationRepository;
        }
        public async Task<IActionResult> Index()
        {
           
            string sessionId = HttpContext.Request.Cookies["SessionId"];

            if (sessionId == null)
            {
                return View("~/Views/Authenticate/MyLogin.cshtml");
            }

            var cart =await userDetailRepository.GetCartItemsAsync(authenticationRepository.RetrieveFromSession(sessionId).Id);

            return View(cart);
          
        }

      


        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(string productId, int delta)
        {
            string sessionId = Request.Cookies["SessionId"];
            var userId = authenticationRepository.RetrieveFromSession(sessionId).Id;
            var updateQuantity = await userDetailRepository.UpdateCartItemQuantityAsync(userId, productId, delta);



            return Ok(updateQuantity);
        }


        [HttpPost]
        public async Task<IActionResult> RemoveItem(string productId)
        {
            string sessionId = Request.Cookies["SessionId"];
            var userId = authenticationRepository.RetrieveFromSession(sessionId).Id;

            await userDetailRepository.RemoveCartItemAsync(userId, productId);
            return Ok();
        }
        [HttpPost]
        public IActionResult Decrease(string productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            if (item != null && item.Quantity > 1) item.Quantity--;
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index");

        }
        [HttpPost]
        public IActionResult Increase(string productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            if (item != null) item.Quantity++;
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Remove(string productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            cart.RemoveAll(x => x.ProductId == productId);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddToCart([FromBody] CartItem item)
        {
            if (item == null)
                return BadRequest("Invalid cart item");


            userDetailRepository.AddCartItemAsync(authenticationRepository.RetrieveFromSession(HttpContext.Request.Cookies["SessionId"]).Id, item);



            // Add item to cart (session, DB, etc.)
            return Ok(new { success = true, message = "Item added to cart." });
        }

    }
}



