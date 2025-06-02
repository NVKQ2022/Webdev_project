using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Webdev_project.Interfaces;

public class BaseController : Controller
{
    protected readonly IUserRepository _userRepository;

    public BaseController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
        {
            var user = _userRepository.GetUserById(userId);
            ViewBag.User = user;
        }
        else
        {
            ViewBag.User = null;
        }
        base.OnActionExecuting(context);
    }
}
