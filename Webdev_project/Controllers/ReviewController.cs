using Microsoft.AspNetCore.Mvc;

namespace Webdev_project.Controllers
{
    public class ReviewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
