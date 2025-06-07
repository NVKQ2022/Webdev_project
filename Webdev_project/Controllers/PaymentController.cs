using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Mvc;
using Webdev_project.Helpers;
using Webdev_project.Models;
using Webdev_project.Interfaces;
using System.Text.Json;
using MongoDB.Bson.IO;
using Newtonsoft.Json;


namespace Webdev_project.Controllers
{
    public class PaymentController : Controller
    {

        private readonly IAuthenticationRepository authenticationRepository;
        private readonly IUserDetailRepository userDetailRepository; 
        public PaymentController(IAuthenticationRepository authenticationRepository, IUserDetailRepository userDetailRepository)
        {
            this.authenticationRepository = authenticationRepository;
            this.userDetailRepository = userDetailRepository;
        }




        [HttpPost]
        public IActionResult BuyItems(string selectedItems)
        {
            var items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CartItem>>(selectedItems);

            // Pass to view
            return View("Index", items);
        }




        [HttpGet]       
        public async Task<IActionResult> IndexOld()
        {
            var user = authenticationRepository.RetrieveFromSession(HttpContext.Request.Cookies["SessionId"]);
            // Lấy giỏ hàng từ session
            var cartItems = await userDetailRepository.GetCartItemsAsync(user.Id);
            var address = await userDetailRepository.GetReceiveInfoAsync(user.Id);

            // Tính tổng tiền đơn hàng
            var total = cartItems.Sum(item => item.UnitPrice * item.Quantity);
            ViewBag.TotalAmount = total;

            // Lấy user đang đăng nhập (giả lập ở đây)
          
           

            ViewBag.User =  user;
            ViewBag.ReceiveInfos = address;

            return View(cartItems); // Truyền giỏ hàng sang view
        }

        [HttpPost]
        public IActionResult PaymentResult(string paymentMethod, decimal totalAmount)
        {
            ViewBag.PaymentMethod = paymentMethod;
            ViewBag.TotalAmount = totalAmount;
            return View();
        }


        [HttpPost]
        public IActionResult SubmitPayment(string paymentMethod, decimal totalAmount)
        {
            return RedirectToAction("PaymentResult", new {
                paymentMethod = paymentMethod,
                totalAmount = totalAmount
            });

        }

    }
}
