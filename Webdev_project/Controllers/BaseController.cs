using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Webdev_project.Interfaces;

namespace Webdev_project.Controllers
{
    public class BaseController : Controller
    {
        private readonly IAuthenticationRepository _authRepo;

        public BaseController(IAuthenticationRepository authRepo)
        {
            _authRepo = authRepo;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string? sessionId = HttpContext.Request.Cookies["SessionId"];

            if (!string.IsNullOrEmpty(sessionId))
            {
                var user = _authRepo.RetrieveFromSession(sessionId);
                if (user != null)
                {
                    ViewBag.isLoggedIn = true;
                    ViewBag.userName = user.Username;
                    return;
                }
            }

            ViewBag.isLoggedIn = false;
            ViewBag.userName = null;

            base.OnActionExecuting(context);
        }
    }
}
