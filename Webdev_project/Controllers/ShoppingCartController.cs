

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
        public async Task<IActionResult> UpdateQuantity(string productId, int quantity)
        {
            string sessionId = Request.Cookies["SessionId"];
            var userId = authenticationRepository.RetrieveFromSession(sessionId).Id;
            
            // check if valid stock (add a function to the product repository
            // if valid
           // var updateQuantity = await userDetailRepository.UpdateCartItemQuantityAsync(userId, productId, quantity);
            //else
            var updateQuantity = await userDetailRepository.InsertCartItemQuantityAsync(userId, productId,quantity);

            return Ok(updateQuantity);
        }


        [HttpPost]
        public async Task<IActionResult> RemoveItem(string productId)
        {
            string sessionId = Request.Cookies["SessionId"];
            var userId = authenticationRepository.RetrieveFromSession(sessionId).Id;

            await userDetailRepository.RemoveCartItemAsync(userId, productId);
            string? cartItemNumberStr = HttpContext.Request.Cookies["CartItemNumber"];
            int cartItemNumber = 0;

            if (!string.IsNullOrEmpty(cartItemNumberStr) && int.TryParse(cartItemNumberStr, out int currentCount))
            {
                cartItemNumber = currentCount - 1;
            }
            else
            {
                cartItemNumber = 1;
            }

            // Update the cookie
            HttpContext.Response.Cookies.Append("CartItemNumber", cartItemNumber.ToString());
            return Ok();
        }
        //[HttpPost]
        //public IActionResult Decrease(string productId)
        //{
        //    var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
        //    var item = cart.FirstOrDefault(x => x.ProductId == productId);
        //    if (item != null && item.Quantity > 1) item.Quantity--;
        //    HttpContext.Session.SetObjectAsJson("Cart", cart);
        //    return RedirectToAction("Index");

        //}
        //[HttpPost]
        //public IActionResult Increase(string productId)
        //{
        //    var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
        //    var item = cart.FirstOrDefault(x => x.ProductId == productId);
        //    if (item != null) item.Quantity++;
        //    HttpContext.Session.SetObjectAsJson("Cart", cart);
        //    return RedirectToAction("Index");
        //}
        //[HttpPost]
        //public IActionResult Remove(string productId)
        //{
        //    var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
        //    cart.RemoveAll(x => x.ProductId == productId);
        //    HttpContext.Session.SetObjectAsJson("Cart", cart);
        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        public IActionResult AddToCart([FromBody] CartItem item)
        {
            if (item == null)
                return BadRequest("Invalid cart item");

            // Get user ID from session
            var sessionId = HttpContext.Request.Cookies["SessionId"];
            var user = authenticationRepository.RetrieveFromSession(sessionId);
            if (user == null)
                return Unauthorized("Session invalid");

            // Add item to the cart
            userDetailRepository.AddCartItemAsync(user.Id, item);

            // Read current cookie value and increment
            string? cartItemNumberStr = HttpContext.Request.Cookies["CartItemNumber"];
            int cartItemNumber = 0;

            if (!string.IsNullOrEmpty(cartItemNumberStr) && int.TryParse(cartItemNumberStr, out int currentCount))
            {
                cartItemNumber = currentCount + 1;
            }
            else
            {
                cartItemNumber = 1;
            }

            // Update the cookie
            HttpContext.Response.Cookies.Append("CartItemNumber", cartItemNumber.ToString()
            //    ,
            //    new CookieOptions
            //{
            //    HttpOnly = false,
            //    SameSite = SameSiteMode.Lax,
            //    Secure = false, // true if you're using HTTPS
            //    Expires = DateTimeOffset.UtcNow.AddDays(7)
            //}
            );

            return Ok(new { success = true, message = "Item added to cart." });
        }


    }
}



