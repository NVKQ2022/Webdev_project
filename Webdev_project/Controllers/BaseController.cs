using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Webdev_project.Interfaces;

namespace Webdev_project.Controllers
{
    public class BaseController : Controller
    {
        private readonly IAuthenticationRepository _authRepo;
        //private readonly IUserDetailRepository userDetailRepository;

        public BaseController(IAuthenticationRepository authRepo/*, IUserDetailRepository userDetailRepository*/)
        {
            _authRepo = authRepo;
            //this.userDetailRepository = userDetailRepository;
           

        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string? sessionId = HttpContext.Request.Cookies["SessionId"];
            string? cartItemNumber = HttpContext.Request.Cookies["CartItemNumber"];

            // Pass to layout via ViewBag (or ViewData)
            


            if (!string.IsNullOrEmpty(sessionId))
            {
                var user = _authRepo.RetrieveFromSession(sessionId);
                if (user != null)
                {
                    ViewBag.isAdmin = user.IsAdmin;
                    ViewBag.isLoggedIn = true;
                    ViewBag.userName = user.Username;
                    ViewBag.cartItemNumber = cartItemNumber;
                    return;
                }
            }

            ViewBag.isLoggedIn = false;
            ViewBag.userName = null;
            ViewBag.cartItemNumber = cartItemNumber;
            base.OnActionExecuting(context);
        }
    }
}
